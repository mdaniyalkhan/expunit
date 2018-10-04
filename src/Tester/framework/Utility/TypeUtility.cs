using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading;
using expunit.framework.Exp;
using Moq;

namespace expunit.framework.Utility
{
    public static class TypeUtility
    {
        private const char NullChar = '\0';
        private const string DefaultCulture = "en-US";
        private const string Xml = "xml";
        private const string Culture = "culture";
        private const string EmptyGuidString = "00000000-0000-0000-0000-000000000000";
        private const string ReferencedBoolean = "System.Boolean&";
        private const string ReferencedString = "System.String&";
        private const string SystemThreadingType = "System.Threading";
        private const string MaxValueProperty = "MaxValue";

        public static bool InitializeInstanceCollectionFields = true;
        public static IDictionary<Type, dynamic> UserDefineTypesValue = new Dictionary<Type, dynamic>();

        /// <summary>
        ///     Create Instance of Specific Type
        /// </summary>
        /// <param name="type">Class Type</param>
        /// <param name="listIndex"></param>
        /// <param name="setDefaultValue"></param>
        /// <param name="levelInitUninitializedMembers">Level to initialize nested properties and fields</param>
        /// <returns>Instance object</returns>
        public static dynamic CreateInstance(
            this Type type,
            int listIndex = 0,
            bool setDefaultValue = false,
            int levelInitUninitializedMembers = 1)
        {
            return CreateInstance(
                type,
                new Dictionary<string, dynamic>(),
                listIndex,
                setDefaultValue,
                levelInitUninitializedMembers);
        }

        /// <summary>
        ///     Create Instance of Specific Type
        /// </summary>
        /// <param name="type">Class Type</param>
        /// <param name="fields">Custom field values</param>
        /// <param name="listIndex"></param>
        /// <param name="setDefaultValue"></param>
        /// <param name="levelInitUninitializedFields">Level to initialize nested properties and fields</param>
        /// <returns>Instance object</returns>
        public static dynamic CreateInstance(
            this Type type,
            IDictionary<string, dynamic> fields,
            int listIndex = 0,
            bool setDefaultValue = false,
            int levelInitUninitializedFields = 1)
        {
            dynamic instance = null;
            if (type.IsAbstract || type.IsInterface)
            {
                instance = GetMock(type);
            }

            if (instance == null)
            {
                instance = Get(type, type.Name, 0, InitializeInstanceCollectionFields);
            }

            if (instance == null)
            {
                instance = CreateInstanceOfTypeHavingDefaultConstructor(type);
            }

            if (instance == null)
            {
                instance = CreateUninitializedObject(type);
            }

            if (instance != null && levelInitUninitializedFields > 0)
            {
                try
                {
                    levelInitUninitializedFields = levelInitUninitializedFields - 1;
                    InitUninitializedFields(
                        instance,
                        levelInitUninitializedFields,
                        fields,
                        setDefaultValue,
                        listIndex);
                }
                catch (Exception exp)
                {
                    Debug.WriteLine(exp);
                }
            }

            return instance;
        }

        /// <summary>
        ///     Generate value for primitive types
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Generated value</returns>
        public static dynamic Get(this Type type)
        {
            return Get(type, string.Empty);
        }

        /// <summary>
        ///     Generate value for primitive types
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="type">Type</param>
        /// <param name="index">Field or Property index</param>
        /// <param name="initializeInstanceCollectionFields">Initialize Instance Collection Field</param>
        /// <param name="setDefaultValues"></param>
        /// <returns>Generated value</returns>
        public static dynamic Get(this Type type,
            string parameterName,
            int index = 0,
            bool initializeInstanceCollectionFields = true,
            bool setDefaultValues = false)
        {
            if (type == null)
            {
                return null;
            }

            type = type.FullName != null && type.FullName.Contains(ReferencedBoolean) ?
                typeof(bool) :
                type;
            type = type.FullName != null && type.FullName.Contains(ReferencedString) ?
                typeof(string) :
                type;

            if (UserDefineTypesValue.ContainsKey(type))
            {
                return UserDefineTypesValue[type];
            }

            parameterName = string.IsNullOrWhiteSpace(parameterName) ?
                type.Name :
                parameterName;

            index = Convert.ToInt32(typeof(int).GetValueLessThanMaxValue(index.ToString()));

            var paramterIntegerValue = parameterName.Sum(Convert.ToInt32);
            if (Convert.ToDouble(index) + paramterIntegerValue < int.MaxValue)
            {
                index += paramterIntegerValue;
            }
            else
            {
                index -= paramterIntegerValue;
            }

            parameterName = parameterName.ToLower();

            if (type.IsNumber())
            {
                type = GetNumberUnderlyingType(type);
                return Convert.ChangeType(setDefaultValues ?
                    "0" :
                    type.GetValueLessThanMaxValue(index.ToString()), type);
            }
            if (type == typeof(BigInteger) || type == typeof(BigInteger?))
            {
                return setDefaultValues ?
                    default(BigInteger) :
                    new BigInteger(index);
            }

            if (type == typeof(char) || type == typeof(char?))
            {
                return setDefaultValues ?
                    NullChar :
                    (char) index;
            }

            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return GetDateTime(index);
            }

