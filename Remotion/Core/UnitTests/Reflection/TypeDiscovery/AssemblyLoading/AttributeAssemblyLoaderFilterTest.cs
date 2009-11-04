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

[assembly: TestMarker]

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyLoading
{
  public class TestMarkerAttribute : Attribute { }

  [TestFixture]
  public class AttributeAssemblyLoaderFilterTest
  {
    [Test]
    public void AttributeConsidering ()
    {
      var filter = new AttributeAssemblyLoaderFilter (typeof (SerializableAttribute)); // attribute type doesn't matter here
      Assert.IsTrue (filter.ShouldConsiderAssembly (typeof (AttributeAssemblyLoaderFilterTest).Assembly.GetName()));
      Assert.IsTrue (filter.ShouldConsiderAssembly (typeof (TestFixtureAttribute).Assembly.GetName()));
      Assert.IsTrue (filter.ShouldConsiderAssembly (typeof (object).Assembly.GetName()));
      Assert.IsTrue (filter.ShouldConsiderAssembly (new AssemblyName ("name does not matter")));
    }

    [Test]
    public void AttributeInclusion ()
    {
      var filter = new AttributeAssemblyLoaderFilter (typeof (TestMarkerAttribute));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (AttributeAssemblyLoaderFilterTest).Assembly));
      Assert.IsFalse (filter.ShouldIncludeAssembly (typeof (TestFixtureAttribute).Assembly));
      Assert.IsFalse (filter.ShouldIncludeAssembly (typeof (object).Assembly));
      Assert.IsFalse (filter.ShouldIncludeAssembly (typeof (Uri).Assembly));

      filter = new AttributeAssemblyLoaderFilter (typeof (CLSCompliantAttribute));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (ApplicationAssemblyLoaderFilter).Assembly));
      Assert.IsFalse (filter.ShouldIncludeAssembly (typeof (AttributeAssemblyLoaderFilterTest).Assembly));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (TestFixtureAttribute).Assembly));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (object).Assembly));
      Assert.IsTrue (filter.ShouldIncludeAssembly (typeof (Uri).Assembly));

      filter = new AttributeAssemblyLoaderFilter (typeof (SerializableAttribute));
      Assert.IsFalse (filter.ShouldIncludeAssembly (typeof (AttributeAssemblyLoaderFilterTest).Assembly));
      Assert.IsFalse (filter.ShouldIncludeAssembly (typeof (TestFixtureAttribute).Assembly));
      Assert.IsFalse (filter.ShouldIncludeAssembly (typeof (object).Assembly));
      Assert.IsFalse (filter.ShouldIncludeAssembly (typeof (Uri).Assembly));
    }
  }
}
