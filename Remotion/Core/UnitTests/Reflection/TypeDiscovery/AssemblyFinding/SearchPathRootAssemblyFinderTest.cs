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
using NUnit.Framework;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyFinding
{
  [TestFixture]
  public class SearchPathRootAssemblyFinderTest
  {
    private IAssemblyLoader _loaderStub;

    [SetUp]
    public void SetUp ()
    {
      _loaderStub = MockRepository.GenerateStub<IAssemblyLoader> ();
    }

    [Test]
    public void CreateCombinedFinder_HoldsBaseDirectory ()
    {
      var finder = new SearchPathRootAssemblyFinder ("baseDirectory", "relativeSearchPath", false, "dynamicDirectory");
      var finderDirectories = GetDirectoriesForCombinedFinder (finder);

      Assert.That (finderDirectories, Has.Member("baseDirectory"));
    }

    [Test]
    public void CreateCombinedFinder_HoldsRelativeSearchPath ()
    {
      var finder = new SearchPathRootAssemblyFinder ("baseDirectory", "relativeSearchPath", false, "dynamicDirectory");
      var finderDirectories = GetDirectoriesForCombinedFinder (finder);

      Assert.That (finderDirectories, Has.Member("relativeSearchPath"));
    }

    [Test]
    public void CreateCombinedFinder_HoldsRelativeSearchPath_Split ()
    {
      var finder = new SearchPathRootAssemblyFinder ("baseDirectory", "relativeSearchPath1;relativeSearchPath2", false, "dynamicDirectory");
      var finderDirectories = GetDirectoriesForCombinedFinder (finder);

      Assert.That (finderDirectories, Has.Member("relativeSearchPath1"));
      Assert.That (finderDirectories, Has.Member("relativeSearchPath2"));
    }

    [Test]
    public void CreateCombinedFinder_Specifications ()
    {
      var finder = new SearchPathRootAssemblyFinder ("baseDirectory", "relativeSearchPath", false, "dynamicDirectory");
      var finderSpecs = GetSpecificationsForCombinedFinder(finder);

      Assert.That (finderSpecs, Is.EquivalentTo (new[] { 
          new FilePatternSpecification ("*.exe", FilePatternSpecificationKind.IncludeFollowReferences), 
          new FilePatternSpecification ("*.dll", FilePatternSpecificationKind.IncludeFollowReferences) }));
    }

    [Test]
    public void CreateCombinedFinder_SearchService ()
    {
      var finder = new SearchPathRootAssemblyFinder ("baseDirectory", "relativeSearchPath", false, "dynamicDirectory");
      var finderService = GetSearchServiceForCombinedFinder (finder);

      Assert.That (finderService, Is.InstanceOf (typeof (FileSystemSearchService)));
    }

    [Test]
    public void CreateCombinedFinder_ConsiderDynamicDirectory_False ()
    {
      var finder = new SearchPathRootAssemblyFinder ("baseDirectory", "relativeSearchPath", false, "dynamicDirectory");
      var finderDirectories = GetDirectoriesForCombinedFinder (finder);

      Assert.That (finderDirectories, Has.No.Member ("dynamicDirectory"));
    }

    [Test]
    public void CreateCombinedFinder_ConsiderDynamicDirectory_True ()
    {
      var finder = new SearchPathRootAssemblyFinder ("baseDirectory", "relativeSearchPath", true, "dynamicDirectory");
      var finderDirectories = GetDirectoriesForCombinedFinder (finder);
      var finderSpecs = GetSpecificationsForCombinedFinder (finder);
      var finderService = GetSearchServiceForCombinedFinder (finder);

      Assert.That (finderDirectories, Has.Member("dynamicDirectory"));
      Assert.That (finderSpecs, Is.EquivalentTo (new[] { 
          new FilePatternSpecification ("*.exe", FilePatternSpecificationKind.IncludeFollowReferences), 
          new FilePatternSpecification ("*.dll", FilePatternSpecificationKind.IncludeFollowReferences) }));
      Assert.That (finderService, Is.InstanceOf (typeof (FileSystemSearchService)));
    }

    [Test]
    public void FindRootAssemblies_UsesCombinedFinder ()
    {
      var innerFinderStub = MockRepository.GenerateStub<IRootAssemblyFinder> ();
      var rootAssembly = new RootAssembly (typeof (object).Assembly, true);
      innerFinderStub.Stub (stub => stub.FindRootAssemblies (_loaderStub)).Return (new[] { rootAssembly });
      innerFinderStub.Replay ();

      var finderMock = new MockRepository ().PartialMock<SearchPathRootAssemblyFinder> (
          "baseDirectory",
          "relativeSearchPath",
          false,
          "dynamicDirectory");
      finderMock.Expect (mock => mock.CreateCombinedFinder ()).Return (new CompositeRootAssemblyFinder (new[] { innerFinderStub }));
      finderMock.Replay ();

      var result = finderMock.FindRootAssemblies (_loaderStub);
      Assert.That (result, Is.EqualTo (new[] { rootAssembly }));

      finderMock.VerifyAllExpectations ();
    }

    [Test]
    public void CreateForCurrentAppDomain ()
    {
      var finder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain (true);

      Assert.That (finder.BaseDirectory, Is.EqualTo (AppDomain.CurrentDomain.BaseDirectory));
      Assert.That (finder.RelativeSearchPath, Is.EqualTo (AppDomain.CurrentDomain.RelativeSearchPath));
      Assert.That (finder.DynamicDirectory, Is.EqualTo (AppDomain.CurrentDomain.DynamicDirectory));
    }

    [Test]
    public void CreateForCurrentAppDomain_ConsiderDynamicDirectoryTrue ()
    {
      var finder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain (true);
      Assert.That (finder.ConsiderDynamicDirectory, Is.True);
    }

    [Test]
    public void CreateForCurrentAppDomain_ConsiderDynamicDirectoryFalse ()
    {
      var finder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain (false);
      Assert.That (finder.ConsiderDynamicDirectory, Is.False);
    }

    private string[] GetDirectoriesForCombinedFinder (SearchPathRootAssemblyFinder finder)
    {
      var combinedFinder = finder.CreateCombinedFinder ();
      return combinedFinder.InnerFinders.Cast<FilePatternRootAssemblyFinder> ().Select (f => f.SearchPath).ToArray ();
    }

    private FilePatternSpecification[] GetSpecificationsForCombinedFinder (SearchPathRootAssemblyFinder finder)
    {
      return finder.CreateCombinedFinder ()
          .InnerFinders
          .Cast<FilePatternRootAssemblyFinder> ()
          .SelectMany (inner => inner.Specifications)
          .Distinct ()
          .ToArray ();
    }

    private IFileSearchService GetSearchServiceForCombinedFinder (SearchPathRootAssemblyFinder finder)
    {
      return finder.CreateCombinedFinder ()
          .InnerFinders
          .Cast<FilePatternRootAssemblyFinder> ()
          .Select (inner => inner.FileSearchService)
          .Distinct ()
          .Single();
    }

  }
}
