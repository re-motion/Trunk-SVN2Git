// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Assertion=Remotion.Utilities.Assertion;
using AssertionException=Remotion.Utilities.AssertionException;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class AssertionTest
  {
    [Test]
    public void TestIsTrueHolds ()
    {
      Assertion.IsTrue (true);
    }

    [Test]
    [ExpectedException (typeof (AssertionException))]
    public void TestIsTrueFails ()
    {
      Assertion.IsTrue (false);
    }

    [Test]
    public void TestIsFalseHolds ()
    {
      Assertion.IsFalse (false);
    }

    [Test]
    [ExpectedException (typeof (AssertionException))]
    public void TestIsFalseFails ()
    {
      Assertion.IsFalse (true);
    }

    [Test]
    public void IsNull_True ()
    {
      Assertion.IsNull (null);
    }

    [Test]
    [ExpectedException (typeof (AssertionException), ExpectedMessage = "Assertion failed: Expression does not evaluate to a null reference.")]
    public void IsNull_False ()
    {
      Assertion.IsNull ("x");
    }

    [Test]
    public void INull_ValueType_True ()
    {
      int? value = null;
      Assertion.IsNull (value);
    }

    [Test]
    [ExpectedException (typeof (AssertionException), ExpectedMessage = "Assertion failed: Expression does not evaluate to a null reference.")]
    public void IsNull_ValueType_False ()
    {
      int? value = 5;
      Assertion.IsNull (value);
    }

    [Test]
    public void IsNull_Message_True ()
    {
      Assertion.IsNull (null, "a");
    }

    [Test]
    [ExpectedException (typeof (AssertionException), ExpectedMessage = "a")]
    public void IsNull_Message_False ()
    {
      Assertion.IsNull ("x", "a");
    }

    [Test]
    public void IsNull_Message_Args_True ()
    {     
      Assertion.IsNull (null, "a{0}b", 5);
    }

    [Test]
    [ExpectedException (typeof (AssertionException), ExpectedMessage = "a5b")]
    public void IsNull_Message_Args_False ()
    {
      Assertion.IsNull ("x", "a{0}b", 5);
    }

    [Test]
    public void IsNotNull_True ()
    {
      var instance = "x";

      var result = Assertion.IsNotNull (instance);

      Assert.That (result, Is.SameAs (instance));
    }

    [Test]
    [ExpectedException (typeof (AssertionException), ExpectedMessage = "Assertion failed: Expression evaluates to a null reference.")]
    public void IsNotNull_False ()
    {
      Assertion.IsNotNull<object> (null);
    }

    [Test]
    public void IsNotNull_ValueType_True ()
    {
      int? value = 5;
      var result = Assertion.IsNotNull (value);
      Assert.That (result, Is.EqualTo (value));
    }

    [Test]
    [ExpectedException (typeof (AssertionException), ExpectedMessage = "Assertion failed: Expression evaluates to a null reference.")]
    public void IsNotNull_ValueType_False ()
    {
      int? value = null;
      Assertion.IsNotNull (value);
    }

    [Test]
    public void IsNotNull_Message_True ()
    {
      var instance = "x";

      var result = Assertion.IsNotNull (instance, "a");

      Assert.That (result, Is.SameAs (instance));
    }

    [Test]
    [ExpectedException (typeof (AssertionException), ExpectedMessage = "a")]
    public void IsNotNull_Message_False ()
    {
      Assertion.IsNotNull<object> (null, "a");
    }

    [Test]
    public void IsNotNull_Message_Args_True ()
    {
      var instance = "x";

      var result = Assertion.IsNotNull (instance, "a{0}b", 5);

      Assert.That (result, Is.SameAs (instance));
    }

    [Test]
    [ExpectedException (typeof (AssertionException), ExpectedMessage = "a5b")]
    public void IsNotNull_Message_Args_False ()
    {
      Assertion.IsNotNull<object> (null, "a{0}b", 5);
    }
  }
}
