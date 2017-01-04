using System;
using System.Globalization;
using Jace.Execution;
using Jace.Util;

namespace Jace
{
    /// <summary>
    /// The CalculationEngine class is the main class of Jace.NET to convert strings containing
    /// mathematical formulas into .NET Delegates and to calculate the result.
    /// It can be configured to run in a number of modes based on the constructor parameters chosen.
    /// </summary>
    public class CalculationEngine : CalculationEngineBase<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CalculationEngine"/> class with
        /// default parameters.
        /// </summary>
        public CalculationEngine()
            : this(CultureInfo.CurrentCulture, ExecutionMode.Compiled)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="CalculationEngine"/> class. The dynamic compiler
        /// is used for formula execution and the optimizer and cache are enabled.
        /// </summary>
        /// <param name="cultureInfo">
        /// The <see cref="CultureInfo"/> required for correctly reading floating point numbers.
        /// </param>
        public CalculationEngine(CultureInfo cultureInfo)
            : this(cultureInfo, ExecutionMode.Compiled)
        {

        }


        /// <summary>
        /// Creates a new instance of the <see cref="CalculationEngine"/> class. The optimizer and 
        /// cache are enabled.
        /// </summary>
        /// <param name="cultureInfo">
        /// The <see cref="CultureInfo"/> required for correctly reading floating point numbers.
        /// </param>
        /// <param name="executionMode">The execution mode that must be used for formula execution.</param>
        public CalculationEngine(CultureInfo cultureInfo, ExecutionMode executionMode)
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
        public CalculationEngine(CultureInfo cultureInfo, ExecutionMode executionMode, bool cacheEnabled, bool optimizerEnabled) :
            base(cultureInfo, cacheEnabled, optimizerEnabled,
            CreateExecutor(executionMode),
            new Optimizer<double>(new Interpreter()), DoubleNumericalOperations.Instance)
        {
            // Register the default constants of Jace.NET into the constant registry
            RegisterDefaultConstants();
            // Register the default functions of Jace.NET into the function registry
            RegisterDefaultFunctions();
        }

