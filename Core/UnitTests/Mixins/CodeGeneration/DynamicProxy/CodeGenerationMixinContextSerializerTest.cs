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
  public class CodeGenerationMixinContextSerializerTest : CodeGenerationBaseTest
  {
    [Test]
    public void IntegrationTest()
    {
      var referenceContext = new MixinContext (MixinKind.Extending, typeof (int), MemberVisibility.Private, new[] {typeof (string), typeof (double)});

      var module = ((ModuleManager) (SavedTypeBuilder.Scope)).Scope.ObtainDynamicModuleWithWeakName ();
      var type = module.DefineType ("CodeGenerationMixinContextSerializerTest.IntegrationTest");
      var method =
          type.DefineMethod ("Test", MethodAttributes.Public | MethodAttributes.Static, typeof (MixinContext), Type.EmptyTypes);
      var emitter = new MethodBuilderEmitter (method);

      var serializer = new CodeGenerationMixinContextSerializer (emitter.CodeBuilder);
      referenceContext.Serialize (serializer);

      emitter.CodeBuilder.AddStatement (new ReturnStatement (serializer.GetConstructorInvocationExpression ()));
      emitter.Generate ();

      Type compiledType = type.CreateType ();
      object result = compiledType.GetMethod ("Test").Invoke (null, null);

      Assert.That (result, Is.EqualTo (referenceContext));
    }
  }
}