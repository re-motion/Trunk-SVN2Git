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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryPropertiesSearchServiceTests
{
  [TestFixture]
  public class SearchGroupType : DomainTest
  {
    private OrganizationalStructureTestHelper _testHelper;
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _property;

    public override void SetUp ()
    {
      base.SetUp();
      
      _testHelper = new OrganizationalStructureTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();

      _searchService = new AccessControlEntryPropertiesSearchService();
      IBusinessObjectClass aceClass = BindableObjectProvider.GetBindableObjectClass (typeof (AccessControlEntry));
      _property = (IBusinessObjectReferenceProperty) aceClass.GetPropertyDefinition ("SpecificGroupType");
      Assert.That (_property, Is.Not.Null);
    }

    [Test]
    public void SupportsProperty ()
    {
      Assert.That (_searchService.SupportsProperty (_property), Is.True);
    }

    [Test]
    public void Search ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      ObjectList<GroupType> expected = GroupType.FindAll();
      Assert.That (expected, Is.Not.Empty);

      IBusinessObject[] actual = _searchService.Search (ace, _property, null);

      Assert.That (actual, Is.EqualTo (expected));
    }
  }
}
