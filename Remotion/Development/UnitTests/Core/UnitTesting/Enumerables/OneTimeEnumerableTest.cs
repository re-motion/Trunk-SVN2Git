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
using System.Linq;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Enumerables;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Enumerables
{
  [TestFixture]
  public class OneTimeEnumerableTest
  {
    [Test]
    public void GetEnumerator_WorksLikeANormalEnumerable ()
    {
      var source1 = new[] { 1, 2, 3 };
      var source2 = new[] { 1, 3, 2 };

      Assert.That (new OneTimeEnumerable<int> (source1), Is.EqualTo (source1));
      Assert.That (new OneTimeEnumerable<int> (source1), Is.Not.EqualTo (source2));
      Assert.That (new OneTimeEnumerable<int> (source1), Is.EquivalentTo (source2));
    }

    [Test]
    public void GetEnumerator_ThrowsForSecondCall ()
    {
      var source = Enumerable.Range(1, 3);

      var oneTime = new OneTimeEnumerable<int> (source);

      Assert.That (() => oneTime.GetEnumerator(), Throws.Nothing);
      Assert.That (
          () => oneTime.GetEnumerator(),
          Throws.TypeOf<InvalidOperationException> ().With.Message.EqualTo ("OneTimeEnumerable can only be iterated once."));
    }

    [Test]
    public void GetEnumerator_ReturnedEnumeratorDoesNotSupportReset ()
    {
      var source = new[] { 1, 2, 3 };

      var oneTimeEnumerator = new OneTimeEnumerable<int> (source).GetEnumerator();
      oneTimeEnumerator.MoveNext();

      Assert.That (
          () => oneTimeEnumerator.Reset(),
          Throws.TypeOf<NotSupportedException>().With.Message.EqualTo ("OneTimeEnumerator does not support Reset()."));
    }
  }
}