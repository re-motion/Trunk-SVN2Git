/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
