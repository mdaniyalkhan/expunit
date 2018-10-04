using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace expunit.framework.tests.TestClass
{
    [ExcludeFromCodeCoverage]
    public class Dummy
    {
        public static readonly Type DummyClassType;
        private int _number1;
        private int _number2;
        private Type _type;
        private Dummy[] _dummyTestArray;
        private Exception _exception;
        private readonly string[] _stringArray;
        private double[] _doubleArray;
        private int[] _intArray;
        private float[] _floatArray;
        private short[] _shortArray;
        private byte[] _byteArray;
        private sbyte[] _sbyteArray;
        private char[] _charArray;
        private long[] _longArray;
        private bool[] _boolArray;
        private List<Dummy> _classList;
        private List<long> _longList;
        private Dictionary<string, string> _dictionary;
        private Dictionary<string, List<Dummy>> _mapListField;
        private string _message = null;
        private Guid _cguid;
        private readonly string _message2 = null;

        public static string Empty()
        {
            return string.Empty;
        }

        public static Type GetDummyClassType()
        {
            return DummyClassType;
        }

        private static double GetSquareRoot(double number)
        {
            return Math.Sqrt(number);
        }

        public void SetLongList(List<Int64> dummyLongList)
        {
            _longList = dummyLongList;
        }

        public string[] GetDummyTestStringArray()
        {
            return _stringArray;
        }

        public void SetDoubleArray(double[] array)
        {
            _doubleArray = array;
        }

        public void SetIntArray(int[] array)
        {
            _intArray = array;
        }

        public void SetFloatArray(Single[] array)
        {
            _floatArray = array;
        }

        public void SetShortArray(short[] array)
        {
            _shortArray = array;
        }

        public void SetByteArray(byte[] array)
        {
            _byteArray = array;
        }

        public void SetCharArray(char[] array)
        {
            _charArray = array;
        }

        public void SetLongArray(long[] array)
        {
            _longArray = array;
        }

        public void SetBoolArray(bool[] array)
        {
            _boolArray = array;
        }

        public int ReturnZero()
        {
            return 0;
        }

        public string GetMessage()
        {
            return _message;
        }

        public void SetMessage(string message)
        {
            _message = message + message;
        }

        public bool IsMessageNotNull()
        {
            return _message != null;
        }

        public bool IsMessageNull()
        {
            return _message == null;
        }

        public bool CharAt1()
        {
            return _message[1] == '1';
        }

        public bool ThrowSystemException()
        {
            throw new SystemException();
        }

        public string Message1PlusMessage2()
        {
            return _message == null ? _message2 : _message + _message2;
        }

        public override string ToString()
        {
            return "_message=" + _message + "message2=" + _message2 + "number1=" + _number1;
        }

        public bool MessagesNotNull()
        {
            return _message != null && _message2 != null;
        }

        public bool MessagesNull()
        {
            return !(_message != null && _message2 != null);
        }

        public bool Equals()
        {
            return _message.Equals(_message2);
        }

        public int Multiplication()
        {
            return _number1 * _number2;
        }

        public int Division()
        {
            return _number1 / _number2;
        }

        public int Sum()
        {
            return _number1 + _number2;
        }

        public int Difference()
        {
            return _number1 - _number2;
        }

        public int Increment()
        {
            return ++_number1;
        }

        public int Decrement()
        {
            return --_number1;
        }

        public int Mod()
        {
            return _number1 % 2;
        }

        public bool Even()
        {
            return Mod() == 0;
        }

        public bool NotEven()
        {
            return !Even();
        }

        public bool Odd()
        {
            return Mod() == 1;
        }

        public int NegativeNumber()
        {
            return _number1 * -1;
        }

        public bool Less()
        {
            return _number1 < _number2;
        }

        public bool Greater()
        {
            return _number1 > _number2;
        }

        public bool LessThanEqualTo()
        {
            return _number1 <= _number2;
        }

        public bool GreaterThanEqualTo()
        {
            return _number1 >= _number2;
        }

        public bool NotEqual()
        {
            return _number1 != _number2;
        }

        public bool Equal()
        {
            return _number1 == _number2;
        }

        public void AddItem(Dummy item)
        {
            if (_classList == null)
            {
                _classList = new List<Dummy>();
            }

            _classList.Add(item);
        }

        public void AddItems(List<Dummy> items)
        {
            if (_classList == null)
            {
                _classList = new List<Dummy>();
            }

            _classList.AddRange(items);
        }

        public void AddLongItem(long item)
        {
            if (_longList == null)
            {
                _longList = new List<long>();
            }

            _longList.Add(item);
        }

        public void AddLongItems(List<long> items)
        {
            if (_longList == null)
            {
                _longList = new List<long>();
            }

            _longList.AddRange(items);
        }

        private string ReturnMessage2(string message)
        {
            return message;
        }

        public Dummy ReturnCurrentInstance()
        {
            return this;
        }

        public Exception GetException()
        {
            return _exception;
        }

        public string ReturnMessage(string message)
        {
            return message;
        }

        private string ReturnMessage(string message, string message2)
        {
            return $"{message} {message2}";
        }

        public string MethodWithListAsParameter(List<string> names)
        {
            return $"{names[0]} {names[1]}";
        }

        public Dummy MethodWithCustomListAsParameter(List<Dummy> names)
        {
            return new Dummy();
        }

        public Dummy ReturnDummyTestClass(string message, Dummy testClass)
        {
            testClass.SetMessage(message);
            return testClass;
        }

        public Dummy ReturnNewDummyClass()
        {
            return new Dummy();
        }

        public char ReturnInteger()
        {
            return (char) 5;
        }

        public int ReturnCharacter()
        {
            return 'a';
        }
    }

    [ExcludeFromCodeCoverage]
    public class DummyWithPrivateConstructor
    {
        private DummyWithPrivateConstructor()
        {
            
        }

        public string Message()
        {
            return "Test_Message";
        }
    }

    [ExcludeFromCodeCoverage]
    public class DummyWithNoConstructor
    {
        private DummyWithNoConstructor()
        {
            throw new UnauthorizedAccessException("Unauthorized Access");
        }

        public string Message()
        {
            return "Test_Message";
        }
    }
}