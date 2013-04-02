﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.TypePipe.MutableReflection;

namespace Remotion.TypePipe.IntegrationTests.TypeAssembly
{
  [TestFixture]
  public class GenerationCompletedEventTest : TypeAssemblerIntegrationTestBase
  {
    [Test]
    public void GetGeneratedMember_Types ()
    {
      Type proxyType = null;
      Type additionalType = null;

      var type = AssembleType<DomainType> (
          typeContext =>
          {
            var addedType = typeContext.CreateType ("MyType", "MyNamespace", TypeAttributes.Public, typeof (List<int>));

            typeContext.GenerationCompleted += ctx =>
            {
              proxyType = ctx.GetGeneratedType (typeContext.ProxyType);
              additionalType = ctx.GetGeneratedType (addedType);

              Assert.That (proxyType.IsRuntimeType(), Is.True);
              Assert.That (additionalType.IsRuntimeType(), Is.True);
            };
          });
      var expectedAdditionalType = type.Assembly.GetType ("MyNamespace.MyType", true);

      Assert.That (proxyType, Is.SameAs (type));
      Assert.That (additionalType, Is.SameAs (expectedAdditionalType));
    }

    [Test]
    public void GetGeneratedMember_Members_OnProxyType ()
    {
      // TODO 5461: MutableGenericParameters on MutableType.
      ConstructorInfo typeInitializer = null;
      FieldInfo field = null;
      ConstructorInfo ctor = null;
      MethodInfo method = null;
      PropertyInfo property = null;
      EventInfo event_ = null;

      var type = AssembleType<DomainType> (
          typeContext =>
          {
            var proxyType = typeContext.ProxyType;

            var addedTypeInitializer = proxyType.AddTypeInitializer (ctx => Expression.Empty());
            var addedField = proxyType.AddField ("MyField", FieldAttributes.Public, typeof (int));
            var addedCtor = proxyType.AddedConstructors.Single();
            var addedMethod = proxyType.AddMethod (
                "MyMethod", MethodAttributes.Public, typeof (void), ParameterDeclaration.None, ctx => Expression.Empty());
            var addedProperty = proxyType.AddProperty (
                "MyProperty", typeof (string), ParameterDeclaration.None, MethodAttributes.Public, null, ctx => Expression.Empty());
            var addedEvent = proxyType.AddEvent (
                "MyEvent", typeof (Action), MethodAttributes.Public, ctx => Expression.Empty(), ctx => Expression.Empty());

            typeContext.GenerationCompleted +=
                ctx =>
                {
                  typeInitializer = ctx.GetGeneratedConstructor (addedTypeInitializer);
                  field = ctx.GetGeneratedField (addedField);
                  ctor = ctx.GetGeneratedConstructor (addedCtor);
                  method = ctx.GetGeneratedMethod (addedMethod);
                  property = ctx.GetGeneratedProperty (addedProperty);
                  event_ = ctx.GetGeneratedEvent (addedEvent);
                };
          });

      Assert.That (typeInitializer, Is.Not.Null.And.EqualTo (type.TypeInitializer));
      Assert.That (field, Is.EqualTo (type.GetFields().Single()));
      Assert.That (method, Is.EqualTo (type.GetMethods().Single (m => m.Name == "MyMethod")));
      Assert.That (ctor, Is.EqualTo (type.GetConstructors().Single()));
      Assert.That (property, Is.EqualTo (type.GetProperties().Single()));
      Assert.That (event_, Is.EqualTo (type.GetEvents().Single()));
    }

    [Test]
    public void GetGeneratedMember_Members_OnAdditionalType ()
    {
      FieldInfo field = null;
      MethodInfo method = null;

      var type = AssembleType<DomainType> (
          typeContext =>
          {
            var newType = typeContext.CreateType ("MyType", null, TypeAttributes.Public, typeof (object));
            var addedField = newType.AddField ("MyField", FieldAttributes.Public, typeof (int));
            var addedMethod = newType.AddMethod (
                "MyMethod", MethodAttributes.Public, typeof (void), ParameterDeclaration.None, ctx => Expression.Empty());

            typeContext.GenerationCompleted +=
                ctx =>
                {
                  field = ctx.GetGeneratedField (addedField);
                  method = ctx.GetGeneratedMethod (addedMethod);
                };
          });
      var additionalType = type.Assembly.GetType ("MyType", true);

      Assert.That (field, Is.EqualTo (additionalType.GetFields().Single()));
      Assert.That (method, Is.EqualTo (additionalType.GetMethods ().Single (m => m.Name == "MyMethod")));
    }

    public class DomainType {}
  }
}