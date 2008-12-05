// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Implementation;
using Remotion.Mixins.BridgeImplementations;
using Rhino.Mocks;

namespace Remotion.UnitTests.Interfaces.Implementation
{
  [TestFixture]
  public class FrameworkVersionRetrieverTest
  {
    [Test]
    [ExpectedException (typeof (InvalidOperationException),ExpectedMessage = "There is no version of Remotion currently loaded or referenced.")]
    public void Retrieve_NotPossible ()
    {
      FrameworkVersionRetriever retriever = new FrameworkVersionRetriever (new Assembly[0]);
      retriever.RetrieveVersion ();
    }

    [Test]
    public void Retrieve_FromRootAssembly ()
    {
      FrameworkVersionRetriever retriever = new FrameworkVersionRetriever (new Assembly[] {typeof (MixedObjectInstantiator).Assembly});
      Assert.AreEqual (typeof (INullObject).Assembly.GetName().Version, retriever.RetrieveVersion());
    }

    [Test]
    public void Retrieve_FromAssemblyReference ()
    {
      FrameworkVersionRetriever retriever = new FrameworkVersionRetriever (new Assembly[] { typeof (FrameworkVersionRetrieverTest).Assembly });
      Assert.AreEqual (typeof (INullObject).Assembly.GetName ().Version, retriever.RetrieveVersion ());
    }

    [Test]
    public void Retrieve_WithMultipleCandidates_WithSameVersions ()
    {
      MockRepository mockRepository = new MockRepository();
      _Assembly assemblyStub1 = mockRepository.Stub<_Assembly> ();
      _Assembly assemblyStub2 = mockRepository.Stub<_Assembly> ();

      AssemblyName assemblyName1 = new AssemblyName("Remotion");
      assemblyName1.Version = new Version (1, 2, 3, 4);
      AssemblyName assemblyName2 = new AssemblyName ("Remotion");
      assemblyName2.Version = new Version (1, 2, 3, 4);

      SetupResult.For (assemblyStub1.GetName ()).Return (assemblyName1);
      SetupResult.For (assemblyStub2.GetName ()).Return (assemblyName2);

      SetupResult.For (assemblyStub1.GetReferencedAssemblies ()).Return (new AssemblyName[0]);
      SetupResult.For (assemblyStub2.GetReferencedAssemblies ()).Return (new AssemblyName[0]);

      mockRepository.ReplayAll();

      FrameworkVersionRetriever retriever = new FrameworkVersionRetriever (new _Assembly[] { assemblyStub1, assemblyStub2 });
      Assert.AreEqual (new Version (1, 2, 3, 4), retriever.RetrieveVersion ());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), 
        ExpectedMessage = "More than one version of Remotion is currently loaded or referenced: 1.2.3.4, 2.3.4.5.")]
    public void Retrieve_WithMultipleCandidates_WithDifferentVersions ()
    {
      MockRepository mockRepository = new MockRepository ();
      _Assembly assemblyStub1 = mockRepository.Stub<_Assembly> ();
      _Assembly assemblyStub2 = mockRepository.Stub<_Assembly> ();

      AssemblyName assemblyName1 = new AssemblyName ("Remotion");
      assemblyName1.Version = new Version (1, 2, 3, 4);
      AssemblyName assemblyName2 = new AssemblyName ("Remotion");
      assemblyName2.Version = new Version (2, 3, 4, 5);

      SetupResult.For (assemblyStub1.GetName ()).Return (assemblyName1);
      SetupResult.For (assemblyStub2.GetName ()).Return (assemblyName2);

      SetupResult.For (assemblyStub1.GetReferencedAssemblies ()).Return (new AssemblyName[0]);
      SetupResult.For (assemblyStub2.GetReferencedAssemblies ()).Return (new AssemblyName[0]);

      mockRepository.ReplayAll ();

      FrameworkVersionRetriever retriever = new FrameworkVersionRetriever (new _Assembly[] { assemblyStub1, assemblyStub2 });
      retriever.RetrieveVersion ();
    }
  }
}
