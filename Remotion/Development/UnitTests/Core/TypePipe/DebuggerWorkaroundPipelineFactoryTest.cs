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
using NUnit.Framework;
using Remotion.Development.TypePipe;
using Remotion.Development.TypePipe.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Diagnostics;
using Remotion.Reflection.TypeDiscovery;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.Configuration;

namespace Remotion.Development.UnitTests.Core.TypePipe
{
  [TestFixture]
  public class DebuggerWorkaroundPipelineFactoryTest
  {
    private DebuggerWorkaroundPipelineFactory _factory;

    [SetUp]
    public void SetUp ()
    {
      _factory = new DebuggerWorkaroundPipelineFactory();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_factory.DebuggerInterface, Is.Not.Null.And.TypeOf<DebuggerInterface>());
      Assert.That (_factory.MaximumTypesPerAssembly, Is.EqualTo (11));
    }

    [Test]
    public void NewReflectionEmitCodeGenerator ()
    {
      var participantID = "ParticipantID";
      var forceStrongNaming = BooleanObjectMother.GetRandomBoolean();
      var keyFilePath = "keyFilePath";
      string assemblyDirectory = null;
      var assemblyNamePattern = "DebuggerAssemblies_{counter}";
      _factory.MaximumTypesPerAssembly = 7;

      var result = _factory.Invoke<object> (
          "NewReflectionEmitCodeGenerator",
          participantID,
          forceStrongNaming,
          keyFilePath,
          assemblyDirectory,
          assemblyNamePattern);

      var actualResult = PrivateInvoke.GetNonPublicField (result, "_reflectionEmitCodeGenerator");
      Assert.That (actualResult, Is.TypeOf<DebuggerWorkaroundCodeGenerator>());
      var debuggerWorkaroundCodeGenerator = (DebuggerWorkaroundCodeGenerator) actualResult;
      Assert.That (debuggerWorkaroundCodeGenerator.MaximumTypesPerAssembly, Is.EqualTo (7));
    }

    [Test]
    public void Integration_CreatedPipeline_AddsNonApplicationAssemblyAttribute_OnModuleCreation ()
    {
      // Creates new in-memory assembly (avoid no-modification optimization).
      var participantConfigurationID = "dummy id";
      var settings = PipelineSettings.New().Build();
      var defaultPipeline = _factory.CreatePipeline (participantConfigurationID, settings, new[] { new ModifyingParticipant() });
      var type = defaultPipeline.ReflectionService.GetAssembledType (typeof (RequestedType));

      Assert.That (type.Assembly.IsDefined (typeof (NonApplicationAssemblyAttribute), false), Is.True);
    }

    public class RequestedType { }
  }
}