using System;
using LegacyFighter.Dietary.Models;
using Xunit;

namespace LegacyFighter.Dietary.Tests
{
    public sealed class CountryCodeTest
    {
        [Fact]
        public void ItCanBeCreated()
        {
            // Arrange & Act
            var countryCode = CountryCode.Of("PL");
            
            // Assert
            Assert.Equal("PL", countryCode.Value);
            Assert.True(CountryCode.Of("PL").Equals(countryCode));
        }
        
        [Fact]
        public void ItCannotBeCreatedWhenValueHasLessThanTwoCharacters()
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                CountryCode.Of("P"));
        }
        
        [Fact]
        public void ItCannotBeCreatedWhenValueIsWhiteSpaces()
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                CountryCode.Of("  "));
        }
    }
}