using System;
using System.Collections.Generic;
using expunit.framework.Utility;

namespace expunit.framework.Model
{
    /// <summary>
    /// Class contains expression to be tested information
    /// </summary>
    public class Expression<T>
    {
        public string MethodName { get; }
        public Type[] ParameterTypes { get; private set; }
        public object[] ParameterValues { get; set; }
        public bool PassNullParameterValues { get; }
        public ExpressionTest<T> ExpressionTest { get; }
        public Type ReturnType { get; set; }

        private Expression(string methodName, bool passNullParameterValues, object[] parameterValues, ExpressionTest<T> expressionTest)
        {
            SetupMethodTest(methodName, expressionTest);
            MethodName = methodName;
            PassNullParameterValues = passNullParameterValues;
            ParameterValues = parameterValues;
            SetParameterTypes(parameterValues);
            ExpressionTest = expressionTest;
        }

        /// <summary>
        ///     Expression to create ExpressionInfo instance usually used to test expression with default parameter values
        /// </summary>
        /// <param name="methodName">Name of expression to be tested</param>
        /// <param name="expressionTest">Expression test information e.g. Target class, expected output and other parameters</param>
        /// <returns>ExpressionInfo object</returns>
        public static Expression<T> Create(string methodName, ExpressionTest<T> expressionTest)
        {
            return new Expression<T>(methodName, false, new object[0], expressionTest);
        }

        /// <summary>
        ///     Expression to create ExpressionInfo instance usually used to test expression with default parameter values
        /// </summary>
        /// <param name="methodName">Name of expression to be tested</param>
        /// <param name="parameterValues">Parameter user defined values</param>
        /// <param name="expressionTest">Expression test information e.g. Target class, expected output and other parameters</param>
        /// <returns>ExpressionInfo object</returns>
        public static Expression<T> Create(string methodName, object[] parameterValues, ExpressionTest<T> expressionTest)
        {
            return new Expression<T>(methodName, false, parameterValues, expressionTest);
        }

        /// <summary>
        ///     Expression to create ExpressionInfo instance usually used to test expression with null parameter values
        /// </summary>
        /// <param name="methodName">Name of expression to be tested</param>
        /// <param name="expressionTest">Expression test information e.g. Target class, expected output and other parameters</param>
        /// <returns>ExpressionInfo object</returns>
        public static Expression<T> CreateWithNullParameters(string methodName, ExpressionTest<T> expressionTest)
        {
            return new Expression<T>(methodName, true, new object[0], expressionTest);
        }

        /// <summary>
        ///   Test Expression
        /// </summary>
        public void Test()
        {
            InvokeMethod();
            ExpressionTest.SetActualOutputs();
        }

        /// <summary>
        ///     Verify expression expected output equals to actual output
        /// </summary>
        public void Verify()
        {
            ExpressionTest.Verify(MethodName);
        }

        /// <summary>
        ///     Test and Verify expression expected output equals to actual output
        /// </summary>
        public void TestAndVerify()
        {
            Test();
            Verify();
        }

        private void SetupMethodTest(string methodName, ExpressionTest<T> expressionTest)
        {
            expressionTest.TargetInstance = expressionTest.Target.Type.CreateInstance(expressionTest.Target.ClassFields);
            expressionTest.Name = $"Testing expression [{methodName}] in ";
            expressionTest.ExpressionInfo = this;
        }

        private void SetParameterTypes(IReadOnlyList<object> value)
        {
            if (value != null)
            {
                ParameterTypes = new Type[value.Count];
                for (var i = 0; i < value.Count; i++)
                {
                    ParameterTypes[i] = value[i].GetType();
                }
            }
        }

        private void InvokeMethod()
        {
            var methodInfo = this.GetMethod();
            var instance = ExpressionTest.TargetInstance;

            if (methodInfo != null)
            {
                try
                {
                    if (methodInfo.ReturnType != typeof(void))
                    {
                        ExpressionTest.TargetInstance =
                            methodInfo.Invoke(methodInfo.IsStatic ?
                                null :
                                instance, ParameterValues);
                    }
                    else
                    {
                        methodInfo.Invoke(methodInfo.IsStatic ?
                            null :
                            instance, ParameterValues);
                    }
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                    {
                        ExpressionTest.TargetInstance = e.InnerException.GetType().Name;
                        ExpressionTest.SetErrorMessages();
                    }
                    else
                    {
                        ExpressionTest.TargetInstance = e.GetType().Name;
                    }
                }
            }
        }
    }
}