            if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
            {
                return setDefaultValues ?
                    type == typeof(DateTimeOffset?) ?
                        (DateTimeOffset?) null :
                        DateTimeOffset.MinValue :
                    GetDateTime(index);
            }

            if (parameterName.EndsWith("id") && type == typeof(object))
            {
                return setDefaultValues ?
                    0 :
                    index;
            }

            if ((parameterName.EndsWith("type") ||
                 parameterName.EndsWith("flag")) &&
                type == typeof(string))
            {
                return setDefaultValues ?
                    null :
                    index.ToString();
            }

            if (parameterName.Contains(Xml) && type == typeof(string))
            {
                return setDefaultValues ?
                    null :
                    $"<UnitTest>{parameterName.ToPipeSeparatedCharacters()}-{index}</UnitTest>";
            }

            if (parameterName.EndsWith(Culture) && type == typeof(string))
            {
                return setDefaultValues ?
                    Thread.CurrentThread.CurrentCulture.Name :
                    DefaultCulture;
            }

            if (type == typeof(string))
            {
                return setDefaultValues ?
                    null :
                    $"{parameterName.ToPipeSeparatedCharacters()}-{index}";
            }

            if (type == typeof(bool) || type == typeof(bool?))
            {
                return !setDefaultValues && bool.Parse((index % 2 == 0).ToString());
            }

            if (type == typeof(Uri))
            {
                return setDefaultValues ?
                    null :
                    new Uri($"https://www.google.com.pk/FieldIndex={parameterName}-{index}");
            }

            if (type.IsAssignableFrom(typeof(Exception)))
            {
                return setDefaultValues ?
                    null :
                    new EmptyException(parameterName, index);
            }

            if (type == typeof(Guid) || type == typeof(Guid?))
            {
                return setDefaultValues ?
                    type == typeof(Guid?) ?
                        (Guid?) null :
                        Guid.Empty :
                    GetGuid(index);
            }

            if (IsACollection(type) &&
                initializeInstanceCollectionFields &&
                (type.GetElementType() != typeof(Type) ||
                 type.GetGenericArguments()[0] != typeof(Type)))
            {
                return setDefaultValues ?
                    null :
                    type.GetElementType()?
                        .GetArrayObjects(index) ??
                    type.GetGenericArguments()[0]
                        .GetListObjects(index);
            }

            return null;
        }

        private static Type GetNumberUnderlyingType(this Type type)
        {
            if (type.IsNumber())
            {
                if (type.IsGenericType)
                {
                    type = Nullable.GetUnderlyingType(type);
                }

                if (type == null)
                {
                    throw new InvalidOperationException(nameof(type));
                }
            }

            return type;
        }

        private static bool IsNumber(this Type type)
        {
            return type == typeof(sbyte) ||
                   type == typeof(sbyte?) ||
                   type == typeof(short) ||
                   type == typeof(short?) ||
                   type == typeof(ushort) ||
                   type == typeof(ushort?) ||
                   type == typeof(int) ||
                   type == typeof(int?) ||
                   type == typeof(uint) ||
                   type == typeof(uint?) ||
                   type == typeof(long) ||
                   type == typeof(long?) ||
                   type == typeof(ulong) ||
                   type == typeof(ulong?) ||
                   type == typeof(double) ||
                   type == typeof(double?) ||
                   type == typeof(float) ||
                   type == typeof(float?) ||
                   type == typeof(decimal) ||
                   type == typeof(decimal?);
        }

