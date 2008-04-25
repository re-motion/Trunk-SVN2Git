using System;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class PopStatementTest : SnippetGenerationBaseTest
  {
    [Test]
    public void Pop ()
    {
      CustomMethodEmitter methodEmitter = GetMethodEmitter (false);
      methodEmitter.AddStatement (new ILStatement (delegate (IMemberEmitter emitter, ILGenerator gen) { gen.Emit (OpCodes.Ldc_I4_0); }));
      methodEmitter.AddStatement (new PopStatement ());
      methodEmitter.AddStatement (new ReturnStatement ());

      InvokeMethod ();
    }
  }
}