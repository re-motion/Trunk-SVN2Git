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
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class StateUsageTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    [ExpectedException (typeof (ConstraintViolationException), ExpectedMessage =
        "The securable class definition 'Remotion.SecurityManager.UnitTests.TestDomain.Order' contains at least one state combination "
        + "that has been defined twice.")]
    public void ValidateDuringCommit_ByTouchOnClass ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (orderClass);
      StateDefinition paidState = paymentProperty[new EnumWrapper (PaymentState.Paid).Name];
      StateDefinition notPaidState = paymentProperty[new EnumWrapper (PaymentState.None).Name];
      StateCombination combination1 = _testHelper.CreateStateCombination (orderClass, paidState);
      StateCombination combination2 = _testHelper.CreateStateCombination (orderClass, notPaidState);
      StateCombination combination3 = _testHelper.CreateStateCombination (orderClass);
      combination1.AccessControlList.AccessControlEntries.Add (AccessControlEntry.NewObject());
      combination2.AccessControlList.AccessControlEntries.Add (AccessControlEntry.NewObject());
      combination3.AccessControlList.AccessControlEntries.Add (AccessControlEntry.NewObject());

      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        StateUsage stateUsage = combination2.StateUsages[0];
        stateUsage.StateDefinition = paidState;

        ClientTransactionScope.CurrentTransaction.Commit();
      }
    }

    private StateCombination GetStatelessCombinationForClass (SecurableClassDefinition classDefinition)
    {
      foreach (StateCombination currentCombination in classDefinition.StateCombinations)
      {
        if (currentCombination.StateUsages.Count == 0)
          return currentCombination;
      }

      return null;
    }

    private static List<StateDefinition> CreateEmptyStateList ()
    {
      return new List<StateDefinition>();
    }
  }
}