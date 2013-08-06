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
using NUnit.Framework;
using Remotion.Mixins.MixerTools;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Reflection.TypeDiscovery;
using Remotion.ServiceLocation;
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core.MixerTools
{
  [TestFixture]
  public class MixerPipelineFactoryTest
  {
    private MixerPipelineFactory _factory;

    [SetUp]
    public void SetUp ()
    {
      _factory = new MixerPipelineFactory ("Assembly");
    }
    
    [Test]
    public void CreatePipeline ()
    {
      var pipeline = _factory.CreatePipeline (@"c:\directory");

      CheckRemotionPipelineFactoryWasUsedForCreation (pipeline);

      var defaultPipeline = SafeServiceLocator.Current.GetInstance<IPipelineRegistry>().DefaultPipeline;
      Assert.That (pipeline.ParticipantConfigurationID, Is.EqualTo (defaultPipeline.ParticipantConfigurationID));
      Assert.That (pipeline.Settings, Is.SameAs (defaultPipeline.Settings));
      Assert.That (pipeline.Participants, Is.EqualTo (defaultPipeline.Participants));

      Assert.That (pipeline.CodeManager.AssemblyNamePattern, Is.EqualTo ("Assembly"));
      Assert.That (pipeline.CodeManager.AssemblyDirectory, Is.EqualTo (@"c:\directory"));
    }

    [Test]
    public void GetModulePath ()
    {
      Assert.That (_factory.GetModulePath (@"c:\directory"), Is.EqualTo (@"c:\directory\Assembly.dll"));
    }

    private void CheckRemotionPipelineFactoryWasUsedForCreation (IPipeline pipeline)
    {
      var assembledType = pipeline.ReflectionService.GetAssembledType (typeof (TargetClassForGlobalMix));
      var assembly = assembledType.Assembly;

      // RemotionPipelineFactory will add the [NonApplicationAssemblyAttribute] to the generated assembly.
      Assert.That (assembly.IsDefined (typeof (NonApplicationAssemblyAttribute), inherit: false), Is.True);
    }
  }
}
