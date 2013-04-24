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
using Remotion.TypePipe.Dlr.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe.MutableReflection;
using System.Linq;

namespace Remotion.TypePipe.IntegrationTests.TypeAssembly
{
  [TestFixture]
  public class ModifyConstructorTest : TypeAssemblerIntegrationTestBase
  {
    [Test]
    public void ModifyCopiedConstructor ()
    {
      var type = AssembleType<DomainType> (
          p =>
          p.AddedConstructors
           .Single (c => c.GetParameters().Length == 1)
           .SetBody (
               ctx => Expression.Block (
                   ctx.InvokePreviousBodyWithArguments (ExpressionHelper.StringConcat (ctx.Parameters[0], Expression.Constant (" cd"))),
                   // TODO 4744: Use Expression.Property (ctx.This, "SettableProperty")
                   Expression.Assign (Expression.Property (ctx.This, typeof (DomainType).GetProperty ("SettableProperty")), ctx.Parameters[0]))));

      var instance = (DomainType) Activator.CreateInstance (type, "ab");

      Assert.That (instance.ctorArg, Is.EqualTo ("ab cd"));
      Assert.That (instance.SettableProperty, Is.EqualTo ("ab"));
    }

    [Test]
    public void ModifyAddedConstructor ()
    {
      var type = AssembleType<DomainType> (
          proxyType => proxyType.AddConstructor (
              MethodAttributes.Public,
              ParameterDeclaration.None,
              ctx => ctx.CallThisConstructor (Expression.Constant ("added"))),
          proxyType =>
          {
            var addedCtor = proxyType.AddedConstructors.Single (c => c.GetParameters().Length == 0);
            addedCtor.SetBody (ctx => ctx.CallThisConstructor (Expression.Constant ("modified added")));
          });

      var instance = (DomainType) Activator.CreateInstance (type);

      Assert.That (instance.ctorArg, Is.EqualTo ("modified added"));
    }

    [Test]
    public void AddedCtor_DelegatesTo_ModifiedCopiedCtor ()
    {
      var type = AssembleType<DomainType> (
          proxyType => proxyType.AddConstructor (
              MethodAttributes.Public,
              ParameterDeclaration.None,
              ctx => ctx.CallThisConstructor (Expression.Constant ("added"))),
          proxyType =>
          {
            var ctor = proxyType.AddedConstructors.Single (c => c.GetParameters().Length == 1);
            ctor.SetBody (ctx => ctx.InvokePreviousBodyWithArguments (Expression.Constant ("modified existing")));
          });

      var instance = (DomainType) Activator.CreateInstance (type);

      Assert.That (instance.ctorArg, Is.EqualTo ("modified existing"));
    }

    [Test]
    public void ModifiedExistingCtor_DelegatesTo_AddedCtor ()
    {
      var type = AssembleType<DomainType> (
          proxyType => proxyType.AddConstructor (
              MethodAttributes.Public,
              ParameterDeclaration.None,
              ctx => ctx.CallThisConstructor (Expression.Constant ("added"))),
          proxyType =>
          {
            var ctor = proxyType.AddedConstructors.Single (c => c.GetParameters().Length == 2);
            ctor.SetBody (
                ctx => Expression.Block (
                    ctx.CallThisConstructor(),
                    // TODO 4744: Use Expression.Property (ctx.This, "SettableProperty")
                    Expression.Assign (Expression.Property (ctx.This, typeof (DomainType).GetProperty ("SettableProperty")), ctx.Parameters[1])));
          });

      var instance = (DomainType) Activator.CreateInstance (type, "ignored", "modified existing");

      Assert.That (instance.ctorArg, Is.EqualTo ("added"));
      Assert.That (instance.SettableProperty, Is.EqualTo ("modified existing"));
    }

    public class DomainType
    {
      public DomainType (string ctorArg)
      {
        this.ctorArg = ctorArg;
      }

      public DomainType (string ctorArg1, string ctorArg2)
      {
        Dev.Null = ctorArg1;
        Dev.Null = ctorArg2;
        SettableProperty = "blah";
      }

      public string ctorArg { get; private set; }
      public string SettableProperty { get; set; }
    }
  }
}