        /// <summary>
        /// Get Default Value
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static dynamic GetDefaultValue(this Type type)
        {
            return type.Get(string.Empty, 0, true, true);
        }

        private static string GetGuidString(this int fieldIndex)
        {
            var guid = EmptyGuidString;
            var fieldLength = fieldIndex.ToString().Length;
            guid = fieldIndex + guid.Substring(fieldLength, guid.Length - fieldLength);

            return guid.Substring(0, guid.Length - fieldLength) + fieldIndex;
        }

        private static dynamic GetGuid(this int fieldIndex)
        {
            return Guid.Parse(GetGuidString(fieldIndex));
        }

        /// <summary>
        ///     Get Field value for primitive types
        /// </summary>
        /// <param name="fieldName">Field Name</param>
        /// <param name="type">Class type</param>
        /// <returns>Field value</returns>
        public static dynamic GetFieldValue(this Type type, string fieldName)
        {
            var field = FindFieldByName(type, fieldName);
            if (field != null)
            {
                return Get(field.FieldType, $"{type.Name}.{fieldName}", GetFieldIndexByName(type, fieldName));
            }

            throw new FieldNotFoundException(fieldName, type.FullName);
        }

        public static dynamic CastValueWithRespectToType(dynamic obj, Type type)
        {
            if (type == null || type == typeof(void))
            {
                return obj;
            }

            try
            {
                obj = Convert.ChangeType(obj, type);
            }
            catch
            {
                // ignored
            }

            return obj;
        }

        /// <summary>
        ///     Get Property value for primitive types
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="type">Class type</param>
        /// <returns>Property value</returns>
        public static dynamic GetPropertyValue(this Type type, string propertyName)
        {
            var property = FindPropertyByName(type, propertyName);
            if (property != null)
            {
                return Get(property.PropertyType, propertyName, GetFieldIndexByName(type, propertyName));
            }

            throw new PropertyNotFoundException(propertyName, type.FullName);
        }

        /// <summary>
        ///     Find Field by name
        /// </summary>
        /// <param name="type">Class type</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static FieldInfo FindFieldByName(this Type type, string name)
        {
            return FindFieldsContainingFieldName(type, name).FirstOrDefault(fieldInfo => fieldInfo.Name == name);
        }

        /// <summary>
        ///     Find fields containing field name
        /// </summary>
        /// <param name="type">Class Type</param>
        /// <param name="name">Field Name</param>
        /// <returns>Fields</returns>
        public static IEnumerable<FieldInfo> FindFieldsContainingFieldName(this Type type, string name)
        {
            var fieldInfos = type.GetFields(ReflectionFlags);

            var parentType = type.BaseType;
            if (parentType != null)
            {
                foreach (var fieldInfo in fieldInfos)
                {
                    if (fieldInfo.Name == name)
                    {
                        return fieldInfos;
                    }
                }

                var parentFieldInfos = FindFieldsContainingFieldName(parentType, name);
                fieldInfos = fieldInfos.Concat(parentFieldInfos).ToArray();
            }


            return fieldInfos;
        }

        /// <summary>
        ///    Get all fields for specific type
        /// </summary>
        /// <param name="type">Class Type</param>
        /// <returns>Fields</returns>
        public static FieldInfo[] GetAllFields(this Type type)
        {
            var fieldInfos = type.GetFields(ReflectionFlags);

            var parentType = type.BaseType;
            if (parentType != null)
            {
                var parentFieldInfos = GetAllFields(parentType);
                fieldInfos = fieldInfos.Concat(parentFieldInfos).ToArray();
            }


            return fieldInfos;
        }

        /// <summary>
        ///     Find Property by Name
        /// </summary>
        /// <param name="type">Class Name</param>
        /// <param name="name">Property Name</param>
        /// <returns>Property name</returns>
        public static PropertyInfo FindPropertyByName(Type type, string name)
        {
            return FindPropertiesContainingPropertyName(type, name)
                .FirstOrDefault(fieldInfo => fieldInfo.Name == name);
        }

