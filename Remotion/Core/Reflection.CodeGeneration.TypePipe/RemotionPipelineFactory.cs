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
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.Implementation;

namespace Remotion.Reflection.CodeGeneration.TypePipe
{
  /// <summary>
  /// Decorates created <see cref="IModuleBuilderFactory"/> instances with <see cref="RemotionModuleBuilderFactoryDecorator"/>.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor (typeof (IPipelineFactory), Lifetime = LifetimeKind.Singleton)]
  public class RemotionPipelineFactory : DefaultPipelineFactory
  {
    [CLSCompliant (false)]
    protected override IModuleBuilderFactory NewModuleBuilderFactory (string participantConfigurationID)
    {
      var moduleBuilderFactory = base.NewModuleBuilderFactory (participantConfigurationID);
      return new RemotionModuleBuilderFactoryDecorator (moduleBuilderFactory);
    }
  }
}
