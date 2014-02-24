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
using Remotion.Reflection.CodeGeneration.TypePipe.Configuration;
using Remotion.Reflection.TypeDiscovery;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.TypePipe.Implementation;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.TypePipe
{
  /// <summary>
  /// Creates and registers a <see cref="IPipelineRegistry.DefaultPipeline"/> containing the specified participants.
  /// Uses the <see cref="RemotionPipelineFactory"/> which creates pipeline instances that immediately apply the
  /// <see cref="NonApplicationAssemblyAttribute"/> to the in-memory assembly in order to retain original re-mix behavior.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor (typeof (IPipelineRegistry), Lifetime = LifetimeKind.Singleton)]
  public class RemotionPipelineRegistry : DefaultPipelineRegistry
  {
    private static IPipeline CreateDefaultPipeline (IEnumerable<IParticipant> defaultPipelineParticipants)
    {
      var remotionPipelineFactory = new RemotionPipelineFactory();
      var settings = new AppConfigBasedSettingsProvider().GetSettings();
      return remotionPipelineFactory.Create ("remotion-default-pipeline", settings, defaultPipelineParticipants);
    }

    public RemotionPipelineRegistry (IEnumerable<IParticipant> defaultPipelineParticipants)
        : base (CreateDefaultPipeline (ArgumentUtility.CheckNotNull ("defaultPipelineParticipants", defaultPipelineParticipants)))
    {
    }
  }
}