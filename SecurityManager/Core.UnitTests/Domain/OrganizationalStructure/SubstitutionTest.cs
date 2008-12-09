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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class SubstitutionTest : DomainTest
  {
    private OrganizationalStructureTestHelper _testHelper;
    private User _substitutingUser;
    private Substitution _substitution;

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new OrganizationalStructureTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();

      _substitutingUser = _testHelper.CreateUser ("userName", null, "lastName", null, null, null);
      _substitution = Substitution.NewObject (_substitutingUser);
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_substitution.SubstitutingUser, Is.SameAs (_substitutingUser));
      Assert.That (_substitution.IsEnabled, Is.True);
      Assert.That (_substitution.BeginDate, Is.Null);
      Assert.That (_substitution.EndDate, Is.Null);
    }

    [Test]
    public void IsActive_BeforeCommit_EvaluatesFalse ()
    {
      Assert.That (_substitution.State, Is.Not.EqualTo (StateType.Unchanged));
      Assert.That (_substitution.IsActive, Is.False);
    }

    [Test]
    public void IsActive_WithIsEnabledTrue_WithoutTimeSpan_EvaluatesTrue ()
    {
      _testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (_substitution.IsActive, Is.True);
    }

    [Test]
    public void IsActive_WithIsEnabledFalse_WithoutTimeSpan_EvaluatesFalse ()
    {
      _substitution.IsEnabled = false;
      _testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (_substitution.IsActive, Is.False);
    }

    [Test]
    public void IsActive_WithIsEnabledTrue_WithBeginDateLessThanCurrentDate_EvaluatesTrue ()
    {
      _substitution.BeginDate = DateTime.Today.AddDays (-2);
      _testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (_substitution.IsActive, Is.True);
    }

    [Test]
    public void IsActive_WithIsEnabledTrue_WithBeginDateSameAsCurrentDate_EvaluatesTrue ()
    {
      _substitution.BeginDate = DateTime.Today;
      _testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (_substitution.IsActive, Is.True);
    }

    [Test]
    public void IsActive_WithIsEnabledTrue_WithBeginDateSameAsCurrentDateButGreaterTime_EvaluatesTrue ()
    {
      _substitution.BeginDate = DateTime.Now.AddMinutes (1);
      _testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (_substitution.IsActive, Is.True);
    }

    [Test]
    public void IsActive_WithIsEnabledTrue_WithBeginDateGreaterThanCurrentDate_EvaluatesFalse ()
    {
      _substitution.BeginDate = DateTime.Today.AddDays (+2);
      _testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (_substitution.IsActive, Is.False);
    }

    [Test]
    public void IsActive_WithIsEnabledTrue_WithEndDateSameAsCurrentDate_EvaluatesTrue ()
    {
      _substitution.EndDate = DateTime.Today;
      _testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (_substitution.IsActive, Is.True);
    }

    [Test]
    public void IsActive_WithIsEnabledTrue_WithEndDateSameAsCurrentDateButLessTime_EvaluatesTrue ()
    {
      _substitution.EndDate = DateTime.Now.AddMinutes (-1);
      _testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (_substitution.IsActive, Is.True);
    }

    [Test]
    public void IsActive_WithIsEnabledTrue_WithEndDateLessThanCurrentDate_EvaluatesFalse ()
    {
      _substitution.EndDate = DateTime.Today.AddDays (-2);
      _testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (_substitution.IsActive, Is.False);
    }
  }
}