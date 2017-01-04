using System;
using System.Globalization;

namespace Jace.Execution
{
    public class DecimalNumericalOperations : INumericalOperations<decimal>

    {
        public static readonly DecimalNumericalOperations Instance = new DecimalNumericalOperations();
        private DecimalNumericalOperations.DecimalConstants _constants;
        private DecimalNumericalOperations()
        {
            _constants = new DecimalConstants();
        }

        public decimal Multiply(decimal n1, decimal n2)
        {
            return n1 * n2;
        }

        public decimal Add(decimal n1, decimal n2)
        {
            return n1 + n2;
        }

        public decimal Subtract(decimal n1, decimal n2)
        {
            return n1 - n2;
        }

        public decimal Modulo(decimal n1, decimal n2)
        {
            return n1 % n2;
        }

        public decimal Divide(decimal n1, decimal n2)
        {
            return n1 / n2;
        }

        public decimal Pow(decimal n, decimal exponent)
        {
            return (decimal)Math.Pow((double)n, (double)exponent);
        }

        public decimal Negate(decimal n)
        {
            return -n;
        }

        public decimal LessThan(decimal n1, decimal n2)
        {
            return n1 < n2 ? 1.0m : 0.0m;
        }

        public decimal LessOrEqualThan(decimal n1, decimal n2)
        {
            return n1 <= n2 ? 1.0m : 0.0m;
        }

        public decimal GreaterThan(decimal n1, decimal n2)
        {
            return n1 > n2 ? 1.0m : 0.0m;
        }

        public decimal GreaterOrEqualThan(decimal n1, decimal n2)
        {
            return n1 >= n2 ? 1.0m : 0.0m;
        }

        public decimal Equal(decimal n1, decimal n2)
        {
            return n1 == n2 ? 1.0m : 0.0m;
        }

        public decimal NotEqual(decimal n1, decimal n2)
        {
            return n1 != n2 ? 1.0m : 0.0m;
        }

        public bool TryParseFloatingPoint(string str, CultureInfo cultureInfo, out decimal numericalValue)
        {
            return decimal.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, cultureInfo, out numericalValue);
        }

        private class DecimalConstants : INumericConstants<decimal>
        {
            public decimal Zero
            {
                get { return 0; }
            }

            public decimal One
            {
                get { return 1; }
            }
        }

        public INumericConstants<decimal> Constants
        {
            get { return _constants; }
        }

        public decimal ConvertFromInt32(int i)
        {
            return i;
        }
    }
}