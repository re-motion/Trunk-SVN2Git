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
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.CodeGeneration;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.TypeAssembly
{
  [TestFixture]
  public class TypeAssemblerTest
  {
    public interface ICachKeyProviderMethod
    {
      object M (ICacheKeyProvider cacheKeyProvider, Type type);
    }

    private IMutableTypeFactory _mutableTypeFactoryMock;
    private ITypeAssemblyContextCodeGenerator _typeAssemblyContextCodeGeneratorMock;

    private Type _requestedType;
    private IDictionary<string, object> _participantState;

    [SetUp]
    public void SetUp ()
    {
      _mutableTypeFactoryMock = MockRepository.GenerateStrictMock<IMutableTypeFactory>();
      _typeAssemblyContextCodeGeneratorMock = MockRepository.GenerateStrictMock<ITypeAssemblyContextCodeGenerator> ();

      _requestedType = ReflectionObjectMother.GetSomeType();
      _participantState = new Dictionary<string, object>();
    }

    [Test]
    public void Initialization ()
    {
      var participantStub = MockRepository.GenerateStub<IParticipant>();
      var participantWithCacheProviderStub = MockRepository.GenerateStub<IParticipant>();
      var cachKeyProviderStub = MockRepository.GenerateStub<ICacheKeyProvider>();
      participantWithCacheProviderStub.Stub (stub => stub.PartialCacheKeyProvider).Return (cachKeyProviderStub);
      var participants = new[] { participantStub, participantWithCacheProviderStub };

      var typeAssembler = new TypeAssembler ("configId", participants.AsOneTime(), _mutableTypeFactoryMock, _typeAssemblyContextCodeGeneratorMock);

      Assert.That (typeAssembler.ParticipantConfigurationID, Is.EqualTo ("configId"));
      var cacheKeyProviders = PrivateInvoke.GetNonPublicField (typeAssembler, "_cacheKeyProviders");
      Assert.That (cacheKeyProviders, Is.EqualTo (new[] { cachKeyProviderStub }));
    }

    [Test]
    public void CodeGenerator ()
    {
      var fakeCodeGenerator = MockRepository.GenerateStub<ICodeGenerator>();
      _typeAssemblyContextCodeGeneratorMock.Expect (mock => mock.CodeGenerator).Return (fakeCodeGenerator);
      var typeAssembler = CreateTypeAssembler();

      Assert.That (typeAssembler.CodeGenerator, Is.SameAs (fakeCodeGenerator));
    }

    [Test]
    public void GetCompoundCacheKey ()
    {
      var participantMock1 = MockRepository.GenerateStrictMock<IParticipant>();
      var participantMock2 = MockRepository.GenerateStrictMock<IParticipant>();
      var partialCacheKeyProviderMock1 = MockRepository.GenerateStrictMock<ICacheKeyProvider>();
      var partialCacheKeyProviderMock2 = MockRepository.GenerateStrictMock<ICacheKeyProvider>();
      participantMock1.Expect (mock => mock.PartialCacheKeyProvider).Return (partialCacheKeyProviderMock1);
      participantMock2.Expect (mock => mock.PartialCacheKeyProvider).Return (partialCacheKeyProviderMock2);
      var typeAssembler = CreateTypeAssembler (participants: new[] { participantMock1, participantMock2 });

      var cachKeyProviderMethod = MockRepository.GenerateStrictMock<ICachKeyProviderMethod>();
      cachKeyProviderMethod.Expect (mock => mock.M (partialCacheKeyProviderMock1, _requestedType)).Return (1);
      cachKeyProviderMethod.Expect (mock => mock.M (partialCacheKeyProviderMock2, _requestedType)).Return ("2");

      var result = typeAssembler.GetCompoundCacheKey (cachKeyProviderMethod.M, _requestedType, 2);

      Assert.That (result, Is.EqualTo (new object[] { null, null, 1, "2" }));
    }

    [Test]
    public void AssembleType ()
    {
      var mockRepository = new MockRepository();
      var participantMock1 = mockRepository.StrictMock<IParticipant>();
      var participantMock2 = mockRepository.StrictMock<IParticipant>();
      var mutableTypeFactoryMock = mockRepository.StrictMock<IMutableTypeFactory>();
      var subclassProxyCreatorMock = mockRepository.StrictMock<ITypeAssemblyContextCodeGenerator>();

      bool generationCompletedEventRaised = false;
      var fakeGeneratedType = ReflectionObjectMother.GetSomeType();
      using (mockRepository.Ordered())
      {
        participantMock1.Expect (mock => mock.PartialCacheKeyProvider);
        participantMock2.Expect (mock => mock.PartialCacheKeyProvider);

        var fakeProxyType = MutableTypeObjectMother.Create();
        var fakeContext = new GeneratedTypeContext (new[] { Tuple.Create (fakeProxyType, fakeGeneratedType) }.AsReadOnly());

        mutableTypeFactoryMock.Expect (mock => mock.CreateProxy (_requestedType)).Return (fakeProxyType);

        TypeAssemblyContext typeAssemblyContext = null;
        participantMock1.Expect (mock => mock.Participate (Arg<TypeAssemblyContext>.Is.Anything)).WhenCalled (
            mi =>
            {
              typeAssemblyContext = (TypeAssemblyContext) mi.Arguments[0];
              Assert.That (typeAssemblyContext.ProxyType, Is.SameAs (fakeProxyType));
              Assert.That (typeAssemblyContext.State, Is.SameAs (_participantState));
              typeAssemblyContext.GenerationCompleted += ctx =>
              {
                Assert.That (ctx, Is.SameAs (fakeContext));
                generationCompletedEventRaised = true;
              };
            });
        participantMock2.Expect (mock => mock.Participate (Arg<TypeAssemblyContext>.Matches (ctx => ctx == typeAssemblyContext)));

        subclassProxyCreatorMock
            .Expect (mock => mock.GenerateTypes (Arg<TypeAssemblyContext>.Matches (ctx => ctx == typeAssemblyContext)))
            .Return (fakeContext)
            .WhenCalled (mi => Assert.That (generationCompletedEventRaised, Is.False));
      }
      mockRepository.ReplayAll();

      var typeAssembler = CreateTypeAssembler (
          mutableTypeFactoryMock, subclassProxyCreatorMock, participants: new[] { participantMock1, participantMock2 });

      var result = typeAssembler.AssembleType (_requestedType, _participantState);

      mockRepository.VerifyAll();
      Assert.That (generationCompletedEventRaised, Is.True);
      Assert.That (result, Is.SameAs (fakeGeneratedType));
    }

    [Test]
    public void AssembleType_ExceptionInCodeGeneraton ()
    {
      _mutableTypeFactoryMock.Stub (stub => stub.CreateProxy (_requestedType)).Return (MutableTypeObjectMother.Create());
      var exception1 = new InvalidOperationException ("blub");
      var exception2 = new NotSupportedException ("blub");
      var exception3 = new Exception();
      _typeAssemblyContextCodeGeneratorMock.Expect (mock => mock.GenerateTypes (Arg<TypeAssemblyContext>.Is.Anything)).Throw (exception1);
      _typeAssemblyContextCodeGeneratorMock.Expect (mock => mock.GenerateTypes (Arg<TypeAssemblyContext>.Is.Anything)).Throw (exception2);
      _typeAssemblyContextCodeGeneratorMock.Expect (mock => mock.GenerateTypes (Arg<TypeAssemblyContext>.Is.Anything)).Throw (exception3);
      var typeAssembler = CreateTypeAssembler (participants: MockRepository.GenerateStub<IParticipant>());

      var expectedMessageRegex = "An error occurred during code generation for '" + _requestedType.Name + "':\r\nblub\r\n"
                                 + @"The following participants are currently configured and may have caused the error: 'IParticipantProxy.*'\.";
      Assert.That (
          () => typeAssembler.AssembleType (_requestedType, _participantState),
          Throws.InvalidOperationException.With.InnerException.SameAs (exception1).And.With.Message.Matches (expectedMessageRegex));
      Assert.That (
          () => typeAssembler.AssembleType (_requestedType, _participantState),
          Throws.TypeOf<NotSupportedException>().With.InnerException.SameAs (exception2).And.With.Message.Matches (expectedMessageRegex));
      Assert.That (() => typeAssembler.AssembleType (_requestedType, _participantState), Throws.Exception.SameAs (exception3));
    }

    private TypeAssembler CreateTypeAssembler (
        IMutableTypeFactory mutableTypeFactory = null, ITypeAssemblyContextCodeGenerator typeAssemblyContextCodeGenerator = null, params IParticipant[] participants)
    {
      mutableTypeFactory = mutableTypeFactory ?? _mutableTypeFactoryMock;
      typeAssemblyContextCodeGenerator = typeAssemblyContextCodeGenerator ?? _typeAssemblyContextCodeGeneratorMock;

      return new TypeAssembler ("id", participants.AsOneTime(), mutableTypeFactory, typeAssemblyContextCodeGenerator);
    }
  }
}