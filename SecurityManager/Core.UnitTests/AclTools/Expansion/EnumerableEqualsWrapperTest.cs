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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class EnumerableEqualsWrapperTest
  {
    [Test]
    [Explicit]
    public void Spike ()
    {
      int i = 17;
      var c = new ComparableTestClass(i);
      var comparer = EqualityComparer<object>.Default;

      To.ConsoleLine.e (c.Number.Equals (c));
      To.ConsoleLine.e (c.Equals (c.Number)).nl(2);

      Console.WriteLine (comparer.Equals (1, 1));
      Console.WriteLine (comparer.Equals (c, 1));
      Console.WriteLine (comparer.Equals (1, c));
      Console.WriteLine (comparer.Equals (c, c));
    }

    [Test]
    [Explicit]
    public void Spike2 ()
    {
      var c = new ComparableTestClass (6758439);
      To.ConsoleLine.e (c.Number.Equals (c));
      To.ConsoleLine.e (c.Equals (c.Number));
    }

    [Test]
    public void EqualsTest ()
    {
      var sequence0 = new [] {  new ComparableTestClass (7), new ComparableTestClass (11), new ComparableTestClass (13) };
      var sequence1 = new [] {  new ComparableTestClass (7), new ComparableTestClass (11), new ComparableTestClass (13) };
      var sequence2 = new [] {  new ComparableTestClass (7), new ComparableTestClass (11), new ComparableTestClass (13), new ComparableTestClass (17) };
      var sequence3 = new [] {  new ComparableTestClass (7), new ComparableTestClass (11), new ComparableTestClass (14) };

      Assert.That (NewEnumerableEqualsWrapper (sequence0).Equals (null), Is.False);

      var numbers0 = new[] { 7, 11, 13 };

      // Even though the ComparableTestClass incorrectly implements IEquatable<int> 
      // (IEquatable<> must only be used in the combination "class T : IEquatable<T>"; seeMSDN help) 
      // the sequences should symetrically not compare equal.
      Assert.That (NewEnumerableEqualsWrapper (sequence0).Equals (numbers0), Is.False);
      Assert.That (NewEnumerableEqualsWrapper (numbers0).Equals (sequence0), Is.False);

      Assert.That (NewEnumerableEqualsWrapper (sequence0).Equals (new[] { 7, 12, 13 }), Is.False);

      Assert.That (NewEnumerableEqualsWrapper (sequence0).Equals (sequence0), Is.True);
      Assert.That (NewEnumerableEqualsWrapper (sequence0).Equals (sequence1), Is.True);
      Assert.That (NewEnumerableEqualsWrapper (sequence1).Equals (sequence2), Is.False);
      Assert.That (NewEnumerableEqualsWrapper (sequence0).Equals (sequence3), Is.False);

      Assert.That (NewEnumerableEqualsWrapper (sequence0).Equals (NewEnumerableEqualsWrapper (sequence0)), Is.True);
      Assert.That (NewEnumerableEqualsWrapper (sequence0).Equals (NewEnumerableEqualsWrapper (sequence2)), Is.False);
    }

    private EnumerableEqualsWrapper<T> NewEnumerableEqualsWrapper<T> (params T[] elements)
    {
      return new EnumerableEqualsWrapper<T> (elements);
    }
  }

  

  internal class ComparableTestClass : IEquatable<int>
  {
    public int Number;

    public ComparableTestClass () { }

    public ComparableTestClass (int number)
    {
      Number = number;
    }

    public bool Equals (int other)
    {
      return Number == other;
    }

    public override bool Equals (object obj)
    {
      if (obj is ComparableTestClass)
      {
        if (Object.Equals (((ComparableTestClass) obj).Number, Number))
        {
          return true;
        }
      }
      //else if (Object.Equals (obj, Number))
      //{
      //  return true;
      //}
    
      return false;
    }


    public override int GetHashCode ()
    {
      return Number;
    }
  }
}
