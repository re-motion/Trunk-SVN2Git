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
using NUnit.Framework;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.SecurityTokenMatcherTests
{
  [TestFixture]
  public class AceForOwningGroup : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _companyHelper = new CompanyStructureHelper (TestHelper.Transaction);

      _ace = TestHelper.CreateAceWithOwningGroup ();

      Assert.That (_ace.TenantCondition, Is.EqualTo (TenantCondition.None));
      Assert.That (_ace.GroupCondition, Is.EqualTo (GroupCondition.OwningGroup));
      Assert.That (_ace.GroupHierarchyCondition, Is.EqualTo (GroupHierarchyCondition.This));
      Assert.That (_ace.UserCondition, Is.EqualTo (UserCondition.None));
      Assert.That (_ace.SpecificAbstractRole, Is.Null);
    }

    [Test]
    public void TokenWithPrincipalUser_Matches ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole (user, owningGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithoutPrincipalUserButWithPrincipalRole_Matches ()
    {
      SecurityToken token = new SecurityToken (
          new Principal (_companyHelper.CompanyTenant, null, new[] { _companyHelper.AustrianCarTeam.Roles[0] }),
          null, _companyHelper.AustrianCarTeam, null, new AbstractRoleDefinition[0]);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithPrincipalInParent_DoesNotMatch ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      Group parentGroup = owningGroup.Parent;
      Assert.That (parentGroup, Is.Not.Null);
      TestHelper.CreateRole (user, parentGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithPrincipalInChild_DoesNotMatch ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group childGroup = _companyHelper.AustrianCarTeam;
      Group owningGroup = childGroup.Parent;
      Assert.That (owningGroup, Is.Not.Null);
      TestHelper.CreateRole (user, childGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithoutPrincipalRoles_DoesNotMatch ()
    {
      SecurityToken token = new SecurityToken (
          new Principal (_companyHelper.CompanyTenant, _companyHelper.CarTeamMember, new Role[0]),
          null, _companyHelper.AustrianCarTeam, null, new AbstractRoleDefinition[0]);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithoutOwningGroup_DoesNotMatch ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      TestHelper.CreateRole (user, _companyHelper.AustrianProjectsDepartment, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void EmptyToken_DoesNotMatch ()
    {     
      SecurityToken token = TestHelper.CreateEmptyToken();

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }
  }
}
