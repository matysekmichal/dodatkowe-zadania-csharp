using System;

namespace LegacyFighter.Dietary.Models
{
    public class TaxRule
    {
        public long Id { get; set; }
        public string TaxCode { get; set; }
        public bool IsLinear { get; set; }
        public int AFactor { get; set; }
        public int BFactor { get; set; }
        public int? CFactor { get; set; }
        public bool IsSquare { get; set; }
        public long TaxConfigId { get; set; }
        public TaxConfig TaxConfig { get; set; }

        private TaxRule(int aFactor, int bFactor, int? cFactor, string taxCode)
        {
            if (aFactor == 0)
            {
                throw new InvalidOperationException("Invalid aFactor");
            }

            AFactor = aFactor;
            BFactor = bFactor;
            CFactor = cFactor;
            IsLinear = cFactor == null;
            IsSquare = cFactor != null;
            TaxCode = taxCode;
        }

        public static TaxRule CreateLinearTaxRule(int aFactor, int bFactor, string taxCode)
        {
            return new TaxRule(aFactor, bFactor, null, taxCode);
        }

        public static TaxRule CreateSquareTaxRule(int aFactor, int bFactor, int cFactor, string taxCode)
        {
            return new TaxRule(aFactor, bFactor, cFactor, taxCode);
        }
    }
}