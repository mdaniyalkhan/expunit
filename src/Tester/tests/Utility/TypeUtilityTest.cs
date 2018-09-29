using System;
using System.Collections.Generic;
using System.Numerics;
using expunit.framework.tests.TestClass;
using expunit.framework.Utility;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace expunit.framework.tests.Utility
{
    [TestFixture]
    public class TypeUtilityTest
    {
        [Test]
        public void Test_GetValue_ForStringType()
        {
            // Assert
            ((string)typeof(string).Get()).ShouldBe("s|t|r|i|n|g-631");
        }

        [Test]
        public void Test_GetValue_ForSByteType()
        {
            // Assert
            ((sbyte)typeof(sbyte).Get()).ShouldBe((sbyte)83);
        }

        [Test]
        public void Test_GetValue_ForShortType()
        {
            // Assert
            ((short)typeof(short).Get()).ShouldBe((short)402);
        }

        [Test]
        public void Test_GetValue_ForUShortType()
        {
            // Assert
            ((ushort)typeof(ushort).Get()).ShouldBe((ushort)487);
        }

        [Test]
        public void Test_GetValue_ForIntegerType()
        {
            // Assert
            ((int)typeof(int).Get()).ShouldBe(400);
        }

        [Test]
        public void Test_GetValue_ForUIntegerType()
        {
            // Assert
            ((uint)typeof(uint).Get()).ShouldBe(485u);
        }

        [Test]
        public void Test_GetValue_ForLongType()
        {
            // Assert
            ((long)typeof(long).Get()).ShouldBe(405L);
        }

        [Test]
        public void Test_GetValue_ForULongType()
        {
            // Assert
            ((ulong)typeof(ulong).Get()).ShouldBe(490uL);
        }

        [Test]
        public void Test_GetValue_ForDoubleType()
        {
            // Assert
            ((double)typeof(double).Get()).ShouldBe(603d);
        }

        [Test]
        public void Test_GetValue_ForFloatType()
        {
            // Assert
            ((float)typeof(float).Get()).ShouldBe(610f);
        }

        [Test]
        public void Test_GetValue_ForDecimalType()
        {
            // Assert
            ((decimal)typeof(decimal).Get()).ShouldBe(687m);
        }

        [Test]
        public void Test_GetValue_ForBigIntegerType()
        {
            // Assert
            ((BigInteger)typeof(BigInteger).Get()).ShouldBe(new BigInteger(992));
        }

        [Test]
        public void Test_GetValue_ForBoolType()
        {
            // Assert
            ((bool)typeof(bool).Get()).ShouldBeTrue();
        }

        [Test]
        public void Test_GetValue_ForDateTimeType()
        {
            // Assert
            ((DateTime)typeof(DateTime).Get()).ShouldBe(new DateTime(2781, 12, 27, 23, 59, 59));
        }

        [Test]
        public void Test_GetValue_ForCharType()
        {
            // Assert
            ((char)typeof(char).Get()).ShouldBe('ž');
        }

        [Test]
        public void Test_GetValue_ForUri()
        {
            // Assert
            ((Uri)typeof(Uri).Get()).ShouldBe(new Uri("https://www.google.com.pk/FieldIndex=uri-304"));
        }

        [Test]
        public void Test_CreateInstanceOfTypeHavingDefaultConstructor()
        {
            // Assert
            ((Dummy)typeof(Dummy).CreateInstanceOfTypeHavingDefaultConstructor()).ShouldNotBeNull();
        }

        [Test]
        public void Test_CreateUninitializedObject()
        {
            // Assert
            ((Dummy)typeof(Dummy).CreateUninitializedObject()).ShouldNotBeNull();
        }

        [Test]
        public void Test_GetMock()
        {
            // Assert
            Mock<Dummy> mock = typeof(Dummy).GetMock();
            mock.ShouldNotBeNull();
        }

        [Test]
        public void Test_IsACollection()
        {
            // Assert
            typeof(List<int>).IsACollection().ShouldBeTrue();
            new Dummy[1].GetType().IsACollection().ShouldBeTrue();
        }

        [Test]
        public void Test_GetFirstObjectFromCollection()
        {
            // Arrange
            var integers = new List<int> { 1, 2, 3 };
            var strings = new[] { "A", "B", "C" };

            // Assert
            ((int)TypeUtility.GetFirstObjectFromCollection(integers)).ShouldBe(1);
            ((string)TypeUtility.GetFirstObjectFromCollection(strings)).ShouldBe("A");
        }
    }
}