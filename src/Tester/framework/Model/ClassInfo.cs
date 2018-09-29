using System;
using System.Collections.Generic;

namespace expunit.framework.Model
{
    /// <summary>
    /// Class information
    /// </summary>
    public class ClassInfo<T>
    {
        private ClassInfo(IDictionary<string, dynamic> fields)
        {
            Type = typeof(T);
            ClassFields = fields;
        }

        /// <summary>
        /// Class Type
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Class Fields
        /// </summary>
        public IDictionary<string, object> ClassFields { get; set; }

        public static ClassInfo<T> Create()
        {
            return new ClassInfo<T>(new Dictionary<string, dynamic>());
        }

        public static ClassInfo<T> Create(IDictionary<string, dynamic> fields)
        {
            return new ClassInfo<T>(fields);
        }
    }
}