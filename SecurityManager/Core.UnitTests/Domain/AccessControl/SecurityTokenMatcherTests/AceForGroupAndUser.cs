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
  public class AceForGroupAndUser : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _companyHelper = new CompanyStructureHelper (TestHelper.Transaction);

      _ace = TestHelper.CreateAceWithOwningGroup();
      _ace.UserCondition = UserCondition.Owner;

      Assert.That (_ace.TenantCondition, Is.EqualTo (TenantCondition.None));
      Assert.That (_ace.GroupCondition, Is.EqualTo (GroupCondition.OwningGroup));
      Assert.That (_ace.UserCondition, Is.EqualTo (UserCondition.Owner));
      Assert.That (_ace.SpecificAbstractRole, Is.Null);
    }

    [Test]
    public void AceWithExactGroup_TokenWithRoleFromOwningUser_Matches ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.This;
      User owningUser = _companyHelper.CarTeamMember;
      Group owningGroup = _companyHelper.AustrianCarTeam;
      User user = owningUser;

      SecurityToken token = TestHelper.CreateToken (user, null, owningGroup, owningUser, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void AceWithExactGroup_TokenWithRoleAndOtherOwningUser_DoesNotMatch ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.This;
      User owningUser = _companyHelper.CarTeamMember;
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianCarTeam;

      SecurityToken token = TestHelper.CreateToken (user, null, owningGroup, owningUser, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void AceWithGroupAndParentGroup_TokenWithRoleInParent_Matches ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent;
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      User owningUser = user;
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      Group parentGroup = owningGroup.Parent;
      Assert.That (parentGroup, Is.Not.Null);
      TestHelper.CreateRole (user, parentGroup, _companyHelper.MemberPosition);

      SecurityToken token = TestHelper.CreateToken (user, null, owningGroup, owningUser, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void AceWithGroupAndChildGroup_TokenWithRoleInChild_Matches ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndChildren;
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      User owningUser = user;
      Group childGroup = _companyHelper.AustrianCarTeam;
      Group owningGroup = childGroup.Parent;
      Assert.That (owningGroup, Is.Not.Null);
      TestHelper.CreateRole (user, childGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateToken (user, null, owningGroup, owningUser, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsTrue (matcher.MatchesToken (token));
    }
  }
}