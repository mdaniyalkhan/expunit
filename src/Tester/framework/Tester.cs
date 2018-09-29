﻿using System;
using System.Collections.Generic;
using expunit.framework.Model;

namespace expunit.framework
{
    public class Tester
    {
        public readonly IList<string> MethodNamesToBeSkipped;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Tester()
        {
            MethodNamesToBeSkipped = new List<string>();
        }

        public void TestMethods<T>(IList<Expression<T>> methodsToBeTested)
        {
            foreach (var methodToBeTested in methodsToBeTested)
            {
                if (!IsMethodToBeSkipped(methodToBeTested))
                {
                    methodToBeTested.Test();
                }
            }
        }

        public void TestAndVerifyMethods<T>(IList<Expression<T>> methodsToBeTest)
        {
            TestMethods(methodsToBeTest);
            VerifyTests(methodsToBeTest);
        }

        private void VerifyTests<T>(IEnumerable<Expression<T>> methodsToBeTest)
        {
            foreach (Expression<T> methodToBeVerify  in methodsToBeTest)
            {
                if (!IsMethodToBeSkipped(methodToBeVerify))
                {
                    methodToBeVerify.Verify();
                }
            }
        }

        private bool IsMethodToBeSkipped<T>(Expression<T> expressionToBeTested)
        {
            Type target = expressionToBeTested.ExpressionTest.Target.Type;
            string targetClassName = target.FullName;
            string methodName = expressionToBeTested.MethodName;
            return target.IsAbstract ||
                   target.IsInterface ||
                   MethodNamesToBeSkipped.Contains(methodName) ||
                   MethodNamesToBeSkipped.Contains($"{targetClassName}.{methodName}") ||
                   MethodNamesToBeSkipped.Contains($"{targetClassName}.*");
        }
    }
}