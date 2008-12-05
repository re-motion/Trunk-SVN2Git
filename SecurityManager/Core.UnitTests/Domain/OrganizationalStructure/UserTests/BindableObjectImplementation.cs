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
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.UserTests
{
  [TestFixture]
  public class BindableObjectImpelementation : UserTestBase
  {
    public override void TearDown ()
    {
      base.TearDown();
      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
    }

    [Test]
    public void GetDisplayName_WithLastNameAndFirstNameAndTitle ()
    {
      User user = CreateUser();
      user.LastName = "UserLastName";
      user.FirstName = "UserFirstName";
      user.Title = "UserTitle";

      Assert.AreEqual ("UserLastName UserFirstName, UserTitle", user.DisplayName);
    }

    [Test]
    public void GetDisplayName_WithLastNameAndFirstName ()
    {
      User user = CreateUser();
      user.LastName = "UserLastName";
      user.FirstName = "UserFirstName";
      user.Title = null;

      Assert.AreEqual ("UserLastName UserFirstName", user.DisplayName);
    }

    [Test]
    public void GetDisplayName_WithLastName ()
    {
      User user = CreateUser();
      user.LastName = "UserLastName";
      user.FirstName = null;
      user.Title = null;

      Assert.AreEqual ("UserLastName", user.DisplayName);
    }

    [Test]
    public void SearchOwningGroups ()
    {
      ISearchAvailableObjectsService searchServiceStub = MockRepository.GenerateStub<ISearchAvailableObjectsService>();
      ISearchAvailableObjectsArguments args = MockRepository.GenerateStub<ISearchAvailableObjectsArguments>();

      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (typeof (UserPropertiesSearchService), searchServiceStub);
      IBusinessObjectClass userClass = BindableObjectProvider.GetBindableObjectClass (typeof (User));
      IBusinessObjectReferenceProperty owningGroupProperty = (IBusinessObjectReferenceProperty) userClass.GetPropertyDefinition ("OwningGroup");
      Assert.That (owningGroupProperty, Is.Not.Null);

      User user = CreateUser();
      Group expectedGroup = TestHelper.CreateGroup ("group", "uid", null, user.Tenant);
      IBusinessObject[] expected = new IBusinessObject[] { expectedGroup };

      searchServiceStub.Stub (stub => stub.SupportsProperty (owningGroupProperty)).Return (true);
      searchServiceStub.Stub (stub => stub.Search (user, owningGroupProperty, args)).Return (expected);

      Assert.That (owningGroupProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actualOwningGroups = owningGroupProperty.SearchAvailableObjects (user, args);
      Assert.That (actualOwningGroups, Is.SameAs (expected));
    }
  }
}
