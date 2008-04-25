using System;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class ExpressionReferenceTest : SnippetGenerationBaseTest
  {
    [Test]
    public void ExpressionReference ()
    {
      CustomMethodEmitter methodEmitter = GetMethodEmitter (false);
      methodEmitter.SetReturnType (typeof (string));
      
      ExpressionReference expressionReference = new ExpressionReference (typeof (string), new ConstReference ("bla").ToExpression(), methodEmitter);
      methodEmitter.ImplementByReturning (new ReferenceExpression (expressionReference));

      Assert.AreEqual ("bla", InvokeMethod());
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Expressions cannot be assigned to.")]
    public void ExpressionReferenceCannotBeStored ()
    {
      SuppressAssemblySave ();

      CustomMethodEmitter methodEmitter = GetMethodEmitter (false);
      ExpressionReference expressionReference = new ExpressionReference (typeof (string), new ConstReference ("bla").ToExpression (), methodEmitter);
      expressionReference.StoreReference (null);
    }

    [Test]
    public void LoadAddressOfExpressionReference ()
    {
      CustomMethodEmitter methodEmitter = GetMethodEmitter (false);
      methodEmitter.SetReturnType (typeof (string));

      ExpressionReference expressionReference =
          new ExpressionReference (typeof (StructWithMethod), new InitObjectExpression (methodEmitter, typeof (StructWithMethod)), methodEmitter);
      ExpressionReference addressReference =
          new ExpressionReference (typeof (StructWithMethod).MakeByRefType(), expressionReference.ToAddressOfExpression(), methodEmitter);
      MethodInvocationExpression methodCall =
          new MethodInvocationExpression (addressReference, typeof (StructWithMethod).GetMethod ("Method"));

      methodEmitter.ImplementByReturning (methodCall);

      Assert.AreEqual ("StructMethod", InvokeMethod());
    }
  }
}