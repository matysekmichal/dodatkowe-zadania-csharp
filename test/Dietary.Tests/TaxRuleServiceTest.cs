using System;
using System.Linq;
using System.Threading.Tasks;
using LegacyFighter.Dietary.Models;
using Xunit;

namespace LegacyFighter.Dietary.Tests
{
    public sealed class TaxRuleServiceTest : IDisposable
    {
        [Fact]
        public async Task ItCanAddLinearTaxRuleToNewTaxConfigAndNotApplyItToInitialOrders()
        {
            // Arrange && Act
            await _taxRuleService.AddTaxRuleToCountryAsync(
                "PL",
                1,
                5,
                "0001");

            // Assert
            var taxConfig = await _taxConfigRepository.FindByCountryCodeAsync(CountryCode.Of("PL"));

            Assert.Single(
                taxConfig.TaxRules
                    .Where(taxRule => taxRule.AFactor.Equals(1) &&
                                      taxRule.BFactor.Equals(5) &&
                                      taxRule.IsLinear.Equals(true) &&
                                      taxRule.IsSquare.Equals(false) &&
                                      taxRule.TaxCode.EndsWith("0001")));

            var orders = await _orderRepository.FindByOrderStateAsync(Order.OrderState.Initial);
            var order = orders.FirstOrDefault(order => order.CustomerOrderGroup.Customer.Type.Equals(Customer.CustomerType.Person));

            Assert.NotNull(order);
            Assert.Empty(order.TaxRules.FindAll(taxRule => taxRule.TaxCode.EndsWith("0001")));
        }

        [Fact]
        public async Task ItCanAddLinearTaxRuleToExistingTaxConfigAndApplyItToInitialOrders()
        {
            // Arrange
            await _taxRuleService.AddTaxRuleToCountryAsync(
                "PL",
                1,
                5,
                "0001");

            // Act
            await _taxRuleService.AddTaxRuleToCountryAsync(
                "PL",
                3,
                2,
                "0002");

            // Assert
            var taxConfig = await _taxConfigRepository.FindByCountryCodeAsync(CountryCode.Of("PL"));

            Assert.Single(
                taxConfig.TaxRules
                    .Where(taxRule => taxRule.AFactor.Equals(1) &&
                                      taxRule.BFactor.Equals(5) &&
                                      taxRule.IsLinear.Equals(true) &&
                                      taxRule.IsSquare.Equals(false) &&
                                      taxRule.TaxCode.EndsWith("0001")));

            var orders = await _orderRepository.FindByOrderStateAsync(Order.OrderState.Initial);
            var order = orders.FirstOrDefault(order => order.CustomerOrderGroup.Customer.Type.Equals(Customer.CustomerType.Person));

            Assert.NotNull(order);
            Assert.NotEmpty(order.TaxRules.FindAll(taxRule => taxRule.TaxCode.EndsWith("0002")));
        }

        [Fact]
        public async Task ItCanAddSquareTaxRuleToNewTaxConfig()
        {
            // Arrange && Act
            await _taxRuleService.AddTaxRuleToCountryAsync(
                "PL",
                1,
                5,
                7,
                "0001");

            // Assert
            var taxConfig = await _taxConfigRepository.FindByCountryCodeAsync(CountryCode.Of("PL"));

            Assert.Single(
                taxConfig.TaxRules
                    .Where(taxRule => taxRule.AFactor.Equals(1) &&
                                      taxRule.BFactor.Equals(5) &&
                                      taxRule.CFactor.Equals(7) &&
                                      taxRule.IsLinear.Equals(false) &&
                                      taxRule.IsSquare.Equals(true) &&
                                      taxRule.TaxCode.EndsWith("0001")));
        }

