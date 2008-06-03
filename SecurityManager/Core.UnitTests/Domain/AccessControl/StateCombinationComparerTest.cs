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
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class StateCombinationComparerTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp ();
      _testHelper = new AccessControlTestHelper ();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void Equals_TwoStatelessCombinations ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      StateCombination combination1 = _testHelper.CreateStateCombination (orderClass);
      StateCombination combination2 = _testHelper.CreateStateCombination (orderClass);

      StateCombinationComparer comparer = new StateCombinationComparer ();

      Assert.IsTrue (comparer.Equals (combination1, combination2));
    }

    [Test]
    public void Equals_OneStatelessAndOneWithAState ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (orderClass);
      StateCombination combination1 = _testHelper.CreateStateCombination (orderClass);
      StateCombination combination2 = _testHelper.CreateStateCombination (orderClass, paymentProperty[new EnumWrapper (PaymentState.Paid).Name]);

      StateCombinationComparer comparer = new StateCombinationComparer ();

      Assert.IsFalse (comparer.Equals (combination1, combination2));
    }

    [Test]
    public void Equals_TwoDifferent ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (orderClass);
      StateCombination combination1 = _testHelper.CreateStateCombination (orderClass, paymentProperty[new EnumWrapper(PaymentState.None).Name]);
      StateCombination combination2 = _testHelper.CreateStateCombination (orderClass, paymentProperty[new EnumWrapper (PaymentState.Paid).Name]);

      StateCombinationComparer comparer = new StateCombinationComparer ();

      Assert.IsFalse (comparer.Equals (combination1, combination2));
    }

    [Test]
    public void GetHashCode_TwoStatelessCombinations ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      StateCombination combination1 = _testHelper.CreateStateCombination (orderClass);
      StateCombination combination2 = _testHelper.CreateStateCombination (orderClass);

      StateCombinationComparer comparer = new StateCombinationComparer ();

      Assert.AreEqual (comparer.GetHashCode (combination1), comparer.GetHashCode (combination2));
    }

    [Test]
    public void GetHashCode_OneStatelessAndOneWithAState ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (orderClass);
      StateCombination combination1 = _testHelper.CreateStateCombination (orderClass);
      StateCombination combination2 = _testHelper.CreateStateCombination (orderClass, paymentProperty[new EnumWrapper (PaymentState.Paid).Name]);

      StateCombinationComparer comparer = new StateCombinationComparer ();

      Assert.AreNotEqual (comparer.GetHashCode (combination1), comparer.GetHashCode (combination2));
    }

    [Test]
    public void GetHashCode_TwoDifferent ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (orderClass);
      StateCombination combination1 = _testHelper.CreateStateCombination (orderClass, paymentProperty[new EnumWrapper(PaymentState.None).Name]);
      StateCombination combination2 = _testHelper.CreateStateCombination (orderClass, paymentProperty[new EnumWrapper (PaymentState.Paid).Name]);

      StateCombinationComparer comparer = new StateCombinationComparer ();

      Assert.AreNotEqual (comparer.GetHashCode (combination1), comparer.GetHashCode (combination2));
    }
  }
}
