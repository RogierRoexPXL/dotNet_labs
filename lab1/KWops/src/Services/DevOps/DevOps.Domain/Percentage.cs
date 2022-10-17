using Domain;
using System;
using System.Globalization;

namespace DevOps.Domain
{
    public class Percentage : ValueObject<Percentage>
    {
        private readonly double _value;

        //p.Value made for accessing value in DeveloperConfiguration (DevOps.Infra), is this wrong? 
        //public double Value { get { return _value; } }

        public Percentage(double value)
        {
            Contracts.Require(value <= 1.0 && value >= 0.0, "Percentage must be between 0 and 100!");
            _value = value;
        }
        public Percentage(string value)
        {
            double numberValue = Double.Parse(value.Replace("%", "")) / 100;
            Contracts.Require(numberValue <= 1.0 && numberValue >= 0.0, "Percentage must be between 0 and 100!");
            _value = numberValue;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _value;
        }

        public override string ToString()
        {
            return _value.ToString("0.##%", CultureInfo.CreateSpecificCulture("fr-FR"));
            //return $"{_value:}";
        }

        public static implicit operator double(Percentage number) => number._value;
        public static implicit operator Percentage(double value) => new Percentage(value);

        public static implicit operator string(Percentage number) => number.ToString();
        public static implicit operator Percentage(string value) => new Percentage(value);
    }
}
