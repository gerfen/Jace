using System;
using System.Globalization;
using Jace.Execution;
using Jace.Util;

namespace Jace
{
    public class NullableDoubleCalculationEngine : CalculationEngineBase<double?>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="NullableDoubleCalculationEngine"/> class with
        /// default parameters.
        /// </summary>
        public NullableDoubleCalculationEngine()
            : this(CultureInfo.CurrentCulture, ExecutionMode.Compiled)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="NullableDoubleCalculationEngine"/> class. The dynamic compiler
        /// is used for formula execution and the optimizer and cache are enabled.
        /// </summary>
        /// <param name="cultureInfo">
        /// The <see cref="CultureInfo"/> required for correctly reading floating point numbers.
        /// </param>
        public NullableDoubleCalculationEngine(CultureInfo cultureInfo)
            : this(cultureInfo, ExecutionMode.Compiled)
        {

        }


        /// <summary>
        /// Creates a new instance of the <see cref="NullableDoubleCalculationEngine"/> class. The optimizer and 
        /// cache are enabled.
        /// </summary>
        /// <param name="cultureInfo">
        /// The <see cref="CultureInfo"/> required for correctly reading floating point numbers.
        /// </param>
        /// <param name="executionMode">The execution mode that must be used for formula execution.</param>
        public NullableDoubleCalculationEngine(CultureInfo cultureInfo, ExecutionMode executionMode)
            : this(cultureInfo, executionMode, true, true)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="CalculationEngine"/> class.
        /// </summary>
        /// <param name="cultureInfo">
        /// The <see cref="CultureInfo"/> required for correctly reading floating point numbers.
        /// </param>
        /// <param name="executionMode">The execution mode that must be used for formula execution.</param>
        /// <param name="cacheEnabled">Enable or disable caching of mathematical formulas.</param>
        /// <param name="optimizerEnabled">Enable or disable optimizing of formulas.</param>
        public NullableDoubleCalculationEngine(CultureInfo cultureInfo, ExecutionMode executionMode, bool cacheEnabled, bool optimizerEnabled) :
            base(cultureInfo, cacheEnabled, optimizerEnabled,
                CreateExecutor(executionMode),
                new Optimizer<double?>(new Interpreter<double?>(NullableDoubleNumericalOperations.Instance)), NullableDoubleNumericalOperations.Instance)
        {
            // Register the default constants of Jace.NET into the constant registry
            RegisterDefaultConstants();
            // Register the default functions of Jace.NET into the function registry
            RegisterDefaultFunctions();
        }

        private static IExecutor<double?> CreateExecutor(ExecutionMode executionMode)
        {

            switch (executionMode)
            {
                case ExecutionMode.Interpreted:
                    return new Interpreter<double?>(NullableDoubleNumericalOperations.Instance);
                case ExecutionMode.Compiled:
                    return new ExpressionExecutor<double?>((variables, functionRegistry) => new FormulaContext<double?>(variables, functionRegistry), NullableDoubleNumericalOperations.Instance);
            }

            //            else if (executionMode == ExecutionMode.Compiled)
            //            {
            //#if !NETFX_CORE && !NETCORE
            //                return new NullableDoubleDynamicCompiler();
            //#else
            //                return new ExpressionExecutor<double?>((variables, functionRegistry) => new FormulaContext<double?>(variables, functionRegistry), NullableDoubleNumericalOperations.Instance);
            //#endif
            //            }

            throw new ArgumentException(string.Format("Unsupported execution mode \"{0}\".", executionMode), "executionMode");

        }

        private void RegisterDefaultFunctions()
        {
            FunctionRegistry.RegisterFunction("sin", (Func<double?, double?>)((a) => Math.Sin(a.Value)), false);
            FunctionRegistry.RegisterFunction("cos", (Func<double?, double?>)((a) => Math.Cos(a.Value)), false);
            FunctionRegistry.RegisterFunction("csc", (Func<double?, double?>)((a) => MathUtil.Csc(a.Value)), false);
            FunctionRegistry.RegisterFunction("sec", (Func<double?, double?>)((a) => MathUtil.Sec(a.Value)), false);
            FunctionRegistry.RegisterFunction("asin", (Func<double?, double?>)((a) => Math.Asin(a.Value)), false);
            FunctionRegistry.RegisterFunction("acos", (Func<double?, double?>)((a) => Math.Acos(a.Value)), false);
            FunctionRegistry.RegisterFunction("tan", (Func<double?, double?>)((a) => Math.Tan(a.Value)), false);
            FunctionRegistry.RegisterFunction("cot", (Func<double?, double?>)((a) => MathUtil.Cot(a.Value)), false);
            FunctionRegistry.RegisterFunction("atan", (Func<double?, double?>)((a) => Math.Atan(a.Value)), false);
            FunctionRegistry.RegisterFunction("acot", (Func<double?, double?>)((a) => MathUtil.Acot(a.Value)), false);
            FunctionRegistry.RegisterFunction("loge", (Func<double?, double?>)((a) => Math.Log(a.Value)), false);
            FunctionRegistry.RegisterFunction("log10", (Func<double?, double?>)((a) => Math.Log10(a.Value)), false);
            FunctionRegistry.RegisterFunction("logn", (Func<double?, double?, double?>)((a, b) => Math.Log(a.Value, b.Value)), false);
            FunctionRegistry.RegisterFunction("sqrt", (Func<double?, double?>)((a) => Math.Sqrt(a.Value)), false);
            FunctionRegistry.RegisterFunction("abs", (Func<double?, double?>)((a) => Math.Abs(a.Value)), false);
            FunctionRegistry.RegisterFunction("max", (Func<double?, double?, double?>)((a, b) => Math.Max(a.Value, b.Value)), false);
            FunctionRegistry.RegisterFunction("min", (Func<double?, double?, double?>)((a, b) => Math.Min(a.Value, b.Value)), false);
            FunctionRegistry.RegisterFunction("if", (Func<double?, double?, double?, double?>)((a, b, c) => (a != 0.0 ? b : c)), false);
            FunctionRegistry.RegisterFunction("ifless", (Func<double?, double?, double?, double?, double?>)((a, b, c, d) => (a < b ? c : d)), false);
            FunctionRegistry.RegisterFunction("ifmore", (Func<double?, double?, double?, double?, double?>)((a, b, c, d) => (a > b ? c : d)), false);
            FunctionRegistry.RegisterFunction("ifequal", (Func<double?, double?, double?, double?, double?>)((a, b, c, d) => (a == b ? c : d)), false);
            FunctionRegistry.RegisterFunction("ceiling", (Func<double?, double?>)((a) => Math.Ceiling(a.Value)), false);
            FunctionRegistry.RegisterFunction("floor", (Func<double?, double?>)((a) => Math.Floor(a.Value)), false);
#if !WINDOWS_PHONE_7
            FunctionRegistry.RegisterFunction("truncate", (Func<double?, double?>)((a) => Math.Truncate(a.Value)), false);
#endif
        }

        private void RegisterDefaultConstants()
        {
            ConstantRegistry.RegisterConstant("e", Math.E, false);
            ConstantRegistry.RegisterConstant("pi", Math.PI, false);
        }
    }
}