// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections;
using NUnit.Framework;
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

    [Test]
    public void Succeed_NonEmptyGuid ()
    {
      Guid guid = Guid.NewGuid();
      var result = ArgumentUtility.CheckNotEmpty ("arg", guid);
      Assert.That (result, Is.EqualTo (guid));
    }

    [Test]
    [ExpectedException(typeof(ArgumentEmptyException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_EmptyGuid ()
    {
      ArgumentUtility.CheckNotEmpty ("arg", Guid.Empty);
    }
  }
}
