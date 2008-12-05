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
using Remotion.Mixins.CodeGeneration.DynamicProxy;

namespace Remotion.UnitTests.Mixins.CodeGeneration.DynamicProxy
{
  [TestFixture]
  public class CodeGenerationSerializerUtilityTest : CodeGenerationBaseTest
  {
    [Test]
    public void DeclareAndFillArrayLocal()
    {
      var array = new[] { "1", "2", "3" };

      var module = ((ModuleManager) (SavedTypeBuilder.Scope)).Scope.ObtainDynamicModuleWithWeakName();
      var type = module.DefineType ("CodeGenerationSerializerUtilityTest.DeclareAndFillArrayLocal");
      var method = 
          type.DefineMethod ("Test", MethodAttributes.Public | MethodAttributes.Static, typeof (string[]), Type.EmptyTypes);
      var emitter = new MethodBuilderEmitter (method);

      var local = CodeGenerationSerializerUtility.DeclareAndFillArrayLocal (array, emitter.CodeBuilder, i => new ConstReference (i + "a").ToExpression ());
      emitter.CodeBuilder.AddStatement (new ReturnStatement (local));
      emitter.Generate ();

      Type compiledType = type.CreateType ();
      object result = compiledType.GetMethod ("Test").Invoke (null, null);

      Assert.That (result, Is.EqualTo (new[] { "1a", "2a", "3a" }));
    }
  }
}