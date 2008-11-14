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
  public class AceForBranchOfOwningGroupAndPosition : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _companyHelper = new CompanyStructureHelper (TestHelper.Transaction);

      _ace = TestHelper.CreateAceWithPosition (_companyHelper.HeadPosition, GroupCondition.BranchOfOwningGroup);
      _ace.SpecificGroupType = _companyHelper.DivisionGroupType;

      Assert.That (_ace.TenantCondition, Is.EqualTo (TenantCondition.None));
      Assert.That (_ace.GroupCondition, Is.EqualTo (GroupCondition.BranchOfOwningGroup));
      Assert.That (_ace.SpecificGroupType, Is.SameAs (_companyHelper.DivisionGroupType));
      Assert.That (_ace.GroupHierarchyCondition, Is.EqualTo (GroupHierarchyCondition.Undefined));
      Assert.That (_ace.UserCondition, Is.EqualTo (UserCondition.SpecificPosition));
      Assert.That (_ace.SpecificPosition, Is.SameAs (_companyHelper.HeadPosition));
      Assert.That (_ace.SpecificAbstractRole, Is.Null);
    }

    [Test]
    public void TokenWithPositionInBranchRoot_Matches ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      TestHelper.CreateRole (user, _companyHelper.AustrianDivsion, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, _companyHelper.AustrianProjectsDepartment);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithOtherPositionInBranchRoot_DoesNotMatch ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      TestHelper.CreateRole (user, _companyHelper.AustrianDivsion, _companyHelper.MemberPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, _companyHelper.AustrianDivsion);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithPositionBelowBranchRoot_DoesNotMatch ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      TestHelper.CreateRole (user, _companyHelper.AustrianProjectsDepartment, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, _companyHelper.AustrianProjectsDepartment);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }
  }
}