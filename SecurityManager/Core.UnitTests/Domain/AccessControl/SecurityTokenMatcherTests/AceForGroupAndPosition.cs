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
  public class AceForGroupAndPosition : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _companyHelper = new CompanyStructureHelper (TestHelper.Transaction);

      _ace = TestHelper.CreateAceWithPositionAndGroupCondition (_companyHelper.HeadPosition, GroupCondition.OwningGroup);

      Assert.That (_ace.TenantCondition, Is.EqualTo (TenantCondition.None));
      Assert.That (_ace.GroupCondition, Is.EqualTo (GroupCondition.OwningGroup));
      Assert.That (_ace.UserCondition, Is.EqualTo (UserCondition.SpecificPosition));
      Assert.That (_ace.SpecificAbstractRole, Is.Null);
    }

    [Test]
    public void AceWithExactGroup_TokenWithPrincipalAndPosition_Matches ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.This;
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole (user, owningGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void AceWithExactGroup_TokenWithPrincipalAndOtherPosition_DoesNotMatch ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.This;
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole (user, owningGroup, _companyHelper.MemberPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void AceWithExactGroup_TokenWithPrincipalAndPositionInParent_DoesNotMatch ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.This;
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      Group parentGroup = owningGroup.Parent;
      Assert.That (parentGroup, Is.Not.Null);
      TestHelper.CreateRole (user, parentGroup, _companyHelper.HeadPosition);
      TestHelper.CreateRole (user, owningGroup, _companyHelper.MemberPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void AceWithExactGroup_TokenWithPrincipalAndPositionInChild_DoesNotMatch ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.This;
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group childGroup = _companyHelper.AustrianCarTeam;
      Group owningGroup = childGroup.Parent;
      Assert.That (owningGroup, Is.Not.Null);
      TestHelper.CreateRole (user, childGroup, _companyHelper.HeadPosition);
      TestHelper.CreateRole (user, owningGroup, _companyHelper.MemberPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void AceWithGroupAndParentGroup_TokenWithPrincipalAndPositionInParent_Matches ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent;
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      Group parentGroup = owningGroup.Parent;
      Assert.That (parentGroup, Is.Not.Null);
      TestHelper.CreateRole (user, parentGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void AceWithGroupAndParentGroup_TokenWithPrincipalAndOtherPositionInParent_DoesNotMatch ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent;
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      Group parentGroup = owningGroup.Parent;
      Assert.That (parentGroup, Is.Not.Null);
      TestHelper.CreateRole (user, parentGroup, _companyHelper.MemberPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void AceWithGroupAndChildGroup_TokenWithPrincipalAndPositionInChild_Matches ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndChildren;
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group childGroup = _companyHelper.AustrianCarTeam;
      Group owningGroup = childGroup.Parent;
      Assert.That (owningGroup, Is.Not.Null);
      TestHelper.CreateRole (user, childGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void AceWithGroupAndChildGroup_TokenWithPrincipalAndOtherPositionInChild_DoesNotMatch ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndChildren;
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group childGroup = _companyHelper.AustrianCarTeam;
      Group owningGroup = childGroup.Parent;
      Assert.That (owningGroup, Is.Not.Null);
      TestHelper.CreateRole (user, childGroup, _companyHelper.MemberPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void EmptyToken_DoesNotMatch ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.This;

      SecurityToken token = TestHelper.CreateEmptyToken();

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }
  }
}