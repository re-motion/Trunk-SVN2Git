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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics;

namespace Remotion.UnitTests.Diagnostics
{
  [TestFixture]
  public class MemoryUsageInfoTest
  {
    [Test]
    public new void ToString ()
    {
      MemoryUsageInfo usage = new MemoryUsageInfo("X", new ByteValue(123), new ByteValue(345), new ByteValue(789));
      Assert.That (
          usage.ToString(),
          Is.EqualTo (
              string.Format (
                  "X:{0}\tWorking set: {1}{0}\tManaged memory before collect: {2}{0}\tAfter collect: {3}",
                  Environment.NewLine,
                  new ByteValue (123),
                  new ByteValue (345),
                  new ByteValue (789)
                  )));
    }

    [Test]
    public void ToCSVString ()
    {
      MemoryUsageInfo usage = new MemoryUsageInfo ("X", new ByteValue (123), new ByteValue (345), new ByteValue (789000000));
      Assert.That (usage.ToCSVString (), Is.EqualTo ("123;345;789000000"));
    }

    [Test]
    public void ToDifferenceString ()
    {
      MemoryUsageInfo comparison = new MemoryUsageInfo ("Y", new ByteValue (999), new ByteValue (999), new ByteValue (999));
      MemoryUsageInfo usage = new MemoryUsageInfo ("X", new ByteValue (123), new ByteValue (345), new ByteValue (789));
      Assert.That (
          usage.ToDifferenceString(comparison),
          Is.EqualTo (
              string.Format (
                  "Compared to Y:{0}\tWorking set: {1}{0}\tManaged memory before collect: {2}{0}\tAfter collect: {3}",
                  Environment.NewLine,
                  (new ByteValue (123) - new ByteValue (999)).ToDifferenceString(),
                  (new ByteValue (345) - new ByteValue (999)).ToDifferenceString(),
                  (new ByteValue (789) - new ByteValue (999)).ToDifferenceString ()
                  )));
    }
  }
}