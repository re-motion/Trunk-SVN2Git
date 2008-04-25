using System;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class LdArrayElementTest : SnippetGenerationBaseTest
  {
    [Test]
    public void LoadArrayElementFromExpression ()
    {
      CustomMethodEmitter method = GetMethodEmitter (false);
      method.SetParameterTypes (new Type[] { typeof (IArrayProvider), typeof (int) });
      method.SetReturnType (typeof (object));
      method.AddStatement (new ILStatement (delegate (IMemberEmitter member, ILGenerator ilgen)
      {
        ilgen.Emit (OpCodes.Ldarg_1); // array provider
        ilgen.Emit (OpCodes.Callvirt, typeof (IArrayProvider).GetMethod ("GetArray")); // array
        ilgen.Emit (OpCodes.Castclass, typeof (object[])); // essentially a nop
        ilgen.Emit (OpCodes.Ldarg_2); // index
        ilgen.Emit (OpCodes.Ldelem, typeof (object));
        ilgen.Emit (OpCodes.Castclass, typeof (object));
        ilgen.Emit (OpCodes.Ret);
      }));

      SimpleArrayProvider provider = new SimpleArrayProvider();
      object result = InvokeMethod (provider, 1);
      Assert.AreEqual (2, result);
    }
  }
}