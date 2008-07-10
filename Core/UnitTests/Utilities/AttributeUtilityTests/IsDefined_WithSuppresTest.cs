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
using Remotion.UnitTests.Utilities.AttributeUtilityTests.SampleTypes;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  [TestFixture]
  public class IsDefined_WithSuppresTest
  {
    [Test]
    public void IsDefined_NoAttributes ()
    {
      Assert.That (AttributeUtility.IsDefined (typeof (DerivedDerivedDerivedSuppressed),typeof(Attribute), true), Is.False);
    }

    [Test]
    public void IsDefined_WithAttributesAndSuppressed()
    {
      Assert.That (AttributeUtility.IsDefined (typeof (DerivedWithAttributesAndSuppressed), typeof (Attribute), true), Is.True);
    }

    [Test]
    public void IsDefined_WithAttributesAndNoSuppressed ()
    {
      Assert.That (AttributeUtility.IsDefined (typeof (BaseClassWithAttribute), typeof (Attribute), true), Is.True);
    }
  }
}