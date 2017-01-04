using System.Globalization;

namespace Jace.Execution
{
    public interface INumericalOperations<T>
    {
        T Multiply(T n1, T n2);
        T Add(T n1, T n2);
        T Subtract(T n1, T n2);
        T Modulo(T n1, T n2);
        T Divide(T n1, T n2);
        T Pow(T n, T exponent);
        T Negate(T n);
        T LessThan(T n1, T n2);
        T LessOrEqualThan(T n1, T n2);
        T GreaterThan(T n1, T n2);
        T GreaterOrEqualThan(T n1, T n2);
        T Equal(T n1, T n2);
        T NotEqual(T n1, T n2);

        bool TryParseFloatingPoint(string str, CultureInfo cultureInfo, out T numericalValue);
        INumericConstants<T> Constants { get; }
        T ConvertFromInt32(int i);
    }



   public interface INumericConstants<T>

    {
        T Zero { get; }
        T One { get; }
    }
}