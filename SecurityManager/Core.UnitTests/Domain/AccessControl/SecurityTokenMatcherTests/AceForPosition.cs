// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.SecurityTokenMatcherTests
{
  [TestFixture]
  public class AceForPosition : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp ();

      _companyHelper = new CompanyStructureHelper (TestHelper.Transaction);

      _ace = TestHelper.CreateAceWithPositionAndGroupCondition (_companyHelper.HeadPosition, GroupCondition.None);

      Assert.That (_ace.TenantCondition, Is.EqualTo (TenantCondition.None));
      Assert.That (_ace.GroupCondition, Is.EqualTo (GroupCondition.None));
      Assert.That (_ace.UserCondition, Is.EqualTo (UserCondition.SpecificPosition));
      Assert.That (_ace.SpecificPosition, Is.SameAs (_companyHelper.HeadPosition));
      Assert.That (_ace.SpecificAbstractRole, Is.Null);
    }

    [Test]
    public void TokenWithPrincipalAndPosition_Matches ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      TestHelper.CreateRole (user, _companyHelper.AustrianProjectsDepartment, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningUser (user, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithPrincipalAndOtherPosition_DoesNotMatch ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      TestHelper.CreateRole (user, _companyHelper.AustrianProjectsDepartment, _companyHelper.MemberPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningUser (user, null);

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