        /// <summary>
        ///     Find Properties containing specific property name
        /// </summary>
        /// <param name="type">Class type</param>
        /// <param name="name"></param>
        /// <returns>Properties</returns>
        public static IEnumerable<PropertyInfo> FindPropertiesContainingPropertyName(Type type, string name)
        {
            var propertyInfos = type.GetProperties(ReflectionFlags);

            var parentType = type.BaseType;
            if (parentType != null)
            {
                if (propertyInfos.Any(fieldInfo => fieldInfo.Name == name))
                {
                    return propertyInfos;
                }

                var parentPropertyInfos = FindPropertiesContainingPropertyName(parentType, name);
                propertyInfos = propertyInfos.Concat(parentPropertyInfos).ToArray();
            }


            return propertyInfos;
        }

        /// <summary>
        ///     Get all properties for specific type
        /// </summary>
        /// <param name="type">Class type</param>
        /// <returns>Properties</returns>
        public static PropertyInfo[] GetAllProperties(this Type type)
        {
            var propertyInfos = type.GetProperties(ReflectionFlags);

            var parentType = type.BaseType;
            if (parentType != null)
            {
                var parentPropertyInfo = GetAllProperties(parentType);
                propertyInfos = propertyInfos.Concat(parentPropertyInfo).ToArray();
            }


            return propertyInfos;
        }

        internal static string TypeToString(dynamic obj)
        {
            if (obj == null)
            {
                return null;
            }

            try
            {
                if (obj is char)
                {
                    obj = Encoding.Unicode.GetString(BitConverter.GetBytes((char) obj));
                }
                else if (obj is string)
                {
                    obj = "'" + obj.ToString().Replace("'", "''") + "'";
                }
                else if (obj is long)
                {
                    obj += "L";
                }
                else if (obj is double)
                {
                    obj += "D";
                }
                else if (obj is float)
                {
                    obj += "F";
                }
                else
                {
                    obj = obj.ToString();
                }
            }
            catch
            {
                // ignored
            }

            return obj.ToString();
        }

        public static bool IsACollection(this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static dynamic GetFirstObjectFromCollection(dynamic obj)
        {
            if (obj != null)
            {
                foreach (var o in obj)
                {
                    return o;
                }
            }

            return null;
        }

        private static dynamic GetArrayObjects(this Type type, int fieldIndex)
        {
            dynamic objects = Array.CreateInstance(type, 25);

            for (var x = 0; x < objects.Length; x++)
            {
                var value = x + fieldIndex.ToString();
                objects[x] = ParseValue(type, fieldIndex, value);
            }

            return objects;
        }

        private static dynamic GetListObjects(this Type type, int fieldIndex)
        {
            dynamic list = Activator.CreateInstance(typeof(List<>).MakeGenericType(type));

            for (var x = 0; x < 25; x++)
            {
                var value = x + fieldIndex.ToString();
                list.Add(ParseValue(type, fieldIndex, value));
            }

            return list;
        }

        private static dynamic ParseValue(Type type, int fieldIndex, string typeValue)
        {
            if (type.IsNumber())
            {
                type = GetNumberUnderlyingType(type);
                typeValue = type.GetValueLessThanMaxValue(typeValue);
                return Convert.ChangeType(typeValue, type);
            }

            if (type == typeof(bool))
            {
                return int.Parse(typeValue) % 2 == 0;
            }

            if (type == typeof(DateTime))
            {
                return GetDateTime(fieldIndex);
            }

            if (type == typeof(char))
            {
                return (char) int.Parse(typeValue);
            }

            if (type == typeof(Guid))
            {
                return GetGuid(fieldIndex);
            }

            return type == typeof(string) ?
                $"Test Output-{typeValue}" :
                CreateInstance(type, 0, false, 0);
        }

        private static string GetValueLessThanMaxValue(this IReflect type, string previousValue)
        {
            var maxValue = type.GetMaxValue();
            var validValue = Convert.ToDouble(previousValue);
            while (validValue > maxValue)
            {
                validValue = validValue - maxValue;
            }

            return validValue.ToString(CultureInfo.InvariantCulture);
        }

        private static int GetValueLessThanEqualToGivenValue(this int integerValue, int maxValue)
        {
            while (integerValue > maxValue)
            {
                integerValue = integerValue - maxValue;
            }

            return integerValue;
        }

        private static int GetFieldIndexByName(this IReflect type, string fieldName)
        {
            for (var i = 0; i < type.GetFields(ReflectionFlags).Length; i++)
            {
                if (type.GetFields(ReflectionFlags)[i].Name == fieldName)
                {
                    return i;
                }
            }

            return -1;
        }

        private static DateTime GetDateTime(int index)
        {
            var max = DateTime.MaxValue;
            return new DateTime(
                (2000 + index).GetValueLessThanEqualToGivenValue(max.Year),
                index.GetValueLessThanEqualToGivenValue(12),
                index.GetValueLessThanEqualToGivenValue(27),
                index.GetValueLessThanEqualToGivenValue(23),
                index.GetValueLessThanEqualToGivenValue(59),
                index.GetValueLessThanEqualToGivenValue(59));
        }

        public static dynamic CreateInstanceOfTypeHavingDefaultConstructor(this Type type)
        {
            return CreateInstanceOfTypeHavingPublicDefaultConstructor(type) ??
                   CreateInstanceOfTypeHavingPrivateDefaultConstructor(type);
        }

        private static dynamic CreateInstanceOfTypeHavingPublicDefaultConstructor(this Type type)
        {
            dynamic instance = null;
            if (IsPublicDefaultConstructorExist(type))
            {
                try
                {
                    instance = Activator.CreateInstance(type);
                }
                catch (Exception exp)
                {
                    Debug.WriteLine(exp);
                }
            }

            return instance;
        }

        private static dynamic CreateInstanceOfTypeHavingPrivateDefaultConstructor(this Type type)
        {
            dynamic instance = null;
            try
            {
                instance = Activator.CreateInstance(type, ReflectionFlags, null, new object[] { }, null);
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp);
            }

            return instance;
        }

