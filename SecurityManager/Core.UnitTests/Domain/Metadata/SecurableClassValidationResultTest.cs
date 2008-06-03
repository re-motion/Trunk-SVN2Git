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
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class SecurableClassValidationResultTest : DomainTest
  {
    [Test]
    public void IsValid_Valid ()
    {
      SecurableClassValidationResult result = new SecurableClassValidationResult ();

      Assert.IsTrue (result.IsValid);
    }

    [Test]
    public void IsValid_InvalidStateCombination ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        StateCombination stateCombination = testHelper.CreateStateCombination (orderClass);

        SecurableClassValidationResult result = new SecurableClassValidationResult();

        result.AddInvalidStateCombination (stateCombination);

        Assert.IsFalse (result.IsValid);
      }
    }

    [Test]
    public void InvalidStateCombinations_AllValid ()
    {
      SecurableClassValidationResult result = new SecurableClassValidationResult ();

      Assert.AreEqual (0, result.InvalidStateCombinations.Count);
    }

    [Test]
    public void InvalidStateCombinations_OneInvalidStateCombination ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        StateCombination stateCombination = testHelper.CreateStateCombination (orderClass);

        SecurableClassValidationResult result = new SecurableClassValidationResult();

        result.AddInvalidStateCombination (stateCombination);

        Assert.AreEqual (1, result.InvalidStateCombinations.Count);
        Assert.Contains (stateCombination, result.InvalidStateCombinations);
      }
    }

    [Test]
    public void InvalidStateCombinations_TwoInvalidStateCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        StatePropertyDefinition paymentProperty = testHelper.CreatePaymentStateProperty (orderClass);
        StateCombination statelessCombination = testHelper.CreateStateCombination (orderClass);
        StateCombination paidStateCombination = testHelper.CreateStateCombination (orderClass, paymentProperty[new EnumWrapper (PaymentState.Paid).Name]);

        SecurableClassValidationResult result = new SecurableClassValidationResult();

        result.AddInvalidStateCombination (statelessCombination);
        result.AddInvalidStateCombination (paidStateCombination);

        Assert.AreEqual (2, result.InvalidStateCombinations.Count);
        Assert.Contains (statelessCombination, result.InvalidStateCombinations);
        Assert.Contains (paidStateCombination, result.InvalidStateCombinations);
      }
    }
  }
}
