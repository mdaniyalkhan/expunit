using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using expunit.framework.Model;
using Spring.Expressions;

namespace expunit.framework.Utility
{
    public static class ExpressionUtility
    {
        public static object[] ResolveExpressionValue<T>(this ExpressionTest<T> expressionTest)
        {
            if (expressionTest.Expressions == null)
            {
                return expressionTest.GetParentExpressionValue();
            }

            var expressionValues = new object[expressionTest.Expressions.Length];
            for (var i = 0; i < expressionValues.Length; i++)
            {
                var current = expressionTest.Expressions[i];
                if (current.Equals("this"))
                {
                    expressionValues[i] = expressionTest.TargetInstance;
                    continue;
                }

                var characterInBetweenSingleColon = "'.*?'";
                var matchesConstantExpressions = AllMatches(characterInBetweenSingleColon, current);
                current = Regex.Replace(current, characterInBetweenSingleColon, "#ContantExpr#");

                var fieldPattern = "@f[(]((.*?)+)[)]";
                foreach (string field in AllMatches(fieldPattern, current))
                {
                    var pattern = Regex.Replace(Regex.Replace(field, "\\(", "\\("), "\\)", "\\)");
                    var fieldValue = expressionTest.Target.Type.GetFieldValue(Regex.Replace(field, fieldPattern, "$1"));
                    current = Regex.Replace(current, pattern, TypeUtility.TypeToString(fieldValue));
                }

                current = Regex.Replace(current, "\\(\\)\\.", ".");
                var expressions = Regex.Split(Regex.Replace(current, "\\s", ""),
                    "!\\(|\\.Equals\\(|\\.CharAt\\(|[(]|[)]|[!]|\\+\\+|--|[+]|[-]|[*]|[/]|[%]|<=|>=|[<]|[>]|[!=]|==|&&|[{]|[}]|[,]|[?]|[:]|\\|\\|");
                var methodExpr = expressionTest.Expressions;
                expressionTest.Expressions = expressions;
                var values = expressionTest.GetParentExpressionValue();
                expressionTest.Expressions = methodExpr;
                if (current.Equals("@p0"))
                {
                    expressionValues[i] = expressionTest.ExpressionInfo?.ParameterValues[0];
                }
                else if (values.Length == 1 && values[0] != expressionTest.TargetInstance)
                {
                    expressionValues[i] = values[0];
                }
                else if (values.Length == 1)
                {
                    try
                    {
                        expressionValues[i] = ExpressionEvaluator.GetValue(null, expressionTest.Expressions[i]);
                    }
                    catch (Exception)
                    {
                        expressionValues[i] = Enum.Enum.Instance.NotFound;
                    }
                }
                else if (values.Length > 1)
                {
                    var expressionsWithValues = new Dictionary<string, object>();
                    for (var j = 0; j < expressions.Length; j++)
                    {
                        if (values[j] == expressionTest.TargetInstance)
                        {
                            continue;
                        }

                        values[j] = SetValue(values[j]);
                        if (!expressionsWithValues.ContainsKey(expressions[j]))
                        {
                            expressionsWithValues.Add(expressions[j], values[j]);
                        }
                    }


                    foreach (var pair in expressionsWithValues.OrderByDescending(x => x.Key))
                    {
                        current = current.Replace(pair.Key, pair.Value.ToString());
                    }

                    var expressionInfo = expressionTest.ExpressionInfo;
                    for (var parameterIndex = 0; parameterIndex < expressionInfo?.ParameterValues.Length; parameterIndex++)
                    {
                        var name = "@p" + parameterIndex;
                        var value = SetValue(expressionInfo.ParameterValues[parameterIndex]);
                        current = current.Replace(name, value?.ToString() ?? "null");
                    }

                    current = Regex.Replace(current, "\\(\\)", "");
                    current = Regex.Replace(current, "2147483648L", "2147483648");
                    current = Regex.Replace(current, "2147483648", "2147483648L");
                    current = Regex.Replace(current, "\\|\\|", "or");
                    current = Regex.Replace(current, "&&", "and");

                    foreach (Match match in matchesConstantExpressions)
                    {
                        var regex = new Regex("#ContantExpr#");
                        current = regex.Replace(current, match.Value, 1);
                    }

                    expressionValues[i] = ExpressionEvaluator.GetValue(null, current);
                }

                if (expressionValues[i] != null && expressionValues[i] is Type)
                {
                    expressionValues[i] = ((Type) expressionValues[i]).FullName;
                }

                if (expressionValues[i] != null && expressionTest.ExpressionInfo != null && expressionTest.ExpressionInfo.ReturnType != typeof(void))
                {
                    expressionValues[i] = TypeUtility.CastValueWithRespectToType(expressionValues[i], expressionTest.ExpressionInfo.ReturnType);
                }
            }

            return expressionValues;
        }

        private static object SetValue(object val)
        {
            if (val == null)
            {
                return null;
            }

            if (val is string)
            {
                val = "'" + val + "'";
            }
            else if (val.GetType().Get() == null)
            {
                val = SerializeUtility.Serialize(val);
            }

            if (val is char)
            {
                val = "'" + val + "'";
            }

            if (val is bool)
            {
                val = val.ToString().ToLower();
            }

            if (!val.GetType().IsArray) return val;

            var builder = new StringBuilder();
            builder.Append("{");
            dynamic array = val;
            for (int index = 0; index < array.Length; index++)
            {
                builder.Append(array[0] is string ?
                    "'" + array[0] + "'" :
                    array[0]);
                if (index < array.Length - 1)
                {
                    builder.Append(",");
                }
            }

            builder.Append("}");
            val = builder.ToString();

            return val;
        }

        private static MatchCollection AllMatches(string characterInBetweenSingleColon, string current)
        {
            return Regex.Matches(current, characterInBetweenSingleColon);
        }
    }
}