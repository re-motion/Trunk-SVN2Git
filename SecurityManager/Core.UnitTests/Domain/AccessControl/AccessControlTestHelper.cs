// This file is part of re-strict (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.TestDomain;
using Remotion.Utilities;

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

    public StatelessAccessControlList CreateStatelessAcl (SecurableClassDefinition classDefinition)
    {
      return CreateStatelessAcl (classDefinition, _transaction);
    }

    public StatelessAccessControlList CreateStatelessAcl (SecurableClassDefinition classDefinition, ClientTransaction transaction)
    {
      using (transaction.EnterNonDiscardingScope ())
      {
        var acl = StatelessAccessControlList.NewObject ();
        acl.Class = classDefinition;
        return acl;
      }
    }

    public StatefulAccessControlList CreateStatefulAcl (SecurableClassDefinition classDefinition, params StateDefinition[] states)
    {
      return CreateStatefulAcl (classDefinition, _transaction, states);
    }

    private StatefulAccessControlList CreateStatefulAcl (SecurableClassDefinition classDefinition, ClientTransaction transaction, params StateDefinition[] states)
    {
      using (transaction.EnterNonDiscardingScope ())
      {
        var acl = StatefulAccessControlList.NewObject ();
        acl.Class = classDefinition;
        StateCombination stateCombination = CreateStateCombination (acl, transaction);

        foreach (StateDefinition state in states)
          stateCombination.AttachState (state);

        return acl;
      }
    }

    public StateCombination CreateStateCombination (StatefulAccessControlList acl)
    {
      return CreateStateCombination (acl, _transaction);
    }

    private StateCombination CreateStateCombination (StatefulAccessControlList acl, ClientTransaction transaction)
    {
      using (transaction.EnterNonDiscardingScope ())
      {
        StateCombination stateCombination = StateCombination.NewObject ();
        stateCombination.AccessControlList = acl;

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
        StatefulAccessControlList acl = CreateStatefulAcl (classDefinition, transaction, states);
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
        orderStateProperty.AddState (EnumWrapper.Get (OrderState.Received).Name, 0);
        orderStateProperty.AddState (EnumWrapper.Get (OrderState.Delivered).Name, 1);
        classDefinition.AddStateProperty (orderStateProperty);

        return orderStateProperty;
      }
    }

    public StatePropertyDefinition CreatePaymentStateProperty (SecurableClassDefinition classDefinition)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        StatePropertyDefinition paymentStateProperty = CreateStateProperty ("Payment");
        paymentStateProperty.AddState (EnumWrapper.Get(PaymentState.None).Name, 0);
        paymentStateProperty.AddState (EnumWrapper.Get (PaymentState.Paid).Name, 1);
        classDefinition.AddStateProperty (paymentStateProperty);

        return paymentStateProperty;
      }
    }

    public StatePropertyDefinition CreateDeliveryStateProperty (SecurableClassDefinition classDefinition)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        StatePropertyDefinition deliveryStateProperty = CreateStateProperty ("Delivery");
        deliveryStateProperty.AddState (EnumWrapper.Get(Delivery.Dhl).Name, 0);
        deliveryStateProperty.AddState (EnumWrapper.Get (Delivery.Post).Name, 1);
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
        stateCombinations.Add (CreateStateCombination (classDefinition, orderState[EnumWrapper.Get (OrderState.Received).Name], paymentState[EnumWrapper.Get(PaymentState.None).Name]));
        stateCombinations.Add (CreateStateCombination (classDefinition, orderState[EnumWrapper.Get (OrderState.Received).Name], paymentState[EnumWrapper.Get (PaymentState.Paid).Name]));
        stateCombinations.Add (CreateStateCombination (classDefinition, orderState[EnumWrapper.Get (OrderState.Delivered).Name], paymentState[EnumWrapper.Get(PaymentState.None).Name]));
        stateCombinations.Add (CreateStateCombination (classDefinition, orderState[EnumWrapper.Get (OrderState.Delivered).Name], paymentState[EnumWrapper.Get (PaymentState.Paid).Name]));

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

    public List<AccessControlList> CreateAclsForOrderAndPaymentStates (SecurableClassDefinition classDefinition)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        StatePropertyDefinition orderState = CreateOrderStateProperty (classDefinition);
        StatePropertyDefinition paymentState = CreatePaymentStateProperty (classDefinition);

        List<AccessControlList> acls = new List<AccessControlList>();
        acls.Add (CreateStatefulAcl (classDefinition, orderState[EnumWrapper.Get (OrderState.Received).Name], paymentState[EnumWrapper.Get(PaymentState.None).Name]));
        acls.Add (CreateStatefulAcl (classDefinition, orderState[EnumWrapper.Get (OrderState.Received).Name], paymentState[EnumWrapper.Get (PaymentState.Paid).Name]));
        acls.Add (CreateStatefulAcl (classDefinition, orderState[EnumWrapper.Get (OrderState.Delivered).Name], paymentState[EnumWrapper.Get(PaymentState.None).Name]));
        acls.Add (CreateStatefulAcl (classDefinition, orderState[EnumWrapper.Get (OrderState.Delivered).Name], paymentState[EnumWrapper.Get (PaymentState.Paid).Name]));
        acls.Add (CreateStatelessAcl (classDefinition));

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
        acls.Add (CreateStatefulAcl (classDefinition, orderState[EnumWrapper.Get (OrderState.Received).Name], paymentState[EnumWrapper.Get(PaymentState.None).Name], deliveryState[EnumWrapper.Get(Delivery.Dhl).Name]));
        acls.Add (CreateStatefulAcl (classDefinition, orderState[EnumWrapper.Get (OrderState.Received).Name], paymentState[EnumWrapper.Get (PaymentState.Paid).Name], deliveryState[EnumWrapper.Get(Delivery.Dhl).Name]));
        acls.Add (CreateStatefulAcl (classDefinition, orderState[EnumWrapper.Get (OrderState.Delivered).Name], paymentState[EnumWrapper.Get(PaymentState.None).Name], deliveryState[EnumWrapper.Get(Delivery.Dhl).Name]));
        acls.Add (CreateStatefulAcl (classDefinition, orderState[EnumWrapper.Get (OrderState.Delivered).Name], paymentState[EnumWrapper.Get (PaymentState.Paid).Name], deliveryState[EnumWrapper.Get(Delivery.Dhl).Name]));
        acls.Add (CreateStatefulAcl (classDefinition, orderState[EnumWrapper.Get (OrderState.Received).Name], paymentState[EnumWrapper.Get(PaymentState.None).Name], deliveryState[EnumWrapper.Get (Delivery.Post).Name]));
        acls.Add (CreateStatefulAcl (classDefinition, orderState[EnumWrapper.Get (OrderState.Received).Name], paymentState[EnumWrapper.Get (PaymentState.Paid).Name], deliveryState[EnumWrapper.Get (Delivery.Post).Name]));
        acls.Add (CreateStatefulAcl (classDefinition, orderState[EnumWrapper.Get (OrderState.Delivered).Name], paymentState[EnumWrapper.Get(PaymentState.None).Name], deliveryState[EnumWrapper.Get (Delivery.Post).Name]));
        acls.Add (CreateStatefulAcl (classDefinition, orderState[EnumWrapper.Get (OrderState.Delivered).Name], paymentState[EnumWrapper.Get (PaymentState.Paid).Name], deliveryState[EnumWrapper.Get (Delivery.Post).Name]));
        acls.Add (CreateStatelessAcl (classDefinition));

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

    public StatefulAccessControlList GetAclForDeliveredAndUnpaidStates (SecurableClassDefinition classDefinition)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        List<AccessControlList> acls = CreateAclsForOrderAndPaymentStates (classDefinition);
        return (StatefulAccessControlList) acls[2];
      }
    }

    public StatelessAccessControlList GetAclForStateless (SecurableClassDefinition classDefinition)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        List<AccessControlList> acls = CreateAclsForOrderAndPaymentStates (classDefinition);
        return (StatelessAccessControlList) acls[4];
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
            states.Add (property[EnumWrapper.Get (OrderState.Delivered).Name]);

          if (property.Name == "Payment")
            states.Add (property[EnumWrapper.Get(PaymentState.None).Name]);
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

    public SecurityToken CreateTokenWithNullPrincipal ()
    {
      return new SecurityToken (Principal.Null, null, null, null, new AbstractRoleDefinition[0]);
    }

    public SecurityToken CreateEmptyToken ()
    {
      Principal principal = new Principal (null, null, new Role[0]);
      return new SecurityToken (principal, null, null, null, new AbstractRoleDefinition[0]);
    }

    public SecurityToken CreateTokenWithOwningTenant (User principalUser, Tenant owningTenant)
    {
      ArgumentUtility.CheckNotNull ("principalUser", principalUser);
      return CreateToken (principalUser, owningTenant, null, null, null);
    }

    public SecurityToken CreateTokenWithAbstractRole (params AbstractRoleDefinition[] roleDefinitions)
    {
      Principal principal = new Principal (null, null, new Role[0]);
      return new SecurityToken (principal, null, null, null, (AbstractRoleDefinition[]) roleDefinitions.Clone());
    }

    public SecurityToken CreateTokenWithOwningGroup (User principalUser, Group owningGroup)
    {
      ArgumentUtility.CheckNotNull ("principalUser", principalUser);
      return CreateToken (principalUser, null, owningGroup, null, null);
    }

    public SecurityToken CreateTokenWithOwningUser (User principalUser, User owningUser)
    {
      ArgumentUtility.CheckNotNull ("principalUser", principalUser);
      return CreateToken (principalUser, null, null, owningUser, null);
    }

    public SecurityToken CreateToken (User principalUser, Tenant owningTenant, Group owningGroup, User owningUser, AbstractRoleDefinition[] abstractRoleDefinitions)
    {
      ArgumentUtility.CheckNotNull ("principalUser", principalUser);
      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();

      if (abstractRoleDefinitions != null)
        abstractRoles.AddRange (abstractRoleDefinitions);

      Principal principal = new Principal (principalUser.Tenant, principalUser, principalUser.Roles);
      return new SecurityToken (principal, owningTenant, owningGroup, owningUser, abstractRoles);
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


    public AccessControlEntry CreateAceWithNoMatchingRestrictions ()
    {
      using (_transaction.EnterNonDiscardingScope ())
      {
        AccessControlEntry entry = AccessControlEntry.NewObject ();
        return entry;
      }
    }


    public AccessControlEntry CreateAceWithOwningUser ()
    {
      using (_transaction.EnterNonDiscardingScope ())
      {
        AccessControlEntry entry = AccessControlEntry.NewObject ();
        entry.UserCondition = UserCondition.Owner;

        return entry;
      }
    }

    public AccessControlEntry CreateAceWithSpecificUser (User user)
    {
      using (_transaction.EnterNonDiscardingScope ())
      {
        AccessControlEntry entry = AccessControlEntry.NewObject ();
        entry.UserCondition = UserCondition.SpecificUser;
        entry.SpecificUser = user;

        return entry;
      }
    }

    public AccessControlEntry CreateAceWithOwningGroup ()
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        AccessControlEntry entry = AccessControlEntry.NewObject ();
        entry.GroupCondition = GroupCondition.OwningGroup;
        entry.GroupHierarchyCondition = GroupHierarchyCondition.This;

        return entry;
      }
    }

    public AccessControlEntry CreateAceWithSpecificGroup (Group group)
    {
      ArgumentUtility.CheckNotNull ("group", group);

      using (_transaction.EnterNonDiscardingScope ())
      {
        AccessControlEntry entry = AccessControlEntry.NewObject ();
        entry.GroupCondition = GroupCondition.SpecificGroup;
        entry.SpecificGroup = group;
        entry.GroupHierarchyCondition = GroupHierarchyCondition.This;

        return entry;
      }
    }

    public AccessControlEntry CreateAceWithBranchOfOwningGroup (GroupType groupType)
    {
      ArgumentUtility.CheckNotNull ("groupType", groupType);
      using (_transaction.EnterNonDiscardingScope ())
      {
        AccessControlEntry entry = AccessControlEntry.NewObject ();
        entry.GroupCondition = GroupCondition.BranchOfOwningGroup;
        entry.SpecificGroupType = groupType;

        return entry;
      }
    }

    public AccessControlEntry CreateAceWithSpecificGroupType (GroupType groupType)
    {
      ArgumentUtility.CheckNotNull ("groupType", groupType);
      using (_transaction.EnterNonDiscardingScope ())
      {
        AccessControlEntry entry = AccessControlEntry.NewObject ();
        entry.GroupCondition = GroupCondition.AnyGroupWithSpecificGroupType;
        entry.SpecificGroupType = groupType;

        return entry;
      }
    }

    public AccessControlEntry CreateAceWithoutGroupCondition ()
    {
      using (_transaction.EnterNonDiscardingScope ())
      {
        AccessControlEntry entry = AccessControlEntry.NewObject ();
        entry.GroupCondition = GroupCondition.None;

        return entry;
      }
    }


    public AccessControlEntry CreateAceWithOwningTenant ()
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        AccessControlEntry entry = AccessControlEntry.NewObject ();
        entry.TenantCondition = TenantCondition.OwningTenant;
        entry.TenantHierarchyCondition = TenantHierarchyCondition.This;

        return entry;
      }
    }

    public AccessControlEntry CreateAceWithSpecificTenant (Tenant tenant)
    {
      ArgumentUtility.CheckNotNull ("tenant", tenant);

      using (_transaction.EnterNonDiscardingScope())
      {
        AccessControlEntry entry = AccessControlEntry.NewObject ();
        entry.TenantCondition = TenantCondition.SpecificTenant;
        entry.SpecificTenant = tenant;
        entry.TenantHierarchyCondition = TenantHierarchyCondition.This;

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

    public AccessControlEntry CreateAceWithPosition (Position position)
    {
      using (_transaction.EnterNonDiscardingScope ())
      {
        AccessControlEntry entry = AccessControlEntry.NewObject ();
        entry.UserCondition = UserCondition.SpecificPosition;
        entry.SpecificPosition = position;
        return entry;
      }
    }

    public AccessControlEntry CreateAceWithPositionAndGroupCondition (Position position, GroupCondition groupCondition)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        AccessControlEntry entry = AccessControlEntry.NewObject ();
        entry.UserCondition = UserCondition.SpecificPosition;
        entry.SpecificPosition = position;
        entry.GroupCondition = groupCondition;
        if (groupCondition == GroupCondition.OwningGroup)
          entry.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent;

        return entry;
      }
    }

    public AccessControlList CreateStatefulAcl (params AccessControlEntry[] aces)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        AccessControlList acl = StatefulAccessControlList.NewObject ();

        foreach (AccessControlEntry ace in aces)
          acl.AccessControlEntries.Add (ace);

        return acl;
      }
    }

    public void AttachAccessType (AccessControlEntry ace, AccessTypeDefinition accessType, bool? allowAccess)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        ace.AttachAccessType (accessType);
        if (!allowAccess.HasValue)
          ace.RemoveAccess (accessType);
        else if (allowAccess.Value)
          ace.AllowAccess (accessType);
        else
          ace.DenyAccess(accessType);        
      }
    }

    public AccessTypeDefinition CreateReadAccessTypeAndAttachToAce (AccessControlEntry ace, bool? allowAccess)
    {
      using (_transaction.EnterNonDiscardingScope ())
      {
        AccessTypeDefinition accessType = CreateReadAccessType ();
        AttachAccessType (ace, accessType, allowAccess);

        return accessType;
      }
    }

    public AccessTypeDefinition CreateWriteAccessTypeAndAttachToAce (AccessControlEntry ace, bool? allowAccess)
    {
      using (_transaction.EnterNonDiscardingScope ())
      {
        AccessTypeDefinition accessType = CreateWriteAccessType ();
        AttachAccessType (ace, accessType, allowAccess);

        return accessType;
      }
    }

    public AccessTypeDefinition CreateDeleteAccessTypeAndAttachToAce (AccessControlEntry ace, bool? allowAccess)
    {
      using (_transaction.EnterNonDiscardingScope ())
      {
        AccessTypeDefinition accessType = CreateDeleteAccessType ();
        AttachAccessType (ace, accessType, allowAccess);

        return accessType;
      }
    }


    public AccessTypeDefinition CreateReadAccessType ()
    {
      return CreateAccessType ("Read", 0);
    }

    public AccessTypeDefinition CreateWriteAccessType ()
    {
      return CreateAccessType ("Write", 1);
    }

    public AccessTypeDefinition CreateDeleteAccessType ()
    {
      return CreateAccessType ("Delete", 2);
    }


    public AccessTypeDefinition CreateAccessType (Guid metadataItemID, string name, int value)
    {
      using (_transaction.EnterNonDiscardingScope ())
      {
        AccessTypeDefinition accessType = AccessTypeDefinition.NewObject (metadataItemID, name, value);
        return accessType;
      }
    }

    public AccessTypeDefinition CreateAccessType (string name, int value)
    {
      return CreateAccessType (Guid.NewGuid (), name, value);
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

    public Group CreateGroup (string name, Group parent, Tenant tenant, GroupType groupType)
    {
      using (_transaction.EnterNonDiscardingScope ())
      {
        Group group = _factory.CreateGroup ();
        group.Name = name;
        group.Parent = parent;
        group.Tenant = tenant;
        group.GroupType = groupType;

        return group;
      }
    }


    public GroupType CreateGroupType (string name)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        GroupType groupType = GroupType.NewObject ();
        groupType.Name = name;
        return groupType;
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


    public void AttachAces (AccessControlList acl, params AccessControlEntry[] aces)
    {
      foreach (AccessControlEntry ace in aces)
      {
        acl.AccessControlEntries.Add (ace);
      }
    }
  }
}
