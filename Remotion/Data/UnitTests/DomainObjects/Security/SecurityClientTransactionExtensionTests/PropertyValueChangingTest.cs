// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Security;
using Remotion.Data.UnitTests.DomainObjects.Security.TestDomain;
using Remotion.Reflection;
using Remotion.Security;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Security.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class PropertyValueChangingTest
  {
    private SecurityClientTransactionExtensionTestHelper _testHelper;
    private IClientTransactionExtension _extension;
    private IPropertyInformation _propertyInformation;
    private PropertyInfo _propertyInfo;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new SecurityClientTransactionExtensionTestHelper ();
      _extension = new SecurityClientTransactionExtension ();
      _propertyInformation = MockRepository.GenerateMock<IPropertyInformation>();
      _propertyInfo = typeof (SecurableObject).GetProperty ("StringProperty");

      _testHelper.SetupSecurityConfiguration ();
    }

    [TearDown]
    public void TearDown ()
    {
      _testHelper.TearDownSecurityConfiguration ();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      DataContainer dataContainer = securableObject.GetDataContainer (_testHelper.Transaction);
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectMemberResolverGetPropertyInformation (_propertyInfo, _propertyInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions (_propertyInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      _extension.PropertyValueChanging (_testHelper.Transaction, dataContainer, dataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.Security.TestDomain.SecurableObject.StringProperty"], "old", "new");

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      DataContainer dataContainer = securableObject.GetDataContainer (_testHelper.Transaction);
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectMemberResolverGetPropertyInformation (_propertyInfo, _propertyInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions (_propertyInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      _extension.PropertyValueChanging (_testHelper.Transaction, dataContainer, dataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.Security.TestDomain.SecurableObject.StringProperty"], "old", "new");
    }

    [Test]
    public void Test_AccessGranted_WithinSecurityFreeSection ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      DataContainer dataContainer = securableObject.GetDataContainer (_testHelper.Transaction);
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _extension.PropertyValueChanging (_testHelper.Transaction, dataContainer, dataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.Security.TestDomain.SecurableObject.StringProperty"], "old", "new");
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithNonSecurableObject ()
    {
      NonSecurableObject nonSecurableObject = _testHelper.CreateNonSecurableObject ();
      DataContainer dataContainer = nonSecurableObject.GetDataContainer (_testHelper.Transaction);
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      _extension.PropertyValueChanging (_testHelper.Transaction, dataContainer, dataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.Security.TestDomain.NonSecurableObject.StringProperty"], "old", "new");

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_RecursiveSecurity ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      DataContainer dataContainer = securableObject.GetDataContainer (_testHelper.Transaction);
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectMemberResolverGetPropertyInformation (_propertyInfo, _propertyInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions (_propertyInformation, TestAccessTypes.First);
      HasAccessDelegate hasAccess = delegate 
      {
        securableObject.OtherStringProperty = "dummy";
        return true;
      };
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, hasAccess);
      _testHelper.ReplayAll ();

      _extension.PropertyValueChanging (_testHelper.Transaction, dataContainer, dataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.Security.TestDomain.SecurableObject.StringProperty"], "old", "new");

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectMemberResolverGetPropertyInformation (_propertyInfo, _propertyInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions (_propertyInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      _testHelper.Transaction.Execute (() => securableObject.StringProperty = "new");

      _testHelper.VerifyAll ();
    }
  }
}
