// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.IO;
using Remotion.Logging;
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.TypePipe.Implementation.Remotion;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Mixins.MixerTools
{
  public class MixerPipelineFactory : IMixerPipelineFactory
  {
    private static readonly ILog s_log = LogManager.GetLogger(typeof(MixerPipelineFactory));

    private readonly string _assemblyName;

    public MixerPipelineFactory (string assemblyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("assemblyName", assemblyName);

      _assemblyName = assemblyName;
    }

    public string AssemblyName
    {
      get { return _assemblyName; }
    }

    public IPipeline CreatePipeline (string assemblyOutputDirectory)
    {
      // Assembly output directory may be null.

      var remotionPipelineFactory = new RemotionPipelineFactory();
      var defaultPipeline = SafeServiceLocator.Current.GetInstance<IPipelineRegistry>().DefaultPipeline;
      var participants = defaultPipeline.Participants.ToArray();

      var participantTypeNames = participants.Select (p => p.GetType().Name).ToArray();
      s_log.InfoFormat (
          "Using pipeline '{0}' with the following participants: {1}.",
          defaultPipeline.ParticipantConfigurationID,
          string.Join (", ", participantTypeNames));
      Assertion.DebugAssert (participants.OfType<MixinParticipant>().Any(), "Mixin participant must be present.");

      var pipeline = remotionPipelineFactory.CreatePipeline (
          defaultPipeline.ParticipantConfigurationID,
          defaultPipeline.Settings,
          participants);

      pipeline.CodeManager.SetAssemblyDirectory (assemblyOutputDirectory);
      pipeline.CodeManager.SetAssemblyNamePattern (_assemblyName);

      return pipeline;
    }

    public string GetModulePath (string assemblyOutputDirectory)
    {
      return Path.Combine (assemblyOutputDirectory, _assemblyName + ".dll");
    }
  }
}
