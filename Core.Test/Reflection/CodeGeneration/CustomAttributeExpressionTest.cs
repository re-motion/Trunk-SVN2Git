using System;
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Collections;
using Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes;
using Remotion.Utilities;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class CustomAttributeExpressionTest : SnippetGenerationBaseTest
  {
    [Test]
    public void CustomAttributeExpression ()
    {
      CustomMethodEmitter methodEmitter = GetMethodEmitter(false)
          .SetReturnType (typeof (Tuple<SimpleAttribute, SimpleAttribute>));

      LocalReference attributeOwner = methodEmitter.DeclareLocal (typeof (Type));
      methodEmitter.AddStatement (new AssignStatement (attributeOwner, new TypeTokenExpression (typeof (ClassWithCustomAttribute))));

      ConstructorInfo tupleCtor =
          typeof (Tuple<SimpleAttribute, SimpleAttribute>).GetConstructor (new Type[] {typeof (SimpleAttribute), typeof (SimpleAttribute)});
      Expression tupleExpression = new NewInstanceExpression (tupleCtor,
          new CustomAttributeExpression (attributeOwner, typeof (SimpleAttribute), 0, true),
          new CustomAttributeExpression (attributeOwner, typeof (SimpleAttribute), 1, true));

      methodEmitter.AddStatement (new ReturnStatement (tupleExpression));

      object[] attributes = typeof (ClassWithCustomAttribute).GetCustomAttributes (typeof (SimpleAttribute), true);

      Tuple<SimpleAttribute, SimpleAttribute> attributeTuple = (Tuple<SimpleAttribute, SimpleAttribute>) InvokeMethod();
      Assert.That (new object[] { attributeTuple.A, attributeTuple.B }, Is.EquivalentTo (attributes));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = "Argument attributeOwner is a System.String, which cannot be assigned "
        + "to type System.Reflection.ICustomAttributeProvider.\r\nParameter name: attributeOwner")]
    public void CustomAttributeExpressionThrowsOnWrongReferenceType ()
    {
      SuppressAssemblySave ();
      new CustomAttributeExpression (new LocalReference (typeof (string)), typeof (SimpleAttribute), 0, true);
    }
  }
}