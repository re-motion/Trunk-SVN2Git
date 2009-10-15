// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Rhino.Mocks;
using System.Reflection;
using System.Linq;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyFinding
{
  [TestFixture]
  public class NamedRootAssemblyFinderTest
  {
    private IAssemblyLoader _loaderMock;

    private Assembly _assembly1;
    private Assembly _assembly2;
    private Assembly _assembly3;
    
    private AssemblyName _name1;
    private AssemblyName _name2;
    private AssemblyName _name3;
    
    private NamedRootAssemblyFinder.Specification _specification1;
    private NamedRootAssemblyFinder.Specification _specification2;
    private NamedRootAssemblyFinder.Specification _specification3;

    [SetUp]
    public void SetUp ()
    {
      _loaderMock = MockRepository.GenerateMock<IAssemblyLoader> ();

      _assembly1 = typeof (object).Assembly;
      _assembly2 = typeof (NamedRootAssemblyFinder).Assembly;
      _assembly3 = typeof (NamedRootAssemblyFinderTest).Assembly;

      _name1 = new AssemblyName ("n1");
      _name2 = new AssemblyName ("n2");
      _name3 = new AssemblyName ("n3, Version=1.0.1.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

      _specification1 = new NamedRootAssemblyFinder.Specification (_name1, true);
      _specification2 = new NamedRootAssemblyFinder.Specification (_name2, false);
      _specification3 = new NamedRootAssemblyFinder.Specification (_name3, true);
    }

    [Test]
    public void FindAssemblies ()
    {
      _loaderMock.Expect (mock => mock.TryLoadAssembly (_name1, "Specification: n1")).Return (_assembly1);
      _loaderMock.Expect (mock => mock.TryLoadAssembly (_name2, "Specification: n2")).Return (_assembly2);
      _loaderMock
          .Expect (mock => mock.TryLoadAssembly (_name3, "Specification: n3, Version=1.0.1.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"))
          .Return (_assembly3);
      _loaderMock.Replay ();

      var finder = new NamedRootAssemblyFinder (new[] { _specification1, _specification2, _specification3 });
      var assemblies = finder.FindRootAssemblies (_loaderMock);

      _loaderMock.VerifyAllExpectations ();
      Assert.That (assemblies.Select (a => a.Assembly).ToArray(), Is.EquivalentTo (new[] { _assembly1, _assembly2, _assembly3 }));
    }

    [Test]
    public void FindAssemblies_NullsRemoved ()
    {
      _loaderMock.Expect (mock => mock.TryLoadAssembly (Arg.Is (_name1), Arg<string>.Is.Anything)).Return (_assembly1);
      _loaderMock.Expect (mock => mock.TryLoadAssembly (Arg.Is (_name2), Arg<string>.Is.Anything)).Return (null);
      _loaderMock.Expect (mock => mock.TryLoadAssembly (Arg.Is (_name3), Arg<string>.Is.Anything)).Return (_assembly3);
      _loaderMock.Replay ();

      var finder = new NamedRootAssemblyFinder (new[] { _specification1, _specification2, _specification3 });
      var assemblies = finder.FindRootAssemblies (_loaderMock);

      _loaderMock.VerifyAllExpectations ();
      Assert.That (assemblies.Length, Is.EqualTo (2));
      Assert.That (assemblies.Select (a => a.Assembly).ToArray (), Is.EquivalentTo (new[] { _assembly1, _assembly3 }));
    }

    [Test]
    public void FindAssemblies_DuplicatesRemoved ()
    {
      _loaderMock.Expect (mock => mock.TryLoadAssembly (Arg.Is (_name1), Arg<string>.Is.Anything)).Return (_assembly1);
      _loaderMock.Expect (mock => mock.TryLoadAssembly (Arg.Is (_name2), Arg<string>.Is.Anything)).Return (_assembly1);
      _loaderMock.Replay ();

      var finder = new NamedRootAssemblyFinder (new[] { _specification1, _specification2 });
      var assemblies = finder.FindRootAssemblies (_loaderMock);

      _loaderMock.VerifyAllExpectations ();
      Assert.That (assemblies.Length, Is.EqualTo (1));
      Assert.That (assemblies.Select (a => a.Assembly).ToArray (), Is.EquivalentTo (new[] { _assembly1 }));
    }

    [Test]
    public void FindAssemblies_FollowReferences ()
    {
      _loaderMock.Expect (mock => mock.TryLoadAssembly (Arg.Is (_name1), Arg<string>.Is.Anything)).Return (_assembly1);
      _loaderMock.Expect (mock => mock.TryLoadAssembly (Arg.Is (_name2), Arg<string>.Is.Anything)).Return (_assembly2);
      _loaderMock.Replay ();

      var finder = new NamedRootAssemblyFinder (new[] { _specification1, _specification2 });
      var assemblies = finder.FindRootAssemblies (_loaderMock).ToDictionary (ra => ra.Assembly);

      _loaderMock.VerifyAllExpectations ();
      Assert.That (assemblies[_assembly1].FollowReferences, Is.True);
      Assert.That (assemblies[_assembly2].FollowReferences, Is.False);
    }

  }
}