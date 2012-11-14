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
using System.Runtime.CompilerServices;
using Remotion.Reflection;
using Remotion.TypePipe;
using Remotion.TypePipe.Caching;

namespace TypePipe.IntegrationTests
{
  public abstract class ObjectFactoryIntegrationTestBase : IntegrationTestBase
  {
    [MethodImpl (MethodImplOptions.NoInlining)]
    protected IObjectFactory CreateObjectFactory (params IParticipant[] participants)
    {
      return CreateObjectFactory (participants, 1);
    }

    [MethodImpl (MethodImplOptions.NoInlining)]
    protected IObjectFactory CreateObjectFactory (IEnumerable<IParticipant> participants, int stackFramesToSkip)
    {
      var testName = GetNameForThisTest (stackFramesToSkip + 1);
      var typeModifier = CreateReflectionEmitTypeModifier (testName);
      var typeAssembler = new TypeAssembler (participants, typeModifier);
      var constructorFinder = new ConstructorFinder();
      var delegateFactory = new DelegateFactory();
      var typeCache = new TypeCache (typeAssembler, constructorFinder, delegateFactory);

      return new ObjectFactory (typeCache);
    }
  }
}