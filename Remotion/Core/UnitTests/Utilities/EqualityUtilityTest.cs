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
  public class EqualityUtilityTest
  {
    [Test]
    public void GetRotatedHashCode_ForEnumerable()
    {
      IEnumerable objects1 = new int[] {1, 2, 3};
      IEnumerable objects2 = new int[] {1, 2, 3};
      Assert.AreEqual (EqualityUtility.GetRotatedHashCode (objects1), EqualityUtility.GetRotatedHashCode (objects2));

      IEnumerable objects3 = new int[] {3, 2, 1};
      Assert.AreNotEqual (EqualityUtility.GetRotatedHashCode (objects1), EqualityUtility.GetRotatedHashCode (objects3));

      IEnumerable objects4 = new int[] { 1, 2, 17 };
      Assert.AreNotEqual (EqualityUtility.GetRotatedHashCode (objects1), EqualityUtility.GetRotatedHashCode (objects4));
    }

    [Test]
    public void GetXorHashCode ()
    {
      IEnumerable objects1 = new int[] { 1, 2, 3 };
      IEnumerable objects2 = new int[] { 1, 2, 3 };
      Assert.AreEqual (EqualityUtility.GetXorHashCode (objects1), EqualityUtility.GetXorHashCode (objects2));

      IEnumerable objects3 = new int[] { 3, 2, 1 };
      Assert.AreEqual (EqualityUtility.GetXorHashCode (objects1), EqualityUtility.GetXorHashCode (objects3));

      IEnumerable objects4 = new int[] { 1, 2, 17 };
      Assert.AreNotEqual (EqualityUtility.GetXorHashCode (objects1), EqualityUtility.GetXorHashCode (objects4));
    }
  }
}
