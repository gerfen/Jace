using System;
using System.Globalization;

namespace Jace.Execution
{
    public class NullableDoubleNumericalOperations : INumericalOperations<double?>
    {
        public static readonly NullableDoubleNumericalOperations Instance = new NullableDoubleNumericalOperations();
        private DoubleConstants _constants;

        public NullableDoubleNumericalOperations()
        {
            _constants = new DoubleConstants();
        }

        public double? Multiply(double? n1, double? n2)
        {
            return n1 * n2;
        }

        public double? Add(double? n1, double? n2)
        {
            return n1 + n2;
        }

        public double? Subtract(double? n1, double? n2)
        {
            return n1 - n2;
        }

        public double? Modulo(double? n1, double? n2)
        {
            return n1 % n2;
        }

        public double? Divide(double? n1, double? n2)
        {
            return n1 / n2;
        }

        public double? Pow(double? n, double? exponent)
        {
            if (!n.HasValue)
            {
                throw new ArgumentNullException("n");
            }

            if (!exponent.HasValue)
            {
                throw new ArgumentNullException("exponent");
            }
            return Math.Pow(n.Value, exponent.Value);
        }

        public double? Negate(double? n)
        {
            return -n;
        }

        public double? LessThan(double? n1, double? n2)
        {
            return n1 < n2 ? 1.0 : 0.0;
        }

        public double? LessOrEqualThan(double? n1, double? n2)
        {
            return n1 <= n2 ? 1.0 : 0.0;
        }

        public double? GreaterThan(double? n1, double? n2)
        {
            return n1 > n2 ? 1.0 : 0.0;
        }

        public double? GreaterOrEqualThan(double? n1, double? n2)
        {
            return n1 >= n2 ? 1.0 : 0.0;
        }

        public double? Equal(double? n1, double? n2)
        {
            return n1 == n2 ? 1.0 : 0.0;
        }

        public double? NotEqual(double? n1, double? n2)
        {
            return n1 != n2 ? 1.0 : 0.0;
        }

        public bool TryParseFloatingPoint(string str, CultureInfo cultureInfo, out double? numericalValue)
        {
            double value;
            var parsed = double.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, cultureInfo, out value);
            numericalValue = value;
            return parsed;
        }

        private class DoubleConstants : INumericConstants<double?>
        {
            public double? Zero
            {
                get { return 0; }
            }

            public double? One
            {
                get { return 1; }
            }
        }

        public INumericConstants<double?> Constants
        {
            get { return _constants; }
        }

        public double? ConvertFromInt32(int i)
        {
            return i;
        }
    }
}