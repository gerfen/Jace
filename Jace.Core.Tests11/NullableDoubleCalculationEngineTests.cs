using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __ANDROID__
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif NETCORE
using Xunit;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Jace.Tests
{
#if !NETCORE
    [TestClass]
#endif
    public class NullableDoubleCalculationEngineTests
    {
#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
#if NETCORE
        public void TestFunc12Test()
        {
            var engine = new NullableDoubleCalculationEngine();
            engine.AddFunction("RO_NDP", RO_NDP);

            var variables = new Dictionary<string, double?>
            {
                {"tempType", 2},
                {"pressType", 1},
                {"fw_psi", 20.2 },
                {"per_psi", 15.2 },
                {"con_psi", 11.2 },
                {"fwtds", 3.6 },
                {"permtds", 6.5 },
                {"contds", 2.2 },
                {"perm_flow", 7 },
                {"feed_flow", 2.5 },
                {"nTemp", 67.2 }

            };

            var result = engine.Calculate("RO_NDP(tempType,pressType,fw_psi,per_psi,con_psi,fwtds,permtds,contds,perm_flow,feed_flow,nTemp)", variables);
#if !NETCORE
            Assert.AreEqual(0.49971001699277529, result);
#else
            Assert.Equal(0.49971001699277529, result);
#endif
        }

#endif



        public static double? RO_NDP(double? tempType, double? pressType, double? fw_psi, double? per_psi, double? con_psi, double? fwtds, double? permtds, double? contds, double? perm_flow, double? feed_flow, double? nTemp)
        {
            ValidateParameter(tempType, "tempType");
            ValidateParameter(pressType, "pressType");
            ValidateParameter(fw_psi, "fw_psi");
            ValidateParameter(per_psi, "per_psi");
            ValidateParameter(con_psi, "con_psi");
            ValidateParameter(fwtds, "fwtds");
            ValidateParameter(permtds, "permtds");
            ValidateParameter(contds, "contds");
            ValidateParameter(perm_flow, "perm_flow");
            ValidateParameter(feed_flow, "feed_flow");
            // NB:  no need to validate 'nTemp', see nTemp.HasValue check below...

            double? num1 = 0.0;
            double num2 = 0.0;
            var num3 = perm_flow / feed_flow;
            var num4 = Math.Log(1.0 / (1.0 / num3.Value)) / num3;
            if (!nTemp.HasValue)
                nTemp = tempType != 1 ? 25.0 : 75.0;
            double? nullable = nTemp;
            if ((nullable.GetValueOrDefault() >= 0.0 ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
            {
                num2 = -1.0;
            }
            else
            {
                if (tempType == 1)
                {
                    nullable = nTemp;
                    nullable = nullable.HasValue ? nullable.GetValueOrDefault() - 32.0 : new double?();
                    nTemp = nullable.HasValue ? 0.0 * nullable.GetValueOrDefault() : new double?();
                }
                num2 = Math.Pow(1.03, 25.0 - nTemp.Value);
            }
            var num5 = num4 >= 20000.0 ? fw_psi - (fw_psi - con_psi) / 2.0 - per_psi - (0.0117 * num4 - 34.0) / 14.23 * (nTemp.Value + 320.0) / 345.0 : fw_psi - (fw_psi - con_psi) / 2.0 - per_psi - num4 * (nTemp.Value + 320.0) / 491000.0;
            num1 = num5;
            return num5;
        }

        private static void ValidateParameter(double? parameter, string name)
        {
            if (!parameter.HasValue)
            {
                throw new ArgumentNullException(string.Format("The '{0}' parameter cannot be null.", name));
            }
        }
    }
}
