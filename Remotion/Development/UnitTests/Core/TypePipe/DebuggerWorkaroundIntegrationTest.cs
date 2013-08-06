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
using Remotion.Diagnostics;
using Remotion.TypePipe;
using Remotion.TypePipe.Implementation;
using Rhino.Mocks;

namespace Remotion.Development.UnitTests.Core.TypePipe
{
  public class DebuggerWorkaroundIntegrationTest
  {
    private IPipelineFactory _previousPipelineFactory;
    private int _maximumTypesPerAssembly;
    private IDebuggerInterface _debuggerStub;

    private IPipeline _pipeline;

    [SetUp]
    public void SetUp ()
    {
      _previousPipelineFactory = (IPipelineFactory) PrivateInvoke.GetNonPublicStaticField (typeof (PipelineFactory), "s_instance");
      _debuggerStub = MockRepository.GenerateStub<IDebuggerInterface>();
      _maximumTypesPerAssembly = 2;

      var debuggerWorkaroundPipelineFactory =
          new DebuggerWorkaroundPipelineFactory { DebuggerInterface = _debuggerStub, MaximumTypesPerAssembly = _maximumTypesPerAssembly };
      PrivateInvoke.SetNonPublicStaticField (typeof (PipelineFactory), "s_instance", debuggerWorkaroundPipelineFactory);

      var modifyingParticipant = new ModifyingParticipant();
      _pipeline = PipelineFactory.Create ("DebuggerWorkaroundIntegrationTest", modifyingParticipant);
    }

    [TearDown]
    public void TearDown ()
    {
      PrivateInvoke.SetNonPublicStaticField (typeof (PipelineFactory), "s_instance", _previousPipelineFactory);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Method FlushCodeToDisk is not supported by DebuggerWorkaroundCodeGenerator.")]
    public void FlushCodeToDisk ()
    {
      _pipeline.CodeManager.FlushCodeToDisk();
    }

    [Test]
    public void AttachedDebugger_StartsNewAssemblyWhenReachingThreshold ()
    {
      _debuggerStub.Stub (stub => stub.IsAttached).Return (true);

      var reflectionService = _pipeline.ReflectionService;
      var type1 = reflectionService.GetAssembledType (typeof (Type1));
      var type2 = reflectionService.GetAssembledType (typeof (Type2));
      var type3 = reflectionService.GetAssembledType (typeof (Type3));
      var type4 = reflectionService.GetAssembledType (typeof (Type4));
      var type5 = reflectionService.GetAssembledType (typeof (Type5));

      Assert.That (_maximumTypesPerAssembly, Is.EqualTo (2));
      Assert.That (type1.Assembly, Is.SameAs (type2.Assembly));
      Assert.That (type3.Assembly, Is.SameAs (type4.Assembly));

      Assert.That (type1.Assembly, Is.Not.SameAs (type3.Assembly));
      Assert.That (type1.Assembly, Is.Not.SameAs (type5.Assembly));
      Assert.That (type3.Assembly, Is.Not.SameAs (type5.Assembly));
    }

    [Test]
    public void NoDebugger_GeneratesSingleAssembly ()
    {
      _debuggerStub.Stub (stub => stub.IsAttached).Return (false);

      var reflectionService = _pipeline.ReflectionService;
      var type1 = reflectionService.GetAssembledType (typeof (Type1));
      var type2 = reflectionService.GetAssembledType (typeof (Type2));
      var type3 = reflectionService.GetAssembledType (typeof (Type3));

      Assert.That (_maximumTypesPerAssembly, Is.EqualTo (2));
      Assert.That (type1.Assembly, Is.SameAs (type2.Assembly));
      Assert.That (type1.Assembly, Is.SameAs (type3.Assembly));
    }

    public class Type1 { }
    public class Type2 { }
    public class Type3 { }
    public class Type4 { }
    public class Type5 { }
  }
}