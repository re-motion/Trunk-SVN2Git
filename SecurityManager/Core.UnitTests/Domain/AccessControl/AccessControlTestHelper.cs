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
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  public class AccessControlTestHelper
  {
    public const int OrderClassPropertyCount = 2;

    private readonly ClientTransaction _transaction;
    private readonly OrganizationalStructureFactory _factory;

    public AccessControlTestHelper ()
    {
      _transaction = ClientTransaction.CreateRootTransaction ();
      _factory = new OrganizationalStructureFactory ();
    }

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public SecurableClassDefinition CreateOrderClassDefinition ()
    {
      return CreateClassDefinition ("Remotion.SecurityManager.UnitTests.TestDomain.Order");
    }

    public SecurableClassDefinition CreateSpecialOrderClassDefinition (SecurableClassDefinition orderClassDefinition)
    {
      return CreateClassDefinition ("Remotion.SecurityManager.UnitTests.TestDomain.SpecialOrder", orderClassDefinition);
    }

    public SecurableClassDefinition CreatePremiumOrderClassDefinition (SecurableClassDefinition orderClassDefinition)
    {
      return CreateClassDefinition ("Remotion.SecurityManager.UnitTests.TestDomain.PremiumOrder", orderClassDefinition);
    }

    public SecurableClassDefinition CreateInvoiceClassDefinition ()
    {
      return CreateClassDefinition ("Remotion.SecurityManager.UnitTests.TestDomain.Invoice");
    }

    public SecurableClassDefinition CreateClassDefinition (string name)
    {
      return CreateClassDefinition (name, null);
    }

    public SecurableClassDefinition CreateClassDefinition (string name, SecurableClassDefinition baseClass)
    {
      using (_transaction.EnterNonDiscardingScope ())
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject ();
        classDefinition.Name = name;
        classDefinition.BaseClass = baseClass;

        return classDefinition;
      }
    }

    public SecurableClassDefinition CreateOrderClassDefinitionWithProperties ()
    {
      SecurableClassDefinition classDefinition = CreateOrderClassDefinition ();
      CreateOrderStateProperty (classDefinition);
      CreatePaymentStateProperty (classDefinition);

      return classDefinition;
    }

    public AccessControlList CreateAcl (SecurableClassDefinition classDefinition, params StateDefinition[] states)
    {
      return CreateAcl (classDefinition, _transaction, states);
    }


    private AccessControlList CreateAcl (SecurableClassDefinition classDefinition, ClientTransaction transaction, params StateDefinition[] states)
    {
      using (transaction.EnterNonDiscardingScope ())
      {
        AccessControlList acl = AccessControlList.NewObject ();
        acl.Class = classDefinition;
        StateCombination stateCombination = CreateStateCombination (acl, transaction);

        foreach (StateDefinition state in states)
          stateCombination.AttachState (state);

        return acl;
      }
    }

    public StateCombination CreateStateCombination (AccessControlList acl)
    {
      return CreateStateCombination (acl, _transaction);
    }

    private StateCombination CreateStateCombination (AccessControlList acl, ClientTransaction transaction)
    {
      using (transaction.EnterNonDiscardingScope ())
      {
        StateCombination stateCombination = StateCombination.NewObject ();
        stateCombination.AccessControlList = acl;
        stateCombination.Class = acl.Class;

        return stateCombination;
      }
    }

    public StateCombination CreateStateCombination (SecurableClassDefinition classDefinition, params StateDefinition[] states)
    {
      return CreateStateCombination (classDefinition, _transaction, states);
    }

    public StateCombination CreateStateCombination (SecurableClassDefinition classDefinition, ClientTransaction transaction, params StateDefinition[] states)
    {
      using (transaction.EnterNonDiscardingScope())
      {
        AccessControlList acl = CreateAcl (classDefinition, transaction, states);
        return acl.StateCombinations[0];
      }    
    }

    public StatePropertyDefinition CreateStateProperty (string name)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        return StatePropertyDefinition.NewObject (Guid.NewGuid(), name);
      }
    }

    public StatePropertyDefinition CreateOrderStateProperty (SecurableClassDefinition classDefinition)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        StatePropertyDefinition orderStateProperty = CreateStateProperty ("State");
        orderStateProperty.AddState (new EnumWrapper (OrderState.Received).Name, 0);
        orderStateProperty.AddState (new EnumWrapper (OrderState.Delivered).Name, 1);
        classDefinition.AddStateProperty (orderStateProperty);

        return orderStateProperty;
      }
    }

    public StatePropertyDefinition CreatePaymentStateProperty (SecurableClassDefinition classDefinition)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        StatePropertyDefinition paymentStateProperty = CreateStateProperty ("Payment");
        paymentStateProperty.AddState (new EnumWrapper(PaymentState.None).Name, 0);
        paymentStateProperty.AddState (new EnumWrapper (PaymentState.Paid).Name, 1);
        classDefinition.AddStateProperty (paymentStateProperty);

        return paymentStateProperty;
      }
    }

    public StatePropertyDefinition CreateDeliveryStateProperty (SecurableClassDefinition classDefinition)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        StatePropertyDefinition deliveryStateProperty = CreateStateProperty ("Delivery");
        deliveryStateProperty.AddState (new EnumWrapper(Delivery.Dhl).Name, 0);
        deliveryStateProperty.AddState (new EnumWrapper (Delivery.Post).Name, 1);
        classDefinition.AddStateProperty (deliveryStateProperty);

        return deliveryStateProperty;
      }
    }

    public List<StateCombination> CreateStateCombinationsForOrder ()
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = CreateOrderClassDefinition();
        return CreateOrderStateAndPaymentStateCombinations (orderClass);
      }
    }

    public List<StateCombination> CreateOrderStateAndPaymentStateCombinations (SecurableClassDefinition classDefinition)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        StatePropertyDefinition orderState = CreateOrderStateProperty (classDefinition);
        StatePropertyDefinition paymentState = CreatePaymentStateProperty (classDefinition);

        List<StateCombination> stateCombinations = new List<StateCombination>();
        stateCombinations.Add (CreateStateCombination (classDefinition, orderState[new EnumWrapper (OrderState.Received).Name], paymentState[new EnumWrapper(PaymentState.None).Name]));
        stateCombinations.Add (CreateStateCombination (classDefinition, orderState[new EnumWrapper (OrderState.Received).Name], paymentState[new EnumWrapper (PaymentState.Paid).Name]));
        stateCombinations.Add (CreateStateCombination (classDefinition, orderState[new EnumWrapper (OrderState.Delivered).Name], paymentState[new EnumWrapper(PaymentState.None).Name]));
        stateCombinations.Add (CreateStateCombination (classDefinition, orderState[new EnumWrapper (OrderState.Delivered).Name], paymentState[new EnumWrapper (PaymentState.Paid).Name]));
        stateCombinations.Add (CreateStateCombination (classDefinition));

        return stateCombinations;
      }
    }

    public StateCombination GetStateCombinationForDeliveredAndUnpaidOrder ()
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        List<StateCombination> stateCombinations = CreateStateCombinationsForOrder();
        return stateCombinations[2];
      }
    }

    public StateCombination GetStateCombinationWithoutStates ()
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        List<StateCombination> stateCombinations = CreateStateCombinationsForOrder();
        return stateCombinations[4];
      }
    }

    public List<AccessControlList> CreateAclsForOrderAndPaymentStates (SecurableClassDefinition classDefinition)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        StatePropertyDefinition orderState = CreateOrderStateProperty (classDefinition);
        StatePropertyDefinition paymentState = CreatePaymentStateProperty (classDefinition);

        List<AccessControlList> acls = new List<AccessControlList>();
        acls.Add (CreateAcl (classDefinition, orderState[new EnumWrapper (OrderState.Received).Name], paymentState[new EnumWrapper(PaymentState.None).Name]));
        acls.Add (CreateAcl (classDefinition, orderState[new EnumWrapper (OrderState.Received).Name], paymentState[new EnumWrapper (PaymentState.Paid).Name]));
        acls.Add (CreateAcl (classDefinition, orderState[new EnumWrapper (OrderState.Delivered).Name], paymentState[new EnumWrapper(PaymentState.None).Name]));
        acls.Add (CreateAcl (classDefinition, orderState[new EnumWrapper (OrderState.Delivered).Name], paymentState[new EnumWrapper (PaymentState.Paid).Name]));
        acls.Add (CreateAcl (classDefinition));

        return acls;
      }
    }

    public List<AccessControlList> CreateAclsForOrderAndPaymentAndDeliveryStates (SecurableClassDefinition classDefinition)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        StatePropertyDefinition orderState = CreateOrderStateProperty (classDefinition);
        StatePropertyDefinition paymentState = CreatePaymentStateProperty (classDefinition);
        StatePropertyDefinition deliveryState = CreateDeliveryStateProperty (classDefinition);

        List<AccessControlList> acls = new List<AccessControlList>();
        acls.Add (CreateAcl (classDefinition, orderState[new EnumWrapper (OrderState.Received).Name], paymentState[new EnumWrapper(PaymentState.None).Name], deliveryState[new EnumWrapper(Delivery.Dhl).Name]));
        acls.Add (CreateAcl (classDefinition, orderState[new EnumWrapper (OrderState.Received).Name], paymentState[new EnumWrapper (PaymentState.Paid).Name], deliveryState[new EnumWrapper(Delivery.Dhl).Name]));
        acls.Add (CreateAcl (classDefinition, orderState[new EnumWrapper (OrderState.Delivered).Name], paymentState[new EnumWrapper(PaymentState.None).Name], deliveryState[new EnumWrapper(Delivery.Dhl).Name]));
        acls.Add (CreateAcl (classDefinition, orderState[new EnumWrapper (OrderState.Delivered).Name], paymentState[new EnumWrapper (PaymentState.Paid).Name], deliveryState[new EnumWrapper(Delivery.Dhl).Name]));
        acls.Add (CreateAcl (classDefinition, orderState[new EnumWrapper (OrderState.Received).Name], paymentState[new EnumWrapper(PaymentState.None).Name], deliveryState[new EnumWrapper (Delivery.Post).Name]));
        acls.Add (CreateAcl (classDefinition, orderState[new EnumWrapper (OrderState.Received).Name], paymentState[new EnumWrapper (PaymentState.Paid).Name], deliveryState[new EnumWrapper (Delivery.Post).Name]));
        acls.Add (CreateAcl (classDefinition, orderState[new EnumWrapper (OrderState.Delivered).Name], paymentState[new EnumWrapper(PaymentState.None).Name], deliveryState[new EnumWrapper (Delivery.Post).Name]));
        acls.Add (CreateAcl (classDefinition, orderState[new EnumWrapper (OrderState.Delivered).Name], paymentState[new EnumWrapper (PaymentState.Paid).Name], deliveryState[new EnumWrapper (Delivery.Post).Name]));
        acls.Add (CreateAcl (classDefinition));

        return acls;
      }
    }

    public AccessControlList GetAclForDeliveredAndUnpaidAndDhlStates (SecurableClassDefinition classDefinition)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        List<AccessControlList> acls = CreateAclsForOrderAndPaymentAndDeliveryStates (classDefinition);
        return acls[2];
      }
    }

    public AccessControlList GetAclForDeliveredAndUnpaidStates (SecurableClassDefinition classDefinition)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        List<AccessControlList> acls = CreateAclsForOrderAndPaymentStates (classDefinition);
        return acls[2];
      }
    }

    public AccessControlList GetAclForStateless (SecurableClassDefinition classDefinition)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        List<AccessControlList> acls = CreateAclsForOrderAndPaymentStates (classDefinition);
        return acls[4];
      }
    }

    public List<StateDefinition> GetDeliveredAndUnpaidStateList (SecurableClassDefinition classDefinition)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        List<StateDefinition> states = new List<StateDefinition>();

        foreach (StatePropertyDefinition property in classDefinition.StateProperties)
        {
          if (property.Name == "State")
            states.Add (property[new EnumWrapper (OrderState.Delivered).Name]);

          if (property.Name == "Payment")
            states.Add (property[new EnumWrapper(PaymentState.None).Name]);
        }

        return states;
      }
    }

    public StatePropertyDefinition CreateTestProperty ()
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        StatePropertyDefinition property = CreateStateProperty ("Test");
        property.AddState ("Test1", 0);
        property.AddState ("Test2", 1);

        return property;
      }
    }

    public AccessTypeDefinition AttachAccessType (SecurableClassDefinition classDefinition, Guid metadataItemID, string name, int value)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        AccessTypeDefinition accessType = AccessTypeDefinition.NewObject (metadataItemID, name, value);
        classDefinition.AddAccessType (accessType);

        return accessType;
      }
    }

    public AccessTypeDefinition AttachJournalizeAccessType (SecurableClassDefinition classDefinition)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        AccessTypeDefinition accessType = CreateJournalizeAccessType();
        classDefinition.AddAccessType (accessType);

        return accessType;
      }
    }

    public AccessTypeDefinition CreateJournalizeAccessType ()
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        return AccessTypeDefinition.NewObject (Guid.NewGuid(), "Journalize", 42);
      }
    }

    public SecurityToken CreateEmptyToken ()
    {
      return CreateToken (null, null, null, null);
    }

    public SecurityToken CreateTokenWithOwningTenant (User user, Tenant owningTenant)
    {
      return CreateToken (user, owningTenant, null, null);
    }

    public SecurityToken CreateTokenWithAbstractRole (params AbstractRoleDefinition[] roleDefinitions)
    {
      return CreateToken (null, null, null, roleDefinitions);
    }

    public SecurityToken CreateTokenWithOwningGroups (User user, params Group[] owningGroups)
    {
      return CreateToken (user, null, owningGroups, null);
    }

    public SecurityToken CreateToken (User user, Tenant owningTenant, Group[] owningGroups, AbstractRoleDefinition[] abstractRoleDefinitions)
    {
      List<Group> owningGroupList = new List<Group> ();
      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();

      if (owningGroups != null)
        owningGroupList.AddRange (owningGroups);

      if (abstractRoleDefinitions != null)
        abstractRoles.AddRange (abstractRoleDefinitions);

      return new SecurityToken (user, owningTenant, owningGroupList, abstractRoles);
    }

    public AbstractRoleDefinition CreateTestAbstractRole ()
    {
      return CreateAbstractRoleDefinition ("Test", 42);
    }

    public AbstractRoleDefinition CreateAbstractRoleDefinition (string name, int value)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        return AbstractRoleDefinition.NewObject (Guid.NewGuid(), name, value);
      }
    }

    public AccessControlEntry CreateAceWithOwningTenant ()
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        AccessControlEntry entry = AccessControlEntry.NewObject ();
        entry.TenantSelection = TenantSelection.OwningTenant;

        return entry;
      }
    }

    public AccessControlEntry CreateAceWithSpecficTenant (Tenant tenant)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        AccessControlEntry entry = AccessControlEntry.NewObject ();
        entry.TenantSelection = TenantSelection.SpecificTenant;
        entry.SpecificTenant = tenant;

        return entry;
      }
    }

    public AccessControlEntry CreateAceWithAbstractRole ()
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        AccessControlEntry entry = AccessControlEntry.NewObject ();
        entry.SpecificAbstractRole = CreateTestAbstractRole();

        return entry;
      }
    }

    public AccessControlEntry CreateAceWithPosition (Position position, GroupSelection groupSelection)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        AccessControlEntry entry = AccessControlEntry.NewObject ();
        entry.UserSelection = UserSelection.SpecificPosition;
        entry.SpecificPosition = position;
        entry.GroupSelection = groupSelection;

        return entry;
      }
    }

    public AccessControlList CreateAcl (params AccessControlEntry[] aces)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        AccessControlList acl = AccessControlList.NewObject ();

        foreach (AccessControlEntry ace in aces)
          acl.AccessControlEntries.Add (ace);

        return acl;
      }
    }

    public void AttachAccessType (AccessControlEntry ace, AccessTypeDefinition accessType, bool? allowed)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        ace.AttachAccessType (accessType);
        if (allowed.HasValue && allowed.Value)
          ace.AllowAccess (accessType);
      }
    }

    public AccessTypeDefinition CreateReadAccessType (AccessControlEntry ace, bool? allowAccess)
    {
      return CreateAccessTypeForAce (ace, allowAccess, Guid.NewGuid (), "Read", 0);
    }

    public AccessTypeDefinition CreateWriteAccessType (AccessControlEntry ace, bool? allowAccess)
    {
      return CreateAccessTypeForAce (ace, allowAccess, Guid.NewGuid (), "Write", 1);
    }

    public AccessTypeDefinition CreateDeleteAccessType (AccessControlEntry ace, bool? allowAccess)
    {
      return CreateAccessTypeForAce (ace, allowAccess, Guid.NewGuid (), "Delete", 2);
    }

    public AccessTypeDefinition CreateAccessTypeForAce (AccessControlEntry ace, bool? allowAccess, Guid metadataItemID, string name, int value)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        AccessTypeDefinition accessType = AccessTypeDefinition.NewObject (metadataItemID, name, value);
        AttachAccessType (ace, accessType, allowAccess);

        return accessType;
      }
    }

    public Tenant CreateTenant (string name)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        Tenant tenant = _factory.CreateTenant ();
        tenant.Name = name;

        return tenant;
      }
    }

    public Group CreateGroup (string name, Group parent, Tenant tenant)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        Group group = _factory.CreateGroup ();
        group.Name = name;
        group.Parent = parent;
        group.Tenant = tenant;

        return group;
      }
    }

    public User CreateUser (string userName, string firstName, string lastName, string title, Group owningGroup, Tenant tenant)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        User user = _factory.CreateUser ();
        user.UserName = userName;
        user.FirstName = firstName;
        user.LastName = lastName;
        user.Title = title;
        user.Tenant = tenant;
        user.OwningGroup = owningGroup;

        return user;
      }
    }

    public Position CreatePosition (string name)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        Position position = _factory.CreatePosition ();
        position.Name = name;

        return position;
      }
    }

    public Role CreateRole (User user, Group group, Position position)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        Role role = Role.NewObject ();
        role.User = user;
        role.Group = group;
        role.Position = position;

        return role;
      }
    }
  }
}
