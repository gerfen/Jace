using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jace.Execution
{
    public class ConstantRegistry : ConstantRegistry<double>
    {
        public ConstantRegistry(bool caseSensitive) : base(caseSensitive)
        {

        }
    }


    public class ConstantRegistry<T> : IConstantRegistry<T>
    {
        private readonly bool _caseSensitive;
        private readonly Dictionary<string, ConstantInfo<T>> _constants;

        public ConstantRegistry(bool caseSensitive)
        {
            _caseSensitive = caseSensitive;
            _constants = new Dictionary<string, ConstantInfo<T>>();

        }

        public IEnumerator<ConstantInfo<T>> GetEnumerator()
        {
            return _constants.Select(p => p.Value).ToList().GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public ConstantInfo<T> GetConstantInfo(string constantName)
        {
            if (string.IsNullOrEmpty(constantName))
            {
                throw new ArgumentNullException("constantName");
            }

            ConstantInfo<T> constantInfo = null;
            return _constants.TryGetValue(ConvertConstantName(constantName), out constantInfo) ? constantInfo : null;
        }

        public bool IsConstantName(string constantName)
        {
            if (string.IsNullOrEmpty(constantName))
            {
                throw new ArgumentNullException("constantName");
            }

            return _constants.ContainsKey(ConvertConstantName(constantName));
        }

        public void RegisterConstant(string constantName, T value)
        {
            RegisterConstant(constantName, value, true);
        }

        public void RegisterConstant(string constantName, T value, bool isOverWritable)
        {
            if (string.IsNullOrEmpty(constantName))
            {
                throw new ArgumentNullException("constantName");
            }

            constantName = ConvertConstantName(constantName);

            if (_constants.ContainsKey(constantName) && !_constants[constantName].IsOverWritable)
            {
                string message = string.Format("The constant \"{0}\" cannot be overwriten.", constantName);
                throw new Exception(message);
            }

            var constantInfo = new ConstantInfo<T>(constantName, value, isOverWritable);

            if (_constants.ContainsKey(constantName))
            {
                _constants[constantName] = constantInfo;
            }
            else
            {
                _constants.Add(constantName, constantInfo);
            }
                
        }

        private string ConvertConstantName(string constantName)
        {
            return _caseSensitive ? constantName : constantName.ToLowerInvariant();
        }
    }
}
