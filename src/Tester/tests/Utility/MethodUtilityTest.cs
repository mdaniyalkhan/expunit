using System.Diagnostics.CodeAnalysis;
using expunit.framework.Model;
using expunit.framework.Utility;
using NUnit.Framework;
using Shouldly;

namespace expunit.framework.tests.Utility
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class MethodUtilityTest
    {
        private const string CreateTestMethod = "CreateTest";
        private const string UnknownMethodTest = "UnknownMethod";

        [Test]
        public void Test_GetMethodsByName()
        {
            // Act
            var methods = typeof(ExpressionTest<int>).GetMethodsByName(CreateTestMethod);
        
            // Assert
            methods.Count.ShouldBe(8);
        }

        [Test]
        public void Test_GetMethodsByName_UnknownMethod()
        {
            // Act
            var methods = typeof(ExpressionTest<int>).GetMethodsByName(UnknownMethodTest);

            // Assert
            methods.Count.ShouldBe(0);
        }

        [Test]
        public void Test_FindMethodByName()
        {
            // Act
            var method = typeof(ExpressionTest<int>).FindMethodByName(CreateTestMethod);

            // Assert
            method.ShouldNotBeNull();
        }

        [Test]
        public void Test_FindMethodByName_UnknownMethod()
        {
            // Act
            var method = typeof(ExpressionTest<int>).FindMethodByName(UnknownMethodTest);

            // Assert
            method.ShouldBeNull();
        }
    }
}