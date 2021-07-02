using System;
using System.Collections.Generic;
using Jace.Execution;

namespace Jace
{
    

    public class FormulaContext<T>
    {
        public FormulaContext(IDictionary<string, T> variables,
            IFunctionRegistry functionRegistry)
        {
            this.Variables = variables;
            this.FunctionRegistry = functionRegistry;
        }

        public IDictionary<string, T> Variables { get; private set; }

        public IFunctionRegistry FunctionRegistry { get; private set; }
    }
}
