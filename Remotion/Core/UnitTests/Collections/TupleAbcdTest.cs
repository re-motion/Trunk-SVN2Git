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
#if NET_3_5
using System;
using NUnit.Framework;
using Remotion.Collections;

namespace Remotion.UnitTests.Collections
{
  using TestTuple = Remotion.Collections.Tuple<int, string, double, DateTime>;

  [TestFixture]
  public class TupelAbcdTest
  {
    private readonly DateTime date1 = new DateTime (2006, 7, 17, 11, 15, 10);
    private readonly DateTime date2 = new DateTime (2005, 7, 17, 11, 15, 10);

    [Test]
    public void Initialize ()
    {
      TestTuple tuple = new TestTuple (1, "X", 2.5, date1);

      Assert.That (tuple.Item1, Is.EqualTo (1));
      Assert.That (tuple.Item2, Is.EqualTo ("X"));
    }

    [Test]
    public void EasyInitialize ()
    {
      TestTuple tuple = Tuple.Create (1, "X", 2.5, date1);
      Assert.That (tuple.Item1, Is.EqualTo (1));
      Assert.That (tuple.Item2, Is.EqualTo ("X"));
      Assert.That (tuple.Item3, Is.EqualTo (2.5));
      Assert.That (tuple.Item4, Is.EqualTo (date1));
    }

    [Test]
    [Obsolete]
    public void EasyInitialize_Obsolete ()
    {
      TestTuple tuple = Tuple.NewTuple (1, "X", 2.5, date1);
      Assert.That (tuple.A, Is.EqualTo (1));
      Assert.That (tuple.B, Is.EqualTo ("X"));
      Assert.That (tuple.C, Is.EqualTo (2.5));
      Assert.That (tuple.D, Is.EqualTo (date1));
    }

    [Test]
    public void Equals_WithNull ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5,date1);

      Assert.That (left.Equals (null), Is.False);
    }

    [Test]
    public void Equals_WithSame ()
    {
      TestTuple tuple = new TestTuple (1, "X", 2.5, date1);

      Assert.That (tuple.Equals (tuple), Is.True);
    }

    [Test]
    public void Equals_WithEqual ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5, date1);
      TestTuple right = new TestTuple (1, "X", 2.5, date1);

      Assert.That (left.Equals (right), Is.True);
      Assert.That (right.Equals (left), Is.True);
    }

    [Test]
    public void Equals_WithDifferentA ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5, date1);
      TestTuple right = new TestTuple (-1, "X", 2.5, date1);

      Assert.That (left.Equals (right), Is.False);
      Assert.That (right.Equals (left), Is.False);
    }

    [Test]
    public void Equals_WithDiffentB ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5, date1);
      TestTuple right = new TestTuple (1, "A", 2.5, date1);

      Assert.That (left.Equals (right), Is.False);
      Assert.That (right.Equals (left), Is.False);
    }

    [Test]
    public void Equals_WithDiffentC ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5, date1);
      TestTuple right = new TestTuple (1, "X", -2.5, date1);

      Assert.That (left.Equals (right), Is.False);
      Assert.That (right.Equals (left), Is.False);
    }

    [Test]
    public void Equals_WithDiffentD ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5, date1);
      TestTuple right = new TestTuple (1, "X", 2.5, date2);

      Assert.That (left.Equals (right), Is.False);
      Assert.That (right.Equals (left), Is.False);
    }

    [Test]
    public void EqualsObject_WithEqual ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5, date1);
      TestTuple right = new TestTuple (1, "X", 2.5, date1);

      Assert.That (left.Equals ((object) right), Is.True);
    }

    [Test]
    public void EqualsObject_WithNull ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5, date1);

      Assert.That (left.Equals ((object) null), Is.False);
    }

    [Test]
    public void EqualsObject_WithObject ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5, date1);

      Assert.That (left.Equals (new object ()), Is.False);
    }

    [Test]
    public void TestGetHashCode ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5, date1);
      TestTuple right = new TestTuple (1, "X", 2.5, date1);

      Assert.That (right.GetHashCode (), Is.EqualTo (left.GetHashCode ()));
    }

  }
}
#endif
