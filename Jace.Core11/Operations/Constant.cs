
namespace Jace.Operations
{
    public abstract class Constant<T> : Operation
    {
        protected Constant(DataType dataType, T value)
            : base(dataType, false)
        {
            Value = value;
        }

        public T Value { get; private set; }

        public override bool Equals(object obj)
        {
            var other = obj as Constant<T>;
            if (other != null)
            {
                return Value.Equals(other.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }

    public class IntegerConstant : Constant<int>
    {
        public IntegerConstant(int value)
            : base(DataType.Integer, value)
        {
        }
    }

    public class FloatingPointConstant<T> : Constant<T>
    {
        public FloatingPointConstant(T value)
            : base(DataType.FloatingPoint, value)
        {
        }
    }
}
