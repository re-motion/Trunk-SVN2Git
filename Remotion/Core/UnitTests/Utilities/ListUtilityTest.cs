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
using System.Collections;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{

[TestFixture]
public class ListUtilityTest
{
  // types

  // static members and constants

  // member fields

  private ArrayList _list;
  private ArrayList _values;

  // construction and disposing

  public ListUtilityTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _list = new ArrayList();
    _list.Add ("A");
    _list.Add ("B");
    _list.Add ("C");
    _list.Add ("D");

    _values = new ArrayList();
    _values.Add ("B");
    _values.Add ("E");
    _values.Add ("D");
  }

  [Test]
  public void IndexOf ()
  {
    Assert.AreEqual (0, ListUtility.IndexOf (_list, "A"));
    Assert.AreEqual (1, ListUtility.IndexOf (_list, "B"));
    Assert.AreEqual (2, ListUtility.IndexOf (_list, "C"));
    Assert.AreEqual (3, ListUtility.IndexOf (_list, "D"));
    Assert.AreEqual (-1, ListUtility.IndexOf (_list, "E"));
  }

  [Test]
  public void IndicesOfIncludeMissing ()
  {
    int[] indices = ListUtility.IndicesOf (_list, _values);

    Assert.IsNotNull (indices);
    Assert.AreEqual (3, indices.Length);
    Assert.AreEqual (1, indices[0]);
    Assert.AreEqual (-1, indices[1]);
    Assert.AreEqual (3, indices[2]);
  }

  [Test]
  public void IndicesOfExcludeMissing ()
  {
    int[] indices = ListUtility.IndicesOf (_list, _values, false);

    Assert.IsNotNull (indices);
    Assert.AreEqual (2, indices.Length);
    Assert.AreEqual (1, indices[0]);
    Assert.AreEqual (3, indices[1]);
  }
}

}
