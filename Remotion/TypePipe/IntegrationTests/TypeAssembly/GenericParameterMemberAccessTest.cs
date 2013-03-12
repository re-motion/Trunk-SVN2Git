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
using JetBrains.Annotations;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;

namespace Remotion.TypePipe.IntegrationTests.TypeAssembly
{
  [TestFixture]
  public class GenericParameterMemberAccessTest : TypeAssemblerIntegrationTestBase
  {
    [Ignore("TODO 5444")]
    [Test]
    public void AccessMembers ()
    {
      SkipDeletion();

      var overriddenMethod =
          NormalizingMemberInfoFromExpressionUtility.GetGenericMethodDefinition ((DomainType o) => o.GenericMethod<Constraint> (null, ""));
      var field = NormalizingMemberInfoFromExpressionUtility.GetField ((Constraint o) => o.Field);
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((Constraint o) => o.Method(""));
      var property = NormalizingMemberInfoFromExpressionUtility.GetProperty ((Constraint o) => o.Property);

      var type = AssembleType<DomainType> (p => p.GetOrAddOverride (overriddenMethod).SetBody (ctx =>
      {
        var parameter = ctx.Parameters[0];
        var variable = Expression.Variable (ctx.GenericParameters[0]);

        return Expression.Block (
            new[] { variable },
            Expression.Assign (variable, Expression.Call (parameter, method, ctx.Parameters[0])),
            Expression.Assign (Expression.Field (parameter, field), variable),
            Expression.Assign (Expression.Property (parameter, property), Expression.Field (parameter, field)),
            Expression.Property (parameter, property));
      }));

      var instance = (DomainType) Activator.CreateInstance (type);
      var arg = new Constraint();

      var result = instance.GenericMethod (arg, "abc");

      Assert.That (arg.Field, Is.EqualTo ("method: abc"));
      Assert.That (arg.Property, Is.EqualTo ("method: abc"));
      Assert.That (result, Is.EqualTo ("method: abc"));
    }

    [Test]
    public void CallVirtualMethod ()
    {
      // TODO 5444: remove
      SkipDeletion();

      var overriddenMethod =
          NormalizingMemberInfoFromExpressionUtility.GetGenericMethodDefinition ((DomainType o) => o.GenericMethod<Constraint> (null, ""));
      var virtualMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((Constraint o) => o.VirtualMethod (""));

      var type = AssembleType<DomainType> (
          p => p.GetOrAddOverride (overriddenMethod).SetBody (ctx => Expression.Call (ctx.Parameters[0], virtualMethod, ctx.Parameters[1])));

      var instance = (DomainType) Activator.CreateInstance (type);
      var arg = new Constraint();

      var result = instance.GenericMethod (arg, "abc");

      Assert.That (result, Is.EqualTo ("virtual method: abc"));
    }

    public class DomainType
    {
      public virtual string GenericMethod<T> (T t, string arg) where T : Constraint { return ""; }
    }

    public class Constraint
    {
      [UsedImplicitly] public string Field;
      // Constructors are not called via 'this' reference.
      public string Method (string arg) { return "method: " + arg; }
      public string Property { get; set; }
      // Events do not have a representation in expression trees.

      public virtual string VirtualMethod (string arg) { return "virtual method: " + arg; }
    }
  }
}