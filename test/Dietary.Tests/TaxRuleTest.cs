using System;
using LegacyFighter.Dietary.Models;
using Xunit;

namespace LegacyFighter.Dietary.Tests
{
    public sealed class TaxRuleTest
    {
        [Fact]
        public void ItCanCreateLinearTaxRule()
        {
            // Arrange & Act
            var linearTaxRule = TaxRule.CreateLinearTaxRule(2, 5, "1001");

            // Assert
            Assert.Equal(2, linearTaxRule.AFactor);
            Assert.Equal(5, linearTaxRule.BFactor);
            Assert.Null(linearTaxRule.CFactor);
            Assert.True(linearTaxRule.IsLinear);
            Assert.False(linearTaxRule.IsSquare);
            Assert.Equal("1001", linearTaxRule.TaxCode);
        }
        
        [Fact]
        public void ItCanCreateSquareTaxRule()
        {
            // Arrange & Act
            var linearTaxRule = TaxRule.CreateSquareTaxRule(2, 5, 2, "1002");

            // Assert
            Assert.Equal(2, linearTaxRule.AFactor);
            Assert.Equal(5, linearTaxRule.BFactor);
            Assert.Equal(2, linearTaxRule.CFactor);
            Assert.False(linearTaxRule.IsLinear);
            Assert.True(linearTaxRule.IsSquare);
            Assert.Equal("1002", linearTaxRule.TaxCode);
        }

        [Fact]
        public void ItCannotBeCreatedWhenAFactorIsZero()
        {
            // Act && Assert
            Assert.Throws<InvalidOperationException>(() =>
                TaxRule.CreateSquareTaxRule(0, 5, 2, "1002"));
        }
    }
}