        internal static BindingFlags ReflectionFlags => BindingFlags.Instance |
                                                        BindingFlags.NonPublic |
                                                        BindingFlags.Public |
                                                        BindingFlags.Static;

        public static dynamic CreateUninitializedObject(this Type type)
        {
            dynamic instance = null;

            try
            {
                instance = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp);
            }

            return instance;
        }

        public static dynamic GetMock(this Type type)
        {
            dynamic mock = null;
            try
            {
                mock = Activator.CreateInstance(typeof(Mock<>).MakeGenericType(type));
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp);
            }

            return mock;
        }

        private static double GetMaxValue(this IReflect type)
        {
            return Convert.ToDouble(type?.GetField(MaxValueProperty, ReflectionFlags)?.GetValue(null));
        }

        private static void InitUninitializedFields(this object instance,
            int levelInitUninitializedMembers,
            IDictionary<string, dynamic> fields,
            bool setDefaultValues = false,
            int listIndex = 0)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var type = instance.GetType();
            var fieldInfos = type.GetAllFields();
            for (var index = 0; index < fieldInfos.Length; index++)
            {
                var fieldInfo = fieldInfos[index];

                try
                {
                    var typeInfo = fieldInfo.FieldType;
                    dynamic defaultValue = fieldInfo.GetValue(instance);

                    if (typeInfo.FullName != null &&
                        !typeInfo.FullName.Contains(SystemThreadingType))
                    {
                        if (fields.ContainsKey(fieldInfo.Name))
                        {
                            fieldInfo.SetValue(instance, fields[fieldInfo.Name]);
                            continue;
                        }

                        if (typeInfo == typeof(Type))
                        {
                            fieldInfo.SetValue(instance, instance.GetType());
                            continue;
                        }

                        var value = Get(typeInfo, $"{type.Name}.{fieldInfo.Name}", index + listIndex, InitializeInstanceCollectionFields, setDefaultValues);
                        if (value != null)
                        {
                            fieldInfo.SetValue(instance, value);
                        }
                        else if (defaultValue == null)
                        {
                            fieldInfo.SetValue(instance, type.CreateInstance(listIndex, setDefaultValues, levelInitUninitializedMembers));
                        }
                    }
                }
                catch (Exception exp)
                {
                    Debug.WriteLine($"Could not Set Property {fieldInfo.Name} because of {exp}");
                }
            }
        }

        private static string ToPipeSeparatedCharacters(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            return str.Aggregate(string.Empty, (current, character) => current + (character + "|")).Trim('|');
        }

        private static bool IsPublicDefaultConstructorExist(this Type classType)
        {
            var constructorInfo = classType.GetConstructor(Type.EmptyTypes);

            return constructorInfo != null && constructorInfo.IsPublic;
        }
    }
}