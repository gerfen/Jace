
using System.Collections.Generic;
using System.Linq;

namespace Jace.Util
{
    /// <summary>
    /// Utility methods of Jace.NET that can be used throughout the engine.
    /// </summary>
    internal  static class EngineUtil
    {
        internal static IDictionary<string, T> ConvertVariableNamesToLowerCase<T>(IDictionary<string, T> variables)
        {
            return variables.ToDictionary(keyValuePair => keyValuePair.Key.ToLowerInvariant(), keyValuePair => keyValuePair.Value);
        }
    }
}
