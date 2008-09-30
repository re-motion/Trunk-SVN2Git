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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Diagnostics;
using Remotion.Security;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using System.Collections.Generic;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.TestDomain;


namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpanderTest : AclToolsTestBase
  {
    private readonly List<AccessControlList> _aclList = new List<AccessControlList>();
    private readonly List<User> _userList = new List<User> ();

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      //SecurableClassDefinition orderClass = TestHelper.CreateOrderClassDefinition ();
      //SecurableClassDefinition premiumOrderClass = TestHelper.CreatePremiumOrderClassDefinition (orderClass);
      //AccessControlList aclForOrder = TestHelper.GetAclForDeliveredAndUnpaidStates (orderClass);
      //AccessControlList aclForPremiumOrder = TestHelper.GetAclForDeliveredAndUnpaidAndDhlStates (premiumOrderClass);
      //SecurityContext context = CreateContextForDeliveredAndUnpaidAndDhlOrder (typeof (SpecialOrder));
      //AccessControlListFinder aclFinder = new AccessControlListFinder ();
      //AccessControlList foundAcl = aclFinder.Find (ClientTransactionScope.CurrentTransaction, premiumOrderClass, context);



      TestHelper.CreateAceWithSpecficTenant (Tenant);
      _aclList.Add (TestHelper.CreateAcl());
    }

    // TODO: Enable and check why owningGroups get a null entry 
    [Test]
    [Ignore]
    public void GetAclExpansionEntryList ()
    {
      List<User> userList = new List<User> ();
      var user = User;
      userList.Add (user);
      var userFinder = new TestAclExpanderUserFinder (userList);
      
      List<AccessControlList> aclList = new List<AccessControlList>();
      var ace = TestHelper.CreateAceWithAbstractRole();
      var acl = TestHelper.CreateAcl (ace);
      aclList.Add (acl);
      var aclFinder = new TestAclExpanderAclFinder (aclList);

      var aclExpander = new AclExpander (userFinder, aclFinder);
      var aclExpansionEntryList = aclExpander.GetAclExpansionEntryList();
      To.ConsoleLine.e (() => aclExpansionEntryList);
    }
  }

  public class TestAclExpanderAclFinder : IAclExpanderAclFinder
  {
    private readonly List<AccessControlList> _acls;
    public TestAclExpanderAclFinder (List<AccessControlList> acls) { _acls = acls; }
    public List<AccessControlList> AccessControlLists { get { return _acls; } }
  }

  public class TestAclExpanderUserFinder : IAclExpanderUserFinder
  {
    private readonly List<User> _users;
    public TestAclExpanderUserFinder (List<User> users) { _users = users; }
    public List<User> Users { get { return _users; } }
  }



}