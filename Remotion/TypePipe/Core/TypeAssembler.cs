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
using System.Linq;
using Remotion.TypePipe.CodeGeneration;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.TypePipe
{
  /// <summary>
  /// Provides functionality for assembling a type by orchestrating <see cref="ITypePipeParticipant"/> instances and an instance of 
  /// <see cref="ITypeModifier"/>.
  /// </summary>
  public class TypeAssembler
  {
    private readonly ReadOnlyCollection<ITypePipeParticipant> _participants;
    private readonly ITypeModifier _typeModifier;

    public TypeAssembler (IEnumerable<ITypePipeParticipant> participants, ITypeModifier typeModifier)
    {
      ArgumentUtility.CheckNotNull ("participants", participants);
      ArgumentUtility.CheckNotNull ("typeModifier", typeModifier);

      _participants = participants.ToList().AsReadOnly();
      _typeModifier = typeModifier;
    }

    public ReadOnlyCollection<ITypePipeParticipant> Participants
    {
      get { return _participants; }
    }

    public ITypeModifier TypeModifier
    {
      get { return _typeModifier; }
    }

    public Type AssembleType (Type requestedType)
    {
      var mutableType = CreateMutableType (requestedType);

      foreach (var participant in _participants)
        participant.ModifyType (mutableType);

      return _typeModifier.ApplyModifications (mutableType);
    }

    private MutableType CreateMutableType (Type requestedType)
    {
      var underlyingTypeDescriptor = UnderlyingTypeDescriptor.Create (requestedType);
      var memberSelector = new MemberSelector (new BindingFlagsEvaluator());
      var relatedMethodFinder = new RelatedMethodFinder();
      var mutableMemberFactory = new MutableMemberFactory (memberSelector, relatedMethodFinder);

      return new MutableType (underlyingTypeDescriptor, memberSelector, relatedMethodFinder, mutableMemberFactory);
    }
  }
}