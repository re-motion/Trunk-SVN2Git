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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe.Serialization.Implementation;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.Serialization.Implementation
{
  [TestFixture]
  public class ReflectionDeserializationSurrogateTest
  {
    private Type _underlyingType;
    private SerializationInfo _serializationInfo;

    private ReflectionDeserializationSurrogate _surrogate;

    [SetUp]
    public void SetUp ()
    {
      _underlyingType = ReflectionObjectMother.GetSomeType();
      _serializationInfo = new SerializationInfo (ReflectionObjectMother.GetSomeDifferentType(), new FormatterConverter());

      _surrogate = new ReflectionDeserializationSurrogate (_serializationInfo, new StreamingContext (StreamingContextStates.File));
    }

    [Test]
    public void CreateRealObject ()
    {
      var context = new StreamingContext (StreamingContextStates.Persistence);
      _serializationInfo.AddValue ("<tp>IntField", 7);

      var objectFactoryMock = MockRepository.GenerateStrictMock<IObjectFactory>();
      var instance = new DomainType();
      objectFactoryMock.Expect (mock => mock.GetUninitializedObject (_underlyingType)).Return (instance);

      var result = PrivateInvoke.InvokeNonPublicMethod (_surrogate, "CreateRealObject", objectFactoryMock, _underlyingType, context);

      objectFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (instance));
      Assert.That (((DomainType) result).IntField, Is.EqualTo (7));
    }

    [Serializable]
    class DomainType
    {
      public int IntField = 0;
    }
  }
}