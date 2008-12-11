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

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.SubstitutionTests
{
  [TestFixture]
  public class IsActive : SubstitutionTestBase
  {
    protected Substitution substitution;

    public override void SetUp ()
    {
      base.SetUp();

      substitution = Substitution.NewObject();
    }

    [Test]
    public void EvaluatesFalse_BeforeCommit ()
    {
      Assert.That (substitution.State, Is.Not.EqualTo (StateType.Unchanged));
      Assert.That (substitution.IsActive, Is.False);
    }

    [Test]
    public void EvaluatesTrue_WithIsEnabledTrue_WithoutTimeSpan ()
    {
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (substitution.IsActive, Is.True);
    }

    [Test]
    public void WithIsEnabledFalse_WithoutTimeSpan()
    {
      substitution.IsEnabled = false;
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (substitution.IsActive, Is.False);
    }

    [Test]
    public void EvaluatesTrue_WithIsEnabledTrue_WithBeginDateLessThanCurrentDate ()
    {
      substitution.BeginDate = DateTime.Today.AddDays (-2);
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (substitution.IsActive, Is.True);
    }

    [Test]
    public void EvaluatesTrue_WithIsEnabledTrue_WithBeginDateSameAsCurrentDate ()
    {
      substitution.BeginDate = DateTime.Today;
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (substitution.IsActive, Is.True);
    }

    [Test]
    public void EvaluatesTrue_WithIsEnabledTrue_WithBeginDateSameAsCurrentDateButGreaterTime ()
    {
      substitution.BeginDate = DateTime.Now.AddMinutes (1);
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (substitution.IsActive, Is.True);
    }

    [Test]
    public void EvaluatesFalse_WithIsEnabledTrue_WithBeginDateGreaterThanCurrentDate()
    {
      substitution.BeginDate = DateTime.Today.AddDays (+2);
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (substitution.IsActive, Is.False);
    }

    [Test]
    public void EvaluatesTrue_WithIsEnabledTrue_WithEndDateSameAsCurrentDate ()
    {
      substitution.EndDate = DateTime.Today;
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (substitution.IsActive, Is.True);
    }

    [Test]
    public void EvaluatesTrue_WithIsEnabledTrue_WithEndDateSameAsCurrentDateButLessTime()
    {
      substitution.EndDate = DateTime.Now.AddMinutes (-1);
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (substitution.IsActive, Is.True);
    }

    [Test]
    public void EvaluatesFalse_WithIsEnabledTrue_WithEndDateLessThanCurrentDate()
    {
      substitution.EndDate = DateTime.Today.AddDays (-2);
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (substitution.IsActive, Is.False);
    }
  }
}