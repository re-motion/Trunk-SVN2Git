// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Security;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Security.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;
using Remotion.Security;

namespace Remotion.Data.UnitTests.DomainObjects.Security.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class PropertyValueReadingTest
  {
    private TestHelper _testHelper;
    private IClientTransactionExtension _extension;

    private PropertyInfo _propertyInfo;
    private IMethodInformation _getMethodInformation;
    private PropertyDefinition _stringPropertyDefinition;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new TestHelper ();
      _extension = new SecurityClientTransactionExtension ();

      _propertyInfo = typeof (SecurableObject).GetProperty ("StringProperty");
      _getMethodInformation =  MethodInfoAdapter.Create(_propertyInfo.GetGetMethod());
      _stringPropertyDefinition = GetPropertyDefinition (_propertyInfo);

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
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_getMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      _extension.PropertyValueReading (_testHelper.Transaction, securableObject, _stringPropertyDefinition, ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_getMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      _extension.PropertyValueReading (_testHelper.Transaction, securableObject, _stringPropertyDefinition, ValueAccess.Current);
    }

    [Test]
    public void Test_AccessGranted_WithNonPublicAccessor ()
    {
      var propertyInfo =
          typeof (SecurableObject).GetProperty ("NonPublicPropertyWithCustomPermission", BindingFlags.NonPublic | BindingFlags.Instance);
      var getMethodInformation = MethodInfoAdapter.Create(propertyInfo.GetGetMethod (true));
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (getMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      _extension.PropertyValueReading (_testHelper.Transaction, securableObject, GetPropertyDefinition (propertyInfo), ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_WithNonPublicAccessor ()
    {
      var propertyInfo =
          typeof (SecurableObject).GetProperty ("NonPublicPropertyWithCustomPermission", BindingFlags.NonPublic | BindingFlags.Instance);
      var getMethodInformation = MethodInfoAdapter.Create(propertyInfo.GetGetMethod (true));
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (getMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      _extension.PropertyValueReading (_testHelper.Transaction, securableObject, GetPropertyDefinition (propertyInfo), ValueAccess.Current);
    }

    [Test]
    public void Test_AccessGranted_WithMissingAccessor ()
    {
      var propertyInfo = typeof (SecurableObject).GetProperty ("PropertyWithMissingGetAccessor");
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (new NullMethodInformation());
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Read, true);
      _testHelper.ReplayAll ();

      _extension.PropertyValueReading (_testHelper.Transaction, securableObject, GetPropertyDefinition (propertyInfo), ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_WithMissingAccessor ()
    {
      var propertyInfo = typeof (SecurableObject).GetProperty ("PropertyWithMissingGetAccessor");
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (new NullMethodInformation());
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Read, false);
      _testHelper.ReplayAll ();

      _extension.PropertyValueReading (_testHelper.Transaction, securableObject, GetPropertyDefinition (propertyInfo), ValueAccess.Current);
    }

    [Test]
    public void Test_AccessGranted_WithinSecurityFreeSection ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _extension.PropertyValueReading (_testHelper.Transaction, securableObject, _stringPropertyDefinition, ValueAccess.Current);
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithNonSecurableObject ()
    {
      var propertyInfo = typeof (NonSecurableObject).GetProperty ("StringProperty");
      NonSecurableObject nonSecurableObject = _testHelper.CreateNonSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      _extension.PropertyValueReading (_testHelper.Transaction, nonSecurableObject, GetPropertyDefinition (propertyInfo, typeof (NonSecurableObject)), ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_RecursiveSecurity ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_getMethodInformation, TestAccessTypes.First);
      HasAccessDelegate hasAccess = delegate
      {
        Dev.Null = securableObject.OtherStringProperty;
        return true;
      };
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, hasAccess);
      _testHelper.ReplayAll ();

      _extension.PropertyValueReading (_testHelper.Transaction, securableObject, _stringPropertyDefinition, ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_getMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      Dev.Null = _testHelper.Transaction.Execute(() => securableObject.StringProperty);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_ID ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      Dev.Null = _testHelper.Transaction.Execute (() => securableObject.ID);

      _testHelper.VerifyAll ();
    }

    private PropertyDefinition GetPropertyDefinition (PropertyInfo propertyInfo, Type classType = null)
    {
      return PropertyDefinitionObjectMother.CreateForPropertyInformation (
          MappingConfiguration.Current.GetTypeDefinition (classType ?? typeof (SecurableObject)),
          StorageClass.Persistent,
          PropertyInfoAdapter.Create (propertyInfo));
    }
  }
}
