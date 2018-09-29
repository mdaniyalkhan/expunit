using System.Text.RegularExpressions;
using expunit.framework.Exp;
using expunit.framework.Utility;

namespace expunit.framework.Model
{
    public class ExpressionTest<T>
    {
        public string Name { get; set; }
        public ClassInfo<T> Target { get; set; }
        public string[] Expressions { get; set; }
        public object ClassInstance { get; private set; }
        public Expression<T> ExpressionInfo { get; set; }
        public object[] ExpectedOutputs { get; set; }
        public object[] ActualOutputs { get; set; }
        public string[] ErrorMessages { get; set; }
        public string[] OutputExpressions { get; set; }

        private object _targetInstance;

        public object TargetInstance
        {
            get => _targetInstance;
            set
            {
                if (ClassInstance == null)
                {
                    ClassInstance = value;
                }

                _targetInstance = value;
            }
        }

        private ExpressionTest(ClassInfo<T> target)
        {
            Target = target;
            ErrorMessages = new string[1];
            ActualOutputs = new object[1];
        }

        /// <summary>
        ///    Create ExpressionTest instance usually used to test expression with primitive, instances or primitive collections return type
        /// </summary>
        /// <param name="expectedOutput">Expected output of expression</param>
        /// <returns>ExpressionTest object</returns>
        public static ExpressionTest<T> CreateTest(object expectedOutput)
        {
            return CreateTest(ClassInfo<T>.Create(), expectedOutput);
        }

        /// <summary>
        ///    Create ExpressionTest instance usually used to test expression with primitive, instances or primitive collections return type
        /// </summary>
        /// <param name="target">Target class in which expression reside</param>
        /// <param name="expectedOutput">Expected output of expression</param>
        /// <returns>ExpressionTest object</returns>
        public static ExpressionTest<T> CreateTest(ClassInfo<T> target, object expectedOutput)
        {
            return new ExpressionTest<T>(target)
            {
                ExpectedOutputs = new[] { expectedOutput }
            };
        }

        /// <summary>
        ///    Create ExpressionTest instance usually used to test expression with specific class return type
        /// </summary>
        /// <param name="expression">Field or property name separated by '.'</param>
        /// <param name="target">Target class in which expression reside</param>
        /// <param name="expectedOutput">Expected output of expression</param>
        /// <returns>ExpressionTest object</returns>
        public static ExpressionTest<T> CreateTest(string expression, ClassInfo<T> target, object expectedOutput)
        {
            return new ExpressionTest<T>(target)
            {
                Expressions = new[] { expression },
                ExpectedOutputs = new[] { expectedOutput }
            };
        }

        /// <summary>
        ///    Create ExpressionTest instance usually used to test expression with specific class return type
        /// </summary>
        /// <param name="expression">Field or property name separated by '.'</param>
        /// <param name="expectedOutput">Expected output of expression</param>
        /// <returns>ExpressionTest object</returns>
        public static ExpressionTest<T> CreateTest(string expression, object expectedOutput)
        {
            return new ExpressionTest<T>(ClassInfo<T>.Create())
            {
                Expressions = new[] { expression },
                ExpectedOutputs = new[] { expectedOutput }
            };
        }

        /// <summary>
        ///    Create ExpressionTest instance usually used to test expression with specific class return type
        /// </summary>
        /// <param name="method">Field or property name separated by '.'</param>
        /// <param name="expectedOutput">Expected output of expression</param>
        /// <returns>ExpressionTest object</returns>
        public static Expression<T> CreateMethodExpression(string method, object expectedOutput)
        {
            return Expression<T>.Create(method, CreateTest(expectedOutput));
        }

        /// <summary>
        ///    Create ExpressionTest instance usually used to test expression with specific class return type
        /// </summary>
        /// <param name="method">Method Name</param>
        /// <param name="expression">Field or property name separated by '.'</param>
        /// <param name="expectedOutput">Expected output of expression</param>
        /// <returns>ExpressionTest object</returns>
        public static Expression<T> CreateTest(string method, string expression, object expectedOutput)
        {
            return Expression<T>.Create(method, CreateTest(expression, expectedOutput));
        }

        /// <summary>
        ///     Create ExpressionTest instance usually used to test expression output with expression 1st parameter value e.g. setters
        /// </summary>
        /// <param name="expression">Field or property name separated by '.'</param>
        /// <returns>ExpressionTest object</returns>
        public static ExpressionTest<T> CreateWithExpression(string expression)
        {
            return new ExpressionTest<T>(ClassInfo<T>.Create())
            {
                Expressions = new[] { expression },
                OutputExpressions = new[] { "@p0" }
            };
        }

        /// <summary>
        ///     Create ExpressionTest instance usually used to test expression output with expression 1st parameter value e.g. setters
        /// </summary>
        /// <param name="expression">Field or property name separated by '.'</param>
        /// <param name="target">Target class in which expression reside</param>
        /// <returns>ExpressionTest object</returns>
        public static ExpressionTest<T> CreateTest(string expression, ClassInfo<T> target)
        {
            return new ExpressionTest<T>(target)
            {
                Expressions = new[] { expression },
                OutputExpressions = new[] { "@p0" }
            };
        }

