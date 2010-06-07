// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Sandboxing
{
  [TestFixture]
  public class DummyTest2
  {
    [Test]
    public void Test1 ()
    {
      var value = Math.Abs (-5);
      Assert.That (value, Is.EqualTo (5));
    }
    
  }
}