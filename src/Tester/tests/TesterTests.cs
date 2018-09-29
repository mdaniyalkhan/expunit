using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using expunit.framework.Exp;
using expunit.framework.Model;
using expunit.framework.tests.TestClass;
using expunit.framework.Utility;
using NUnit.Framework;

namespace expunit.framework.tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class TesterTests
    {
        private Tester _tester;

        [SetUp]
        public void Setup()
        {
            _tester = new Tester();
        }

        [Test]
        public void TestMethod_ReturnMessage()
        {
            // Arrange
            var methods = new List<Expression<Dummy>>
            {
                Expression<Dummy>
                    .Create(nameof(Dummy.ReturnMessage), new object[] {"ExpressionTest-1", "ExpressionTest-2"},
                        ExpressionTest<Dummy>.CreateTest("ExpressionTest-1 ExpressionTest-2"))
            };

            // Assert
            _tester.TestAndVerifyMethods(methods);
        }

        [Test]
        public void TestMethod_GetMessage()
        {
            ExpressionTest<Dummy>
                .CreateMethodExpression(nameof(Dummy.GetMessage), typeof(Dummy).GetFieldValue("_message"))
                .TestAndVerify();
        }

        [Test]
        public void TestMethod_GetMessage_WrongFieldName()
        {
            Assert.Throws<FieldNotFoundException>(
                () =>
                    ExpressionTest<Dummy>
                        .CreateMethodExpression(nameof(Dummy.GetMessage), typeof(Dummy).GetFieldValue("_wrong_field_name"))
                        .TestAndVerify());
        }

        [Test]
        public void TestMethod_GetSquareRoot()
        {
            ExpressionTest<Dummy>.CreateTest("GetSquareRoot()", 25.9036676939772).TestAndVerify();
        }

        [Test]
        public void TestMethod_MethodWithListAsParameter()
        {
            ExpressionTest<Dummy>
                .CreateMethodExpression(nameof(Dummy.MethodWithListAsParameter), "Test Output-0554 Test Output-1554").TestAndVerify();
        }

        [Test]
        public void TestMethod_ReturnCurrentInstance()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.ReturnCurrentInstance), "this").TestAndVerify();
        }

        [Test]
        public void TestMethod_SetMessage()
        {
            ExpressionTest<Dummy>.CreateWithOutputExpression("SetMessage", "_message", "@p0 + @p0").TestAndVerify();
        }

        [Test]
        public void TestMethod_GetException()
        {
            ExpressionTest<Dummy>.CreateWithOutputExpression("GetException", "_exception").TestAndVerify();
        }

        [Test]
        public void TestMethod_Empty()
        {
            ExpressionTest<Dummy>.CreateMethodExpression(nameof(Dummy.Empty), string.Empty).TestAndVerify();
        }

        [Test]
        public void TestMethods_UsingExpressions()
        {
            var methods = new List<Expression<Dummy>>
            {
                Expression<Dummy>.Create(nameof(GetType), ExpressionTest<Dummy>.CreateTest("Name", "Dummy")),
                Expression<Dummy>.Create(nameof(GetType), ExpressionTest<Dummy>.CreateTest("GetFields[0].Name", "DummyClassType"))
            };

            _tester.TestAndVerifyMethods(methods);
        }

        [Test]
        public void TestMethod_GetType()
        {
            ExpressionTest<Dummy>.CreateMethodExpression(nameof(GetType), "expunit.framework.tests.TestClass.Dummy").TestAndVerify();
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(GetType), "_type").TestAndVerify();
        }

        [Test]
        public void TestMethod_GetMessage_LengthExpression()
        {
            ExpressionTest<Dummy>.CreateWithOutputExpression($"{nameof(Dummy.GetMessage)}.Length", "_message.Length").TestAndVerify();
        }

        [Test]
        public void TestMethod_IsMessageNotNull()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.IsMessageNotNull), "_message != null").TestAndVerify();
        }

        [Test]
        public void TestMethod_IsMessageNull()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.IsMessageNull), "_message == null").TestAndVerify();
        }

        [Test]
        public void TestMethod_Message1PlusMessage2()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.Message1PlusMessage2), "_message == null ? _message2 : _message + _message2").TestAndVerify();
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.Message1PlusMessage2), "_message + _message2").TestAndVerify();
        }

        [Test]
        public void TestMethod_MessagesNotNull()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.MessagesNotNull), "_message != null && _message2 != null").TestAndVerify();
        }

        [Test]
        public void TestMethod_MessagesNull()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.MessagesNull), "_message == null && _message2 == null").TestAndVerify();
        }

        [Test]
        public void TestMethod_Equals()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.Equals), "_message.Equals(_message2)").TestAndVerify();
        }

        [Test]
        public void TestMethod_Multiplication()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.Multiplication), "_number1 * _number2").TestAndVerify();
        }

        [Test]
        public void TestMethod_Division()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.Division), "_number1 / _number2").TestAndVerify();
        }

        [Test]
        public void TestMethod_Sum()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.Sum), "_number1 + _number2").TestAndVerify();
        }

        [Test]
        public void TestMethod_Difference()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.Difference), "_number1 - _number2").TestAndVerify();
        }

        [Test]
        public void TestMethod_Mod()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.Mod), "_number1 % 2").TestAndVerify();
        }

        [Test]
        public void TestMethod_Even()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.Even), "Mod % 2 == 0").TestAndVerify();
        }

        [Test]
        public void TestMethod_NotEven()
        {
            ExpressionTest<Dummy>.CreateWithOutputExpression(nameof(Dummy.NotEven), "!Even").TestAndVerify();
        }

        [Test]
        public void TestMethod_Odd()
        {
            ExpressionTest<Dummy>.CreateWithOutputExpression(nameof(Dummy.Odd), "Mod % 2 == 1").TestAndVerify();
        }

        [Test]
        public void TestMethod_Less()
        {
            ExpressionTest<Dummy>.CreateWithOutputExpression(nameof(Dummy.Less), "_number1 < _number2").TestAndVerify();
        }

        [Test]
        public void TestMethod_NegativeNumber()
        {
            ExpressionTest<Dummy>.CreateWithOutputExpression(nameof(Dummy.NegativeNumber), "_number1 * -1").TestAndVerify();
        }

        [Test]
        public void TestMethod_Greater()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.Greater), "_number1 > _number2").TestAndVerify();
        }

        [Test]
        public void TestMethod_Increment()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.Increment), "++_number1").TestAndVerify();
        }

        [Test]
        public void TestMethod_Decrement()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.Decrement), "--_number1").TestAndVerify();
        }

        [Test]
        public void TestMethod_LessThanEqualTo()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.LessThanEqualTo), "_number1 <= _number2").TestAndVerify();
        }

        [Test]
        public void TestMethod_GreaterThanEqualTo()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.GreaterThanEqualTo), "_number1 >= _number2").TestAndVerify();
        }

        [Test]
        public void TestMethod_NotEqual()
        {
            ExpressionTest<Dummy>.CreateWithOutputExpression(nameof(Dummy.NotEqual), "_number1 != _number2").TestAndVerify();
        }

        [Test]
        public void TestMethod_Equal()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.Equals), "_number1 == _number2").TestAndVerify();
        }

        [Test]
        public void TestMethod_GetMessage_WithUserDefinedFields()
        {
            var classInfo = ClassInfo<Dummy>.Create(new Dictionary<string, object>
            {
                {"_message", "CustomMessage"}
            });

            ExpressionTest<Dummy>.CreateTest("_message", classInfo, "CustomMessage").TestAndVerify();
        }

        [Test]
        public void TestMethod_GetDummyTestStringArray()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.GetDummyTestStringArray), "_stringArray").TestAndVerify();
        }

        [Test]
        public void TestMethod_WithNullParameters()
        {
            Expression<Dummy>.CreateWithNullParameters("ReturnMessage2", ExpressionTest<Dummy>.CreateTest(null)).TestAndVerify();
        }

        [Test]
        public void TestMethod_WithUserDefinedParameters()
        {
            Expression<Dummy>.Create("ReturnMessage2", new object[] {"Test"}, ExpressionTest<Dummy>.CreateTest("Test")).TestAndVerify();
        }

        [Test]
        public void TestMethod_AddItem()
        {
            ExpressionTest<Dummy>.CreateTest(nameof(Dummy.AddItem), "_classList.Count", 26).TestAndVerify();

            TypeUtility.InitializeInstanceCollectionFields = false;

            ExpressionTest<Dummy>.CreateTest(nameof(Dummy.AddItem), "_classList.Count", 1).TestAndVerify();

            TypeUtility.InitializeInstanceCollectionFields = true;
        }

        [Test]
        public void TestMethod_AddItems()
        {
            ExpressionTest<Dummy>.CreateTest(nameof(Dummy.AddItems), "_classList.Count", 50).TestAndVerify();

            TypeUtility.InitializeInstanceCollectionFields = false;

            ExpressionTest<Dummy>.CreateTest(nameof(Dummy.AddItems), "_classList.Count", 25).TestAndVerify();

            TypeUtility.InitializeInstanceCollectionFields = true;
        }

        [Test]
        public void TestMethod_AddLongItem()
        {
            ExpressionTest<Dummy>.CreateTest(nameof(Dummy.AddLongItem), "_longList.Count", 26).TestAndVerify();

            TypeUtility.InitializeInstanceCollectionFields = false;

            ExpressionTest<Dummy>.CreateTest(nameof(Dummy.AddLongItem), "_longList.Count", 1).TestAndVerify();

            TypeUtility.InitializeInstanceCollectionFields = true;
        }

        [Test]
        public void TestMethod_AddLongItems()
        {
            ExpressionTest<Dummy>.CreateTest(nameof(Dummy.AddLongItems), "_longList.Count", 50).TestAndVerify();

            TypeUtility.InitializeInstanceCollectionFields = false;

            ExpressionTest<Dummy>.CreateTest(nameof(Dummy.AddLongItems), "_longList.Count", 25).TestAndVerify();

            TypeUtility.InitializeInstanceCollectionFields = true;
        }

        [Test]
        public void TestMethod_WrongMethod()
        {
            Assert.Throws<MethodNotFoundException>(
                () => ExpressionTest<Dummy>.CreateMethodExpression("WrongMethod", null).TestAndVerify());
        }

        [Test]
        public void TestMethod_ReturnMessage_WithWrongExpectedOutput()
        {
            Assert.Throws<NoMatchedException>(
                () => ExpressionTest<Dummy>.CreateMethodExpression(nameof(Dummy.ReturnMessage), "Wrong Expected Output").TestAndVerify());
        }

        [Test]
        public void TestMethod_CharAt1()
        {
            var classInfo = ClassInfo<Dummy>.Create(new Dictionary<string, dynamic>
            {
                {"_message", "012345789"}
            });

            ExpressionTest<Dummy>.CreateMethodExpression(nameof(Dummy.CharAt1), classInfo, "_message[1] == '1'").TestAndVerify();
        }

        [Test]
        public void TestMethod_ToString()
        {
            ExpressionTest<Dummy>.CreateWithOutputExpression(nameof(Dummy.ToString), "'_message=' + _message + 'message2=' + _message2 + 'number1=' + _number1")
                .TestAndVerify();
        }

        [Test]
        public void TestMethod_ReturnInteger()
        {
            ExpressionTest<Dummy>.CreateMethodOutputExpression(nameof(Dummy.ReturnInteger), "5").TestAndVerify();
        }

        [Test]
        public void TestMethod_ReturnCharacter()
        {
            ExpressionTest<Dummy>.CreateTest(nameof(Dummy.ReturnCharacter), 'a').TestAndVerify();
        }

        [Test]
        public void TestMethod_Message_ClassWithPrivateConstructor()
        {
            ExpressionTest<DummyWithPrivateConstructor>.CreateTest(nameof(DummyWithPrivateConstructor.Message), "Test_Message").TestAndVerify();
        }

        [Test]
        public void TestMethod_Message_ClassWithNoConstructor()
        {
            ExpressionTest<DummyWithNoConstructor>.CreateTest(nameof(DummyWithNoConstructor.Message), "Test_Message").TestAndVerify();
        }

        [Test]
        public void ThrowSystemException()
        {
            ExpressionTest<Dummy>.CreateMethodExpression(nameof(Dummy.ThrowSystemException), typeof(System.SystemException).Name).TestAndVerify();
        }
    }
}