        /// <summary>
        ///      Create ExpressionTest instance usually used to test expression with specific class return type
        /// </summary>
        /// <param name="expressions">Field or property name separated by '.'</param>
        /// <param name="expectedOutputs">Expected output of expressions</param>
        /// <returns>ExpressionTest object</returns>
        public static ExpressionTest<T> CreateTest(string[] expressions, object[] expectedOutputs)
        {
            return CreateTest(expressions, ClassInfo<T>.Create(), expectedOutputs);
        }

        /// <summary>
        ///      Create ExpressionTest instance usually used to test expression with specific class return type
        /// </summary>
        /// <param name="expressions">Field or property name separated by '.'</param>
        /// <param name="target">Target class in which expression reside</param>
        /// <param name="expectedOutputs">Expected output of expressions</param>
        /// <returns>ExpressionTest object</returns>
        public static ExpressionTest<T> CreateTest(string[] expressions, ClassInfo<T> target, object[] expectedOutputs)
        {
            return new ExpressionTest<T>(target)
            {
                Expressions = expressions,
                ExpectedOutputs = expectedOutputs,
                ErrorMessages = new string[expectedOutputs.Length],
                ActualOutputs = new object[expectedOutputs.Length]
            };
        }

        /// <summary>
        ///    Create ExpressionTest instance usually used to test expression with primitive, instances or primitive collections return type
        /// </summary>
        /// <param name="outputExpression">Expression e.g. field, expression or property to get Expected output of expression</param>
        /// <returns>ExpressionTest object</returns>
        public static ExpressionTest<T> CreateWithOutputExpression(string outputExpression)
        {
            return CreateWithOutputExpression(ClassInfo<T>.Create(), outputExpression);
        }

        /// <summary>
        ///    Create ExpressionTest instance usually used to test expression with primitive, instances or primitive collections return type
        /// </summary>
        /// <param name="target"> Target class in which expression reside</param>
        /// <param name="outputExpression">Expression e.g. field, expression or property to get Expected output of expression</param>
        /// <returns>ExpressionTest object</returns>
        public static ExpressionTest<T> CreateWithOutputExpression(ClassInfo<T> target, string outputExpression)
        {
            return new ExpressionTest<T>(target)
            {
                OutputExpressions = new[] { outputExpression }
            };
        }

        /// <summary>
        ///    Create ExpressionTest instance usually used to test expression with primitive, instances or primitive collections return type
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="outputExpression">Expression e.g. field, expression or property to get Expected output of expression</param>
        /// <returns>ExpressionTest object</returns>
        public static ExpressionTest<T> CreateWithOutputExpression(string expression, string outputExpression)
        {
            return CreateWithOutputExpression(expression, ClassInfo<T>.Create(), outputExpression);
        }

        /// <summary>
        ///    Create ExpressionTest instance usually used to test expression with primitive, instances or primitive collections return type
        /// </summary>
        /// <param name="method"></param>
        /// <param name="outputExpression">Expression e.g. field, expression or property to get Expected output of expression</param>
        /// <returns>ExpressionTest object</returns>
        public static Expression<T> CreateMethodOutputExpression(string method, string outputExpression)
        {
            return Expression<T>.Create(method, CreateWithOutputExpression(ClassInfo<T>.Create(), outputExpression));
        }


        /// <summary>
        ///    Create ExpressionTest instance usually used to test expression with primitive, instances or primitive collections return type
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="target"> Target class in which expression reside</param>
        /// <param name="outputExpression">Expression e.g. field, expression or property to get Expected output of expression</param>
        /// <returns>ExpressionTest object</returns>
        public static ExpressionTest<T> CreateWithOutputExpression(string expression, ClassInfo<T> target, string outputExpression)
        {
            return new ExpressionTest<T>(target)
            {
                Expressions = new[] { expression },
                OutputExpressions = new[] { outputExpression }
            };
        }

        /// <summary>
        ///    Create ExpressionTest instance usually used to test expression with primitive, instances or primitive collections return type
        /// </summary>
        /// <param name="method">Method Name</param>
        /// <param name="target"> Target class in which expression reside</param>
        /// <param name="outputExpression">Expression e.g. field, expression or property to get Expected output of expression</param>
        /// <returns>ExpressionTest object</returns>
        public static Expression<T> CreateMethodExpression(string method, ClassInfo<T> target, string outputExpression)
        {
            return Expression<T>.Create(method, CreateWithOutputExpression(target, outputExpression));
        }

        /// <summary>
        ///     Create ExpressionTest instance usually used to test expression with primitive, instances
        /// </summary>
        /// <param name="expressions">Field or property name separated by '.'</param>
        /// <param name="outputExpressions">Expression e.g. field, expression or property to get Expected output of expression</param>
        /// <returns>ExpressionTest object</returns>
        public static ExpressionTest<T> CreateWithOutputExpression(string[] expressions, string[] outputExpressions)
        {
            return CreateWithOutputExpression(expressions, ClassInfo<T>.Create(), outputExpressions);
        }

