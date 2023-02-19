using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LegacyFighter.Dietary.Models
{
    [Route("")]
    public class TaxConfigController : ControllerBase
    {
        private readonly TaxRuleService _taxRuleService;

        public TaxConfigController(TaxRuleService taxRuleService)
        {
            _taxRuleService = taxRuleService;
        }

        [HttpGet("config")]
        public async Task<ActionResult<Dictionary<string, List<TaxRule>>>> TaxConfigs()
        {
            var taxConfigs = await _taxRuleService.FindAllConfigsAsync();
            var map = new Dictionary<string, List<TaxRule>>();
            foreach (var tax in taxConfigs)
            {
                if (!map.ContainsKey(tax.CountryCode.Value))
                {
                    if (tax.TaxRules is null)
                    {
                        tax.TaxRules = new List<TaxRule>();
                    }

                    map.Add(tax.CountryCode.Value, tax.TaxRules);
                }
                else
                {
                    map[tax.CountryCode.Value].AddRange(tax.TaxRules);
                }
            }
            
            return Ok(map);
        }
    }
}