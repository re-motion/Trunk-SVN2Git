// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
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
      SecurableClassValidationResult result = new SecurableClassValidationResult();

      Assert.IsTrue (result.IsValid);
    }

    [Test]
    public void IsValid_DuplicateStateCombination ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        StateCombination stateCombination = testHelper.CreateStateCombination (orderClass);

        SecurableClassValidationResult result = new SecurableClassValidationResult();

        result.AddDuplicateStateCombination (stateCombination);

        Assert.IsFalse (result.IsValid);
      }
    }

    [Test]
    public void DuplicateStateCombinations_AllValid ()
    {
      SecurableClassValidationResult result = new SecurableClassValidationResult();

      Assert.AreEqual (0, result.DuplicateStateCombinations.Count);
    }

    [Test]
    public void DuplicateStateCombinations_OneInvalidStateCombination ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        StateCombination stateCombination = testHelper.CreateStateCombination (orderClass);

        SecurableClassValidationResult result = new SecurableClassValidationResult();

        result.AddDuplicateStateCombination (stateCombination);

        Assert.AreEqual (1, result.DuplicateStateCombinations.Count);
        Assert.Contains (stateCombination, result.DuplicateStateCombinations);
      }
    }

    [Test]
    public void DuplicateStateCombinations_TwoInvalidStateCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        StatePropertyDefinition paymentProperty = testHelper.CreatePaymentStateProperty (orderClass);
        StateCombination statelessCombination = testHelper.CreateStateCombination (orderClass);
        StateCombination paidStateCombination = testHelper.CreateStateCombination (
            orderClass, paymentProperty[new EnumWrapper (PaymentState.Paid).Name]);

        SecurableClassValidationResult result = new SecurableClassValidationResult();

        result.AddDuplicateStateCombination (statelessCombination);
        result.AddDuplicateStateCombination (paidStateCombination);

        Assert.AreEqual (2, result.DuplicateStateCombinations.Count);
        Assert.Contains (statelessCombination, result.DuplicateStateCombinations);
        Assert.Contains (paidStateCombination, result.DuplicateStateCombinations);
      }
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
      SecurableClassValidationResult result = new SecurableClassValidationResult();

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
        StateCombination paidStateCombination = testHelper.CreateStateCombination (
            orderClass, paymentProperty[new EnumWrapper (PaymentState.Paid).Name]);

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
