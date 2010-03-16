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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.TenantTests
{
  [TestFixture]
  public class BindableObjectImplementation : TenantTestBase
  {
    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void UniqueIdentifier_SameIdentifierTwice ()
    {
      var dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();
      
      TestHelper.CreateTenant ("TestTenant", "UID: testTenant");

      ClientTransactionScope.CurrentTransaction.Commit();

      TestHelper.CreateTenant ("TestTenant", "UID: testTenant");

      ClientTransactionScope.CurrentTransaction.Commit ();
    }

    [Test]
    public void Get_UniqueIdentifier ()
    {
      Tenant tenant = TestHelper.CreateTenant ("TestTenant", "UID: testTenant");

      Assert.IsNotEmpty (tenant.UniqueIdentifier);
    }

    [Test]
    public void GetDisplayName ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Tenantname", "UID");

      Assert.AreEqual ("Tenantname", tenant.DisplayName);
    }

    [Test]
    public void GetAndSet_UniqueIdentifier ()
    {
      Tenant tenant = TestHelper.CreateTenant ("TestTenant", string.Empty);

      tenant.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual ("My Unique Identifier", tenant.UniqueIdentifier);
    }

    [Test]
    public void GetAndSet_UniqueIdentifierFromBusinessObjectWithIdentity ()
    {
      Tenant tenant = TestHelper.CreateTenant ("TestTenant", string.Empty);
      IBusinessObjectWithIdentity businessObject = tenant;

      tenant.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual (tenant.ID.ToString(), businessObject.UniqueIdentifier);
    }

    [Test]
    public void GetProperty_UniqueIdentifier ()
    {
      Tenant tenant = TestHelper.CreateTenant ("TestTenant", string.Empty);
      IBusinessObjectWithIdentity businessObject = tenant;

      tenant.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual ("My Unique Identifier", businessObject.GetProperty ("UniqueIdentifier"));
      Assert.AreEqual (tenant.ID.ToString(), businessObject.UniqueIdentifier);
    }

    [Test]
    public void SetProperty_UniqueIdentifier ()
    {
      Tenant tenant = TestHelper.CreateTenant ("TestTenant", string.Empty);
      IBusinessObjectWithIdentity businessObject = tenant;

      businessObject.SetProperty ("UniqueIdentifier", "My Unique Identifier");
      Assert.AreEqual ("My Unique Identifier", tenant.UniqueIdentifier);
      Assert.AreEqual (tenant.ID.ToString(), businessObject.UniqueIdentifier);
    }

    [Test]
    public void GetPropertyDefinition_UniqueIdentifier ()
    {
      Tenant tenant = TestHelper.CreateTenant ("TestTenant", string.Empty);
      IBusinessObjectWithIdentity businessObject = tenant;
      tenant.UniqueIdentifier = "My Unique Identifier";

      IBusinessObjectProperty property = businessObject.BusinessObjectClass.GetPropertyDefinition ("UniqueIdentifier");

      Assert.IsInstanceOfType (typeof (IBusinessObjectStringProperty), property);
      Assert.AreEqual ("My Unique Identifier", businessObject.GetProperty (property));
    }

    [Test]
    public void GetPropertyDefinitions_CheckForUniqueIdentifier ()
    {
      Tenant tenant = TestHelper.CreateTenant ("TestTenant", string.Empty);
      IBusinessObjectWithIdentity businessObject = tenant;

      IBusinessObjectProperty[] properties = businessObject.BusinessObjectClass.GetPropertyDefinitions();

      bool isFound = false;
      foreach (PropertyBase property in properties)
      {
        if (property.Identifier == "UniqueIdentifier" && property.PropertyInfo.DeclaringType == typeof (Tenant))
        {
          isFound = true;
          break;
        }
      }

      Assert.IsTrue (isFound, "Property UnqiueIdentifier declared on Tenant was not found.");
    }
  }
}