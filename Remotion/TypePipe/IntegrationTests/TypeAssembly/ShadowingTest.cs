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
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.TypePipe.MutableReflection;

namespace Remotion.TypePipe.IntegrationTests.TypeAssembly
{
  [TestFixture]
  public class ShadowingTest : TypeAssemblerIntegrationTestBase
  {
    [Test]
    public void ShadowMethod_NonVirtual ()
    {
      var type = AssembleType<DomainType> (
          proxyType =>
          {
            var shadowedMethod = typeof (DomainType).GetMethod ("OverridableMethod");
            var mutableMethodInfo = AddEquivalentMethod (
                proxyType, 
                shadowedMethod,
                MethodAttributes.Public,
                ctx =>
                {
                  Assert.That (ctx.HasBaseMethod, Is.False);
                  Assert.That (
                      () => ctx.BaseMethod,
                      Throws.TypeOf<NotSupportedException>().With.Message.EqualTo ("This method does not override another method."));

                  return ExpressionHelper.StringConcat (
                      ctx.CallBase ("OverridableMethod", ctx.Parameters.Cast<Expression>()), Expression.Constant (" shadowed"));
                });
            Assert.That (mutableMethodInfo.BaseMethod, Is.Null);
            Assert.That (mutableMethodInfo.GetBaseDefinition(), Is.SameAs (mutableMethodInfo));

            Assert.That (
                proxyType.GetMethods ().Where (mi => mi.Name == "OverridableMethod"),
                Is.EquivalentTo (new[] { mutableMethodInfo, typeof (DomainType).GetMethod ("OverridableMethod") }));
          });

      var instance = (DomainType) Activator.CreateInstance (type);
      var method = GetDeclaredMethod (type, "OverridableMethod");

      Assert.That (method.GetBaseDefinition(), Is.SameAs (method));

      var result = method.Invoke (instance, null);
      Assert.That (result, Is.EqualTo ("DomainType shadowed"));
      Assert.That (instance.OverridableMethod(), Is.EqualTo ("DomainType"));
    }

    [Test]
    public void ShadowMethod_VirtualAndNewSlot ()
    {
      var type = AssembleType<DomainType> (
          proxyType =>
          {
            var shadowedMethod = typeof (DomainType).GetMethod ("OverridableMethod");
            var mutableMethodInfo = AddEquivalentMethod (
                proxyType,
                shadowedMethod,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot,
                ctx =>
                {
                  Assert.That (ctx.HasBaseMethod, Is.False);

                  return ExpressionHelper.StringConcat (
                      ctx.CallBase ("OverridableMethod", ctx.Parameters.Cast<Expression>()), Expression.Constant (" shadowed"));
                });
            Assert.That (mutableMethodInfo.BaseMethod, Is.Null);
            Assert.That (mutableMethodInfo.GetBaseDefinition(), Is.SameAs (mutableMethodInfo));

            Assert.That (
                proxyType.GetMethods().Where (mi => mi.Name == "OverridableMethod"),
                Is.EquivalentTo (new[] { mutableMethodInfo, typeof (DomainType).GetMethod ("OverridableMethod") }));
          });

      var instance = (DomainType) Activator.CreateInstance (type);
      var method = GetDeclaredMethod (type, "OverridableMethod");

      Assert.That (method.GetBaseDefinition(), Is.SameAs (method));

      var result = method.Invoke (instance, null);
      Assert.That (result, Is.EqualTo ("DomainType shadowed"));
      Assert.That (instance.OverridableMethod (), Is.EqualTo ("DomainType"));
    }

    [Ignore ("TODO 4774")]
    [Test]
    public void ShadowGenericMethod_VirtualAndNewSlot ()
    {
      var type = AssembleType<DomainType> (
          proxyType =>
          {
            var mutableMethod = proxyType.AddGenericMethod (
                "OverridableGenericMethod", 
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot,
                new[]{new GenericParameterDeclaration("T")},
                ctx => typeof(string),
                ctx => ParameterDeclaration.None,
                ctx =>
                {
                  Assert.That (ctx.HasBaseMethod, Is.False);

                  return ExpressionHelper.StringConcat (
                      ctx.CallBase ("OverridableGenericMethod", ctx.Parameters.Cast<Expression> ()), Expression.Constant (" shadowed"));
                });
            Assert.That (mutableMethod.BaseMethod, Is.Null);
            Assert.That (mutableMethod.GetBaseDefinition(), Is.SameAs (mutableMethod));

            Assert.That (
                proxyType.GetMethods().Where (mi => mi.Name == "OverridableGenericMethod"),
                Is.EquivalentTo (new[] { mutableMethod, typeof (DomainType).GetMethod ("OverridableGenericMethod") }));
          });

      var instance = (DomainType) Activator.CreateInstance (type);
      var genericMethod = GetDeclaredMethod (type, "OverridableGenericMethod");

      Assert.That (genericMethod.GetBaseDefinition(), Is.SameAs (genericMethod));

      var method = genericMethod.MakeGenericMethod (typeof (string));
      var result = method.Invoke (instance, null);
      Assert.That (result, Is.EqualTo ("DomainType String shadowed"));
      Assert.That (instance.OverridableMethod(), Is.EqualTo ("DomainType"));
    }

    public class DomainType
    {
      public virtual string OverridableMethod ()
      {
        return "DomainType";
      }

      public virtual string OverridableGenericMethod<T> ()
      {
        return "DomainType " + typeof (T).Name;
      }
    }
  }
}