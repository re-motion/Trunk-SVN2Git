// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using NUnit.Framework;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Sandboxing
{
  [TestFixture]
  public class DummyTest4
  {
    private int _value = Math.Abs(-5);

    [TearDown]
    public void TearDown ()
    {
      _value = Math.Abs (-5);
    }

    [Test]
    public void Test1 ()
    {
      Assert.That (_value, Is.EqualTo (5));
      _value = 100;
    }

    [Test]
    public void Test2 ()
    {
      Assert.That (_value, Is.EqualTo (5));
      _value = 200;
    }
  }
}