        /// <summary>
        ///     Create ExpressionTest instance usually used to test expression with primitive, instances
        /// </summary>
        /// <param name="expressions">Field Method or property name separated by '.'</param>
        /// <param name="target">Target class in which expression reside</param>
        /// <param name="outputExpressions">Expression e.g. field, expression or property to get Expected output of expression</param>
        /// <returns>ExpressionTest object</returns>
        public static ExpressionTest<T> CreateWithOutputExpression(string[] expressions, ClassInfo<T> target, string[] outputExpressions)
        {
            return new ExpressionTest<T>(target)
            {
                Expressions = expressions,
                OutputExpressions = outputExpressions,
                ErrorMessages = new string[outputExpressions.Length],
                ActualOutputs = new object[outputExpressions.Length]
            };
        }

        /// <summary>
        ///     Create ExpressionTest instance usually used to test expression with primitive, instances
        /// </summary>
        /// <param name="expression">Field, Method or property name separated by '.'</param>
        /// <param name="outputExpression">Expression e.g. field, expression or property to get Expected output of expression</param>
        /// <param name="method">Method name</param>
        /// <returns>ExpressionTest object</returns>
        public static Expression<T> CreateWithOutputExpression(string method, string expression, string outputExpression)
        {
            return Expression<T>.Create(method, CreateWithOutputExpression(expression, outputExpression));
        }

        /// <summary>
        ///   Set Error Messages
        /// </summary>
        public void SetErrorMessages()
        {
            for (int i = 0; i < ExpectedOutputs.Length; i++)
            {
                var expression = string.Empty;
                if (Expressions != null && OutputExpressions != null)
                {
                    expression = $"Testing Expression [{Expressions[i]}] and Output Expression [{OutputExpressions[i]}]";
                }

                var targetMember = string.IsNullOrEmpty(TargetMembers()[i]) ?
                    string.Empty :
                    TargetMembers()[i];
                ErrorMessages[i] =
                    $"[{Name} class [{Target.Type.FullName}] - {expression} Expected {targetMember} value: {SerializeUtility.Serialize(ExpectedOutputs[i])} != Actual {targetMember} value: {SerializeUtility.Serialize(ActualOutputs[i])}]";
            }
        }

        private string[] TargetMembers()
        {
            if (Expressions != null)
            {
                var targetMembers = new string[Expressions.Length];

                for (int x = 0; x < Expressions.Length; x++)
                {
                    targetMembers[x] = !string.IsNullOrEmpty(Expressions[x]) ?
                        Expressions[x] :
                        string.Empty;
                }

                return targetMembers;
            }

            return new string[ExpectedOutputs.Length];
        }

        /// <summary>
        ///     Verify expression expected output equals to actual output
        /// </summary>
        public void TestAndVerify()
        {
            Test();
            Verify();
        }

        /// <summary>
        ///     Test expression and set actual output
        /// </summary>
        public void Test()
        {
            if (TargetInstance == null)
            {
                TargetInstance = Target.Type.CreateInstance(Target.ClassFields);
            }
            SetActualOutputs();
        }

        /// <summary>
        ///     Set test actual outputs
        /// </summary>
        public void SetActualOutputs()
        {
            var expressions = Expressions;
            object targetInstance = TargetInstance;
            if (OutputExpressions != null)
            {
                Expressions = OutputExpressions;
                TargetInstance = ClassInstance;
                ExpectedOutputs = this.ResolveExpressionValue();
                Expressions = expressions;
                TargetInstance = targetInstance;
            }

            this.SetTestActualOutput(this.ResolveExpressionValue());
            SetErrorMessages();
        }

        /// <summary>
        ///     Verify expression expected output equals to actual output
        /// </summary>
        /// <param name="message"></param>
        public void Verify(string message = "")
        {
            for (int i = 0; i < ExpectedOutputs.Length; i++)
            {
                if (ExpectedOutputs[i] is char)
                {
                    ExpectedOutputs[i] = TypeUtility.CastValueWithRespectToType(ExpectedOutputs[i], typeof(int));
                }

                if (ExpressionInfo?.ReturnType == typeof(char))
                {
                    ExpectedOutputs[i] = TypeUtility.CastValueWithRespectToType(ExpectedOutputs[i], typeof(char));
                }

                if (!Regex.Replace(SerializeUtility.Serialize(ExpectedOutputs[i]), "\\\\u", "\\u").Equals(
                    Regex.Replace(SerializeUtility.Serialize(ActualOutputs[i]), "\\\\u", "\\u")))
                {
                    string methodName = string.IsNullOrEmpty(message) ?
                        "" :
                        $"Testing Expression: {message}";

                    throw new NoMatchedException($"{methodName} {ErrorMessages[i]}");
                }
            }
        }
    }
}