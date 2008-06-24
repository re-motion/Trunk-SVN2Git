/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
