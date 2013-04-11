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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe.Configuration;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.Configuration
{
  [TestFixture]
  public class DefaultPipelineRegistryTest
  {
    [Test]
    public void Initialization_RegistersDefaultPipeline_WithParticipantsFromServiceLocator ()
    {
      var participant = MockRepository.GenerateStub<IParticipant>();

      DefaultPipelineRegistry registry;
      using (new ServiceLocatorScope (typeof (IParticipant), () => participant))
        registry = new DefaultPipelineRegistry();

      var defaultPipeline = registry.DefaultPipeline;
      Assert.That (defaultPipeline, Is.Not.Null);
      Assert.That (defaultPipeline.ParticipantConfigurationID, Is.EqualTo ("<default pipeline>"));
      Assert.That (defaultPipeline.Participants, Is.EqualTo (new[] { participant }));
    }
  }
}