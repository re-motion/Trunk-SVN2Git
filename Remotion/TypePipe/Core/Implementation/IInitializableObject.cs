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
using System.Runtime.Serialization;

namespace Remotion.TypePipe.Implementation
{
  /// <summary>
  /// This is an infastructure interface and not meant to be used outside of TypePipe code.
  /// If a generated type implements this interface, the <see cref="Pipeline"/> will invoke the <see cref="Initialize"/> method when creating 
  /// instances of it.
  /// </summary>
  /// <remarks>
  /// If a type generated by the pipeline is instantiated not by calling a constructor but through
  /// <see cref="FormatterServices.GetUninitializedObject"/>, the resulting instance must be prepared with
  /// <see cref="IPipeline.PrepareExternalUninitializedObject"/> before usage.
  /// </remarks>
  public interface IInitializableObject
  {
    void Initialize (InitializationSemantics initializationSemantics);
  }
}