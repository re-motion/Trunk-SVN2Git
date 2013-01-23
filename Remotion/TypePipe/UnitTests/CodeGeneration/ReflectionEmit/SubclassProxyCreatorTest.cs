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
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using Remotion.TypePipe.UnitTests.Expressions;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.CodeGeneration.ReflectionEmit
{
  [TestFixture]
  public class SubclassProxyCreatorTest
  {
    private MockRepository _mockRepository;

    private IReflectionEmitCodeGenerator _codeGeneratorMock;
    private IMemberEmitterFactory _memberEmitterFactoryMock;
    private IInitializationBuilder _initializationBuilderMock;
    private IProxySerializationEnabler _proxySerializationEnablerMock;

    private SubclassProxyCreator _creator;

    private IMemberEmitter _memberEmitterMock;
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

      _codeGeneratorMock = _mockRepository.StrictMock<IReflectionEmitCodeGenerator>();
      _memberEmitterFactoryMock = _mockRepository.StrictMock<IMemberEmitterFactory>();
      _memberEmitterMock = _mockRepository.StrictMock<IMemberEmitter>();
      _initializationBuilderMock = _mockRepository.StrictMock<IInitializationBuilder>();
      _proxySerializationEnablerMock = _mockRepository.StrictMock<IProxySerializationEnabler>();

      _typeBuilderMock = _mockRepository.StrictMock<ITypeBuilder>();
      _debugInfoGeneratorMock = _mockRepository.StrictMock<DebugInfoGenerator>();
      _emittableOperandProviderMock = _mockRepository.StrictMock<IEmittableOperandProvider>();

      _creator = new SubclassProxyCreator (_codeGeneratorMock, _memberEmitterFactoryMock, _initializationBuilderMock, _proxySerializationEnablerMock);

      _fakeInitializationField = ReflectionObjectMother.GetSomeField();
      _fakeInitializationMethod = ReflectionObjectMother.GetSomeMethod();
      _fakeInitializationMembers = Tuple.Create (_fakeInitializationField, _fakeInitializationMethod);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_creator.CodeGenerator, Is.SameAs (_codeGeneratorMock));
    }

    [Test]
    public void CreateProxy ()
    {
      var baseType = ReflectionObjectMother.GetSomeSubclassableType();
      var fullName = "AbcProxy";
      var attributes = (TypeAttributes) 7;
      var proxyType = ProxyTypeObjectMother.Create (baseType: baseType, fullName: fullName, attributes: attributes);

      var typeInitializer = proxyType.AddTypeInitializer (ctx => Expression.Empty());

      var instanceInitialization = ExpressionTreeObjectMother.GetSomeExpression ();
      proxyType.AddInitialization (ctx => instanceInitialization);

      var customAttribute = CustomAttributeDeclarationObjectMother.Create ();
      proxyType.AddCustomAttribute (customAttribute);

      var @interface = typeof (IDisposable);
      proxyType.AddInterface (@interface);

      var field = proxyType.AddField();
      var constructor = proxyType.AddConstructor();
      var method = proxyType.AddMethod();

      var fakeType = ReflectionObjectMother.GetSomeType();

      using (_mockRepository.Ordered())
      {
        _codeGeneratorMock.Expect (mock => mock.EmittableOperandProvider).Return (_emittableOperandProviderMock);
        _memberEmitterFactoryMock.Expect (mock => mock.CreateMemberEmitter (_emittableOperandProviderMock)).Return (_memberEmitterMock);

        _codeGeneratorMock.Expect (mock => mock.DefineType (fullName, attributes, baseType)).Return (_typeBuilderMock);
        _typeBuilderMock.Expect (mock => mock.RegisterWith (_emittableOperandProviderMock, proxyType));

        _codeGeneratorMock.Expect (mock => mock.DebugInfoGenerator).Return (_debugInfoGeneratorMock);

        CodeGenerationContext context = null;
        var buildActionCalled = false;
        _memberEmitterMock
            .Expect (mock => mock.AddConstructor (Arg<CodeGenerationContext>.Is.Anything, Arg.Is (typeInitializer)))
            .WhenCalled (
                mi =>
                {
                  context = (CodeGenerationContext) mi.Arguments[0];
                  context.PostDeclarationsActionManager.AddAction (() => buildActionCalled = true);
                });

        _initializationBuilderMock.Expect (mock => mock.CreateInitializationMembers (proxyType)).Return (_fakeInitializationMembers);

        _proxySerializationEnablerMock.Expect (mock => mock.MakeSerializable (proxyType, _fakeInitializationMethod));

        _typeBuilderMock.Expect (mock => mock.SetCustomAttribute (customAttribute));

        _typeBuilderMock.Expect (mock => mock.AddInterfaceImplementation (@interface));

        _memberEmitterMock.Expect (mock => mock.AddField (Arg<CodeGenerationContext>.Matches(c => c == context), Arg.Is(field)));
        _initializationBuilderMock.Expect (
            mock => mock.WireConstructorWithInitialization (constructor, _fakeInitializationMembers, _proxySerializationEnablerMock));
        _memberEmitterMock.Expect (mock => mock.AddConstructor (Arg<CodeGenerationContext>.Matches (c => c == context), Arg.Is (constructor)));
        _memberEmitterMock
            .Expect (mock => mock.AddMethod (Arg<CodeGenerationContext>.Matches (c => c == context), Arg.Is (method), Arg.Is (method.Attributes)))
            .WhenCalled (mi => Assert.That (buildActionCalled, Is.False));

        // PostDeclarationsActionManager.ExecuteAllActions() cannot setup expectations.

        _typeBuilderMock
            .Expect (mock => mock.CreateType())
            .Return (fakeType)
            .WhenCalled (mi => Assert.That (buildActionCalled, Is.True));
      }
      _mockRepository.ReplayAll();

      var result = _creator.CreateProxy (proxyType);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.SameAs (fakeType));
    }

    [Test]
    public void Build_NoTypeInitializer_NoInitializations ()
    {
      var proxyType = ProxyTypeObjectMother.Create();
      Assert.That (proxyType.MutableTypeInitializer, Is.Null);

      _codeGeneratorMock.Expect (mock => mock.EmittableOperandProvider).Return (_emittableOperandProviderMock);
      _memberEmitterFactoryMock.Expect (mock => mock.CreateMemberEmitter (_emittableOperandProviderMock)).Return (_memberEmitterMock);
      _codeGeneratorMock.Expect (mock => mock.DefineType (proxyType.FullName, proxyType.Attributes, proxyType.BaseType)).Return (_typeBuilderMock);
      _typeBuilderMock.Expect (mock => mock.RegisterWith (_emittableOperandProviderMock, proxyType));
      _codeGeneratorMock.Expect (mock => mock.DebugInfoGenerator).Return (_debugInfoGeneratorMock);
      // No call to AddConstructor for because of null type initializer.
      _initializationBuilderMock.Expect (mock => mock.CreateInitializationMembers (proxyType)).Return (null);
      _proxySerializationEnablerMock.Expect (mock => mock.MakeSerializable (proxyType, null));
      _typeBuilderMock.Expect (mock => mock.CreateType());
      _mockRepository.ReplayAll();

      _creator.CreateProxy (proxyType);

      _mockRepository.VerifyAll();
    }
  }
}