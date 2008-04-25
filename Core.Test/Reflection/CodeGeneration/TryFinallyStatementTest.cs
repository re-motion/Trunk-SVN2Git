using System;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class TryFinallyStatementTest : SnippetGenerationBaseTest
  {
    [Test]
    public void TryFinallyWithoutException ()
    {
      FieldReference tryField = ClassEmitter.CreateField ("TryExecuted", typeof (bool));
      FieldReference finallyField = ClassEmitter.CreateField ("FinallyExecuted", typeof (bool));

      CustomMethodEmitter methodEmitter = GetMethodEmitter (false);
      Statement[] tryBlock = new Statement[]
      {
        new AssignStatement (tryField, new ConstReference (true).ToExpression())
      };
      Statement[] finallyBlock = new Statement[]
      {
        new AssignStatement (finallyField, new ConstReference (true).ToExpression())
      };

      methodEmitter.AddStatement (new TryFinallyStatement (tryBlock, finallyBlock));
      methodEmitter.AddStatement (new ReturnStatement ());

      InvokeMethod ();
      Assert.IsTrue ((bool) PrivateInvoke.GetPublicField (GetBuiltInstance (), tryField.Reference.Name));
      Assert.IsTrue ((bool) PrivateInvoke.GetPublicField (GetBuiltInstance (), finallyField.Reference.Name));
    }

    [Test]
    public void TryFinallyWithException ()
    {
      FieldReference tryField = ClassEmitter.CreateField ("TryExecuted", typeof (bool));
      FieldReference finallyField = ClassEmitter.CreateField ("FinallyExecuted", typeof (bool));

      CustomMethodEmitter methodEmitter = GetMethodEmitter (false);
      Statement[] tryBlock = new Statement[]
      {
        new ThrowStatement (typeof (Exception), "Expected exception"),
        new AssignStatement (tryField, new ConstReference (true).ToExpression())
      };
      Statement[] finallyBlock = new Statement[]
      {
        new AssignStatement (finallyField, new ConstReference (true).ToExpression())
      };

      methodEmitter.AddStatement (new TryFinallyStatement (tryBlock, finallyBlock));
      methodEmitter.AddStatement (new ReturnStatement ());

      try
      {
        InvokeMethod ();
        Assert.Fail ("Expected exception");
      }
      catch (Exception ex)
      {
        Assert.AreEqual (typeof (Exception), ex.GetType ());
        Assert.AreEqual ("Expected exception", ex.Message);
      }
      Assert.IsFalse ((bool) PrivateInvoke.GetPublicField (GetBuiltInstance (), tryField.Reference.Name));
      Assert.IsTrue ((bool) PrivateInvoke.GetPublicField (GetBuiltInstance (), finallyField.Reference.Name));
    }
  }
}