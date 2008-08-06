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
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class StateCombinationBuilderTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;
    private SecurableClassDefinition _orderClass;
    private StateCombinationBuilder _stateCombinationBuilder;

    public override void SetUp ()
    {
      base.SetUp ();

      _testHelper = new AccessControlTestHelper ();
      _testHelper.Transaction.EnterNonDiscardingScope();
      _orderClass = _testHelper.CreateOrderClassDefinition ();
      _stateCombinationBuilder = new StateCombinationBuilder (_orderClass);
    }

    [Test]
    public void Create_WithoutStateProperty ()
    {
      Assert.That (_orderClass.StateProperties, Is.Empty);

      StateDefinition[][] expected = new StateDefinition[][] { };

      StateDefinition[][] actual = _stateCombinationBuilder.CreatePropertyProduct ();

      Check (actual, expected);
    }

    [Test]
    public void Create_WithSingleStateProperty ()
    {
      StatePropertyDefinition orderStateProperty = _testHelper.CreateOrderStateProperty (_orderClass);
      Assert.That (_orderClass.StateProperties.Count, Is.EqualTo (1));

      StateDefinition[][] expected =
          new[]
          {
              new[] { orderStateProperty.DefinedStates[0] },
              new[] { orderStateProperty.DefinedStates[1] },
          };

      StateDefinition[][] actual = _stateCombinationBuilder.CreatePropertyProduct ();

      Check (actual, expected);
    }

    [Test]
    public void Create_WithTwoStateProperties ()
    {
      StatePropertyDefinition orderStateProperty = _testHelper.CreateOrderStateProperty (_orderClass);
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (_orderClass);
      Assert.That (_orderClass.StateProperties.Count, Is.EqualTo (2));

      StateDefinition[][] expected =
          new[]
          {
              new[] { orderStateProperty.DefinedStates[0], paymentProperty.DefinedStates[0] },
              new[] { orderStateProperty.DefinedStates[0], paymentProperty.DefinedStates[1] },
              new[] { orderStateProperty.DefinedStates[1], paymentProperty.DefinedStates[0] },
              new[] { orderStateProperty.DefinedStates[1], paymentProperty.DefinedStates[1] },
          };

      StateDefinition[][] actual = _stateCombinationBuilder.CreatePropertyProduct ();

      Check (actual, expected);
    }

    [Test]
    public void Create_WithThreeStatePropertiesAndOneOfThemEmpty ()
    {
      StatePropertyDefinition orderStateProperty = _testHelper.CreateOrderStateProperty (_orderClass);
      StatePropertyDefinition emptyProperty = _testHelper.CreateStateProperty ("Empty");
      _orderClass.AddStateProperty (emptyProperty);
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (_orderClass);
      Assert.That (_orderClass.StateProperties.Count, Is.EqualTo (3));

      StateDefinition[][] expected =
          new[]
          {
              new[] { orderStateProperty.DefinedStates[0], null, paymentProperty.DefinedStates[0] },
              new[] { orderStateProperty.DefinedStates[0], null, paymentProperty.DefinedStates[1] },
              new[] { orderStateProperty.DefinedStates[1], null, paymentProperty.DefinedStates[0] },
              new[] { orderStateProperty.DefinedStates[1], null, paymentProperty.DefinedStates[1] },
          };

      StateDefinition[][] actual = _stateCombinationBuilder.CreatePropertyProduct ();

      Check (actual, expected);
    }

    private void Check (StateDefinition[][] actual, StateDefinition[][] expected)
    {
      Assert.That (actual.Length, Is.EqualTo (expected.Length));
      for (int i = 0; i < actual.Length; i++)
        Assert.That (actual[0], Is.EquivalentTo (expected[0]));
    }
  }
}
