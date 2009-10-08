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

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyFinding
{
  [TestFixture]
  public class CompositeRootAssemblyFinderTest
  {
    private RootAssembly _root1;
    private RootAssembly _root2;
    private RootAssembly _root3;
    private IAssemblyLoader _loaderStub;

    [SetUp]
    public void SetUp ()
    {
      _root1 = new RootAssembly (typeof (object).Assembly, true);
      _root2 = new RootAssembly (typeof (CompositeRootAssemblyFinder).Assembly, true);
      _root3 = new RootAssembly (typeof (CompositeRootAssemblyFinderTest).Assembly, true);
      _loaderStub = MockRepository.GenerateStub<IAssemblyLoader> ();
    }

    [Test]
    public void FindRootAssemblies_NoInnerFinders ()
    {
      var finder = new CompositeRootAssemblyFinder (new IRootAssemblyFinder[0]);

      var rootAssemblies = finder.FindRootAssemblies(_loaderStub);
      Assert.That (rootAssemblies, Is.Empty);
    }

    [Test]
    public void FindRootAssemblies_InnerFinder ()
    {
      IRootAssemblyFinder innerFinderStub = CreateInnerFinderStub (_root1, _root2);
      var finder = new CompositeRootAssemblyFinder (new[] { innerFinderStub });

      var rootAssemblies = finder.FindRootAssemblies (_loaderStub);
      Assert.That (rootAssemblies, Is.EquivalentTo (new[] { _root1, _root2 }));
    }

    [Test]
    public void FindRootAssemblies_MultipleInnerFinders ()
    {
      IRootAssemblyFinder innerFinderStub1 = CreateInnerFinderStub (_root1, _root2);
      IRootAssemblyFinder innerFinderStub2 = CreateInnerFinderStub (_root3);

      var finder = new CompositeRootAssemblyFinder (new[] { innerFinderStub1, innerFinderStub2 });

      var rootAssemblies = finder.FindRootAssemblies (_loaderStub);
      Assert.That (rootAssemblies, Is.EquivalentTo (new[] { _root1, _root2, _root3 }));
    }

    [Test]
    public void FindRootAssemblies_RemovesDuplicates ()
    {
      IRootAssemblyFinder innerFinderStub1 = CreateInnerFinderStub (_root1, _root2, _root2);
      IRootAssemblyFinder innerFinderStub2 = CreateInnerFinderStub (_root3, _root2, _root1);

      var finder = new CompositeRootAssemblyFinder (new[] { innerFinderStub1, innerFinderStub2 });

      var rootAssemblies = finder.FindRootAssemblies (_loaderStub);
      Assert.That (rootAssemblies, Is.EquivalentTo (new[] { _root1, _root2, _root3 }));
    }

    private IRootAssemblyFinder CreateInnerFinderStub (params RootAssembly[] assemblies)
    {
      var innerFinderStub = MockRepository.GenerateStub<IRootAssemblyFinder> ();
      innerFinderStub.Stub (stub => stub.FindRootAssemblies (_loaderStub)).Return (assemblies);
      innerFinderStub.Replay ();
      return innerFinderStub;
    }
  }
}