        private static IExecutor<double> CreateExecutor(ExecutionMode executionMode)
        {
            if (executionMode == ExecutionMode.Interpreted)
            {
                return new Interpreter();
            }
            else if (executionMode == ExecutionMode.Compiled)
            {
#if !NETFX_CORE && !NETCORE
                return new DynamicCompiler();
#else
                return new ExpressionExecutor<double>((variables, functionRegistry) => new FormulaContext<double>(variables, functionRegistry), DoubleNumericalOperations.Instance);
#endif
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported execution mode \"{0}\".", executionMode),
                    "executionMode");
            }
        }

        private void RegisterDefaultFunctions()
        {
            FunctionRegistry.RegisterFunction("sin", (Func<double, double>)((a) => Math.Sin(a)), false);
            FunctionRegistry.RegisterFunction("cos", (Func<double, double>)((a) => Math.Cos(a)), false);
            FunctionRegistry.RegisterFunction("csc", (Func<double, double>)((a) => MathUtil.Csc(a)), false);
            FunctionRegistry.RegisterFunction("sec", (Func<double, double>)((a) => MathUtil.Sec(a)), false);
            FunctionRegistry.RegisterFunction("asin", (Func<double, double>)((a) => Math.Asin(a)), false);
            FunctionRegistry.RegisterFunction("acos", (Func<double, double>)((a) => Math.Acos(a)), false);
            FunctionRegistry.RegisterFunction("tan", (Func<double, double>)((a) => Math.Tan(a)), false);
            FunctionRegistry.RegisterFunction("cot", (Func<double, double>)((a) => MathUtil.Cot(a)), false);
            FunctionRegistry.RegisterFunction("atan", (Func<double, double>)((a) => Math.Atan(a)), false);
            FunctionRegistry.RegisterFunction("acot", (Func<double, double>)((a) => MathUtil.Acot(a)), false);
            FunctionRegistry.RegisterFunction("loge", (Func<double, double>)((a) => Math.Log(a)), false);
            FunctionRegistry.RegisterFunction("log10", (Func<double, double>)((a) => Math.Log10(a)), false);
            FunctionRegistry.RegisterFunction("logn", (Func<double, double, double>)((a, b) => Math.Log(a, b)), false);
            FunctionRegistry.RegisterFunction("sqrt", (Func<double, double>)((a) => Math.Sqrt(a)), false);
            FunctionRegistry.RegisterFunction("abs", (Func<double, double>)((a) => Math.Abs(a)), false);
            FunctionRegistry.RegisterFunction("max", (Func<double, double, double>)((a, b) => Math.Max(a, b)), false);
            FunctionRegistry.RegisterFunction("min", (Func<double, double, double>)((a, b) => Math.Min(a, b)), false);
            FunctionRegistry.RegisterFunction("if", (Func<double, double, double, double>)((a, b, c) => (a != 0.0 ? b : c)), false);
            FunctionRegistry.RegisterFunction("ifless", (Func<double, double, double, double, double>)((a, b, c, d) => (a < b ? c : d)), false);
            FunctionRegistry.RegisterFunction("ifmore", (Func<double, double, double, double, double>)((a, b, c, d) => (a > b ? c : d)), false);
            FunctionRegistry.RegisterFunction("ifequal", (Func<double, double, double, double, double>)((a, b, c, d) => (a == b ? c : d)), false);
            FunctionRegistry.RegisterFunction("ceiling", (Func<double, double>)((a) => Math.Ceiling(a)), false);
            FunctionRegistry.RegisterFunction("floor", (Func<double, double>)((a) => Math.Floor(a)), false);
#if !WINDOWS_PHONE_7
            FunctionRegistry.RegisterFunction("truncate", (Func<double, double>)((a) => Math.Truncate(a)), false);
#endif
        }

        private void RegisterDefaultConstants()
        {
            ConstantRegistry.RegisterConstant("e", Math.E, false);
            ConstantRegistry.RegisterConstant("pi", Math.PI, false);
        }
    }

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



            if (executionMode == ExecutionMode.Interpreted)
            {
                return new Interpreter<double?>(NullableDoubleNumericalOperations.Instance);
            }
            else if (executionMode == ExecutionMode.Compiled)
            {
#if !NETFX_CORE && !NETCORE
                return new NullableDoubleDynamicCompiler();
#else
                return new ExpressionExecutor<double?>((variables, functionRegistry) => new FormulaContext<double?>(variables, functionRegistry), NullableDoubleNumericalOperations.Instance);
#endif
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported execution mode \"{0}\".", executionMode),
                    "executionMode");
            }
        }

        private void RegisterDefaultFunctions()
        {
            FunctionRegistry.RegisterFunction("sin", (Func<double, double>)((a) => Math.Sin(a)), false);
            FunctionRegistry.RegisterFunction("cos", (Func<double, double>)((a) => Math.Cos(a)), false);
            FunctionRegistry.RegisterFunction("csc", (Func<double, double>)((a) => MathUtil.Csc(a)), false);
            FunctionRegistry.RegisterFunction("sec", (Func<double, double>)((a) => MathUtil.Sec(a)), false);
            FunctionRegistry.RegisterFunction("asin", (Func<double, double>)((a) => Math.Asin(a)), false);
            FunctionRegistry.RegisterFunction("acos", (Func<double, double>)((a) => Math.Acos(a)), false);
            FunctionRegistry.RegisterFunction("tan", (Func<double, double>)((a) => Math.Tan(a)), false);
            FunctionRegistry.RegisterFunction("cot", (Func<double, double>)((a) => MathUtil.Cot(a)), false);
            FunctionRegistry.RegisterFunction("atan", (Func<double, double>)((a) => Math.Atan(a)), false);
            FunctionRegistry.RegisterFunction("acot", (Func<double, double>)((a) => MathUtil.Acot(a)), false);
            FunctionRegistry.RegisterFunction("loge", (Func<double, double>)((a) => Math.Log(a)), false);
            FunctionRegistry.RegisterFunction("log10", (Func<double, double>)((a) => Math.Log10(a)), false);
            FunctionRegistry.RegisterFunction("logn", (Func<double, double, double>)((a, b) => Math.Log(a, b)), false);
            FunctionRegistry.RegisterFunction("sqrt", (Func<double, double>)((a) => Math.Sqrt(a)), false);
            FunctionRegistry.RegisterFunction("abs", (Func<double, double>)((a) => Math.Abs(a)), false);
            FunctionRegistry.RegisterFunction("max", (Func<double, double, double>)((a, b) => Math.Max(a, b)), false);
            FunctionRegistry.RegisterFunction("min", (Func<double, double, double>)((a, b) => Math.Min(a, b)), false);
            FunctionRegistry.RegisterFunction("if", (Func<double, double, double, double>)((a, b, c) => (a != 0.0 ? b : c)), false);
            FunctionRegistry.RegisterFunction("ifless", (Func<double, double, double, double, double>)((a, b, c, d) => (a < b ? c : d)), false);
            FunctionRegistry.RegisterFunction("ifmore", (Func<double, double, double, double, double>)((a, b, c, d) => (a > b ? c : d)), false);
            FunctionRegistry.RegisterFunction("ifequal", (Func<double, double, double, double, double>)((a, b, c, d) => (a == b ? c : d)), false);
            FunctionRegistry.RegisterFunction("ceiling", (Func<double, double>)((a) => Math.Ceiling(a)), false);
            FunctionRegistry.RegisterFunction("floor", (Func<double, double>)((a) => Math.Floor(a)), false);
#if !WINDOWS_PHONE_7
            FunctionRegistry.RegisterFunction("truncate", (Func<double, double>)((a) => Math.Truncate(a)), false);
#endif
        }

        private void RegisterDefaultConstants()
        {
            ConstantRegistry.RegisterConstant("e", Math.E, false);
            ConstantRegistry.RegisterConstant("pi", Math.PI, false);
        }
    }
}
