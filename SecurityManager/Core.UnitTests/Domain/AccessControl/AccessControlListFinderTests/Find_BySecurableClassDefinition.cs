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
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlListFinderTests
{
  [TestFixture]
  public class Find_BySecurableClassDefinition : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void Succeed_WithoutStateProperties ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      AccessControlList acl = _testHelper.CreateAcl (classDefinition);
      SecurityContext context = CreateStatelessContext();

      AccessControlListFinder aclFinder = new AccessControlListFinder();
      AccessControlList foundAcl = aclFinder.Find (ClientTransactionScope.CurrentTransaction, classDefinition, context);

      Assert.AreSame (acl, foundAcl);
    }

    [Test]
    public void Succeed_WithStates ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      AccessControlList acl = _testHelper.GetAclForDeliveredAndUnpaidStates (classDefinition);
      SecurityContext context = CreateContextForDeliveredAndUnpaidOrder();

      AccessControlListFinder aclFinder = new AccessControlListFinder();
      AccessControlList foundAcl = aclFinder.Find (ClientTransactionScope.CurrentTransaction, classDefinition, context);

      Assert.AreSame (acl, foundAcl);
    }

    [Test]
    public void Succeed_WithStatesAndStateless ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      AccessControlList acl = _testHelper.GetAclForStateless (classDefinition);
      SecurityContext context = CreateStatelessContext();

      AccessControlListFinder aclFinder = new AccessControlListFinder();
      AccessControlList foundAcl = aclFinder.Find (ClientTransactionScope.CurrentTransaction, classDefinition, context);

      Assert.AreSame (acl, foundAcl);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "The state 'Payment' is missing in the security context.")]
    public void Fail_WithSecurityContextDoesNotContainAllStates ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      _testHelper.CreateAclsForOrderAndPaymentStates (classDefinition);
      SecurityContext context = CreateContextWithoutPaymentState();

      AccessControlListFinder aclFinder = new AccessControlListFinder();
      aclFinder.Find (ClientTransactionScope.CurrentTransaction, classDefinition, context);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage =
        "The state 'None|Remotion.SecurityManager.UnitTests.TestDomain.PaymentState, Remotion.SecurityManager.UnitTests' is not defined for the "
        + "property 'State' of the securable class 'Remotion.SecurityManager.UnitTests.TestDomain.Order, Remotion.SecurityManager.UnitTests' "
        + "or its base classes.")]
    public void Fail_WithSecurityContextContainsStateWithInvalidValue ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      _testHelper.CreateAclsForOrderAndPaymentStates (classDefinition);
      Dictionary<string, Enum> states = new Dictionary<string, Enum>();
      states.Add ("State", PaymentState.None);
      SecurityContext context = SecurityContext.Create (typeof (Order), "owner", "ownerGroup", "ownerTenant", states, new Enum[0]);

      AccessControlListFinder aclFinder = new AccessControlListFinder();
      aclFinder.Find (ClientTransactionScope.CurrentTransaction, classDefinition, context);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage =
        "The ACL for the securable class 'Remotion.SecurityManager.UnitTests.TestDomain.Order, Remotion.SecurityManager.UnitTests' "
        + "could not be found.")]
    public void Fail_WithSecurityContextContainsInvalidState ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      _testHelper.CreateAclsForOrderAndPaymentStates (classDefinition);
      Dictionary<string, Enum> states = new Dictionary<string, Enum>();
      states.Add ("State", OrderState.Delivered);
      states.Add ("Payment", PaymentState.None);
      states.Add ("New", PaymentState.None);
      SecurityContext context = SecurityContext.Create (typeof (Order), "owner", "ownerGroup", "ownerTenant", states, new Enum[0]);

      AccessControlListFinder aclFinder = new AccessControlListFinder();
      AccessControlList acl = aclFinder.Find (ClientTransactionScope.CurrentTransaction, classDefinition, context);

      Assert.IsNull (acl);
    }

    [Test]
    public void Succeed_WithDerivedClassDefinitionAndPropertiesInBaseClass ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      SecurableClassDefinition specialOrderClass = _testHelper.CreateSpecialOrderClassDefinition (orderClass);
      AccessControlList acl = _testHelper.GetAclForDeliveredAndUnpaidStates (orderClass);
      SecurityContext context = CreateContextForDeliveredAndUnpaidOrder (typeof (SpecialOrder));

      AccessControlListFinder aclFinder = new AccessControlListFinder();
      AccessControlList foundAcl = aclFinder.Find (ClientTransactionScope.CurrentTransaction, specialOrderClass, context);

      Assert.AreSame (acl, foundAcl);
    }

    [Test]
    public void Succeed_WithDerivedClassDefinitionAndSameProperties ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      SecurableClassDefinition specialOrderClass = _testHelper.CreateSpecialOrderClassDefinition (orderClass);
      AccessControlList aclForOrder = _testHelper.GetAclForDeliveredAndUnpaidStates (orderClass);
      AccessControlList aclForSpecialOrder = _testHelper.GetAclForDeliveredAndUnpaidStates (specialOrderClass);
      SecurityContext context = CreateContextForDeliveredAndUnpaidOrder (typeof (SpecialOrder));

      AccessControlListFinder aclFinder = new AccessControlListFinder();
      AccessControlList foundAcl = aclFinder.Find (ClientTransactionScope.CurrentTransaction, specialOrderClass, context);

      Assert.AreSame (aclForSpecialOrder, foundAcl);
    }

    [Test]
    public void Succeed_WithDerivedClassDefinitionAndAdditionalProperty ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      SecurableClassDefinition premiumOrderClass = _testHelper.CreatePremiumOrderClassDefinition (orderClass);
      AccessControlList aclForOrder = _testHelper.GetAclForDeliveredAndUnpaidStates (orderClass);
      AccessControlList aclForPremiumOrder = _testHelper.GetAclForDeliveredAndUnpaidAndDhlStates (premiumOrderClass);
      SecurityContext context = CreateContextForDeliveredAndUnpaidAndDhlOrder (typeof (SpecialOrder));

      AccessControlListFinder aclFinder = new AccessControlListFinder();
      AccessControlList foundAcl = aclFinder.Find (ClientTransactionScope.CurrentTransaction, premiumOrderClass, context);

      Assert.AreSame (aclForPremiumOrder, foundAcl);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "The state 'Delivery' is missing in the security context.")]
    public void Fail_WithDerivedClassDefinitionAndMissingStatePropertyInSecurityContext ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      SecurableClassDefinition premiumOrderClass = _testHelper.CreatePremiumOrderClassDefinition (orderClass);
      AccessControlList aclForOrder = _testHelper.GetAclForDeliveredAndUnpaidStates (orderClass);
      AccessControlList aclForPremiumOrder = _testHelper.GetAclForDeliveredAndUnpaidAndDhlStates (premiumOrderClass);
      SecurityContext context = CreateContextForDeliveredAndUnpaidOrder (typeof (SpecialOrder));

      AccessControlListFinder aclFinder = new AccessControlListFinder();
      aclFinder.Find (ClientTransactionScope.CurrentTransaction, premiumOrderClass, context);
    }

    private SecurityContext CreateStatelessContext ()
    {
      return SecurityContext.Create (typeof (Order), "owner", "ownerGroup", "ownerTenant", new Dictionary<string, Enum>(), new Enum[0]);
    }

    private SecurityContext CreateContextForDeliveredAndUnpaidOrder ()
    {
      return CreateContextForDeliveredAndUnpaidOrder (typeof (Order));
    }

    private SecurityContext CreateContextForDeliveredAndUnpaidOrder (Type type)
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum>();
      states.Add ("State", OrderState.Delivered);
      states.Add ("Payment", PaymentState.None);

      return SecurityContext.Create (type, "owner", "ownerGroup", "ownerTenant", states, new Enum[0]);
    }

    private SecurityContext CreateContextForDeliveredAndUnpaidAndDhlOrder (Type type)
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum>();
      states.Add ("State", OrderState.Delivered);
      states.Add ("Payment", PaymentState.None);
      states.Add ("Delivery", Delivery.Dhl);

      return SecurityContext.Create (type, "owner", "ownerGroup", "ownerTenant", states, new Enum[0]);
    }

    private SecurityContext CreateContextWithoutPaymentState ()
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum>();
      states.Add ("State", OrderState.Delivered);

      return SecurityContext.Create (typeof (Order), "owner", "ownerGroup", "ownerTenant", states, new Enum[0]);
    }
  }
}