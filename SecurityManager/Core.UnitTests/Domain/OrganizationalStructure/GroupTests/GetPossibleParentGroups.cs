// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.GroupTests
{
  [TestFixture]
  public class GetPossibleParentGroups : GroupTestBase
  {
    private DatabaseFixtures _dbFixtures;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      _dbFixtures = new DatabaseFixtures();
      Tenant tenant = _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.CreateRootTransaction());
      _expectedTenantID = tenant.ID;
    }

    //TODO: Refactor to mock
    [Test]
    public void SearchParentGroups ()
    {
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (
          typeof (GroupPropertiesSearchService), new GroupPropertiesSearchService());
      IBusinessObjectClass groupClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (Group));
      IBusinessObjectReferenceProperty parentGroupProperty = (IBusinessObjectReferenceProperty) groupClass.GetPropertyDefinition ("Parent");
      Assert.That (parentGroupProperty, Is.Not.Null);

      Group group = Group.FindByUnqiueIdentifier ("UID: group0");
      Assert.That (group, Is.Not.Null);
      List<Group> expectedParentGroups = group.GetPossibleParentGroups (group.Tenant.ID);
      Assert.That (expectedParentGroups, Is.Not.Empty);

      Assert.That (parentGroupProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actualParentGroups = parentGroupProperty.SearchAvailableObjects (group, null);
      Assert.That (actualParentGroups, Is.EquivalentTo (expectedParentGroups));
    }
  }
}