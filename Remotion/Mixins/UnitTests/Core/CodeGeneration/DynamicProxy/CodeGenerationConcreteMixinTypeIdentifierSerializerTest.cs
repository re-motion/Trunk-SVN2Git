// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.DynamicProxy
{
  [TestFixture]
  public class CodeGenerationConcreteMixinTypeIdentifierSerializerTest : CodeGenerationBaseTest
  {
    private MethodInfo _simpleMethod1;
    private MethodInfo _simpleMethod2;
    private MethodInfo _genericMethod;
    private MethodInfo _methodOnGenericClosedWithReferenceType;
    private MethodInfo _methodOnGenericClosedWithValueType;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      _simpleMethod1 = typeof (BT1Mixin1).GetMethod ("VirtualMethod");
      _simpleMethod2 = typeof (BT1Mixin2).GetMethod ("VirtualMethod");
      _genericMethod = typeof (BaseType7).GetMethod ("One");
      _methodOnGenericClosedWithReferenceType = typeof (GenericClassWithAllKindsOfMembers<string>).GetMethod ("Method");
      _methodOnGenericClosedWithValueType = typeof (GenericClassWithAllKindsOfMembers<int>).GetMethod ("Method");
    }

    [Test]
    public void IntegrationTest()
    {
      TypeBuilder type = DefineType ("IntegrationTest");
      MethodBuilderEmitter emitter = DefineMethod (type, typeof (ConcreteMixinTypeIdentifier));

      var referenceIdentifier = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          new HashSet<MethodInfo> { _simpleMethod1 },
          new HashSet<MethodInfo> { _simpleMethod2 });

      var serializer = new CodeGenerationConcreteMixinTypeIdentifierSerializer (emitter.CodeBuilder);
      referenceIdentifier.Serialize (serializer);

      object result = BuildTypeAndInvokeMethod(type, emitter, serializer.GetConstructorInvocationExpression ());

      Assert.That (result, Is.EqualTo (referenceIdentifier));
    }

    [Test]
    public void IntegrationTest_MethodsOnGenericType ()
    {
      TypeBuilder type = DefineType ("IntegrationTest_OnGenericType");
      MethodBuilderEmitter emitter = DefineMethod (type, typeof (ConcreteMixinTypeIdentifier));

      var referenceIdentifier = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          new HashSet<MethodInfo> { _methodOnGenericClosedWithReferenceType, _methodOnGenericClosedWithValueType },
          new HashSet<MethodInfo> { _methodOnGenericClosedWithReferenceType, _methodOnGenericClosedWithValueType });

      var serializer = new CodeGenerationConcreteMixinTypeIdentifierSerializer (emitter.CodeBuilder);
      referenceIdentifier.Serialize (serializer);

      object result = BuildTypeAndInvokeMethod (type, emitter, serializer.GetConstructorInvocationExpression ());

      Assert.That (result, Is.EqualTo (referenceIdentifier));
    }

    [Test]
    public void IntegrationTest_GenericMethods ()
    {
      TypeBuilder type = DefineType ("IntegrationTest_GenericMethods");
      MethodBuilderEmitter emitter = DefineMethod (type, typeof (ConcreteMixinTypeIdentifier));

      var referenceIdentifier = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          new HashSet<MethodInfo> { _genericMethod },
          new HashSet<MethodInfo> { _genericMethod });

      var serializer = new CodeGenerationConcreteMixinTypeIdentifierSerializer (emitter.CodeBuilder);
      referenceIdentifier.Serialize (serializer);

      object result = BuildTypeAndInvokeMethod (type, emitter, serializer.GetConstructorInvocationExpression ());

      Assert.That (result, Is.EqualTo (referenceIdentifier));
    }

    private TypeBuilder DefineType (string testName)
    {
      var module = ConcreteTypeBuilderTestHelper.GetModuleManager (SavedTypeBuilder).Scope.ObtainDynamicModuleWithWeakName ();
      return module.DefineType ("CodeGenerationConcreteMixinTypeIdentifierSerializerTest." + testName);
    }

    private MethodBuilderEmitter DefineMethod (TypeBuilder type, Type returnType)
    {
      var method =
          type.DefineMethod ("Test", MethodAttributes.Public | MethodAttributes.Static, returnType, Type.EmptyTypes);
      return new MethodBuilderEmitter (method);
    }

    private object BuildTypeAndInvokeMethod (TypeBuilder type, MethodBuilderEmitter emitter, Expression expressionToReturn)
    {
      emitter.CodeBuilder.AddStatement (new ReturnStatement (expressionToReturn));
      emitter.Generate ();

      Type compiledType = type.CreateType ();
      return compiledType.GetMethod ("Test").Invoke (null, null);
    }
  }
}
