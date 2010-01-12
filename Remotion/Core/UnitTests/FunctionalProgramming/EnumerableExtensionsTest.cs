// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.FunctionalProgramming;

namespace Remotion.UnitTests.FunctionalProgramming
{
  [TestFixture]
  public class EnumerableExtensionsTest
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

    private class Element
    {
      private readonly int _value;
      public readonly Element Parent;

      public Element (int value, Element parent)
      {
        _value = value;
        Parent = parent;
      }

      public override string ToString ()
      {
        return _value.ToString();
      }
    }

    [Test]
    public void CreateSequence_WhileNotNull ()
    {
      Element first = new Element (1, null);
      Element second = new Element (2, first);
      Element third = new Element (3, second);
      Element fourth = new Element (4, third);

      IEnumerable<Element> actual = fourth.CreateSequence (e => e.Parent);
      Assert.That (actual.ToArray (), Is.EqualTo (new[] { fourth, third, second, first }));
    }

    [Test]
    public void CreateSequence_WhilePredicateEvaluatesTrue ()
    {
      Element first = new Element (1, new Element (0, null));
      Element second = new Element (2, first);
      Element third = new Element (3, second);
      Element fourth = new Element (4, third);

      IEnumerable<Element> actual = fourth.CreateSequence (e => e.Parent, e => e != first);
      Assert.That (actual.ToArray (), Is.EqualTo (new[] { fourth, third, second }));
    }

    [Test]
    public void CreateSequence_WhilePredicateEvaluatesTrue_WithNull ()
    {
      IEnumerable<Element> actual = ((Element)null).CreateSequence (e => e.Parent, e => e != null);
      Assert.That (actual.ToArray (), Is.Empty);
    }

    [Test]
    public void CreateSequence_WhilePredicateEvaluatesTrue_WithSingleElement ()
    {
      Element element = new Element (0, null);

      IEnumerable<Element> actual = element.CreateSequence (e => e.Parent, e => e != null);
      Assert.That (actual.ToArray (), Is.EqualTo (new[] { element }));
    }

    [Test]
    public void CreateSequence_WhilePredicateEvaluatesTrue_WithValueType ()
    {
      IEnumerable<int> actual = 4.CreateSequence (e => e - 1, e => e > 0);
      Assert.That (actual.ToArray(), Is.EqualTo (new[] { 4, 3, 2, 1 }));
    }

    [Test]
    public void ToEnumerable_WithObject ()
    {
      Element element = new Element (0, null);
      IEnumerable<Element> actual = element.ToEnumerable ();
      Assert.That (actual.ToArray (), Is.EqualTo (new[] { element }));
    }

    [Test]
    public void ToEnumerable_WithNull ()
    {
      IEnumerable<Element> actual = ((Element)null).ToEnumerable ();
      Assert.That (actual.ToArray (), Is.EqualTo (new Element[] { null }));
    }
    
    [Test]
    public void ToEnumerable_WithValueType ()
    {      
      IEnumerable<int> actual = 0.ToEnumerable ();
      Assert.That (actual.ToArray (), Is.EqualTo (new[] { 0 }));
    }

    [Test]
    public void SetEquals_True ()
    {
      IEnumerable<int> first = new[] { 1, 2, 3 };
      IEnumerable<int> second = new[] { 1, 2, 3 };
      Assert.That (first.SetEquals (second), Is.True);
    }

    [Test]
    public void SetEquals_True_Empty ()
    {
      IEnumerable<int> first = Enumerable.Empty<int> ();
      IEnumerable<int> second = Enumerable.Empty<int> ();
      Assert.That (first.SetEquals (second), Is.True);
    }

    [Test]
    public void SetEquals_True_DifferentOrder ()
    {
      IEnumerable<int> first = new[] { 1, 2, 3 };
      IEnumerable<int> second = new[] { 3, 1, 2 };
      Assert.That (first.SetEquals (second), Is.True);
    }

    [Test]
    public void SetEquals_DifferentCount ()
    {
      IEnumerable<int> first = new[] { 1, 2, 3, 1, 2, 2 };
      IEnumerable<int> second = new[] { 1, 2, 3 };
      Assert.That (first.SetEquals (second), Is.True);
    }

    [Test]
    public void SetEquals_False_FirstNotInSecond ()
    {
      IEnumerable<int> first = new[] { 1, 2, 3 };
      IEnumerable<int> second = new[] { 1, 2 };
      Assert.That (first.SetEquals (second), Is.False);
    }

    [Test]
    public void SetEquals_False_SecondNotInFirst ()
    {
      IEnumerable<int> first = new[] { 1 };
      IEnumerable<int> second = new[] { 1, 2 };
      Assert.That (first.SetEquals (second), Is.False);
    }
  }
}