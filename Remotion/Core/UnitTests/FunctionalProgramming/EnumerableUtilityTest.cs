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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.FunctionalProgramming;
using System.Linq;
using Remotion.UnitTests.FunctionalProgramming.TestDomain;

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
    public void Singleton_WithObject ()
    {
      var element = new object ();
      var actual = EnumerableUtility.Singleton (element);
      Assert.That (actual.ToArray (), Is.EqualTo (new[] { element }));
    }

    [Test]
    public void Singleton_WithNull ()
    {
      var actual = EnumerableUtility.Singleton (((object) null));
      Assert.That (actual.ToArray (), Is.EqualTo (new object[] { null }));
    }

    [Test]
    public void Singleton_WithValueType ()
    {
      var actual = EnumerableUtility.Singleton (0);
      Assert.That (actual.ToArray (), Is.EqualTo (new[] { 0 }));
    }

    [Test]
    public void SelectRecursiveDepthFirst_Single ()
    {
      var item = new RecursiveItem();

      var result = EnumerableUtility.SelectRecursiveDepthFirst (item, i => i.Children).ToArray();

      Assert.That (result, Is.EqualTo (new[] { item }));
    }

    [Test]
    public void SelectRecursiveDepthFirst_Nested ()
    {
      var item0a = new RecursiveItem ();
      var item0b = new RecursiveItem();
      var item0c = new RecursiveItem();
      var item1a = new RecursiveItem (item0a, item0b);
      var item1b = new RecursiveItem();
      var item1c = new RecursiveItem(item0c);
      var item2a = new RecursiveItem (item1a);
      var item2b = new RecursiveItem (item1b, item1c);
      var item3 = new RecursiveItem (item2a, item2b);

      var result = EnumerableUtility.SelectRecursiveDepthFirst (item3, i => i.Children).ToArray ();

      Assert.That (result, Is.EqualTo (new[] { item3, item2a, item1a, item0a, item0b, item2b, item1b, item1c, item0c }));
    }
  }
}