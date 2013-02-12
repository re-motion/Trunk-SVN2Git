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
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.Expressions.ReflectionAdapters;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.TypePipe.Serialization;
using Remotion.TypePipe.Serialization.Implementation;
using Remotion.TypePipe.UnitTests.Expressions;
using Remotion.TypePipe.UnitTests.MutableReflection;

namespace Remotion.TypePipe.UnitTests.Serialization
{
  [TestFixture]
  public class SerializationParticipantTest
  {
    private const string c_factoryIdentifier = "expectedObjectFactory identifier";

    private SerializationParticipant _participant;

    [SetUp]
    public void SetUp ()
    {
      _participant = new SerializationParticipant (c_factoryIdentifier);
    }

    [Test]
    public void PartialCacheKeyProvider ()
    {
      Assert.That (_participant.PartialCacheKeyProvider, Is.Null);
    }

    [Test]
    public void ModifyType_SerializableType ()
    {
      var proxyType = ProxyTypeObjectMother.Create (baseType: typeof (SomeType), attributes: TypeAttributes.Serializable);

      _participant.ModifyType (proxyType);

      Assert.That (proxyType.AddedInterfaces, Is.EqualTo (new[] { typeof (ISerializable) }));
      Assert.That (proxyType.AddedConstructors, Is.Empty);
      Assert.That (proxyType.AddedMethods, Has.Count.EqualTo (1));

      var method = proxyType.AddedMethods.Single();
      var serializationInfo = method.ParameterExpressions[0];
      var expectedMethodBody = Expression.Block (
          typeof (void),
          Expression.Call (serializationInfo, "SetType", Type.EmptyTypes, Expression.Constant (typeof (ObjectWithoutDeserializationConstructorProxy))),
          Expression.Call (
              serializationInfo,
              "AddValue",
              Type.EmptyTypes,
              Expression.Constant ("<tp>baseType"),
              Expression.Constant (typeof (SomeType).AssemblyQualifiedName)),
          Expression.Call (
              serializationInfo, "AddValue", Type.EmptyTypes, Expression.Constant ("<tp>factoryIdentifier"), Expression.Constant (c_factoryIdentifier)),
          Expression.Call (
              typeof (ReflectionSerializationHelper), "AddFieldValues", Type.EmptyTypes, serializationInfo, new ThisExpression (proxyType)));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedMethodBody, method.Body);
    }

    [Test]
    public void ModifyType_SerializableInterfaceType ()
    {
      var proxyType = ProxyTypeObjectMother.Create (typeof (SerializableInterfaceType), attributes: TypeAttributes.Serializable);
      var baseMethod = proxyType.GetMethod ("GetObjectData");

      _participant.ModifyType (proxyType);

      Assert.That (proxyType.AddedInterfaces, Is.Empty);
      Assert.That (proxyType.AddedMethods, Has.Count.EqualTo (1));

      var method = proxyType.AddedMethods.Single();
      var serializationInfo = method.ParameterExpressions[0];
      var expectedBody = Expression.Block (
          Expression.Call (
              new ThisExpression (proxyType), new NonVirtualCallMethodInfoAdapter (baseMethod), method.ParameterExpressions.Cast<Expression>()),
          Expression.Call (serializationInfo, "SetType", Type.EmptyTypes, Expression.Constant (typeof (ObjectWithDeserializationConstructorProxy))),
          Expression.Call (
              serializationInfo,
              "AddValue",
              Type.EmptyTypes,
              Expression.Constant ("<tp>baseType"),
              Expression.Constant (typeof (SerializableInterfaceType).AssemblyQualifiedName)),
          Expression.Call (
              serializationInfo, "AddValue", Type.EmptyTypes, Expression.Constant ("<tp>factoryIdentifier"), Expression.Constant (c_factoryIdentifier)));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedBody, method.Body);
    }

    [Test]
    public void ModifyType_SomeType ()
    {
      var proxyType = ProxyTypeObjectMother.Create (typeof (SomeType));

      _participant.ModifyType (proxyType);

      Assert.That (proxyType.AddedInterfaces, Is.Empty);
      Assert.That (proxyType.AddedMethods, Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException),
        ExpectedMessage = "The proxy type implements ISerializable but GetObjectData cannot be overridden. "
                          + "Make sure that GetObjectData is implemented implicitly (not explicitly) and virtual.")]
    public void ModifyType_CannotOverrideGetObjectData ()
    {
      var proxyType = ProxyTypeObjectMother.Create (typeof (ExplicitSerializableInterfaceType), attributes: TypeAttributes.Serializable);

      _participant.ModifyType (proxyType);
    }

    public class SomeType { }

    public class SerializableInterfaceType : ISerializable
    {
      public virtual void GetObjectData (SerializationInfo info, StreamingContext context) { }
    }

    public class ExplicitSerializableInterfaceType : ISerializable
    {
      void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context) { }
    }
  }
}