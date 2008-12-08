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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;
using Remotion.SecurityManager.UnitTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class SecurableClassDefinitionTest : DomainTest
  {
    [Test]
    public void AddAccessType_TwoNewAccessTypes ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        AccessTypeDefinition accessType0 = AccessTypeDefinition.NewObject ();
        AccessTypeDefinition accessType1 = AccessTypeDefinition.NewObject ();
        SecurableClassDefinitionWrapper classDefinitionWrapper = new SecurableClassDefinitionWrapper (SecurableClassDefinition.NewObject ());
        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          Assert.AreEqual (StateType.Unchanged, classDefinitionWrapper.SecurableClassDefinition.State);

          classDefinitionWrapper.SecurableClassDefinition.AddAccessType (accessType0);
          classDefinitionWrapper.SecurableClassDefinition.AddAccessType (accessType1);

          Assert.AreEqual (2, classDefinitionWrapper.SecurableClassDefinition.AccessTypes.Count);
          Assert.AreSame (accessType0, classDefinitionWrapper.SecurableClassDefinition.AccessTypes[0]);
          Assert.AreSame (accessType1, classDefinitionWrapper.SecurableClassDefinition.AccessTypes[1]);
          DomainObjectCollection references = classDefinitionWrapper.AccessTypeReferences;
          Assert.AreEqual (0, ((AccessTypeReference) references[0]).Index);
          Assert.AreEqual (1, ((AccessTypeReference) references[1]).Index);
          Assert.AreEqual (StateType.Changed, classDefinitionWrapper.SecurableClassDefinition.State);
        }
      }
    }

    [Test]
    public void AddStateProperty ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        StatePropertyDefinition stateProperty = StatePropertyDefinition.NewObject ();
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject ();

        classDefinition.AddStateProperty (stateProperty);

        Assert.AreEqual (1, classDefinition.StateProperties.Count);
        Assert.AreSame (stateProperty, classDefinition.StateProperties[0]);
      }
    }

    [Test]
    public void FindStateCombination_ValidStates ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        StateCombination expectedCombination = testHelper.GetStateCombinationForDeliveredAndUnpaidOrder ();
        SecurableClassDefinition orderClass = expectedCombination.Class;
        List<StateDefinition> states = testHelper.GetDeliveredAndUnpaidStateList (orderClass);

        StateCombination stateCombination = orderClass.FindStateCombination (states);
        Assert.AreSame (expectedCombination, stateCombination);
      }
    }

    [Test]
    public void StateProperties_Empty ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();

        Assert.IsEmpty (orderClass.StateProperties);
      }
    }

    [Test]
    public void StateProperties_OrderStateAndPaymentState ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinitionWithProperties ();

        Assert.AreEqual (AccessControlTestHelper.OrderClassPropertyCount, orderClass.StateProperties.Count);
      }
    }

    [Test]
    public void StateProperties_IsCached ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinitionWithProperties ();

        DomainObjectCollection firstCollection = orderClass.StateProperties;
        DomainObjectCollection secondCollection = orderClass.StateProperties;

        Assert.AreSame (firstCollection, secondCollection);
      }
    }

    [Test]
    public void StateProperties_IsReadOnly ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinitionWithProperties ();

        Assert.IsTrue (orderClass.StateProperties.IsReadOnly);
      }
    }

    [Test]
    public void StateProperties_IsResetByAddStateProperty ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinitionWithProperties ();

        DomainObjectCollection firstCollection = orderClass.StateProperties;
        orderClass.AddStateProperty (testHelper.CreateTestProperty ());
        DomainObjectCollection secondCollection = orderClass.StateProperties;

        Assert.AreNotSame (firstCollection, secondCollection);
      }
    }

    [Test]
    public void AccessTypes_Empty ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();

        Assert.IsEmpty (orderClass.AccessTypes);
      }
    }

    [Test]
    public void AccessTypes_OneAccessType ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
        testHelper.AttachJournalizeAccessType (orderClass);

        Assert.AreEqual (1, orderClass.AccessTypes.Count);
      }
    }

    [Test]
    public void AccessTypes_IsCached ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
        testHelper.AttachJournalizeAccessType (orderClass);

        DomainObjectCollection firstCollection = orderClass.AccessTypes;
        DomainObjectCollection secondCollection = orderClass.AccessTypes;

        Assert.AreSame (firstCollection, secondCollection);
      }
    }

    [Test]
    public void AccessTypes_IsReadOnly ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
        testHelper.AttachJournalizeAccessType (orderClass);

        Assert.IsTrue (orderClass.AccessTypes.IsReadOnly);
      }
    }

    [Test]
    public void AccessTypes_IsResetByAddAccessType ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();

        DomainObjectCollection firstCollection = orderClass.AccessTypes;
        orderClass.AddAccessType (testHelper.CreateJournalizeAccessType ());
        DomainObjectCollection secondCollection = orderClass.AccessTypes;

        Assert.AreNotSame (firstCollection, secondCollection);
      }
    }

    [Test]
    public void FindByName_ValidClassName ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();

      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition invoiceClass;
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
        invoiceClass = testHelper.CreateInvoiceClassDefinition ();
        testHelper.Transaction.Commit ();
      }

      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        SecurableClassDefinition foundClass = SecurableClassDefinition.FindByName ("Remotion.SecurityManager.UnitTests.TestDomain.Invoice");

        MetadataObjectAssert.AreEqual (invoiceClass, testHelper.Transaction, foundClass);
      }
    }

    [Test]
    public void FindByName_InvalidClassName ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();

      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
        SecurableClassDefinition invoiceClass = testHelper.CreateInvoiceClassDefinition ();
        testHelper.Transaction.Commit ();
      }

      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        SecurableClassDefinition foundClass = SecurableClassDefinition.FindByName ("Invce");

        Assert.IsNull (foundClass);
      }
    }

    [Test]
    public void FindAll_EmptyResult ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();

      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        DomainObjectCollection result = SecurableClassDefinition.FindAll ();

        Assert.AreEqual (0, result.Count);
      }
    }

    [Test]
    public void FindAll_TenFound ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      SecurableClassDefinition[] expectedClassDefinitions;
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        expectedClassDefinitions = dbFixtures.CreateAndCommitSecurableClassDefinitions (10, ClientTransactionScope.CurrentTransaction);
      }

      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        DomainObjectCollection result = SecurableClassDefinition.FindAll ();

        Assert.AreEqual (10, result.Count);
        for (int i = 0; i < result.Count; i++)
          Assert.AreEqual (expectedClassDefinitions[i].ID, result[i].ID, "Wrong Index.");
      }
    }

    [Test]
    public void FindAllBaseClasses_TenFound ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      SecurableClassDefinition[] expectedClassDefinitions;
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        expectedClassDefinitions =
            dbFixtures.CreateAndCommitSecurableClassDefinitionsWithSubClassesEach (10, 10, ClientTransactionScope.CurrentTransaction);
      }

      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        DomainObjectCollection result = SecurableClassDefinition.FindAllBaseClasses ();

        Assert.AreEqual (10, result.Count);
        for (int i = 0; i < result.Count; i++)
          Assert.AreEqual (expectedClassDefinitions[i].ID, result[i].ID, "Wrong Index.");
      }
    }

    [Test]
    public void GetDerivedClasses_TenFound ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      SecurableClassDefinition expectedBaseClassDefinition;
      ObjectList<SecurableClassDefinition> expectedDerivedClasses;
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        SecurableClassDefinition[] expectedBaseClassDefinitions =
            dbFixtures.CreateAndCommitSecurableClassDefinitionsWithSubClassesEach (10, 10, ClientTransactionScope.CurrentTransaction);
        expectedBaseClassDefinition = expectedBaseClassDefinitions[4];
        expectedDerivedClasses = expectedBaseClassDefinition.DerivedClasses;
      }

      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        SecurableClassDefinition actualBaseClassDefinition = SecurableClassDefinition.GetObject (expectedBaseClassDefinition.ID);

        Assert.AreEqual (10, actualBaseClassDefinition.DerivedClasses.Count);
        for (int i = 0; i < actualBaseClassDefinition.DerivedClasses.Count; i++)
          Assert.AreEqual (expectedDerivedClasses[i].ID, actualBaseClassDefinition.DerivedClasses[i].ID, "Wrong Index.");
      }
    }

    [Test]
    public void CreateStatelessAccessControlList ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject ();
        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          Assert.AreEqual (StateType.Unchanged, classDefinition.State);

          StatelessAccessControlList accessControlList = classDefinition.CreateStatelessAccessControlList ();

          Assert.AreSame (classDefinition, accessControlList.Class);
          Assert.IsNotEmpty (accessControlList.AccessControlEntries);
          Assert.AreEqual (StateType.Changed, classDefinition.State);
        }
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "A SecurableClassDefinition only supports a single StatelessAccessControlList at a time.")]
    public void CreateStatelessAccessControlList_Twice ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject ();
        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          Assert.AreEqual (StateType.Unchanged, classDefinition.State);

          classDefinition.CreateStatelessAccessControlList ();
          classDefinition.CreateStatelessAccessControlList ();
        }
      }
    }

    [Test]
    public void CreateStatefulAccessControlList ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject ();
        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          Assert.AreEqual (StateType.Unchanged, classDefinition.State);

          StatefulAccessControlList accessControlList = classDefinition.CreateStatefulAccessControlList ();

          Assert.AreSame (classDefinition, accessControlList.Class);
          Assert.IsNotEmpty (accessControlList.AccessControlEntries);
          Assert.IsNotEmpty (accessControlList.StateCombinations);
          Assert.AreEqual (StateType.Changed, classDefinition.State);
        }
      }
    }

    [Test]
    public void CreateStatefulAccessControlList_TwoNewAcls ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject ();
        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          Assert.AreEqual (StateType.Unchanged, classDefinition.State);

          StatefulAccessControlList acccessControlList0 = classDefinition.CreateStatefulAccessControlList ();
          StatefulAccessControlList acccessControlListl = classDefinition.CreateStatefulAccessControlList ();

          Assert.AreEqual (2, classDefinition.StatefulAccessControlLists.Count);
          Assert.AreSame (acccessControlList0, classDefinition.StatefulAccessControlLists[0]);
          Assert.AreEqual (0, acccessControlList0.Index);
          Assert.AreSame (acccessControlListl, classDefinition.StatefulAccessControlLists[1]);
          Assert.AreEqual (1, acccessControlListl.Index);
          Assert.AreEqual (StateType.Changed, classDefinition.State);
        }
      }
    }

    [Test]
    public void Get_AccessTypesFromDatabase ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      SecurableClassDefinition expectedClassDefinition;
      ObjectList<AccessTypeDefinition> expectedAccessTypes;
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        expectedClassDefinition = dbFixtures.CreateAndCommitSecurableClassDefinitionWithAccessTypes (10, ClientTransactionScope.CurrentTransaction);
        expectedAccessTypes = expectedClassDefinition.AccessTypes;
      }

      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        SecurableClassDefinition actualClassDefinition = SecurableClassDefinition.GetObject (expectedClassDefinition.ID);

        Assert.AreEqual (10, actualClassDefinition.AccessTypes.Count);
        for (int i = 0; i < 10; i++)
          Assert.AreEqual (expectedAccessTypes[i].ID, actualClassDefinition.AccessTypes[i].ID);
      }
    }

    [Test]
    public void Get_AccessControlListsFromDatabase ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();

      SecurableClassDefinition expectedClassDefinition;
      ObjectList<StatefulAccessControlList> expectedAcls;
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        expectedClassDefinition =
            dbFixtures.CreateAndCommitSecurableClassDefinitionWithAccessControlLists (10, ClientTransactionScope.CurrentTransaction);
        expectedAcls = expectedClassDefinition.StatefulAccessControlLists;
      }

      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        SecurableClassDefinition actualClassDefinition = SecurableClassDefinition.GetObject (expectedClassDefinition.ID);

        Assert.AreEqual (9, actualClassDefinition.StatefulAccessControlLists.Count);
        for (int i = 0; i < 9; i++)
          Assert.AreEqual (expectedAcls[i].ID, actualClassDefinition.StatefulAccessControlLists[i].ID);
      }
    }

    [Test]
    public void GetChangedAt_AfterCreation ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject ();

        Assert.AreEqual (StateType.New, classDefinition.State);
      }
    }

    [Test]
    public void Touch_AfterCreation ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject ();

        Assert.AreEqual (StateType.New, classDefinition.State);

        classDefinition.Touch ();

        Assert.AreEqual (StateType.New, classDefinition.State);
      }
    }

    [Test]
    public void Validate_Valid ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
        testHelper.CreateOrderStateAndPaymentStateCombinations (orderClass);

        SecurableClassValidationResult result = orderClass.Validate ();

        Assert.IsTrue (result.IsValid);
      }
    }

    [Test]
    public void Validate_DoubleStateCombination ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
        List<StateCombination> stateCombinations = testHelper.CreateOrderStateAndPaymentStateCombinations (orderClass);
        StatePropertyDefinition orderStateProperty = stateCombinations[0].StateUsages[0].StateDefinition.StateProperty;
        StatePropertyDefinition paymentProperty = stateCombinations[0].StateUsages[1].StateDefinition.StateProperty;
        testHelper.CreateStateCombination (
            orderClass, orderStateProperty[new EnumWrapper (OrderState.Received).Name], paymentProperty[new EnumWrapper (PaymentState.Paid).Name]);

        SecurableClassValidationResult result = orderClass.Validate ();

        Assert.IsFalse (result.IsValid);
      }
    }

    [Test]
    public void ValidateUniqueStateCombinations_NoStateCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();

        SecurableClassValidationResult result = new SecurableClassValidationResult ();
        orderClass.ValidateUniqueStateCombinations (result);

        Assert.IsTrue (result.IsValid);
      }
    }

    [Test]
    public void ValidateUniqueStateCombinations_TwoEmptyStateCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
        StateCombination statelessCombination1 = testHelper.CreateStateCombination (orderClass);
        StateCombination statelessCombination2 = testHelper.CreateStateCombination (orderClass);

        SecurableClassValidationResult result = new SecurableClassValidationResult ();
        orderClass.ValidateUniqueStateCombinations (result);

        Assert.IsFalse (result.IsValid);
        Assert.Contains (statelessCombination1, result.DuplicateStateCombinations);
        Assert.Contains (statelessCombination2, result.DuplicateStateCombinations);
      }
    }

    [Test]
    public void ValidateUniqueStateCombinations_TwoStateCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
        StatePropertyDefinition paymentProperty = testHelper.CreatePaymentStateProperty (orderClass);
        StateCombination paidCombination1 = testHelper.CreateStateCombination (orderClass, paymentProperty[new EnumWrapper (PaymentState.Paid).Name]);
        StateCombination paidCombination2 = testHelper.CreateStateCombination (orderClass, paymentProperty[new EnumWrapper (PaymentState.Paid).Name]);
        testHelper.CreateStateCombination (orderClass, paymentProperty[new EnumWrapper (PaymentState.None).Name]);

        SecurableClassValidationResult result = new SecurableClassValidationResult ();
        orderClass.ValidateUniqueStateCombinations (result);

        Assert.IsFalse (result.IsValid);
        Assert.AreEqual (2, result.DuplicateStateCombinations.Count);
        Assert.Contains (paidCombination1, result.DuplicateStateCombinations);
        Assert.Contains (paidCombination2, result.DuplicateStateCombinations);
      }
    }

    [Test]
    [ExpectedException (typeof (ConstraintViolationException), ExpectedMessage =
        "The securable class definition 'Remotion.SecurityManager.UnitTests.TestDomain.Order' contains at least one state combination "
        + "that has been defined twice.")]
    public void Commit_TwoStateCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
        StatePropertyDefinition paymentProperty = testHelper.CreatePaymentStateProperty (orderClass);
        testHelper.CreateStateCombination (orderClass, paymentProperty[new EnumWrapper (PaymentState.Paid).Name]);
        testHelper.CreateStateCombination (orderClass, paymentProperty[new EnumWrapper (PaymentState.Paid).Name]);
        testHelper.CreateStateCombination (orderClass, paymentProperty[new EnumWrapper (PaymentState.None).Name]);

        testHelper.Transaction.Commit ();
      }
    }

    [Test]
    public void ValidateUniqueStateCombinations_DoubleStateCombinationAndObjectIsDeleted ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
        List<StateCombination> stateCombinations = testHelper.CreateOrderStateAndPaymentStateCombinations (orderClass);
        StatePropertyDefinition orderStateProperty = stateCombinations[0].StateUsages[0].StateDefinition.StateProperty;
        StatePropertyDefinition paymentProperty = stateCombinations[0].StateUsages[1].StateDefinition.StateProperty;

        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          testHelper.CreateStateCombination (
              orderClass,
              ClientTransaction.Current,
              orderStateProperty[new EnumWrapper (OrderState.Received).Name],
              paymentProperty[new EnumWrapper (PaymentState.Paid).Name]);
          Assert.That (orderClass.StateCombinations, Is.Not.Empty);
          orderClass.Delete ();

          SecurableClassValidationResult result = new SecurableClassValidationResult ();
          orderClass.ValidateUniqueStateCombinations (result);

          Assert.IsTrue (result.IsValid);
          Assert.AreEqual (StateType.Deleted, orderClass.State);
        }
      }
    }

    [Test]
    public void ValidateStateCombinationsAgainstStateProperties_EmptyStateCombinationInClassWithStateProperties ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
        testHelper.CreateOrderStateAndPaymentStateCombinations (orderClass);
        StateCombination statelessCombination = testHelper.CreateStateCombination (orderClass);

        SecurableClassValidationResult result = new SecurableClassValidationResult ();
        orderClass.ValidateStateCombinationsAgainstStateProperties (result);

        Assert.IsFalse (result.IsValid);
        Assert.That (result.InvalidStateCombinations, Is.EquivalentTo (new[] { statelessCombination }));
      }
    }

    [Test]
    [ExpectedException (typeof (ConstraintViolationException), ExpectedMessage =
        "The securable class definition 'Remotion.SecurityManager.UnitTests.TestDomain.Order' contains at least one state combination "
        + "that does not match the class's properties.")]
    public void Commit_EmptyStateCombinationInClassWithStateProperties ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
        testHelper.CreateOrderStateAndPaymentStateCombinations (orderClass);
        testHelper.CreateStateCombination (orderClass);

        testHelper.Transaction.Commit ();
      }
    }

    [Test]
    public void GetStatePropertyTest_ValidName ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
        List<StateCombination> stateCombinations = testHelper.CreateOrderStateAndPaymentStateCombinations (orderClass);
        StatePropertyDefinition orderStateProperty = stateCombinations[0].StateUsages[0].StateDefinition.StateProperty;
        StatePropertyDefinition paymentProperty = stateCombinations[0].StateUsages[1].StateDefinition.StateProperty;

        Assert.That (orderClass.GetStateProperty (orderStateProperty.Name), Is.EqualTo (orderStateProperty));
        Assert.That (orderClass.GetStateProperty (paymentProperty.Name), Is.EqualTo (paymentProperty));
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "A state property with the name 'Invalid' is not defined for the secureable class definition 'Remotion.SecurityManager.UnitTests.TestDomain.Order'."
        + "\r\nParameter name: propertyName")]
    public void GetStatePropertyTest_InvalidName ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();

        Assert.That (orderClass.GetStateProperty ("Invalid"), Is.Null);
      }
    }

    [Test]
    public void GetStateCombinations_TwoCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
        StateCombination stateCombination = testHelper.CreateStateCombination (orderClass);
        List<StateCombination> stateCombinations = testHelper.CreateOrderStateAndPaymentStateCombinations (orderClass);

        Assert.That (orderClass.StateCombinations, Is.EqualTo (ArrayUtility.Combine (stateCombination, stateCombinations.ToArray ())));
      }
    }
  }
}
