// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class StateCombinationTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void GetClass ()
    {
      StateCombination combination = _testHelper.GetStateCombinationForDeliveredAndUnpaidOrder();

      Assert.That (combination.AccessControlList.Class, Is.Not.Null);
      Assert.That (combination.Class, Is.SameAs (combination.AccessControlList.Class));
    }

    [Test]
    public void MatchesStates_StatefulAndWithoutDemandedStates ()
    {
      StateCombination combination = _testHelper.GetStateCombinationForDeliveredAndUnpaidOrder();
      List<StateDefinition> states = CreateEmptyStateList();

      Assert.IsFalse (combination.MatchesStates (states));
    }

    [Test]
    public void MatchesStates_DeliveredAndUnpaid ()
    {
      StateCombination combination = _testHelper.GetStateCombinationForDeliveredAndUnpaidOrder();
      StateDefinition[] states = combination.GetStates ();

      Assert.IsTrue (combination.MatchesStates (states));
    }

    [Test]
    [Ignore ("TODO: Implement")]
    public void MatchesStates_StatefulWithWildcard ()
    {
      StateCombination combination = _testHelper.GetStateCombinationForDeliveredAndUnpaidOrder ();
      StateDefinition[] states = combination.GetStates ();

      Assert.Fail ("TODO: Implement");
      Assert.IsTrue (combination.MatchesStates (states));
    }

    [Test]
    public void AttachState_NewState ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      StateCombination combination = _testHelper.CreateStateCombination (classDefinition);
      StatePropertyDefinition property = _testHelper.CreateTestProperty();
      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        combination.AttachState (property["Test1"]);

        var states = combination.GetStates();
        Assert.AreEqual (1, states.Length);
        Assert.AreSame (property["Test1"], states[0]);
      }
    }

    [Test]
    public void AttachState_WithoutClassDefinition ()
    {
      StateCombination combination = StateCombination.NewObject();
      StatePropertyDefinition property = _testHelper.CreateTestProperty();

      combination.AttachState (property["Test1"]);

      Assert.AreEqual (1, combination.GetStates().Length);
    }

    [Test]
    public void ClearStates ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      StateCombination combination = _testHelper.CreateStateCombination (classDefinition);
      StatePropertyDefinition property = _testHelper.CreateTestProperty();
      combination.AttachState (property["Test1"]);
      Assert.That (combination.GetStates(), Is.Not.Empty);

      combination.ClearStates();

      Assert.That (combination.GetStates(), Is.Empty);
    }

    [Test]
    public void GetStates_Empty ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      StateCombination combination = _testHelper.CreateStateCombination (classDefinition);

      StateDefinition[] states = combination.GetStates();

      Assert.AreEqual (0, states.Length);
    }

    [Test]
    public void GetStates_OneState ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      StatePropertyDefinition property = _testHelper.CreatePaymentStateProperty (classDefinition);
      StateDefinition state = property.DefinedStates[1];
      StateCombination combination = _testHelper.CreateStateCombination (classDefinition, state);

      StateDefinition[] states = combination.GetStates();

      Assert.AreEqual (1, states.Length);
      Assert.AreSame (state, states[0]);
    }

    [Test]
    public void GetStates_MultipleStates ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (classDefinition);
      StateDefinition paidState = paymentProperty.DefinedStates[1];
      StatePropertyDefinition orderStateProperty = _testHelper.CreateOrderStateProperty (classDefinition);
      StateDefinition deliveredState = orderStateProperty.DefinedStates[1];
      StateCombination combination = _testHelper.CreateStateCombination (classDefinition, paidState, deliveredState);

      StateDefinition[] states = combination.GetStates();

      Assert.AreEqual (2, states.Length);
      Assert.Contains (paidState, states);
      Assert.Contains (deliveredState, states);
    }

    [Test]
    [ExpectedException (typeof (ConstraintViolationException), ExpectedMessage =
        "The securable class definition 'Remotion.SecurityManager.UnitTests.TestDomain.Order' contains at least one state combination "
        + "that has been defined twice.")]
    public void ValidateDuringCommit_ByTouchOnClassForChangedStateUsagesCollection ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (orderClass);
      StateDefinition paidState = paymentProperty[EnumWrapper.Get (PaymentState.Paid).Name];
      StateDefinition notPaidState = paymentProperty[EnumWrapper.Get (PaymentState.None).Name];
      StateCombination combination1 = _testHelper.CreateStateCombination (orderClass, paidState);
      StateCombination combination2 = _testHelper.CreateStateCombination (orderClass, notPaidState);
      StateCombination combination3 = _testHelper.CreateStateCombination (orderClass);
      combination1.AccessControlList.AccessControlEntries.Add (AccessControlEntry.NewObject());
      combination2.AccessControlList.AccessControlEntries.Add (AccessControlEntry.NewObject());
      combination3.AccessControlList.AccessControlEntries.Add (AccessControlEntry.NewObject());
      var dupicateStateCombination = orderClass.CreateStatefulAccessControlList().StateCombinations[0];

      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        dupicateStateCombination.AttachState (paidState);

        ClientTransaction.Current.Commit();
      }
    }

    [Test]
    public void Commit_DeletedStateCombination ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StateCombination combination = _testHelper.CreateStateCombination (orderClass);
      combination.AccessControlList.AccessControlEntries.Add (AccessControlEntry.NewObject());

      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (StateType.Unchanged, GetStateFromDataContainer (orderClass));
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          Assert.AreEqual (StateType.Unchanged, GetStateFromDataContainer (orderClass));
          combination.AccessControlList.Delete();
          Assert.IsNull (combination.Class);

          Assert.AreEqual (StateType.Unchanged, GetStateFromDataContainer (orderClass));
          ClientTransaction.Current.Commit();
        }
        Assert.AreEqual (StateType.Unchanged, GetStateFromDataContainer (orderClass));
      }
    }

    private StateType GetStateFromDataContainer (DomainObject orderClass)
    {
      var transaction = orderClass.HasBindingTransaction ? orderClass.GetBindingTransaction () : ClientTransaction.Current;

      var dataManager = DataManagementService.GetDataManager (transaction);
      var dataContainer = dataManager.GetDataContainerWithLazyLoad (orderClass.ID, true);
      return dataContainer.State;
    }

    [Test]
    public void SetAndGet_Index ()
    {
      StateCombination stateCombination = StateCombination.NewObject();

      stateCombination.Index = 1;
      Assert.AreEqual (1, stateCombination.Index);
    }

    [Test]
    public void TouchClassOnCommit ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StateCombination combination = _testHelper.CreateStateCombination (orderClass);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        bool commitOnClassWasCalled = false;
        orderClass.Committing += delegate { commitOnClassWasCalled = true; };
        combination.RegisterForCommit();

        ClientTransaction.Current.Commit();

        Assert.That (commitOnClassWasCalled, Is.True);
      }
    }

    private static List<StateDefinition> CreateEmptyStateList ()
    {
      return new List<StateDefinition>();
    }
  }
}
