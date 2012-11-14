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
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.TypePipe;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Rhino.Mocks;

namespace TypePipe.IntegrationTests.TypeAssembly
{
  public abstract class TypeAssemblerIntegrationTestBase : IntegrationTestBase
  {
    private const BindingFlags c_allDeclared =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    [MethodImpl (MethodImplOptions.NoInlining)]
    protected Type AssembleType<T> (params Action<MutableType>[] participantActions)
    {
      return AssembleType (typeof (T), participantActions, 1);
    }

    [MethodImpl (MethodImplOptions.NoInlining)]
    protected Type AssembleType (Type requestedType, params Action<MutableType>[] participantActions)
    {
      return AssembleType (requestedType, participantActions, 1);
    }

    [MethodImpl (MethodImplOptions.NoInlining)]
    protected Type AssembleType (Type requestedType, IEnumerable<Action<MutableType>> participantActions, int stackFramesToSkip)
    {
      var testName = GetNameForThisTest (stackFramesToSkip + 1);
      return AssembleType (testName, requestedType, participantActions);
    }

    protected MethodInfo GetDeclaredMethod (Type type, string name)
    {
      var method = type.GetMethod (name, c_allDeclared);
      Assert.That (method, Is.Not.Null);
      return method;
    }

    protected MethodInfo GetDeclaredExplicitOverrideMethod (Type type, string nameSuffix)
    {
      var method = type.GetMethods (c_allDeclared).SingleOrDefault (m => m.Name.EndsWith ("_" + nameSuffix));
      Assert.That (method, Is.Not.Null);
      return method;
    }

    protected MutableMethodInfo AddEquivalentMethod (
        MutableType mutableType,
        MethodInfo template,
        MethodAttributes adjustedAttributes,
        Func<MethodBodyCreationContext, Expression> bodyProvider = null)
    {
      return mutableType.AddMethod (
          template.Name,
          adjustedAttributes,
          template.ReturnType,
          ParameterDeclaration.CreateForEquivalentSignature (template),
          bodyProvider ?? (ctx => Expression.Default (template.ReturnType)));
    }

    private Type AssembleType (string testName, Type requestedType, IEnumerable<Action<MutableType>> participantActions)
    {
      var participants = participantActions.Select (CreateParticipant).AsOneTime();
      var typeModifier = CreateReflectionEmitTypeModifier (testName);
      var typeAssembler = new TypeAssembler (participants, typeModifier);
      var assembledType = typeAssembler.AssembleType (requestedType);

      return assembledType;
    }
  }
}