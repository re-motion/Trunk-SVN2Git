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
using NUnit.Framework.SyntaxHelpers;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.SecurityTokenMatcherTests
{
  [TestFixture]
  public class AceForOwningTenantAndParents : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _companyHelper = new CompanyStructureHelper (TestHelper.Transaction);

      _ace = TestHelper.CreateAceWithOwningTenant ();
      _ace.TenantHierarchyCondition = TenantHierarchyCondition.ThisAndParent;

      Assert.That (_ace.TenantCondition, Is.EqualTo (TenantCondition.OwningTenant));
      Assert.That (_ace.TenantHierarchyCondition, Is.EqualTo (TenantHierarchyCondition.ThisAndParent));
      Assert.That (_ace.GroupCondition, Is.EqualTo (GroupCondition.None));
      Assert.That (_ace.UserCondition, Is.EqualTo (UserCondition.None));
      Assert.That (_ace.SpecificAbstractRole, Is.Null);
    }

    [Test]
    public void TokenWithPrincipal_Matches ()
    {
      User principal = CreateUser (_companyHelper.CompanyTenant, null);
      Tenant owningTenant = _companyHelper.CompanyTenant;
      
      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (principal, owningTenant);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithPrincipalInParent_Matches ()
    {
      Tenant owningTenant = _companyHelper.CompanyTenant;
      Tenant principalTenant = TestHelper.CreateTenant ("Tenant");
      owningTenant.Parent = principalTenant;
      User principal = CreateUser (principalTenant, null);

      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (principal, owningTenant);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithPrincipalInGrandParent_Matches ()
    {
      Tenant owningTenant = _companyHelper.CompanyTenant;
      Tenant parentTenant = TestHelper.CreateTenant ("Parent");
      owningTenant.Parent = parentTenant;
      Tenant grandParentTenant = TestHelper.CreateTenant ("GrangParent");
      parentTenant.Parent = grandParentTenant;
      User principal = CreateUser (grandParentTenant, null);

      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (principal, owningTenant);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithPrincipalInChild_DoesNotMatch ()
    {
      Tenant owningTenant = _companyHelper.CompanyTenant;
      Tenant principalTenant = TestHelper.CreateTenant ("Tenant");
      principalTenant.Parent = owningTenant;
      User principal = CreateUser (principalTenant, null);

      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (principal, owningTenant);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithPrincipalInSibling_DoesNotMatch ()
    {
      Tenant owningTenant = _companyHelper.CompanyTenant;
      Tenant parentTenant = TestHelper.CreateTenant ("Parent");
      owningTenant.Parent = parentTenant;
      Tenant siblingTenant = TestHelper.CreateTenant ("Sibling");
      siblingTenant.Parent = parentTenant;
      User principal = CreateUser (siblingTenant, null);

      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (principal, owningTenant);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }
  }
}