using System;
using System.Reflection;
using System.Text.RegularExpressions;
using expunit.framework.Model;
using Spring.Expressions;

namespace expunit.framework.Utility
{
    public static class MethodTestUtility
    {
        internal static void SetTestActualOutput<T>(this ExpressionTest<T> expressionTest, object[] expressionValues)
        {
            for (int i = 0; i < expressionValues.Length; i++)
            {
                try
                {
                    if (
                        SerializeUtility.Serialize(expressionTest.ExpectedOutputs[i])
                            .Equals(SerializeUtility.Serialize(Enum.Enum.Instance.NotNull)) &&
                        expressionValues[i] != null)
                    {
                        expressionTest.ActualOutputs[i] = Enum.Enum.Instance.NotNull;
                    }
                    else if (
                        SerializeUtility.Serialize(expressionTest.ExpectedOutputs[i])
                            .Equals(SerializeUtility.Serialize(Enum.Enum.Instance.Null)) && expressionValues[i] == null)
                    {
                        expressionTest.ActualOutputs[i] = Enum.Enum.Instance.Null;
                    }
                    else if (expressionValues[i] != null && expressionValues[i] is Type)
                    {
                        expressionTest.ActualOutputs[i] = ((Type) expressionValues[i]).FullName;
                    }
                    else
                    {
                        expressionTest.ActualOutputs[i] = expressionValues[i];
                    }
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                    {
                        expressionTest.ActualOutputs[i] = e.InnerException.GetType().FullName;
                    }
                    else
                    {
                        expressionTest.ActualOutputs[i] = e.GetType().FullName;
                    }
                }
            }
        }

        internal static object[] GetParentExpressionValue<T>(this ExpressionTest<T> expressionTest)
        {
            if (expressionTest.Expressions == null)
            {
                return new[] {expressionTest.TargetInstance};
            }

            var parentExpressions = new object[expressionTest.Expressions.Length];

            for (int i = 0; i < expressionTest.Expressions.Length; i++)
            {
                string expression = expressionTest.Expressions[i];
                expression = Regex.Replace(expression, "\\[", ".[");
                expression = expression.Replace("this.", "");
                string[] members = expression.Split('.');

                for (int index = 0; index < members.Length; index++)
                {
                    string member = Regex.Replace(members[index], "\\(\\)", "");
                    if (parentExpressions[i] == null)
                    {
                        parentExpressions[i] = expressionTest.TargetInstance;
                    }

                    Type parentExpressionClass = parentExpressions[i] == null
                        ? expressionTest.Target.Type
                        : parentExpressions[i].GetType();
                    FieldInfo fieldInfo = parentExpressionClass.FindFieldByName(member);
                    PropertyInfo propertyGetter = TypeUtility.FindPropertyByName(parentExpressionClass, member);
                    MethodInfo method = parentExpressionClass.FindMethodByName(member);

                    if (fieldInfo == null && propertyGetter == null && method == null)
                    {
                        if (parentExpressionClass.IsACollection())
                        {
                            try
                            {
                                parentExpressions[i] = ExpressionEvaluator.GetValue(parentExpressions[i], members[index]);
                            }
                            catch (Exception)
                            {
                                parentExpressions[i] = TypeUtility.GetFirstObjectFromCollection(parentExpressions[i]);
                                index--;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    parentExpressions[i] = SetParentExpressionValue(fieldInfo, propertyGetter, method,
                        parentExpressions[i]);
                }
            }
            return parentExpressions;
        }

        private static object SetParentExpressionValue(FieldInfo fieldInfo, PropertyInfo property, MethodInfo method,
            object parentExpression)
        {
            if (method != null)
            {
                object[] methodParameters = method.GetMethodParametersValues();
                parentExpression = method.Invoke(!method.IsStatic ? parentExpression : null, methodParameters);
            }
            if (fieldInfo != null)
            {
                parentExpression = fieldInfo.GetValue(!fieldInfo.IsStatic ? parentExpression : null);
            }
            else if (property != null)
            {
                parentExpression = property.GetMethod.Invoke(!property.GetMethod.IsStatic ? parentExpression : null,
                    new object[0]);
            }

            return parentExpression;
        }
    }
}