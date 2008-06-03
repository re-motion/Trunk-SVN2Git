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
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{

[TestFixture]
public class WorkContextTest
{
  enum ThrowLocation { main_inside, main_outside, sub1_inside, sub1_outside, sub2_inside, sub2_outside, sub2_1_inside}

  // use inner catch

  [Test]
  public void TestCatchMainInside ()
  {
    Assert.AreEqual (
      "? main",
      PerformTest (ThrowLocation.main_inside, true));
  }

  [Test]
  public void TestCatchMainOutside ()
  {
    Assert.AreEqual (
      "",
      PerformTest (ThrowLocation.main_outside, true));
  }

  [Test]
  public void TestCatchSub1Inside()
  {
    Assert.AreEqual (
      "main\n" + 
      "? sub1",
      PerformTest (ThrowLocation.sub1_inside, true));
  }

  [Test]
  public void TestCatchSub1Outside()
  {
    Assert.AreEqual (
      "main",
      PerformTest (ThrowLocation.sub1_outside, true));
  }

  [Test]
  public void TestCatchSub2Inside()
  {
    Assert.AreEqual (
      "main\n" + 
      "? sub2",
      PerformTest (ThrowLocation.sub2_inside, true));
  }

  [Test]
  public void TestCatchSub2Outside()
  {
    Assert.AreEqual (
      "main",
      PerformTest (ThrowLocation.sub2_outside, true));
  }

  [Test]
  public void TestCatchSub2_1Inside()
  {
    Assert.AreEqual (
      "main\n" + 
      "? sub2\n" + 
      "? sub2.1",
      PerformTest (ThrowLocation.sub2_1_inside, true));
  }

  // do not use inner catch

  [Test]
  public void TestNoCatchMainInside ()
  {
    Assert.AreEqual (
      "? main",
      PerformTest (ThrowLocation.main_inside, false));
  }

  [Test]
  public void TestNoCatchMainOutside ()
  {
    Assert.AreEqual (
      "",
      PerformTest (ThrowLocation.main_outside, false));
  }

  [Test]
  public void TestNoCatchSub1Inside()
  {
    Assert.AreEqual (
      "? main\n" + 
      "? sub1",
      PerformTest (ThrowLocation.sub1_inside, false));
  }

  [Test]
  public void TestNoCatchSub1Outside()
  {
    Assert.AreEqual (
      "? main",
      PerformTest (ThrowLocation.sub1_outside, false));
  }

  [Test]
  public void TestNoCatchSub2Inside()
  {
    Assert.AreEqual (
      "? main\n" + 
      "? sub2",
      PerformTest (ThrowLocation.sub2_inside, false));
  }

  [Test]
  public void TestNoCatchSub2Outside()
  {
    Assert.AreEqual (
      "? main",
      PerformTest (ThrowLocation.sub2_outside, false));
  }

  [Test]
  public void TestNoCatchSub2_1Inside()
  {
    Assert.AreEqual (
      "? main\n" + 
      "? sub2\n" + 
      "? sub2.1",
      PerformTest (ThrowLocation.sub2_1_inside, false));
  }

  private string PerformTest (ThrowLocation location, bool catchInInnerHandler)
  {
    try
    {
      using (WorkContext ctxMain = WorkContext.EnterNew ("main"))
      {
        try
        {
          using (WorkContext ctxSub1 = WorkContext.EnterNew ("sub1"))
          {
            if (location == ThrowLocation.sub1_inside) throw new Exception (location.ToString());
            ctxSub1.Done();
          }
          if (location == ThrowLocation.sub1_outside) throw new Exception (location.ToString());
        }
        catch (Exception e)
        {
          if (!catchInInnerHandler)
            throw;
          Assert.AreEqual (location.ToString(), e.Message);
          return WorkContext.Stack.ToString();
        }
        try
        {
          using (WorkContext ctxSub2 = WorkContext.EnterNew ("sub2"))
          {
            using (WorkContext ctxSub2_1 = WorkContext.EnterNew ("sub2.1"))
            {
              if (location == ThrowLocation.sub2_1_inside) throw new Exception (location.ToString());
              ctxSub2_1.Done();
            }
            if (location == ThrowLocation.sub2_inside) throw new Exception (location.ToString());
            ctxSub2.Done();
          }
          if (location == ThrowLocation.sub2_outside) throw new Exception (location.ToString());
        }
        catch (Exception e)
        {
          if (!catchInInnerHandler)
            throw;
          Assert.AreEqual (location.ToString(), e.Message);
          return WorkContext.Stack.ToString();
        }
        if (location == ThrowLocation.main_inside) throw new Exception (location.ToString());
        ctxMain.Done();
      }
      if (location == ThrowLocation.main_outside) throw new Exception (location.ToString());
      return null;
    }
    catch (Exception e)
    {
      Assert.AreEqual (location.ToString(), e.Message);
      return WorkContext.Stack.ToString();
    }
  }
}

}
