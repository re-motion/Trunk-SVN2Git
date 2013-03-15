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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.CodeGeneration;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Remotion.TypePipe.UnitTests.MutableReflection.Implementation;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.TypeAssembly
{
  [TestFixture]
  public class TypeAssemblerTest
  {
    private IMutableTypeFactory _mutableTypeFactoryMock;
    private IMutableTypeCodeGenerator _mutableTypeCodeGeneratorMock;
    
    private Type _requestedType;

    [SetUp]
    public void SetUp ()
    {
      _mutableTypeFactoryMock = MockRepository.GenerateStrictMock<IMutableTypeFactory>();
      _mutableTypeCodeGeneratorMock = MockRepository.GenerateStrictMock<IMutableTypeCodeGenerator> ();

      _requestedType = CustomTypeObjectMother.Create (name: "RequestedType");
    }

    [Test]
    public void Initialization ()
    {
      var participantStub = MockRepository.GenerateStub<IParticipant>();
      var participantWithCacheProviderStub = MockRepository.GenerateStub<IParticipant>();
      var cachKeyProviderStub = MockRepository.GenerateStub<ICacheKeyProvider>();
      participantWithCacheProviderStub.Stub (stub => stub.PartialCacheKeyProvider).Return (cachKeyProviderStub);

      var participants = new[] { participantStub, participantWithCacheProviderStub };
      var typeAssembler = new TypeAssembler (participants.AsOneTime (), _mutableTypeFactoryMock, _mutableTypeCodeGeneratorMock);

      Assert.That (typeAssembler.CacheKeyProviders, Is.EqualTo (new[] { cachKeyProviderStub }));
    }

    [Test]
    public void CodeGenerator ()
    {
      var fakeCodeGenerator = MockRepository.GenerateStub<ICodeGenerator>();
      _mutableTypeCodeGeneratorMock.Expect (mock => mock.CodeGenerator).Return (fakeCodeGenerator);
      var typeAssembler = CreateTypeAssembler();

      Assert.That (typeAssembler.CodeGenerator, Is.SameAs (fakeCodeGenerator));
    }

    [Test]
    public void AssembleType ()
    {
      var mockRepository = new MockRepository();
      var participantMock1 = mockRepository.StrictMock<IParticipant>();
      var participantMock2 = mockRepository.StrictMock<IParticipant>();
      var mutableTypeFactoryMock = mockRepository.StrictMock<IMutableTypeFactory>();
      var subclassProxyCreatorMock = mockRepository.StrictMock<IMutableTypeCodeGenerator>();

      var fakeResult = ReflectionObjectMother.GetSomeType();
      using (mockRepository.Ordered())
      {
        participantMock1.Expect (mock => mock.PartialCacheKeyProvider);
        participantMock2.Expect (mock => mock.PartialCacheKeyProvider);

        var fakeProxyType = MutableTypeObjectMother.Create();
        mutableTypeFactoryMock.Expect (mock => mock.CreateProxyType (_requestedType)).Return (fakeProxyType);

        TypeContext typeContext = null;
        participantMock1.Expect (mock => mock.Modify (Arg<TypeContext>.Is.Anything)).WhenCalled (
            mi =>
            {
              typeContext = mi.Arguments[0].As<TypeContext>();
              Assert.That (typeContext.ProxyType, Is.SameAs (fakeProxyType));
            });
        participantMock2.Expect (mock => mock.Modify (Arg<TypeContext>.Matches (ctx => ctx == typeContext)));

        subclassProxyCreatorMock.Expect (mock => mock.CreateProxy (Arg<TypeContext>.Matches(ctx => ctx == typeContext))).Return (fakeResult);
      }
      mockRepository.ReplayAll();

      var typeAssembler = CreateTypeAssembler (
          mutableTypeFactoryMock, subclassProxyCreatorMock, participants: new[] { participantMock1, participantMock2 });

      var result = typeAssembler.AssembleType (_requestedType);

      mockRepository.VerifyAll();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void AssembleType_ExceptionInCodeGeneraton ()
    {
      _mutableTypeFactoryMock.Stub (stub => stub.CreateProxyType (_requestedType)).Return (MutableTypeObjectMother.Create());
      var exception1 = new InvalidOperationException ("blub");
      var exception2 = new NotSupportedException ("blub");
      var exception3 = new Exception();
      _mutableTypeCodeGeneratorMock.Expect (mock => mock.CreateProxy (Arg<TypeContext>.Is.Anything)).Throw (exception1);
      _mutableTypeCodeGeneratorMock.Expect (mock => mock.CreateProxy (Arg<TypeContext>.Is.Anything)).Throw (exception2);
      _mutableTypeCodeGeneratorMock.Expect (mock => mock.CreateProxy (Arg<TypeContext>.Is.Anything)).Throw (exception3);
      var typeAssembler = CreateTypeAssembler (participants: MockRepository.GenerateStub<IParticipant>());

      var expectedMessageRegex = "An error occurred during code generation for 'RequestedType':\r\nblub\r\n"
                                 + @"The following participants are currently configured and may have caused the error: 'IParticipantProxy.*'\.";
      Assert.That (
          () => typeAssembler.AssembleType (_requestedType),
          Throws.InvalidOperationException.With.InnerException.SameAs (exception1).And.With.Message.Matches (expectedMessageRegex));
      Assert.That (
          () => typeAssembler.AssembleType (_requestedType),
          Throws.TypeOf<NotSupportedException>().With.InnerException.SameAs (exception2).And.With.Message.Matches (expectedMessageRegex));
      Assert.That (() => typeAssembler.AssembleType (_requestedType), Throws.Exception.SameAs (exception3));
    }

    [Test]
    public void GetCompoundCacheKey ()
    {
      var requestedType = ReflectionObjectMother.GetSomeSubclassableType();
      var participantMock1 = CreateCacheKeyReturningParticipantMock (requestedType, 1);
      var participantMock2 = CreateCacheKeyReturningParticipantMock (requestedType, "2");
      var typeAssembler = CreateTypeAssembler (participants: new[] { participantMock1, participantMock2 });

      var result = typeAssembler.GetCompoundCacheKey (requestedType, 3);

      Assert.That (result, Is.EqualTo (new object[] { null, null, null, requestedType, 1, "2" }));
    }

    private TypeAssembler CreateTypeAssembler (
        IMutableTypeFactory mutableTypeFactory = null, IMutableTypeCodeGenerator mutableTypeCodeGenerator = null, params IParticipant[] participants)
    {
      mutableTypeFactory = mutableTypeFactory ?? _mutableTypeFactoryMock;
      mutableTypeCodeGenerator = mutableTypeCodeGenerator ?? _mutableTypeCodeGeneratorMock;

      return new TypeAssembler (participants.AsOneTime(), mutableTypeFactory, mutableTypeCodeGenerator);
    }

    private IParticipant CreateCacheKeyReturningParticipantMock (Type requestedType, object cacheKey)
    {
      var participantMock = MockRepository.GenerateStrictMock<IParticipant>();
      var cacheKeyProviderMock = MockRepository.GenerateStrictMock<ICacheKeyProvider>();

      participantMock.Expect (mock => mock.PartialCacheKeyProvider).Return (cacheKeyProviderMock);
      cacheKeyProviderMock.Expect (mock => mock.GetCacheKey (requestedType)).Return (cacheKey);

      return participantMock;
    }
  }
}