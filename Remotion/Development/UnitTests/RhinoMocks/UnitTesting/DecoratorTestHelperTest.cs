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
using Remotion.Development.RhinoMocks.UnitTesting;
using Rhino.Mocks;
using Rhino.Mocks.Exceptions;

namespace Remotion.Development.UnitTests.RhinoMocks.UnitTesting
{
  [TestFixture]
  public class DecoratorTestHelperTest
  {
    private DecoratorTestHelper<IMyInterface> _helperForDecorator;
    private DecoratorTestHelper<IMyInterface> _helperForNonDelegatingDecorator;
    private DecoratorTestHelper<IMyInterface> _helperForFaultyDecorator;

    [SetUp]
    public void SetUp ()
    {
      _helperForDecorator = CreateDecoratorTestHelper (
          inner => inner.Get (),
          (inner, s) => inner.Do (s));

      _helperForNonDelegatingDecorator = CreateDecoratorTestHelper (inner => "Abc", (inner, s) => { });
      _helperForFaultyDecorator = CreateDecoratorTestHelper (
          inner =>
          {
            inner.Get ();
            return "faulty";
          },
          (inner, s) => inner.Do ("faulty"));
    }

    [Test]
    public void CheckDelegation_Func ()
    {
      Assert.That (() => _helperForDecorator.CheckDelegation (d => d.Get (), "Abc"), Throws.Nothing);
      Assert.That (
          () => _helperForNonDelegatingDecorator.CheckDelegation (d => d.Get (), "Abc"),
          Throws.TypeOf<ExpectationViolationException> ().And.Message.EqualTo ("IMyInterface.Get(); Expected #1, Actual #0."));
      Assert.That (
          () => _helperForFaultyDecorator.CheckDelegation (d => d.Get (), "Abc"),
          Throws.TypeOf<AssertionException> ().And.Message.StringStarting ("  Expected string length 3 but was 6. Strings differ at index 0."));
    }

    [Test]
    public void CheckDelegation_Func_MultipleCallsForSameMock ()
    {
      Assert.That (() => _helperForDecorator.CheckDelegation (d => d.Get (), "Abc"), Throws.Nothing);
      Assert.That (() => _helperForDecorator.CheckDelegation (d => d.Get (), "Abc"), Throws.Nothing);
    }

    [Test]
    public void CheckDelegation_Action ()
    {
      Assert.That (() => _helperForDecorator.CheckDelegation (d => d.Do ("Abc")), Throws.Nothing);
      Assert.That (
          () => _helperForNonDelegatingDecorator.CheckDelegation (d => d.Do ("Abc")),
          Throws.TypeOf<ExpectationViolationException> ().And.Message.EqualTo ("IMyInterface.Do(\"Abc\"); Expected #1, Actual #0."));
      Assert.That (
          () => _helperForFaultyDecorator.CheckDelegation (d => d.Do ("Abc")),
          Throws.TypeOf<ExpectationViolationException> ().And.Message.EqualTo (
              "IMyInterface.Do(\"faulty\"); Expected #0, Actual #1.\r\nIMyInterface.Do(\"Abc\"); Expected #1, Actual #0."));
    }

    [Test]
    public void CheckDelegation_Action_MultipleCallsForSameMock ()
    {
      Assert.That (() => _helperForDecorator.CheckDelegation (d => d.Do ("Abc")), Throws.Nothing);
      Assert.That (() => _helperForDecorator.CheckDelegation (d => d.Do ("Abc")), Throws.Nothing);
      Assert.That (() => _helperForDecorator.CheckDelegation (d => d.Do ("Abc")), Throws.Nothing);
    }

    [Test]
    public void CheckDelegation_Mixed_MultipleCallsForSameMock ()
    {
      Assert.That (() => _helperForDecorator.CheckDelegation (d => d.Do ("Abc")), Throws.Nothing);
      Assert.That (() => _helperForDecorator.CheckDelegation (d => d.Get (), "test"), Throws.Nothing);
    }

    private DecoratorTestHelper<IMyInterface> CreateDecoratorTestHelper (Func<IMyInterface, string> getMethod, Action<IMyInterface, string> doMethod)
    {
      var innerMock = MockRepository.GenerateStrictMock<IMyInterface> ();
      var decorator = new Decorator (innerMock, getMethod, doMethod);

      return new DecoratorTestHelper<IMyInterface> (decorator, innerMock);
    }

    public interface IMyInterface
    {
      string Get ();
      void Do (string s);
    }

    private class Decorator : IMyInterface
    {
      private readonly IMyInterface _inner;
      private readonly Func<IMyInterface, string> _getMethod;
      private readonly Action<IMyInterface, string> _doMethod;

      public Decorator (IMyInterface inner, Func<IMyInterface, string> getMethod, Action<IMyInterface, string> doMethod)
      {
        _inner = inner;
        _getMethod = getMethod;
        _doMethod = doMethod;
      }

      public string Get ()
      {
        return _getMethod (_inner);
      }

      public void Do (string s)
      {
        _doMethod (_inner, s);
      }
    }
  }
}