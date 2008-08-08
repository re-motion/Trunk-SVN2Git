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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class EnumerableTest
  {
    private IEnumerable<string> _enumerableWithoutValues;
    private IEnumerable<string> _enumerableWithOneValue;
    private IEnumerable<string> _enumerableWithThreeValues;

    [SetUp]
    public void SetUp ()
    {
      _enumerableWithoutValues = new string[0];
      _enumerableWithOneValue = new[] { "test" };
      _enumerableWithThreeValues = new[] { "test1", "test2", "test3" };
    }

    [Test]
    public void First_ThrowCustomException_WithOneValue ()
    {
      string actual = _enumerableWithOneValue.First (() => new ApplicationException ("ExpectedText"));

      Assert.That (actual, Is.EqualTo ("test"));
    }

    [Test]
    public void First_ThrowCustomException_WithThreeValues ()
    {
      string actual = _enumerableWithThreeValues.First (() => new ApplicationException ("ExpectedText"));

      Assert.That (actual, Is.EqualTo ("test1"));
    }

    [Test]
    [ExpectedException (typeof (ApplicationException), ExpectedMessage = "ExpectedText")]
    public void First_ThrowCustomException_Empty ()
    {
      _enumerableWithoutValues.First (() => new ApplicationException ("ExpectedText"));
    }

    [Test]
    public void First_WithPredicate_ThrowCustomException_WithOneValue ()
    {
      string actual = _enumerableWithOneValue.First (s => s == "test", () => new ApplicationException ("ExpectedText"));

      Assert.That (actual, Is.EqualTo ("test"));
    }

    [Test]
    public void First_WithPredicate_ThrowCustomException_WithThreeValues ()
    {
      string actual = _enumerableWithThreeValues.First (s => s == "test2", () => new ApplicationException ("ExpectedText"));

      Assert.That (actual, Is.EqualTo ("test2"));
    }

    [Test]
    [ExpectedException (typeof (ApplicationException), ExpectedMessage = "ExpectedText")]
    public void First_WithPredicate_ThrowCustomException_Empty ()
    {
      _enumerableWithoutValues.First (s => s == "test2", () => new ApplicationException ("ExpectedText"));
    }

    [Test]
    [ExpectedException (typeof (ApplicationException), ExpectedMessage = "ExpectedText")]
    public void First_WithPredicate_ThrowCustomException_NoMatch ()
    {
      _enumerableWithThreeValues.First (s => s == "invalid", () => new ApplicationException ("ExpectedText"));
    }

    [Test]
    public void Single_ThrowCustomException_WithOneValue ()
    {
      string actual = _enumerableWithOneValue.Single (() => new ApplicationException ("ExpectedText"));

      Assert.That (actual, Is.EqualTo ("test"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Sequence contains more than one element.")]
    public void Single_ThrowCustomException_WithThreeValues ()
    {
      _enumerableWithThreeValues.Single (() => new ApplicationException ("ExpectedText"));
    }

    [Test]
    [ExpectedException (typeof (ApplicationException), ExpectedMessage = "ExpectedText")]
    public void Single_ThrowCustomException_Empty ()
    {
      _enumerableWithoutValues.Single (() => new ApplicationException ("ExpectedText"));
    }

    [Test]
    public void Single_WithPredicate_ThrowCustomException_WithThreeValuesAndSingleMatch ()
    {
      string actual = _enumerableWithThreeValues.Single (s => s == "test2", () => new ApplicationException ("ExpectedText"));

      Assert.That (actual, Is.EqualTo ("test2"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Sequence contains more than one matching element.")]
    public void Single_WithPredicate_ThrowCustomException_WithThreeValuesAndMultipleMatches ()
    {
      _enumerableWithThreeValues.Single (s => true, () => new ApplicationException ("ExpectedText"));
    }

    [Test]
    [ExpectedException (typeof (ApplicationException), ExpectedMessage = "ExpectedText")]
    public void Single_WithPredicate_ThrowCustomException_Empty ()
    {
      _enumerableWithoutValues.Single (s => s == "test2", () => new ApplicationException ("ExpectedText"));
    }

    [Test]
    [ExpectedException (typeof (ApplicationException), ExpectedMessage = "ExpectedText")]
    public void Single_WithPredicate_ThrowCustomException_NoMatch ()
    {
      _enumerableWithThreeValues.Single (s => s == "invalid", () => new ApplicationException ("ExpectedText"));
    }
  }
}