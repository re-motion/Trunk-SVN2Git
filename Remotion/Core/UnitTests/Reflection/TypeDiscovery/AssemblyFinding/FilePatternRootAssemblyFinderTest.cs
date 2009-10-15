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
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Rhino.Mocks;
using System.Linq;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyFinding
{
  [TestFixture]
  public class FilePatternRootAssemblyFinderTest
  {
    private IFileSearchService _searchServiceMock;
    private IAssemblyLoader _loaderMock;
    private Assembly _assembly1;
    private Assembly _assembly2;
    private Assembly _assembly3;

    [SetUp]
    public void SetUp ()
    {
      _searchServiceMock = MockRepository.GenerateMock<IFileSearchService> ();
      _loaderMock = MockRepository.GenerateMock<IAssemblyLoader> ();
      _assembly1 = typeof (object).Assembly;
      _assembly2 = typeof (FilePatternRootAssemblyFinder).Assembly;
      _assembly3 = typeof (FilePatternRootAssemblyFinderTest).Assembly;
    }

    [Test]
    public void FindAssemblies ()
    {
      var specification1 = new FilePatternRootAssemblyFinder.Specification ("*.dll", true);
      var specification2 = new FilePatternRootAssemblyFinder.Specification ("*.exe", true);

      _searchServiceMock.Expect (mock => mock.GetFiles ("searchPath", "*.dll", SearchOption.TopDirectoryOnly)).Return (new[] { "1.dll", "2.dll" });
      _searchServiceMock.Expect (mock => mock.GetFiles ("searchPath", "*.exe", SearchOption.TopDirectoryOnly)).Return (new[] { "1.exe" });
      _searchServiceMock.Replay();

      _loaderMock.Expect (mock => mock.TryLoadAssembly ("1.dll")).Return (_assembly1);
      _loaderMock.Expect (mock => mock.TryLoadAssembly ("2.dll")).Return (_assembly2);
      _loaderMock.Expect (mock => mock.TryLoadAssembly ("1.exe")).Return (_assembly3);
      _loaderMock.Replay();

      var finder = new FilePatternRootAssemblyFinder ("searchPath", new[] { specification1, specification2 }, _searchServiceMock);

      var rootAssemblies = finder.FindRootAssemblies (_loaderMock).Select (ra => ra.Assembly).ToArray();

      _searchServiceMock.VerifyAllExpectations();
      _loaderMock.VerifyAllExpectations();
      Assert.That (rootAssemblies, Is.EquivalentTo (new[] { _assembly1, _assembly2, _assembly3 }));
    }

    [Test]
    public void FindAssemblies_NullsRemoved ()
    {
      var specification1 = new FilePatternRootAssemblyFinder.Specification ("*.dll", true);
      var specification2 = new FilePatternRootAssemblyFinder.Specification ("*.exe", true);

      _searchServiceMock.Expect (mock => mock.GetFiles ("searchPath", "*.dll", SearchOption.TopDirectoryOnly)).Return (new[] { "1.dll", "2.dll" });
      _searchServiceMock.Expect (mock => mock.GetFiles ("searchPath", "*.exe", SearchOption.TopDirectoryOnly)).Return (new[] { "1.exe", "2.exe" });
      _searchServiceMock.Replay ();

      _loaderMock.Expect (mock => mock.TryLoadAssembly ("1.dll")).Return (_assembly1);
      _loaderMock.Expect (mock => mock.TryLoadAssembly ("2.dll")).Return (null);
      _loaderMock.Expect (mock => mock.TryLoadAssembly ("1.exe")).Return (_assembly3);
      _loaderMock.Expect (mock => mock.TryLoadAssembly ("2.exe")).Return (null);
      _loaderMock.Replay ();

      var finder = new FilePatternRootAssemblyFinder ("searchPath", new[] { specification1, specification2 }, _searchServiceMock);

      var rootAssemblies = finder.FindRootAssemblies (_loaderMock).Select (ra => ra.Assembly).ToArray ();

      _searchServiceMock.VerifyAllExpectations ();
      _loaderMock.VerifyAllExpectations ();
      Assert.That (rootAssemblies.Length, Is.EqualTo (2));
      Assert.That (rootAssemblies, Is.EquivalentTo (new[] { _assembly1, _assembly3 }));
    }

    [Test]
    public void FindAssemblies_DuplicatesRemoved ()
    {
      var specification1 = new FilePatternRootAssemblyFinder.Specification ("*.dll", true);
      var specification2 = new FilePatternRootAssemblyFinder.Specification ("*.exe", true);

      _searchServiceMock.Expect (mock => mock.GetFiles ("searchPath", "*.dll", SearchOption.TopDirectoryOnly)).Return (new[] { "1.dll", "2.dll" });
      _searchServiceMock.Expect (mock => mock.GetFiles ("searchPath", "*.exe", SearchOption.TopDirectoryOnly)).Return (new[] { "1.exe", "2.exe" });
      _searchServiceMock.Replay ();

      _loaderMock.Expect (mock => mock.TryLoadAssembly ("1.dll")).Return (_assembly1);
      _loaderMock.Expect (mock => mock.TryLoadAssembly ("2.dll")).Return (_assembly2);
      _loaderMock.Expect (mock => mock.TryLoadAssembly ("1.exe")).Return (_assembly1);
      _loaderMock.Expect (mock => mock.TryLoadAssembly ("2.exe")).Return (_assembly2);
      _loaderMock.Replay ();

      var finder = new FilePatternRootAssemblyFinder ("searchPath", new[] { specification1, specification2 }, _searchServiceMock);

      var rootAssemblies = finder.FindRootAssemblies (_loaderMock).Select (ra => ra.Assembly).ToArray ();

      _searchServiceMock.VerifyAllExpectations ();
      _loaderMock.VerifyAllExpectations ();
      Assert.That (rootAssemblies.Length, Is.EqualTo (2));
      Assert.That (rootAssemblies, Is.EquivalentTo (new[] { _assembly1, _assembly2 }));
    }

    [Test]
    public void FindAssemblies_FollowReferences ()
    {
      var specification1 = new FilePatternRootAssemblyFinder.Specification ("*.dll", true);
      var specification2 = new FilePatternRootAssemblyFinder.Specification ("*.exe", false);

      _searchServiceMock.Expect (mock => mock.GetFiles ("searchPath", "*.dll", SearchOption.TopDirectoryOnly)).Return (new[] { "1.dll" });
      _searchServiceMock.Expect (mock => mock.GetFiles ("searchPath", "*.exe", SearchOption.TopDirectoryOnly)).Return (new[] { "1.exe" });
      _searchServiceMock.Replay ();

      _loaderMock.Expect (mock => mock.TryLoadAssembly ("1.dll")).Return (_assembly1);
      _loaderMock.Expect (mock => mock.TryLoadAssembly ("1.exe")).Return (_assembly2);
      _loaderMock.Replay ();

      var finder = new FilePatternRootAssemblyFinder ("searchPath", new[] { specification1, specification2 }, _searchServiceMock);

      var rootAssemblies = finder.FindRootAssemblies (_loaderMock).ToDictionary (ra => ra.Assembly);

      _searchServiceMock.VerifyAllExpectations ();
      _loaderMock.VerifyAllExpectations ();
      Assert.That (rootAssemblies[_assembly1].FollowReferences, Is.True);
      Assert.That (rootAssemblies[_assembly2].FollowReferences, Is.False);
    }

  }
}