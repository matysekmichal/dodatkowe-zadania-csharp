using System;

namespace LegacyFighter.Dietary.Models
{
    public sealed class CountryCode
    {
        public long Id { get; private set; } = 1;
        public readonly string Value;

        private CountryCode()
        {
        }

        private CountryCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length == 1)
            {
                throw new InvalidOperationException("Invalid country code");
            }

            Value = value;
        }

        public static CountryCode Of(string countryCode)
        {
            return new CountryCode(countryCode);
        }

        public bool Equals(CountryCode countryCode)
        {
            if (ReferenceEquals(null, countryCode)) return false;
            if (ReferenceEquals(this, countryCode)) return true;
            return countryCode.Value == Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj);
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}