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
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
  [TestFixture]
  public class CheckNotEmpty
  {
    [Test]
    public void Succeed_NonEmptyString ()
    {
      var result = ArgumentUtility.CheckNotEmpty ("arg", "x");
      Assert.That (result, Is.EqualTo ("x"));
    }

    [Test]
    public void Succeed_NullString ()
    {
      const string s = null;
      var result = ArgumentUtility.CheckNotEmpty ("arg", s);
      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentEmptyException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_EmptyString ()
    {
      ArgumentUtility.CheckNotEmpty ("arg", "");
    }

    [Test]
    public void Succeed_NonEmptyCollection ()
    {
      var result = ArgumentUtility.CheckNotEmpty ("arg", new[] {1});
      Assert.That (result, Is.EqualTo (new[] {1}));
    }

    [Test]
    public void Succeed_NullCollection ()
    {
      var result = ArgumentUtility.CheckNotEmpty ("arg", (IEnumerable) null);
      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentEmptyException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_EmptyCollection ()
    {
      ArgumentUtility.CheckNotEmpty ("arg", Type.EmptyTypes);
    }
  }
}