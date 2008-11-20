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
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.SecurityTokenMatcherTests
{
  [TestFixture]
  public class AceWithInvalidValues : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;

    public override void SetUp ()
    {
      base.SetUp();

      _companyHelper = new CompanyStructureHelper (TestHelper.Transaction);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The value 'Undefined' is not a valid value for matching the 'GroupHierarchyCondition'.")]
    public void GroupHierarchyCondition_UndefinedValue ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole (user, owningGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, owningGroup);

      AccessControlEntry ace = TestHelper.CreateAceWithOwningGroup();
      ace.GroupHierarchyCondition = GroupHierarchyCondition.Undefined;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (ace);

      matcher.MatchesToken (token);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The value 'Parent' is not a valid value for matching the 'GroupHierarchyCondition'.")]
    public void GroupHierarchyCondition_Parent ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole (user, owningGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, owningGroup);

      AccessControlEntry ace = TestHelper.CreateAceWithOwningGroup ();
      ace.GroupHierarchyCondition = GroupHierarchyCondition.Parent;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (ace);

      matcher.MatchesToken (token);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The value 'Children' is not a valid value for matching the 'GroupHierarchyCondition'.")]
    public void GroupHierarchyCondition_Children ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole (user, owningGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, owningGroup);

      AccessControlEntry ace = TestHelper.CreateAceWithOwningGroup ();
      ace.GroupHierarchyCondition = GroupHierarchyCondition.Children;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (ace);

      matcher.MatchesToken (token);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The value '1000' is not a valid value for 'GroupHierarchyCondition'.")]
    public void GroupHierarchyCondition_InvalidValue ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole (user, owningGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, owningGroup);

      AccessControlEntry ace = TestHelper.CreateAceWithOwningGroup ();
      ace.GroupHierarchyCondition = (GroupHierarchyCondition)1000;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (ace);

      matcher.MatchesToken (token);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The value 'Undefined' is not a valid value for matching the 'TenantHierarchyCondition'.")]
    public void TenantHierarchyCondition_UndefinedValue ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Tenant owningTenant = _companyHelper.CompanyTenant;

      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (user, owningTenant);

      AccessControlEntry ace = TestHelper.CreateAceWithOwningTenant ();
      ace.TenantHierarchyCondition = TenantHierarchyCondition.Undefined;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (ace);

      matcher.MatchesToken (token);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The value 'Parent' is not a valid value for matching the 'TenantHierarchyCondition'.")]
    public void TenantHierarchyCondition_Parent ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Tenant owningTenant = _companyHelper.CompanyTenant;

      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (user, owningTenant);

      AccessControlEntry ace = TestHelper.CreateAceWithOwningTenant ();
      ace.TenantHierarchyCondition = TenantHierarchyCondition.Parent;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (ace);

      matcher.MatchesToken (token);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The value '1000' is not a valid value for 'TenantHierarchyCondition'.")]
    public void TenantHierarchyCondition_InvalidValue ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Tenant owningTenant = _companyHelper.CompanyTenant;

      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (user, owningTenant);

      AccessControlEntry ace = TestHelper.CreateAceWithOwningTenant ();
      ace.TenantHierarchyCondition = (TenantHierarchyCondition) 1000;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (ace);

      matcher.MatchesToken (token);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The value '1000' is not a valid value for 'TenantCondition'.")]
    public void TenantCondition_InvalidValue ()
    {
      SecurityToken token = TestHelper.CreateEmptyToken ();

      AccessControlEntry ace = TestHelper.CreateAceWithOwningTenant();
      ace.TenantCondition = (TenantCondition) 1000;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (ace);

      matcher.MatchesToken (token);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The value '1000' is not a valid value for 'GroupCondition'.")]
    public void GroupCondition_InvalidValue ()
    {
      SecurityToken token = TestHelper.CreateEmptyToken();

      AccessControlEntry ace = TestHelper.CreateAceWithOwningGroup ();
      ace.GroupCondition = (GroupCondition) 1000;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (ace);

      matcher.MatchesToken (token);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The value '1000' is not a valid value for 'UserCondition'.")]
    public void UserCondition_InvalidValue ()
    {
      SecurityToken token = TestHelper.CreateEmptyToken ();

      AccessControlEntry ace = TestHelper.CreateAceWithOwningUser ();
      ace.UserCondition = (UserCondition) 1000;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (ace);

      matcher.MatchesToken (token);
    }
  }
}