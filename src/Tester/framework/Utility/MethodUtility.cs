using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using expunit.framework.Exp;
using expunit.framework.Model;

namespace expunit.framework.Utility
{
    public static class MethodUtility
    {
        public static MethodInfo GetMethod<T>(this Expression<T> expression)
        {
            MethodInfo result = null;
            var target = expression.ExpressionTest.Target.Type;
            var methodName = expression.MethodName;
            foreach (MethodInfo methodInfo in target.GetMethodsByName(methodName))
            {
                if (IsMethodParametersMatched(methodInfo.GetParameters(), expression.ParameterTypes))
                {
                    result = methodInfo;
                    break;
                }
            }

            if (result == null)
            {
                result = target.GetMethodsByName(methodName).FirstOrDefault();

                if (result == null)
                {
                    throw new MethodNotFoundException(methodName, target.FullName);
                }
            }

            if (expression.ParameterValues.Length == 0)
            {
                expression.ParameterValues = expression.PassNullParameterValues ?
                    result.GetMethodNullParameterValues() :
                    result.GetMethodParametersValues();
            }

            expression.ReturnType = result.ReturnType;

            object[] expectedOutputs = expression.ExpressionTest.ExpectedOutputs;
            if (expectedOutputs != null)
            {
                for (int i = 0; i < expectedOutputs.Length; i++)
                {
                    object expectedValue = expectedOutputs[i];
                    if (expectedValue != null)
                    {
                        expression.ExpressionTest.ExpectedOutputs[i] = TypeUtility.CastValueWithRespectToType(expectedValue, expression.ReturnType);
                    }
                }
            }

            return result;
        }

        internal static object[] GetMethodParametersValues(this MethodInfo methodInfo)
        {
            int totalNumberOfFields = methodInfo.GetTotalNumberOfMembers();
            ParameterInfo[] parameterInfo = methodInfo.GetParameters();
            object[] parameters = new object[parameterInfo.Length];
            for (int index = 0; index < parameterInfo.Length; index++)
            {
                ParameterInfo parameterType = parameterInfo[index];

                if (parameterType.ParameterType == typeof(Type))
                {
                    parameters[index] = methodInfo.GetType();
                    continue;
                }

                object defaultValue = parameterType.ParameterType.Get(parameterInfo[index].Name, index + totalNumberOfFields + 1);
                if (defaultValue != null)
                {
                    parameters[index] = defaultValue;
                }
                else
                {
                    parameters[index] = parameterType.ParameterType.CreateInstance();
                }
            }
            return parameters;
        }

        private static object[] GetMethodNullParameterValues(this MethodInfo methodInfo)
        {
            var parameterInfos = methodInfo.GetParameters();
            var parameters = new object[parameterInfos.Length];
            for (var index = 0; index < parameterInfos.Length; index++)
            {
                ParameterInfo parameterType = parameterInfos[index];
                parameters[index] = parameterType.ParameterType.GetDefaultValue();
            }

            return parameters;
        }

        private static int GetTotalNumberOfMembers(this MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType.GetAllFields().Length +
                   methodInfo.DeclaringType.GetAllProperties().Length;
        }

        private static bool IsMethodParametersMatched(IReadOnlyCollection<ParameterInfo> methodParametersTypes, IReadOnlyList<Type> matchParametersTypes)
        {
            if (methodParametersTypes.Count != matchParametersTypes.Count)
            {
                return false;
            }

            return
                !methodParametersTypes.Where((t, i) =>
                    !t.ParameterType.Name.Equals(matchParametersTypes[i].Name)).Any();
        }

        public static IList<MethodInfo> GetMethodsByName(this Type target, string methodName)
        {
            return target.GetAllMethods().Where(x => x.Name == methodName).ToList();
        }

        public static MethodInfo[] GetAllMethods(this Type type)
        {
            MethodInfo[] result = type.GetMethods(TypeUtility.BindingFlags());

            Type parentType = type.BaseType;
            if (parentType != null)
            {
                MethodInfo[] parentClassMethods = parentType.GetAllMethods();
                result = result.Concat(parentClassMethods).ToArray();
            }

            return result;
        }

        public static MethodInfo FindMethodByName(this Type type, string name)
        {
            return type.GetAllMethods().OrderBy(x => x.GetParameters().Length).FirstOrDefault(x => x.Name == name);
        }
    }
}