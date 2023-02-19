using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LegacyFighter.Dietary.Models
{
    public class TaxRuleService
    {
        private readonly ITaxRuleRepository _taxRuleRepository;
        private readonly ITaxConfigRepository _taxConfigRepository;
        private readonly IOrderRepository _orderRepository;

        public TaxRuleService(ITaxRuleRepository taxRuleRepository, ITaxConfigRepository taxConfigRepository,
            IOrderRepository orderRepository)
        {
            _taxRuleRepository = taxRuleRepository;
            _taxConfigRepository = taxConfigRepository;
            _orderRepository = orderRepository;
        }

        public async Task AddTaxRuleToCountryAsync(string countryCode, int aFactor, int bFactor, string taxCode)
        {
            var taxRule = TaxRule.CreateLinearTaxRule(
                aFactor,
                bFactor,
                $"A. 899. {DateTime.UtcNow.Year}{taxCode}");
            
            var taxConfig = await _taxConfigRepository.FindByCountryCodeAsync(CountryCode.Of(countryCode));
            if (taxConfig is null)
            {
                await CreateTaxConfigWithRuleAsync(countryCode, taxRule);
                return;
            }

            taxConfig.AddTaxRule(taxRule);

            var byOrderState = await _orderRepository.FindByOrderStateAsync(Order.OrderState.Initial);

            byOrderState.ForEach(async order =>
            {
                if (order.CustomerOrderGroup.Customer.Type.Equals(Customer.CustomerType.Person))
                {
                    order.TaxRules.Add(taxRule);
                    await _orderRepository.SaveAsync(order);
                }
            });
        }

        public async Task<TaxConfig> CreateTaxConfigWithRuleAsync(string countryCode, TaxRule taxRule)
        {
            var taxConfig = new TaxConfig(CountryCode.Of(countryCode), 10, new List<TaxRule>{taxRule});
            await _taxConfigRepository.SaveAsync(taxConfig);
            return taxConfig;
        }
        
        public async Task<TaxConfig> CreateTaxConfigWithRuleAsync(string countryCode, int maxRulesCount, TaxRule taxRule)
        {
            var taxConfig = new TaxConfig(CountryCode.Of(countryCode), maxRulesCount, new List<TaxRule>{taxRule});
            await _taxConfigRepository.SaveAsync(taxConfig);
            return taxConfig;
        }

        public async Task AddTaxRuleToCountryAsync(string countryCode, int aFactor, int bFactor, int cFactor, string taxCode)
        {
            var taxRule = TaxRule.CreateSquareTaxRule(
                aFactor,
                bFactor,
                cFactor,
                $"A. 899. {DateTime.UtcNow.Year}{taxCode}");
            
            var taxConfig = await _taxConfigRepository.FindByCountryCodeAsync(CountryCode.Of(countryCode));
            if (taxConfig is null)
            {
                await CreateTaxConfigWithRuleAsync(countryCode, taxRule);
                return;
            }
            
            taxConfig.AddTaxRule(taxRule);
        }

        public async Task DeleteRuleAsync(long taxRuleId, long configId)
        {
            var taxRule = await _taxRuleRepository.FindByIdAsync(taxRuleId);
            var taxConfig = await _taxConfigRepository.FindByIdAsync(configId);
            
            taxConfig.RemoveTaxRule(taxRule);
            
            await _taxRuleRepository.DeleteAsync(taxRule);
        }

        public Task<List<TaxConfig>> FindAllConfigsAsync() => _taxConfigRepository.FindAllAsync();
    }
}