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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Security.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Reflection;
using Remotion.Security;

namespace Remotion.Data.DomainObjects.Security.UnitTests.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class PropertyValueChangingTest : TestBase
  {
    private TestHelper _testHelper;
    private IClientTransactionExtension _extension;

    private PropertyInfo _propertyInfo;
    private IMethodInformation _setMethodInformation;
    private PropertyDefinition _stringPropertyDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new TestHelper ();
      _extension = new SecurityClientTransactionExtension ();

      _propertyInfo = typeof (SecurableObject).GetProperty ("StringProperty");
      _setMethodInformation = MethodInfoAdapter.Create(_propertyInfo.GetSetMethod ());
      _stringPropertyDefinition = PropertyDefinitionObjectMother.CreatePropertyDefinition (_propertyInfo);

      _testHelper.SetupSecurityIoCConfiguration ();
    }

    public override void TearDown ()
    {
      _testHelper.TearDownSecurityIoCConfiguration ();

      base.TearDown();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_setMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      _extension.PropertyValueChanging (_testHelper.Transaction, securableObject, _stringPropertyDefinition, "old", "new");

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_setMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      _extension.PropertyValueChanging (_testHelper.Transaction, securableObject, _stringPropertyDefinition, "old", "new");
    }

    [Test]
    public void Test_AccessGranted_WithNonPublicAccessor ()
    {
      var propertyInfo =
          typeof (SecurableObject).GetProperty ("NonPublicPropertyWithCustomPermission", BindingFlags.NonPublic | BindingFlags.Instance);
      var setMethodInformation = MethodInfoAdapter.Create (propertyInfo.GetSetMethod (true));
      SecurableObject securableObject = _testHelper.CreateSecurableObject();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (setMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll();

      _extension.PropertyValueChanging (
          _testHelper.Transaction,
          securableObject,
          PropertyDefinitionObjectMother.CreatePropertyDefinition (propertyInfo),
          "old",
          "new");

      _testHelper.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_WithNonPublicAccessor ()
    {
      var propertyInfo =
          typeof (SecurableObject).GetProperty ("NonPublicPropertyWithCustomPermission", BindingFlags.NonPublic | BindingFlags.Instance);
      var setMethodInformation = MethodInfoAdapter.Create (propertyInfo.GetSetMethod (true));
      SecurableObject securableObject = _testHelper.CreateSecurableObject();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (setMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, false);
      _testHelper.ReplayAll();

      _extension.PropertyValueChanging (
          _testHelper.Transaction,
          securableObject,
          PropertyDefinitionObjectMother.CreatePropertyDefinition (propertyInfo),
          "old",
          "new");
    }

    [Test]
    public void Test_AccessGranted_WithMissingAccessor ()
    {
      var propertyInfo = typeof (SecurableObject).GetProperty ("PropertyWithMissingSetAccessor");
      SecurableObject securableObject = _testHelper.CreateSecurableObject();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (new NullMethodInformation());
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Edit, true);
      _testHelper.ReplayAll();

      _extension.PropertyValueChanging (
          _testHelper.Transaction,
          securableObject,
          PropertyDefinitionObjectMother.CreatePropertyDefinition (propertyInfo),
          "old",
          "new");

      _testHelper.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_WithMissingAccessor ()
    {
      var propertyInfo = typeof (SecurableObject).GetProperty ("PropertyWithMissingSetAccessor");
      SecurableObject securableObject = _testHelper.CreateSecurableObject();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (new NullMethodInformation());
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Edit, false);
      _testHelper.ReplayAll();

      _extension.PropertyValueChanging (
          _testHelper.Transaction,
          securableObject,
          PropertyDefinitionObjectMother.CreatePropertyDefinition (propertyInfo),
          "old",
          "new");
    }

    [Test]
    public void Test_AccessGranted_WithinSecurityFreeSection ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      using (SecurityFreeSection.Create())
      {
        _extension.PropertyValueChanging (_testHelper.Transaction, securableObject, _stringPropertyDefinition, "old", "new");
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithNonSecurableObject ()
    {
      var propertyInfo = typeof (NonSecurableObject).GetProperty ("StringProperty");
      NonSecurableObject nonSecurableObject = _testHelper.CreateNonSecurableObject();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll();

      _extension.PropertyValueChanging (
          _testHelper.Transaction,
          nonSecurableObject,
          PropertyDefinitionObjectMother.CreatePropertyDefinition (propertyInfo, typeof (NonSecurableObject)),
          "old",
          "new");

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_RecursiveSecurity ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_setMethodInformation, TestAccessTypes.First);
      HasAccessDelegate hasAccess = delegate 
      {
        securableObject.OtherStringProperty = "dummy";
        return true;
      };
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, hasAccess);
      _testHelper.ReplayAll ();

      _extension.PropertyValueChanging (_testHelper.Transaction, securableObject, _stringPropertyDefinition, "old", "new");

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_setMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      _testHelper.Transaction.ExecuteInScope (() => securableObject.StringProperty = "new");

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithActiveTransactionMatchingTransactionPassedAsArgument_DoesNotCreateScope ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject();
      using (var scope = _testHelper.Transaction.EnterNonDiscardingScope())
      {
        _testHelper.AddExtension (_extension);
        _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_setMethodInformation, TestAccessTypes.First);
        _testHelper.ExpectObjectSecurityStrategyHasAccessWithMatchingScope (securableObject, scope);
        _testHelper.ReplayAll();

        _extension.PropertyValueChanging (_testHelper.Transaction, securableObject, _stringPropertyDefinition, "old", "new");
      }

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_WithActiveTransactionNotMatchingTransactionPassedAsArgument_CreatesScope ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_setMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll();

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        _extension.PropertyValueChanging (_testHelper.Transaction, securableObject, _stringPropertyDefinition, "old", "new");
      }

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_WithInactiveTransaction_CreatesScope ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_setMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      using (_testHelper.Transaction.EnterNonDiscardingScope())
      {
        using (ClientTransactionTestHelper.MakeInactive (_testHelper.Transaction))
        {
          _extension.PropertyValueChanging (_testHelper.Transaction, securableObject, _stringPropertyDefinition, "old", "new");
        }
      }

      _testHelper.VerifyAll ();
    }
  }
}
