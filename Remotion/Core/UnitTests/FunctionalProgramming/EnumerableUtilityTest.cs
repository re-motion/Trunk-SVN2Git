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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.FunctionalProgramming;
using System.Linq;

namespace Remotion.UnitTests.FunctionalProgramming
{
  [TestFixture]
  public class EnumerableUtilityTest
  {
    [Test]
    public void Combine ()
    {
      var combined = EnumerableUtility.Combine (new[] {1, 2, 3}, new List<int> (new[] {3, 4, 5}));
      Assert.That (combined.ToArray (), Is.EqualTo (new[] { 1, 2, 3, 3, 4, 5 }));
    }

    [Test]
    public void ToSingletonEnumerable_WithObject ()
    {
      var element = new object ();
      var actual = EnumerableUtility.Singleton (element);
      Assert.That (actual.ToArray (), Is.EqualTo (new[] { element }));
    }

    [Test]
    public void ToSingletonEnumerable_WithNull ()
    {
      var actual = EnumerableUtility.Singleton (((object) null));
      Assert.That (actual.ToArray (), Is.EqualTo (new object[] { null }));
    }

    [Test]
    public void ToSingletonEnumerable_WithValueType ()
    {
      var actual = EnumerableUtility.Singleton (0);
      Assert.That (actual.ToArray (), Is.EqualTo (new[] { 0 }));
    }

  }
}