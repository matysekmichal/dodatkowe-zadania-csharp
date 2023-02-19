using System;
using System.Collections.Generic;

namespace LegacyFighter.Dietary.Models
{
    public class TaxConfig
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public string CountryReason { get; set; }
        public CountryCode CountryCode { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public int CurrentRulesCount { get; set; }
        public int MaxRulesCount { get; set; }
        public List<TaxRule> TaxRules { get; set; }

        public TaxConfig()
        {
            TaxRules = new List<TaxRule>();
        }
        
        public TaxConfig(CountryCode countryCode, int maxRulesCount, List<TaxRule> taxRules)
        {
            if (taxRules.Count < 1)
            {
                throw new InvalidOperationException("Tax configuration must have at least one tax rule");
            }

            if (taxRules.Count > maxRulesCount)
            {
                throw new InvalidOperationException("Exceeded number of tax rules");
            }
            
            CountryCode = countryCode;
            MaxRulesCount = maxRulesCount;
            CurrentRulesCount = taxRules.Count;
            TaxRules = taxRules;
            LastModifiedDate = DateTime.UtcNow;
        }

        public void AddTaxRule(TaxRule taxRule)
        {
            if (TaxRules.Count >= MaxRulesCount)
            {
                throw new InvalidOperationException("Tax rule cannot be added due to too many tax rules");
            }
            
            TaxRules.Add(taxRule);
            CurrentRulesCount++;
            LastModifiedDate = DateTime.UtcNow;
        }

        public void RemoveTaxRule(TaxRule taxRule)
        {
            if (TaxRules.Count == 1)
            {
                throw new InvalidOperationException("Tax configuration must have at least one tax rule");
            }
            
            if (!TaxRules.Remove(taxRule))
            {
                throw new InvalidOperationException("Tax rule does not exists in this tax configuration");
            }
            
            CurrentRulesCount--;
            LastModifiedDate = DateTime.UtcNow;
        }
    }
}