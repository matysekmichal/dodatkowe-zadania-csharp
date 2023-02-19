namespace LegacyFighter.Dietary.Models
{
    public class TaxRuleDto
    {
        public string FormattedTaxCode { get; set; }
        public bool IsLinear { get; set; }
        public int AFactor { get; set; }
        public int BFactor { get; set; }
        public int? CFactor { get; set; }
        public bool IsSquare { get; set; }

        public TaxRuleDto(TaxRule taxRule)
        {
            FormattedTaxCode = $" informal 671 {taxRule.TaxCode}  *** ";
            AFactor = taxRule.AFactor;
            BFactor = taxRule.BFactor;
            CFactor = taxRule.CFactor;
            IsLinear = taxRule.IsLinear;
            IsSquare = taxRule.IsSquare;
        }
    }
}