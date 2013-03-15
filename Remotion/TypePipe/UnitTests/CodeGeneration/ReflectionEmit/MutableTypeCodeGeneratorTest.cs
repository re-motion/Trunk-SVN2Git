// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.UnitTests.Expressions;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.TypePipe.UnitTests.CodeGeneration.ReflectionEmit
{
  [TestFixture]
  public class MutableTypeCodeGeneratorTest
  {
    private MockRepository _mockRepository;

    private MutableType _mutableType;
    private IReflectionEmitCodeGenerator _codeGeneratorMock;
    private IMemberEmitter _memberEmitterMock;
    private IInitializationBuilder _initializationBuilderMock;
    private IProxySerializationEnabler _proxySerializationEnablerMock;

    private MutableTypeCodeGenerator _generator;

    // Context members
    private ITypeBuilder _typeBuilderMock;
    private DebugInfoGenerator _debugInfoGeneratorMock;
    private IEmittableOperandProvider _emittableOperandProviderMock;

    private FieldInfo _fakeInitializationField;
    private MethodInfo _fakeInitializationMethod;
    private Tuple<FieldInfo, MethodInfo> _fakeInitializationMembers;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _mutableType = MutableTypeObjectMother.Create();
      _codeGeneratorMock = _mockRepository.StrictMock<IReflectionEmitCodeGenerator>();
      _memberEmitterMock = _mockRepository.StrictMock<IMemberEmitter>();
      _initializationBuilderMock = _mockRepository.StrictMock<IInitializationBuilder>();
      _proxySerializationEnablerMock = _mockRepository.StrictMock<IProxySerializationEnabler>();

      _generator = new MutableTypeCodeGenerator (
          _mutableType, _codeGeneratorMock, _memberEmitterMock, _initializationBuilderMock, _proxySerializationEnablerMock);

      _typeBuilderMock = _mockRepository.StrictMock<ITypeBuilder>();
      _debugInfoGeneratorMock = _mockRepository.StrictMock<DebugInfoGenerator>();
      _emittableOperandProviderMock = _mockRepository.StrictMock<IEmittableOperandProvider>();

      _fakeInitializationField = ReflectionObjectMother.GetSomeField();
      _fakeInitializationMethod = ReflectionObjectMother.GetSomeMethod();
      _fakeInitializationMembers = Tuple.Create (_fakeInitializationField, _fakeInitializationMethod);
    }

    [Test]
    public void DeclareType ()
    {
      using (_mockRepository.Ordered())
      {
        _codeGeneratorMock.Expect (mock => mock.EmittableOperandProvider).Return (_emittableOperandProviderMock);
        _codeGeneratorMock.Expect (mock => mock.DebugInfoGenerator).Return (_debugInfoGeneratorMock);
        _codeGeneratorMock.Expect (mock => mock.DefineType (_mutableType.FullName, _mutableType.Attributes)).Return (_typeBuilderMock);
        _typeBuilderMock.Expect (mock => mock.RegisterWith (_emittableOperandProviderMock, _mutableType));
      }
      _mockRepository.ReplayAll();

      _generator.DeclareType();

      _mockRepository.VerifyAll();
      var context = (CodeGenerationContext) PrivateInvoke.GetNonPublicField (_generator, "_context");
      Assert.That (context, Is.Not.Null);
      Assert.That (context.MutableType, Is.SameAs (_mutableType));
      Assert.That (context.TypeBuilder, Is.SameAs (_typeBuilderMock));
      Assert.That (context.DebugInfoGenerator, Is.SameAs (_debugInfoGeneratorMock));
      Assert.That (context.EmittableOperandProvider, Is.SameAs (_emittableOperandProviderMock));
    }

    [Test]
    public void DefineTypeFacet ()
    {
      var typeInitializer = _mutableType.AddTypeInitializer (ctx => Expression.Empty());

      var instanceInitialization = ExpressionTreeObjectMother.GetSomeExpression();
      _mutableType.AddInitialization (ctx => instanceInitialization);

      var customAttribute = CustomAttributeDeclarationObjectMother.Create();
      _mutableType.AddCustomAttribute (customAttribute);

      var @interface = typeof (IDisposable);
      _mutableType.AddInterface (@interface);

      var field = _mutableType.AddField();
      var constructor = _mutableType.AddConstructor();
      var method = _mutableType.AddMethod();
      var property = _mutableType.AddProperty();
      var event_ = _mutableType.AddEvent();

      var context = PopulateContext (_generator, 1);

      using (_mockRepository.Ordered())
      {
        _typeBuilderMock.Expect (mock => mock.SetParent (_mutableType.BaseType));
        _typeBuilderMock.Expect (mock => mock.AddInterfaceImplementation (@interface));
        _typeBuilderMock.Expect (mock => mock.SetCustomAttribute (customAttribute));

        _memberEmitterMock.Expect (mock => mock.AddConstructor (context, typeInitializer));
        _initializationBuilderMock.Expect (mock => mock.CreateInitializationMembers (_mutableType)).Return (_fakeInitializationMembers);
        _proxySerializationEnablerMock.Expect (mock => mock.MakeSerializable (_mutableType, _fakeInitializationMethod));

        _memberEmitterMock.Expect (mock => mock.AddField (context, field));
        _initializationBuilderMock
            .Expect (mock => mock.WireConstructorWithInitialization (constructor, _fakeInitializationMembers, _proxySerializationEnablerMock));
        _memberEmitterMock.Expect (mock => mock.AddConstructor (context, constructor));
        _memberEmitterMock.Expect (mock => mock.AddMethod (context, method));
        SetupExpectationsForAccessors (_memberEmitterMock, _mutableType.AddedMethods.Except (new[] { method }));
        _memberEmitterMock.Expect (mock => mock.AddProperty (context, property));
        _memberEmitterMock.Expect (mock => mock.AddEvent (context, event_));
      }
      _mockRepository.ReplayAll();

      _generator.DefineTypeFacet();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void DefineTypeFacet_NoTypeInitializer_NoInitializations ()
    {
      PopulateContext (_generator, 1);
      Assert.That (_mutableType.MutableTypeInitializer, Is.Null);

      _typeBuilderMock.SetParent (_mutableType.BaseType);
      // No call to AddConstructor because of null type initializer.
      _initializationBuilderMock.Expect (mock => mock.CreateInitializationMembers (_mutableType)).Return (null);
      _proxySerializationEnablerMock.Expect (mock => mock.MakeSerializable (_mutableType, null));
      _mockRepository.ReplayAll();

      _generator.DefineTypeFacet();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CreateType ()
    {
      var context = PopulateContext (_generator, 2);
      bool wasCalled = false;
      context.PostDeclarationsActionManager.AddAction (() => wasCalled = true);
      var fakeType = ReflectionObjectMother.GetSomeType();
      _typeBuilderMock.Expect (mock => mock.CreateType()).Return (fakeType);
      _mockRepository.ReplayAll();

      var result = _generator.CreateType();

      _mockRepository.VerifyAll();
      Assert.That (wasCalled, Is.True);
      Assert.That (result, Is.SameAs (fakeType));
    }

    [Test]
    public void ThrowsForInvalidOperation ()
    {
      var codeGeneratorStub = MockRepository.GenerateStub<IReflectionEmitCodeGenerator>();
      var memberEmitterStub = MockRepository.GenerateStub<IMemberEmitter>();
      var initializationBuilderStub = MockRepository.GenerateStub<IInitializationBuilder>();
      var proxySerializationEnablerStub = MockRepository.GenerateStub<IProxySerializationEnabler>();
      codeGeneratorStub.Stub (stub => stub.DefineType (null, 0)).IgnoreArguments().Return (_typeBuilderMock);
      codeGeneratorStub.Stub (stub => stub.DebugInfoGenerator).Return (_debugInfoGeneratorMock);
      codeGeneratorStub.Stub (stub => stub.EmittableOperandProvider).Return (_emittableOperandProviderMock);

      var generator = new MutableTypeCodeGenerator (
          _mutableType, codeGeneratorStub, memberEmitterStub, initializationBuilderStub, proxySerializationEnablerStub);

      CheckThrowsForInvalidOperation (generator.DefineTypeFacet);
      CheckThrowsForInvalidOperation (() => generator.CreateType());
      Assert.That (() => generator.DeclareType(), Throws.Nothing);

      CheckThrowsForInvalidOperation (generator.DeclareType);
      CheckThrowsForInvalidOperation (() => generator.CreateType());
      Assert.That (() => generator.DefineTypeFacet(), Throws.Nothing);

      CheckThrowsForInvalidOperation (generator.DeclareType);
      CheckThrowsForInvalidOperation (generator.DefineTypeFacet);
      Assert.That (() => generator.CreateType(), Throws.Nothing);
    }

    private CodeGenerationContext PopulateContext (MutableTypeCodeGenerator generator, int currentState)
    {
      var context = new CodeGenerationContext (_mutableType, _typeBuilderMock, _debugInfoGeneratorMock, _emittableOperandProviderMock);
      PrivateInvoke.SetNonPublicField (generator, "_context", context);
      PrivateInvoke.SetNonPublicField (generator, "_state", currentState);

      return context;
    }

    private void SetupExpectationsForAccessors (IMemberEmitter memberEmitterMock, IEnumerable<MutableMethodInfo> methods)
    {
      foreach (var method in methods)
      {
        var m = method;
        memberEmitterMock.Expect (mock => mock.AddMethod (Arg<CodeGenerationContext>.Is.Anything, Arg.Is (m)));
      }
    }

    private void CheckThrowsForInvalidOperation (Action action)
    {
      Assert.That (
          () => action(),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Methods DeclareType, DefineTypeFacet and CreateType must be called exactly once and in the correct order."));
    }
  }
}