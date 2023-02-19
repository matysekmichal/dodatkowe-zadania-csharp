using System;
using System.Collections.Generic;
using LegacyFighter.Dietary.Models;
using Xunit;

namespace LegacyFighter.Dietary.Tests
{
    public sealed class TaxConfigTest
    {
        [Fact]
        public void ItCanBeCreated()
        {
            // Act
            var taxConfig = new TaxConfig(
                CountryCode.Of("PL"),
                5,
                new List<TaxRule> { TaxRule.CreateLinearTaxRule(2, 5, "1001") });

            // Assert
            Assert.True(taxConfig.CountryCode.Equals(CountryCode.Of("PL")));
            Assert.Equal(5, taxConfig.MaxRulesCount);
            Assert.Equal(1, taxConfig.CurrentRulesCount);
            Assert.Single(taxConfig.TaxRules);
            Assert.True(taxConfig.LastModifiedDate > DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public void ItCannotBeCreatedWhenNotProvidedTaxRule()
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new TaxConfig(
                CountryCode.Of("PL"),
                5,
                new List<TaxRule>()));
        }

        [Fact]
        public void ItCannotBeCreatedWhenTaxRulesIsMoreThanMaxRules()
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new TaxConfig(
                CountryCode.Of("PL"),
                0,
                new List<TaxRule> { TaxRule.CreateLinearTaxRule(2, 5, "1001") }));
        }

        [Fact]
        public void ItCanAddTaxRule()
        {
            // Arrange
            var taxConfig = new TaxConfig(
                CountryCode.Of("PL"),
                5,
                new List<TaxRule> { TaxRule.CreateLinearTaxRule(2, 5, "1001") });
            var taxConfigLastModifiedDate = taxConfig.LastModifiedDate;

            // Act
            taxConfig.AddTaxRule(TaxRule.CreateLinearTaxRule(3, 4, "1002"));

            // Assert
            Assert.Equal(2, taxConfig.TaxRules.Count);
            Assert.Equal(2, taxConfig.CurrentRulesCount);
            Assert.NotEqual(taxConfigLastModifiedDate, taxConfig.LastModifiedDate);
        }

        [Fact]
        public void ItCannotAddTaxRuleWhenRulesNumberWillExceedLimit()
        {
            // Arrange
            var taxConfig = new TaxConfig(
                CountryCode.Of("PL"),
                1,
                new List<TaxRule> { TaxRule.CreateLinearTaxRule(2, 5, "1001") });

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                taxConfig.AddTaxRule(TaxRule.CreateLinearTaxRule(3, 4, "1002")));
        }

        [Fact]
        public void ItCanRemoveTaxRule()
        {
            // Arrange
            var taxRuleToDelete = TaxRule.CreateSquareTaxRule(2, 5, 2, "1002");
            var taxConfig = new TaxConfig(
                CountryCode.Of("PL"),
                5,
                new List<TaxRule>
                {
                    TaxRule.CreateLinearTaxRule(2, 5, "1001"),
                    taxRuleToDelete
                });
            var taxConfigLastModifiedDate = taxConfig.LastModifiedDate;

            // Act
            taxConfig.RemoveTaxRule(taxRuleToDelete);

            // Assert
            Assert.Single(taxConfig.TaxRules);
            Assert.Equal("1001", taxConfig.TaxRules.Find(x => x.TaxCode.EndsWith("1001")).TaxCode);
            Assert.Equal(1, taxConfig.CurrentRulesCount);
            Assert.NotEqual(taxConfigLastModifiedDate, taxConfig.LastModifiedDate);
        }

        [Fact]
        public void ItCannotRemoveTaxRuleWhenItIsTheLast()
        {
            // Arrange
            var taxRuleToDelete = TaxRule.CreateLinearTaxRule(2, 5, "1001");
            var taxConfig = new TaxConfig(
                CountryCode.Of("PL"),
                1,
                new List<TaxRule> { taxRuleToDelete });

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                taxConfig.RemoveTaxRule(taxRuleToDelete));
        }

        [Fact]
        public void ItCannotRemoveTaxRuleWhenRuleDoesNotExsit()
        {
            // Arrange
            var taxRuleToDelete = TaxRule.CreateLinearTaxRule(2, 5, "1111");
            var taxConfig = new TaxConfig(
                CountryCode.Of("PL"),
                5,
                new List<TaxRule>
                {
                    TaxRule.CreateLinearTaxRule(2, 5, "1001"),
                    TaxRule.CreateSquareTaxRule(2, 5, 2, "1002")
                });

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                taxConfig.RemoveTaxRule(taxRuleToDelete));
        }
    }
}