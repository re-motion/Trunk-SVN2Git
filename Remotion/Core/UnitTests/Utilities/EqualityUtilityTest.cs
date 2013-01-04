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
      Assert.That (EqualityUtility.GetRotatedHashCode (objects2), Is.EqualTo (EqualityUtility.GetRotatedHashCode (objects1)));

      IEnumerable objects3 = new int[] {3, 2, 1};
      Assert.That (EqualityUtility.GetRotatedHashCode (objects3), Is.Not.EqualTo (EqualityUtility.GetRotatedHashCode (objects1)));

      IEnumerable objects4 = new int[] { 1, 2, 17 };
      Assert.That (EqualityUtility.GetRotatedHashCode (objects4), Is.Not.EqualTo (EqualityUtility.GetRotatedHashCode (objects1)));
    }

    [Test]
    public void GetXorHashCode ()
    {
      IEnumerable objects1 = new int[] { 1, 2, 3 };
      IEnumerable objects2 = new int[] { 1, 2, 3 };
      Assert.That (EqualityUtility.GetXorHashCode (objects2), Is.EqualTo (EqualityUtility.GetXorHashCode (objects1)));

      IEnumerable objects3 = new int[] { 3, 2, 1 };
      Assert.That (EqualityUtility.GetXorHashCode (objects3), Is.EqualTo (EqualityUtility.GetXorHashCode (objects1)));

      IEnumerable objects4 = new int[] { 1, 2, 17 };
      Assert.That (EqualityUtility.GetXorHashCode (objects4), Is.Not.EqualTo (EqualityUtility.GetXorHashCode (objects1)));
    }

    [Test]
    public void GetRotatedHashCode_Nulls ()
    {
      var array1 = new object[] { 1, null, 2 };
      var array2 = new object[] { 1, null, 2 };
      var array3 = new object[] { 1, null, null, 2 };

      Assert.That (EqualityUtility.GetRotatedHashCode (array1), Is.EqualTo (EqualityUtility.GetRotatedHashCode (array2)));
      Assert.That (EqualityUtility.GetRotatedHashCode (array1), Is.Not.EqualTo (EqualityUtility.GetRotatedHashCode (array3)));

      Assert.That (EqualityUtility.GetRotatedHashCode ((IEnumerable) array1), Is.EqualTo (EqualityUtility.GetRotatedHashCode ((IEnumerable) array2)));
      Assert.That (EqualityUtility.GetRotatedHashCode ((IEnumerable) array1), Is.Not.EqualTo (EqualityUtility.GetRotatedHashCode ((IEnumerable) array3)));

      Assert.That (EqualityUtility.GetRotatedHashCode (array1), Is.EqualTo (EqualityUtility.GetRotatedHashCode ((IEnumerable) array1)));
      Assert.That (EqualityUtility.GetRotatedHashCode (array3), Is.EqualTo (EqualityUtility.GetRotatedHashCode ((IEnumerable) array3)));
    }
  }
}
