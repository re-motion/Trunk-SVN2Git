// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Sandboxing
{
  [TestFixture]
  public class DummyTest3
  {
    private int _value;

    [SetUp]
    public void SetUp ()
    {
      _value = 10;
    }

    [Test]
    public void Test1 ()
    {
      Assert.That (_value, Is.EqualTo (10));
      _value = 100;
    }

    [Test]
    public void Test2 ()
    {
      Assert.That (_value, Is.EqualTo (10));
      _value = 200;
    }
   
  }
}