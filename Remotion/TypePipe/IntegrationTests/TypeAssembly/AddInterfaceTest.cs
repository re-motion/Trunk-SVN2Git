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
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.TypePipe.MutableReflection;

namespace Remotion.TypePipe.IntegrationTests.TypeAssembly
{
  [TestFixture]
  public class AddInterfaceTest : TypeAssemblerIntegrationTestBase
  {
    [Test]
    public void MarkerInterface ()
    {
      Assert.That (typeof (DomainType).GetInterfaces(), Is.EquivalentTo (new[] { typeof (IOriginalInterface) }));

      var type = AssembleType<DomainType> (mutableType => mutableType.AddInterface (typeof (IMarkerInterface)));

      Assert.That (type.GetInterfaces(), Is.EquivalentTo (new[] { typeof (IOriginalInterface), typeof (IMarkerInterface) }));
    }

    [Test]
    public void AddMethodAsExplicitInterfaceImplementation ()
    {
      var interfaceMethod = GetDeclaredMethod (typeof (IInterfaceWithMethod), "Method");
      var type = AssembleType<DomainType> (
          mutableType =>
          {
            mutableType.AddInterface (typeof (IInterfaceWithMethod));
            var mutableMethodInfo = mutableType.AddMethod (
                "DifferentName",
                MethodAttributes.Private | MethodAttributes.Virtual,
                typeof (string),
                ParameterDeclaration.EmptyParameters,
                ctx =>
                {
                  Assert.That (ctx.HasBaseMethod, Is.False);
                  return Expression.Constant ("explicitly implemented");
                });
            mutableMethodInfo.AddExplicitBaseDefinition (interfaceMethod);
            Assert.That (mutableMethodInfo.AddedExplicitBaseDefinitions, Is.EqualTo (new[] { interfaceMethod }));
            Assert.That (mutableMethodInfo.BaseMethod, Is.Null);
            Assert.That (mutableMethodInfo.GetBaseDefinition (), Is.EqualTo (mutableMethodInfo));
          });

      var instance = (DomainType) Activator.CreateInstance (type);
      Assert.That (instance, Is.AssignableTo<IInterfaceWithMethod>());

      var method = GetDeclaredMethod (type, "DifferentName");

      // Reflection doesn't handle explicit overrides in GetBaseDefinition.
      // If this changes, MutableMethodInfo.GetBaseDefinition() must be changed as well.
      Assert.That (method.GetBaseDefinition (), Is.EqualTo (method));

      var result = method.Invoke (instance, null);
      Assert.That (result, Is.EqualTo ("explicitly implemented"));
      Assert.That (((IInterfaceWithMethod) instance).Method (), Is.EqualTo ("explicitly implemented"));
    }

    public class DomainType : IOriginalInterface { }

    public interface IOriginalInterface { }

    public interface IMarkerInterface { }

    public interface IInterfaceWithMethod
    {
      string Method ();
    }
  }
}