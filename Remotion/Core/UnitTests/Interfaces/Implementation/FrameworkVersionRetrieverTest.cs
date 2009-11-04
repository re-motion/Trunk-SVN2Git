// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Implementation;
using Remotion.Logging;
using Remotion.Mixins.BridgeImplementations;
using Rhino.Mocks;

namespace Remotion.UnitTests.Interfaces.Implementation
{
  [TestFixture]
  public class FrameworkVersionRetrieverTest
  {
    [SetUp]
    public void SetUp ()
    {
      LogManager.InitializeConsole ();
    }

    [Test]
    [ExpectedException (typeof (FrameworkVersionNotFoundException), 
        ExpectedMessage = "NonExistent is neither loaded nor referenced, and trying to load it by name ('NonExistent') didn't work either.")]
    public void Retrieve_NotPossible ()
    {
      var retriever = new FrameworkVersionRetriever ("NonExistent", new Assembly[0]);
      retriever.RetrieveVersion();
    }

    [Test]
    public void Retrieve_FromRootAssembly ()
    {
      var retriever = new FrameworkVersionRetriever ("Remotion", new[] { typeof (ObjectFactoryImplementation).Assembly });
      Assert.That (retriever.RetrieveVersion (), Is.EqualTo (typeof (INullObject).Assembly.GetName ().Version));
    }

    [Test]
    public void Retrieve_FromAssemblyReference ()
    {
      var retriever = new FrameworkVersionRetriever ("Remotion", new[] { typeof (FrameworkVersionRetrieverTest).Assembly });
      Assert.That (retriever.RetrieveVersion (), Is.EqualTo (typeof (INullObject).Assembly.GetName ().Version));
    }

    [Test]
    public void Retrieve_WithMultipleCandidates_WithSameVersions ()
    {
      var mockRepository = new MockRepository();
      var assemblyStub1 = mockRepository.Stub<_Assembly>();
      var assemblyStub2 = mockRepository.Stub<_Assembly>();

      var assemblyName1 = new AssemblyName ("Remotion");
      assemblyName1.Version = new Version (1, 2, 3, 4);
      var assemblyName2 = new AssemblyName ("Remotion");
      assemblyName2.Version = new Version (1, 2, 3, 4);

      SetupResult.For (assemblyStub1.GetName()).Return (assemblyName1);
      SetupResult.For (assemblyStub2.GetName()).Return (assemblyName2);

      SetupResult.For (assemblyStub1.GetReferencedAssemblies()).Return (new AssemblyName[0]);
      SetupResult.For (assemblyStub2.GetReferencedAssemblies()).Return (new AssemblyName[0]);

      mockRepository.ReplayAll();

      var retriever = new FrameworkVersionRetriever ("Remotion", new[] { assemblyStub1, assemblyStub2 });
      Assert.That (retriever.RetrieveVersion (), Is.EqualTo (new Version (1, 2, 3, 4)));
    }

    [Test]
    [ExpectedException (typeof (FrameworkVersionNotFoundException),
        ExpectedMessage = "More than one version of Remotion is currently loaded or referenced: 1.2.3.4, 2.3.4.5.")]
    public void Retrieve_WithMultipleCandidates_WithDifferentVersions ()
    {
      var mockRepository = new MockRepository();
      var assemblyStub1 = mockRepository.Stub<_Assembly>();
      var assemblyStub2 = mockRepository.Stub<_Assembly>();

      var assemblyName1 = new AssemblyName ("Remotion");
      assemblyName1.Version = new Version (1, 2, 3, 4);
      var assemblyName2 = new AssemblyName ("Remotion");
      assemblyName2.Version = new Version (2, 3, 4, 5);

      SetupResult.For (assemblyStub1.GetName()).Return (assemblyName1);
      SetupResult.For (assemblyStub2.GetName()).Return (assemblyName2);

      SetupResult.For (assemblyStub1.GetReferencedAssemblies()).Return (new AssemblyName[0]);
      SetupResult.For (assemblyStub2.GetReferencedAssemblies()).Return (new AssemblyName[0]);

      mockRepository.ReplayAll();

      var retriever = new FrameworkVersionRetriever ("Remotion", new[] { assemblyStub1, assemblyStub2 });
      retriever.RetrieveVersion();
    }

    [Test]
    public void Retrieve_FromDisk ()
    {
      var retriever = new FrameworkVersionRetriever ("Remotion", new Assembly[0]);
      Assert.That (retriever.RetrieveVersion (), Is.EqualTo (typeof (ObjectFactoryImplementation).Assembly.GetName ().Version));
    }
  }
}
