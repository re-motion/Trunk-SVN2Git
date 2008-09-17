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
using Remotion.Development.UnitTesting;

namespace Remotion.Development.UnitTests.Core.UnitTesting
{
  [TestFixture]
  public class MemoryUsageInfoTest
  {
    [Test]
    public void MemoryValue_MegaBytes ()
    {
      var zero = new MemoryUsage.MemoryValue (0);
      var oneByte = new MemoryUsage.MemoryValue (1);
      var oneKB = new MemoryUsage.MemoryValue (1024);
      var oneMB = new MemoryUsage.MemoryValue (1024 * 1024);
      var oneGB = new MemoryUsage.MemoryValue (1024 * 1024 * 1024);
      Assert.That (zero.MegaBytes, Is.EqualTo (0m));
      Assert.That (oneByte.MegaBytes, Is.EqualTo (0.00000095367431640625m));
      Assert.That (oneKB.MegaBytes, Is.EqualTo (0.0009765625m));
      Assert.That (oneMB.MegaBytes, Is.EqualTo (1m));
      Assert.That (oneGB.MegaBytes, Is.EqualTo (1024m));
    }

    [Test]
    public void MemoryValue_ToString ()
    {
      var onePointFiveMB = new MemoryUsage.MemoryValue (1024 * 1024 * 3 / 2);
      Assert.That (onePointFiveMB.ToString (), Is.EqualTo (1.5m.ToString ("N2") + " MB"));
    }

    [Test]
    public void MemoryValue_ToDifferenceString ()
    {
      var oneMB = new MemoryUsage.MemoryValue (1024 * 1024);
      var minusOneMB = new MemoryUsage.MemoryValue (-1024 * 1024);
      var zero = new MemoryUsage.MemoryValue (0);
      Assert.That (oneMB.ToDifferenceString (), Is.EqualTo ("+" + oneMB.ToString()));
      Assert.That (minusOneMB.ToDifferenceString (), Is.EqualTo (minusOneMB.ToString ()));
      Assert.That (zero.ToDifferenceString (), Is.EqualTo (zero.ToString ()));
    }

    [Test]
    public void MemoryValue_OperatorMinus ()
    {
      var threeMB = new MemoryUsage.MemoryValue (3 * 1024 * 1024);
      var twoMB = new MemoryUsage.MemoryValue (2 * 1024 * 1024);
      var oneMB = new MemoryUsage.MemoryValue (1024 * 1024);
      var minusOneMB = new MemoryUsage.MemoryValue (-1024 * 1024);
      var zero = new MemoryUsage.MemoryValue (0);

      Assert.That (threeMB - twoMB, Is.EqualTo (oneMB));
      Assert.That (oneMB - twoMB, Is.EqualTo (minusOneMB));
      Assert.That (oneMB - oneMB, Is.EqualTo (zero));
    }
  }
}