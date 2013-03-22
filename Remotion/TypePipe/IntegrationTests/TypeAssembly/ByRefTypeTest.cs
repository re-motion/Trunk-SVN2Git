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
using System.Reflection;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.TypePipe.MutableReflection;

namespace Remotion.TypePipe.IntegrationTests.TypeAssembly
{
  [TestFixture]
  public class ByRefTypeTest : TypeAssemblerIntegrationTestBase
  {
    [Ignore ("TOOD 5417")]
    [Test]
    public void MutableByRefTypeAsMethodParameter ()
    {
      var type = AssembleType<DomainType> (
          typeContext =>
          {
            var byRefType = typeContext.ProxyType.MakeByRefType();
            typeContext.ProxyType.AddMethod (
                "Method",
                MethodAttributes.Public | MethodAttributes.Static,
                typeof (void),
                new[] { new ParameterDeclaration (byRefType) },
                ctx => Expression.Assign (ctx.Parameters[0], Expression.New (typeContext.ProxyType)));
          });

      var method = type.GetMethod ("Method");
      var arguments = new object[] { null };
      method.Invoke (null, arguments);

      Assert.That (arguments[0], Is.Not.Null.And.TypeOf (type));
    }

    public class DomainType {}
  }
}