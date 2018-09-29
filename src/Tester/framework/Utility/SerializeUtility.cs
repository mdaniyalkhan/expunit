using System;
using Newtonsoft.Json;

namespace expunit.framework.Utility
{
    /// <summary>
    /// Utility for object serialization
    /// </summary>
    public static class SerializeUtility
    {
        /// <summary>
        /// Serialize model or object
        /// </summary>
        public static string Serialize<T>(T model)
        {
            try
            {
                return JsonConvert.SerializeObject(model);
            }
            catch (Exception)
            {
                return $@"'{model}'";
            }
        }
    }
}