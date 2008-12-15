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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.SecurityTokenMatcherTests
{
  [TestFixture]
  public class AceForAnyGroupWithSpecificGroupType : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _companyHelper = new CompanyStructureHelper (TestHelper.Transaction);

      _ace = TestHelper.CreateAceWithSpecificGroupType(_companyHelper.DepartmentGroupType);

      Assert.That (_ace.TenantCondition, Is.EqualTo (TenantCondition.None));
      Assert.That (_ace.GroupCondition, Is.EqualTo (GroupCondition.AnyGroupWithSpecificGroupType));
      Assert.That (_ace.SpecificGroupType, Is.SameAs (_companyHelper.DepartmentGroupType));
      Assert.That (_ace.GroupHierarchyCondition, Is.EqualTo (GroupHierarchyCondition.Undefined));
      Assert.That (_ace.UserCondition, Is.EqualTo (UserCondition.None));
      Assert.That (_ace.SpecificAbstractRole, Is.Null);
    }

    [Test]
    public void TokenWithPrincipalInGroupMatchingGroupType_Matches ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group userGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole (user, userGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithoutPrincipalUserButWithPrincipalRole_Matches ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group userGroup = _companyHelper.AustrianProjectsDepartment;
      Role userRole = TestHelper.CreateRole (user, userGroup, _companyHelper.HeadPosition);
      
      SecurityToken token = new SecurityToken (
          new Principal (_companyHelper.CompanyTenant, null, new[] { userRole }),
          null, userGroup, null, new AbstractRoleDefinition[0]);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithPrincipalInGroupWithOtherGroupType_DoesNotMatch ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group userGroup = _companyHelper.AustrianCarTeam;
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole (user, userGroup, _companyHelper.HeadPosition);

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
    public void TokenWithoutPrincipalAndWithoutOwningGroup_DoesNotMatch ()
    {
      SecurityToken token = TestHelper.CreateEmptyToken();

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }
  }
}