        [Fact]
        public async Task ItCanAddSquareTaxRuleToExistingTaxConfig()
        {
            // Arrange
            await _taxRuleService.AddTaxRuleToCountryAsync(
                "PL",
                1,
                5,
                7,
                "0001");

            // Act
            await _taxRuleService.AddTaxRuleToCountryAsync(
                "PL",
                3,
                2,
                7,
                "0002");

            // Assert
            var taxConfig = await _taxConfigRepository.FindByCountryCodeAsync(CountryCode.Of("PL"));

            Assert.Single(
                taxConfig.TaxRules
                    .Where(taxRule => taxRule.AFactor.Equals(3) &&
                                      taxRule.BFactor.Equals(2) &&
                                      taxRule.CFactor.Equals(7) &&
                                      taxRule.IsLinear.Equals(false) &&
                                      taxRule.IsSquare.Equals(true) &&
                                      taxRule.TaxCode.EndsWith("0002")));
        }

        [Fact]
        public async Task ItCanCreateTaxConfigWithRule()
        {
            // Arrange & Act
            await _taxRuleService.CreateTaxConfigWithRuleAsync(
                "PL",
                TaxRule.CreateLinearTaxRule(1, 5, $"A. 899. {DateTime.UtcNow.Year}1001"));

            // Assert
            var taxConfig = await _taxConfigRepository.FindByCountryCodeAsync(CountryCode.Of("PL"));

            Assert.Single(taxConfig.TaxRules.FindAll(taxRule => taxRule.TaxCode.EndsWith("1001")));
            Assert.True(taxConfig.CountryCode.Equals(CountryCode.Of("PL")));
            Assert.Equal(1, taxConfig.CurrentRulesCount);
            Assert.Equal(10, taxConfig.MaxRulesCount);
            Assert.True(taxConfig.LastModifiedDate > DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public async Task ItCanCreateTaxConfigWithRuleAndOwnMaxRulesCount()
        {
            // Arrange & Act
            await _taxRuleService.CreateTaxConfigWithRuleAsync(
                "PL",
                5,
                TaxRule.CreateLinearTaxRule(1, 5, $"A. 899. {DateTime.UtcNow.Year}1001"));

            // Assert
            var taxConfig = await _taxConfigRepository.FindByCountryCodeAsync(CountryCode.Of("PL"));

            Assert.Single(taxConfig.TaxRules.FindAll(taxRule => taxRule.TaxCode.EndsWith("1001")));
            Assert.True(taxConfig.CountryCode.Equals(CountryCode.Of("PL")));
            Assert.Equal(1, taxConfig.CurrentRulesCount);
            Assert.Equal(5, taxConfig.MaxRulesCount);
            Assert.True(taxConfig.LastModifiedDate > DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public async Task ItCanDeleteTaxRule()
        {
            // Arrange
            var taxConfig = await _taxRuleService.CreateTaxConfigWithRuleAsync(
                "PL", 5, TaxRule.CreateLinearTaxRule(1, 5, $"A. 899. {DateTime.UtcNow.Year}1001"));

            await _taxRuleService.AddTaxRuleToCountryAsync("PL", 1, 5, "0002");

            var taxRuleId = taxConfig.TaxRules.FirstOrDefault()!.Id;

            // Act
            await _taxRuleService.DeleteRuleAsync(taxRuleId, taxConfig.Id);

            // Assert
            Assert.Null(await _taxRuleRepository.FindByIdAsync(taxRuleId));
        }

        private readonly TestDb _testDb;
        private readonly TaxRuleService _taxRuleService;
        private readonly TaxRuleRepository _taxRuleRepository;
        private readonly TaxConfigRepository _taxConfigRepository;
        private readonly OrderRepository _orderRepository;

        public TaxRuleServiceTest()
        {
            _testDb = new TestDb();
            var dbContext = _testDb.DbContext;
            _taxRuleRepository = new TaxRuleRepository(dbContext);
            _taxConfigRepository = new TaxConfigRepository(dbContext);
            _orderRepository = new OrderRepository(dbContext);
            _taxRuleService = new TaxRuleService(_taxRuleRepository, _taxConfigRepository, _orderRepository);
        }

        public void Dispose()
        {
            _testDb.Dispose();
        }
    }
}