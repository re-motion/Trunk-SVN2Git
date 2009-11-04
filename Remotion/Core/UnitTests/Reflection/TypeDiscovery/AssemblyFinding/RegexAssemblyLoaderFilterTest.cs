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
using NUnit.Framework;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyLoading;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyFinding
{
  [TestFixture]
  public class RegexAssemblyLoaderFilterTest
  {
    [Test]
    public void RegexConsidering_SimpleName ()
    {
      var filter = new RegexAssemblyLoaderFilter ("^Remotion.*$", RegexAssemblyLoaderFilter.MatchTargetKind.SimpleName);
      Assert.AreEqual ("^Remotion.*$", filter.MatchExpressionString);
      Assert.IsTrue (filter.ShouldConsiderAssembly (typeof (AttributeAssemblyLoaderFilterTest).Assembly.GetName()));
      Assert.IsFalse (filter.ShouldConsiderAssembly (typeof (TestFixtureAttribute).Assembly.GetName()));
      Assert.IsFalse (filter.ShouldConsiderAssembly (typeof (object).Assembly.GetName()));
      Assert.IsFalse (filter.ShouldConsiderAssembly (new AssemblyName ("this is not a Remotion assembly")));
    }

    [Test]
    public void RegexConsidering_FullName ()
    {
      var filter = new RegexAssemblyLoaderFilter (
          typeof (object).Assembly.FullName,
          RegexAssemblyLoaderFilter.MatchTargetKind.FullName);
      Assert.IsTrue (filter.MatchExpressionString.StartsWith ("mscorlib"));
      Assert.IsFalse (filter.ShouldConsiderAssembly (typeof (AttributeAssemblyLoaderFilterTest).Assembly.GetName()));
      Assert.IsFalse (filter.ShouldConsiderAssembly (typeof (TestFixtureAttribute).Assembly.GetName()));
      Assert.IsTrue (filter.ShouldConsiderAssembly (typeof (object).Assembly.GetName()));
      Assert.IsFalse (filter.ShouldConsiderAssembly (new AssemblyName ("this is not mscorlib")));
    }

    [Test]
    public void RegexInclusion_AlwaysTrue ()
    {
      var filter = new RegexAssemblyLoaderFilter ("spispopd", RegexAssemblyLoaderFilter.MatchTargetKind.SimpleName);
      Assert.AreEqual ("spispopd", filter.MatchExpressionString);
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (AttributeAssemblyLoaderFilterTest).Assembly));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (TestFixtureAttribute).Assembly));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (object).Assembly));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (Uri).Assembly));
    }
  }
}
