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
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class EnumerableUtilityTest
  {
    [Test]
    public void Cast()
    {
      int[] sourceArray = new int[] {1, 2, 3};
      IEnumerable sourceEnumerable = sourceArray;
      IEnumerable<int> castEnumerable1 = EnumerableUtility.Cast<int> (sourceEnumerable);
      IEnumerable<object> castEnumerable2 = EnumerableUtility.Cast<object> (sourceEnumerable);
      Assert.IsNotNull (castEnumerable1);
      Assert.IsNotNull (castEnumerable2);
      Assert.That (EnumerableUtility.ToArray (castEnumerable1), Is.EqualTo (sourceArray));
      Assert.That (EnumerableUtility.ToArray (castEnumerable2), Is.EqualTo (sourceArray));
    }

    [Test]
    public void CombineToArray ()
    {
      int[] combined = EnumerableUtility.CombineToArray (new int[] {1, 2, 3}, new List<int> (new int[] {3, 4, 5}));
      Assert.That (combined, Is.EqualTo (new int[] {1, 2, 3, 3, 4, 5}));
    }

    [Test]
    public void FirstOrDefault ()
    {
      int[] sourceArray = new int[] { 1, 2, 3 };
      Assert.That (EnumerableUtility.FirstOrDefault (sourceArray), Is.EqualTo (1));
      Assert.That (EnumerableUtility.FirstOrDefault (new int[0]), Is.EqualTo (0));
      Assert.That (EnumerableUtility.FirstOrDefault (new object[0]), Is.Null);
    }
  }
}
