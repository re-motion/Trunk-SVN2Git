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
using System.Collections.ObjectModel;

namespace Remotion.TypePipe.Caching
{
  /// <summary>
  /// Caches the <see cref="Type"/>s generated by the pipeline and supports loading of <see cref="Type"/>s.
  /// Also caches <see cref="Delegate"/>s that perform constructor calls to enable efficient object creation.
  /// </summary>
  public interface ITypeCache
  {
    string ParticipantConfigurationID { get; }
    ReadOnlyCollection<IParticipant> Participants { get; }

    Type GetOrCreateType (Type requestedType);
    Type GetOrCreateType (AssembledTypeID typeID);

    Delegate GetOrCreateConstructorCall (Type requestedType, Type delegateType, bool allowNonPublic);

    // TODO 5370: Remove from interface?
    Delegate GetOrCreateConstructorCall (AssembledTypeID typeID, Type delegateType, bool allowNonPublic);

    void LoadTypes (IEnumerable<Type> generatedTypes);

    Type GetOrCreateAdditionalType (object additionalTypeID);
  }
}