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
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.CodeGeneration;

namespace Remotion.TypePipe.TypeAssembly.Implementation
{
  /// <summary>
  /// Generates types for requested types and computes compound identifiers to enabled efficient caching of generated types.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  public interface ITypeAssembler
  {
    string ParticipantConfigurationID { get; }
    ReadOnlyCollection<IParticipant> Participants { get; }

    bool IsAssembledType ([NotNull] Type type);

    [NotNull]
    Type GetRequestedType ([NotNull] Type assembledType);

    /// <summary>
    /// Computes the <see cref="AssembledTypeID"/> from a requested type.
    /// </summary>
    AssembledTypeID ComputeTypeID ([NotNull] Type requestedType);

    AssembledTypeID ExtractTypeID ([NotNull] Type assembledType);

    [NotNull]
    TypeAssemblyResult AssembleType (
        AssembledTypeID typeID,
        [NotNull] IParticipantState participantState,
        [NotNull] IMutableTypeBatchCodeGenerator codeGenerator);

    [NotNull]
    TypeAssemblyResult AssembleAdditionalType (
        [NotNull] object additionalTypeID,
        [NotNull] IParticipantState participantState,
        [NotNull] IMutableTypeBatchCodeGenerator codeGenerator);

    [CanBeNull]
    object GetAdditionalTypeID ([NotNull] Type additionalType);
  }
}