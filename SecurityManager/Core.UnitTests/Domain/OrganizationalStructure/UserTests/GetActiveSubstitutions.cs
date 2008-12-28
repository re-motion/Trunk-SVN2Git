// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.UserTests
{
  [TestFixture]
  public class GetActiveSubstitutions : UserTestBase
  {
    [Test]
    public void Test ()
    {
      User substitutingUser = CreateUser();
      
      Substitution enabledSubstitution1 = Substitution.NewObject();
      enabledSubstitution1.SubstitutingUser = substitutingUser;
      enabledSubstitution1.SubstitutedUser = CreateUser();

      Substitution disabledFromFlagSubstitution = Substitution.NewObject ();
      disabledFromFlagSubstitution.SubstitutingUser = substitutingUser;
      disabledFromFlagSubstitution.SubstitutedUser = CreateUser ();
      disabledFromFlagSubstitution.IsEnabled = false;

      Substitution disabledFromDateSubstitution = Substitution.NewObject ();
      disabledFromDateSubstitution.SubstitutingUser = substitutingUser;
      disabledFromDateSubstitution.SubstitutedUser = CreateUser ();
      disabledFromDateSubstitution.BeginDate = DateTime.Today.AddDays (1);

      Substitution changedSubstitution = Substitution.NewObject ();
      changedSubstitution.SubstitutedUser = CreateUser ();
      changedSubstitution.IsEnabled = true;

      Substitution enabledSubstitution2 = Substitution.NewObject ();
      enabledSubstitution2.SubstitutingUser = substitutingUser;
      enabledSubstitution2.SubstitutedUser = CreateUser ();

      ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ();

      changedSubstitution.SubstitutingUser = substitutingUser;

      Assert.That (substitutingUser.GetActiveSubstitutions(), Is.EquivalentTo (new[] { enabledSubstitution1, enabledSubstitution2 }));
    }
  }
}
