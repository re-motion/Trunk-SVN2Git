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
  public class ByteValueTest
  {
    [Test]
    public void MegaBytes ()
    {
      var zero = new ByteValue (0);
      var oneByte = new ByteValue (1);
      var oneKB = new ByteValue (1024);
      var oneMB = new ByteValue (1024 * 1024);
      var oneGB = new ByteValue (1024 * 1024 * 1024);
      Assert.That (zero.MegaBytes, Is.EqualTo (0m));
      Assert.That (oneByte.MegaBytes, Is.EqualTo (0.00000095367431640625m));
      Assert.That (oneKB.MegaBytes, Is.EqualTo (0.0009765625m));
      Assert.That (oneMB.MegaBytes, Is.EqualTo (1m));
      Assert.That (oneGB.MegaBytes, Is.EqualTo (1024m));
    }

    [Test]
    public void ToString ()
    {
      var onePointFiveMB = new ByteValue (1024 * 1024 * 3 / 2);
      Assert.That (onePointFiveMB.ToString (), Is.EqualTo (1.5m.ToString ("N2") + " MB"));
    }

    [Test]
    public void ToStringBelowOneMB ()
    {
      var fiveHundredKB = new ByteValue (1024 * 500);
      Assert.That (fiveHundredKB.ToString (), Is.EqualTo ((1024 * 500).ToString ("N0") + " bytes"));
    }

    [Test]
    public void ToDifferenceString ()
    {
      var oneMB = new ByteValue (1024 * 1024);
      var minusOneMB = new ByteValue (-1024 * 1024);
      var zero = new ByteValue (0);
      Assert.That (oneMB.ToDifferenceString (), Is.EqualTo ("+" + oneMB.ToString()));
      Assert.That (minusOneMB.ToDifferenceString (), Is.EqualTo (minusOneMB.ToString ()));
      Assert.That (zero.ToDifferenceString (), Is.EqualTo (zero.ToString ()));
    }

    [Test]
    public void OperatorPlus ()
    {
      var threeMB = new ByteValue (3 * 1024 * 1024);
      var twoMB = new ByteValue (2 * 1024 * 1024);
      var oneMB = new ByteValue (1024 * 1024);
      var minusOneMB = new ByteValue (-1024 * 1024);
      var zero = new ByteValue (0);

      Assert.That (oneMB + twoMB, Is.EqualTo (threeMB));
      Assert.That (minusOneMB + oneMB, Is.EqualTo (zero));
    }

    [Test]
    public void OperatorMinus ()
    {
      var threeMB = new ByteValue (3 * 1024 * 1024);
      var twoMB = new ByteValue (2 * 1024 * 1024);
      var oneMB = new ByteValue (1024 * 1024);
      var minusOneMB = new ByteValue (-1024 * 1024);
      var zero = new ByteValue (0);

      Assert.That (threeMB - twoMB, Is.EqualTo (oneMB));
      Assert.That (oneMB - twoMB, Is.EqualTo (minusOneMB));
      Assert.That (oneMB - oneMB, Is.EqualTo (zero));
    }
  }
}