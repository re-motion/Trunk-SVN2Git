// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.DynamicProxy
{
  [TestFixture]
  public class CodeGenerationConcreteMixinTypeIdentifierSerializerTest : CodeGenerationBaseTest
  {
    private MethodInfo _simpleExternalMethod;
    private MethodInfo _genericExternalMethod;
    private MethodInfo _externalMethodOnGenericClosedWithReferenceType;
    private MethodInfo _externalMethodOnGenericClosedWithValueType;
    private MethodInfo _simpleMethodOnMixinType;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      _simpleExternalMethod = typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes);
      _genericExternalMethod = typeof (BaseType7).GetMethod ("One");
      _externalMethodOnGenericClosedWithReferenceType = typeof (GenericClassWithAllKindsOfMembers<string>).GetMethod ("Method");
      _externalMethodOnGenericClosedWithValueType = typeof (GenericClassWithAllKindsOfMembers<int>).GetMethod ("Method");
      _simpleMethodOnMixinType = typeof (BT1Mixin1).GetMethod ("VirtualMethod");
    }

    [Test]
    public void IntegrationTest()
    {
      TypeBuilder type = DefineType ("IntegrationTest");
      MethodBuilderEmitter emitter = DefineMethod (type, typeof (ConcreteMixinTypeIdentifier));

      var referenceIdentifier = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1), 
          new HashSet<MethodInfo> { _simpleExternalMethod },
          new HashSet<MethodInfo> { _simpleMethodOnMixinType });

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
          new HashSet<MethodInfo> { _externalMethodOnGenericClosedWithReferenceType, _externalMethodOnGenericClosedWithValueType },
          new HashSet<MethodInfo> { _externalMethodOnGenericClosedWithReferenceType, _externalMethodOnGenericClosedWithValueType });

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
          new HashSet<MethodInfo> { _genericExternalMethod },
          new HashSet<MethodInfo> { _genericExternalMethod });

      var serializer = new CodeGenerationConcreteMixinTypeIdentifierSerializer (emitter.CodeBuilder);
      referenceIdentifier.Serialize (serializer);

      object result = BuildTypeAndInvokeMethod (type, emitter, serializer.GetConstructorInvocationExpression ());

      Assert.That (result, Is.EqualTo (referenceIdentifier));
    }

    private TypeBuilder DefineType (string testName)
    {
      var module = ((ModuleManager) (SavedTypeBuilder.Scope)).Scope.ObtainDynamicModuleWithWeakName ();
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
