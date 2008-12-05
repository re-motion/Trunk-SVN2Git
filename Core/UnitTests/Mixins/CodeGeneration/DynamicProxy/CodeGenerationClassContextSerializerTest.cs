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
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Context;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.DynamicProxy
{
  [TestFixture]
  public class CodeGenerationClassContextSerializerTest : CodeGenerationBaseTest
  {
    [Test]
    public void IntegrationTest()
    {
      var referenceMixinContext1 = new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private, new[] { typeof (string), typeof (double) });
      var referenceMixinContext2 = new MixinContext (MixinKind.Used, typeof (BT1Mixin2), MemberVisibility.Public);
      var referenceClassContext = new ClassContext (typeof (BaseType1), new[] { referenceMixinContext1, referenceMixinContext2 }, new[] { typeof (DateTime) });

      var module = ((ModuleManager) (SavedTypeBuilder.Scope)).Scope.ObtainDynamicModuleWithWeakName ();
      var type = module.DefineType ("CodeGenerationClassContextSerializerTest.IntegrationTest");
      var method =
          type.DefineMethod ("Test", MethodAttributes.Public | MethodAttributes.Static, typeof (ClassContext), Type.EmptyTypes);
      var emitter = new MethodBuilderEmitter (method);

      var serializer = new CodeGenerationClassContextSerializer (emitter.CodeBuilder);
      referenceClassContext.Serialize (serializer);

      emitter.CodeBuilder.AddStatement (new ReturnStatement (serializer.GetConstructorInvocationExpression ()));
      emitter.Generate ();

      Type compiledType = type.CreateType ();
      object result = compiledType.GetMethod ("Test").Invoke (null, null);

      Assert.That (result, Is.EqualTo (referenceClassContext));
    }
  }
}