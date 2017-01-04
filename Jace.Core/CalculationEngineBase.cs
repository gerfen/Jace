using System;
using System.Collections.Generic;
using System.Globalization;
using Jace.Execution;
using Jace.Operations;
using Jace.Tokenizer;
using Jace.Util;

namespace Jace
{
    public abstract class CalculationEngineBase<T> : ICalculationEngine<T>
    {
        private readonly IExecutor<T> executor;
        private readonly Optimizer<T> optimizer;
        private readonly CultureInfo cultureInfo;
        private readonly MemoryCache<string, Func<IDictionary<string, T>, T>> executionFormulaCache;
        private readonly bool cacheEnabled;
        private readonly bool optimizerEnabled;
        private readonly INumericalOperations<T> numericalOperations;

        public IFunctionRegistry FunctionRegistry { get; protected set; }

        public IConstantRegistry<T> ConstantRegistry { get; protected set; }
        protected CalculationEngineBase
        (
            CultureInfo cultureInfo,
            bool cacheEnabled,
            bool optimizerEnabled,
            IExecutor<T> executor,
            Optimizer<T> optimizer,
            INumericalOperations<T> numericalOperations
        )
        {
            this.executionFormulaCache = new MemoryCache<string, Func<IDictionary<string, T>, T>>();

            this.FunctionRegistry = new FunctionRegistry<T>(false);
            this.ConstantRegistry = new ConstantRegistry<T>(false);
            this.cultureInfo = cultureInfo;
            this.cacheEnabled = cacheEnabled;
            this.optimizerEnabled = optimizerEnabled;
            this.executor = executor;
            this.optimizer = optimizer;
            this.numericalOperations = numericalOperations;
        }

        public T Calculate(string formulaText)
        {
            return Calculate(formulaText, new Dictionary<string, T>());
        }

        public T Calculate(string formulaText, IDictionary<string, T> variables)
        {
            if (string.IsNullOrEmpty(formulaText))
                throw new ArgumentNullException("formulaText");

            if (variables == null)
                throw new ArgumentNullException("variables");

            variables = EngineUtil.ConvertVariableNamesToLowerCase(variables);
            VerifyVariableNames(variables);

            // Add the reserved variables to the dictionary
            foreach (var constant in ConstantRegistry)
                variables.Add(constant.ConstantName, constant.Value);

            if (IsInFormulaCache(formulaText))
            {
                var formula = executionFormulaCache[formulaText];
                return formula(variables);
            }
            else
            {
                Operation operation = BuildAbstractSyntaxTree(formulaText);
                var function = BuildFormula(formulaText, operation);

                return function(variables);
            }
        }

        public FormulaBuilder<T> Formula(string formulaText)
        {
            if (string.IsNullOrEmpty(formulaText))
                throw new ArgumentNullException("formulaText");
            return new FormulaBuilder<T>(formulaText, (ICalculationEngine<T>)this);
        }

        /// <summary>
        /// Build a .NET func for the provided formula.
        /// </summary>
        /// <param name="formulaText">The formula that must be converted into a .NET func.</param>
        /// <returns>A .NET func for the provided formula.</returns>

        public Func<Dictionary<string, T>, T> Build(string formulaText)
        {
            if (string.IsNullOrEmpty(formulaText))
                throw new ArgumentNullException("formulaText");

            if (IsInFormulaCache(formulaText))
            {
                return executionFormulaCache[formulaText];
            }
            else
            {
                Operation operation = BuildAbstractSyntaxTree(formulaText);
                return BuildFormula(formulaText, operation);
            }
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implementation of the function.</param>
        public void AddFunction(string functionName, Func<T> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implementation of the function.</param>
        public void AddFunction(string functionName, Func<T, T> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implementation of the function.</param>
        public void AddFunction(string functionName, Func<T, T, T> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implementation of the function.</param>
        public void AddFunction(string functionName, Func<T, T, T, T> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implementation of the function.</param>
        public void AddFunction(string functionName, Func<T, T, T, T, T> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

#if !WINDOWS_PHONE_7
        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implementation of the function.</param>
        public void AddFunction(string functionName, Func<T, T, T, T, T, T> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implementation of the function.</param>
        public void AddFunction(string functionName, Func<T, T, T, T, T, T, T> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

#endif

        /// <summary>
        /// Add a constant to the calculation engine.
        /// </summary>
        /// <param name="constantName">The name of the constant. This name can be used in mathematical formulas.</param>
        /// <param name="value">The value of the constant.</param>
        public void AddConstant(string constantName, T value)
        {
            ConstantRegistry.RegisterConstant(constantName, value);
        }

        /// <summary>
        /// Build the abstract syntax tree for a given formula. The formula string will
        /// be first tokenized.
        /// </summary>
        /// <param name="formulaText">A string containing the mathematical formula that must be converted 
        /// into an abstract syntax tree.</param>
        /// <returns>The abstract syntax tree of the formula.</returns>
        private Operation BuildAbstractSyntaxTree(string formulaText)
        {
            var tokenReader = new TokenReader<T>(cultureInfo, this.numericalOperations);
            List<Token> tokens = tokenReader.Read(formulaText);

            var astBuilder = new AstBuilder<T>(FunctionRegistry);
            Operation operation = astBuilder.Build(tokens);

            if (optimizerEnabled)
                return optimizer.Optimize(operation, this.FunctionRegistry);
            else
                return operation;
        }

        private Func<IDictionary<string, T>, T> BuildFormula(string formulaText, Operation operation)
        {
            return executionFormulaCache.GetOrAdd(formulaText, v => executor.BuildFormula(operation, this.FunctionRegistry));
        }

        private bool IsInFormulaCache(string formulaText)
        {
            return cacheEnabled && executionFormulaCache.ContainsKey(formulaText);
        }

        /// <summary>
        /// Verify a collection of variables to ensure that all the variable names are valid.
        /// Users are not allowed to overwrite reserved variables or use function names as variables.
        /// If an invalid variable is detected an exception is thrown.
        /// </summary>
        /// <param name="variables">The colletion of variables that must be verified.</param>
        private void VerifyVariableNames(IDictionary<string, T> variables)
        {
            foreach (string variableName in variables.Keys)
            {
                if (ConstantRegistry.IsConstantName(variableName) && !ConstantRegistry.GetConstantInfo(variableName).IsOverWritable)
                    throw new ArgumentException(string.Format("The name \"{0}\" is a reservered variable name that cannot be overwritten.", variableName), "variables");

                if (FunctionRegistry.IsFunctionName(variableName))
                    throw new ArgumentException(string.Format("The name \"{0}\" is a function name. Parameters cannot have this name.", variableName), "variables");
            }
        }
    }
}