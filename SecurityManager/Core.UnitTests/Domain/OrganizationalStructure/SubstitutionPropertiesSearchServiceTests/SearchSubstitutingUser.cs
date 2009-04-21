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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.SubstitutionPropertiesSearchServiceTests
{
  [TestFixture]
  public class SearchSubstitutingUser : SubstitutionPropertiesSearchServiceTestBase
  {
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _substitutingUserProperty;
    private User _user;

    public override void SetUp ()
    {
      base.SetUp();

      _searchService = new SubstitutionPropertiesSearchService();
      IBusinessObjectClass substitutionClass = BindableObjectProvider.GetBindableObjectClass (typeof (Substitution));
      _substitutingUserProperty = (IBusinessObjectReferenceProperty) substitutionClass.GetPropertyDefinition ("SubstitutingUser");
      Assert.That (_substitutingUserProperty, Is.Not.Null);

      _user = User.FindByUserName ("group0/user1");
      Assert.That (_user, Is.Not.Null);
    }

    [Test]
    public void SupportsProperty ()
    {
      Assert.That (_searchService.SupportsProperty (_substitutingUserProperty), Is.True);
    }

    [Test]
    public void Search ()
    {
      DomainObjectCollection expectedUsers = User.FindByTenantID (_user.Tenant.ID);
      Assert.That (expectedUsers, Is.Not.Empty);

      IBusinessObject[] actualUsers = _searchService.Search (null, _substitutingUserProperty, new DefaultSearchArguments (_user.Tenant.ID.ToString()));

      Assert.That (actualUsers, Is.EquivalentTo (expectedUsers));
    }
  }
}
