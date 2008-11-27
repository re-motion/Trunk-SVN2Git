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

#if(false)
      // TODO: Reactivate these tests as soon as it is clear how best to support the fact that
      // ComparableTestClass implements IEquatable<int> and the sequences should therefore 
      // in fact compare equal.
      Assert.That (NewEnumerableEqualsWrapper (sequence0).Equals (numbers0), Is.True);
      Assert.That (NewEnumerableEqualsWrapper (numbers0).Equals (sequence0), Is.True);
#endif

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