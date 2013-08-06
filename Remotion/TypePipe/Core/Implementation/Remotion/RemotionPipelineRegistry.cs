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
using Remotion.Reflection.TypeDiscovery;
using Remotion.TypePipe.Configuration;
using Remotion.Utilities;

namespace Remotion.TypePipe.Implementation.Remotion
{
  // TODO 5545: Move this to Remotion.Core.
  /// <summary>
  /// Creates and registers a <see cref="IPipelineRegistry.DefaultPipeline"/> containing the specified participants.
  /// Uses the <see cref="RemotionPipelineFactory"/> which creates pipeline instances that immediately apply the
  /// <see cref="NonApplicationAssemblyAttribute"/> to the in-memory assembly in order to retain original re-mix behavior.
  /// </summary>
  public class RemotionPipelineRegistry : PipelineRegistry
  {
    private static IPipeline CreateDefaultPipeline (IEnumerable<IParticipant> defaultPipelineParticipants)
    {
      var remotionPipelineFactory = new RemotionPipelineFactory();
      var settings = new AppConfigBasedSettingsProvider().GetSettings();
      // TODO 5730: This should be set via config files when implemented.
      // It is now implemented, so we just need to add config files!!!
      var settings2 = PipelineSettings.From (settings).SetEnableSerializationWithoutAssemblySaving (true).Build();

      return remotionPipelineFactory.CreatePipeline ("remotion-default-pipeline", settings2, defaultPipelineParticipants);
    }

    public RemotionPipelineRegistry (IEnumerable<IParticipant> defaultPipelineParticipants)
        : base (CreateDefaultPipeline (ArgumentUtility.CheckNotNull ("defaultPipelineParticipants", defaultPipelineParticipants)))
    {
    }
  }
}