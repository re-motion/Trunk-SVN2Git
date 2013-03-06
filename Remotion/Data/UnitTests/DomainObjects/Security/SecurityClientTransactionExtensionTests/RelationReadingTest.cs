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
using Remotion.Data.DomainObjects.Security;
using Remotion.Data.UnitTests.DomainObjects.Core;
using Remotion.Data.UnitTests.DomainObjects.Security.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;
using Remotion.Security;

namespace Remotion.Data.UnitTests.DomainObjects.Security.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class RelationReadingTest
  {
    private TestHelper _testHelper;
    private IClientTransactionExtension _extension;
    private PropertyInfo _propertyInfo;
    private IMethodInformation _getMethodInformation;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new TestHelper ();
      _extension = new SecurityClientTransactionExtension ();
      _propertyInfo = typeof (SecurableObject).GetProperty ("Parent");
      _getMethodInformation = MethodInfoAdapter.Create(_propertyInfo.GetGetMethod());
      
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

      var securableEndPointDefintion = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".Parent");

      _extension.RelationReading (_testHelper.Transaction, securableObject, securableEndPointDefintion, ValueAccess.Current);

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

      var securableEndPointDefintion = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".Parent");

      _extension.RelationReading (_testHelper.Transaction, securableObject, securableEndPointDefintion, ValueAccess.Current);
    }

    [Test]
    public void Test_AccessGranted_WithNonPublicAccessor ()
    {
      var propertyInfo =
          typeof (SecurableObject).GetProperty ("NonPublicRelationPropertyWithCustomPermission", BindingFlags.NonPublic | BindingFlags.Instance);
      var getMethodInformation = MethodInfoAdapter.Create(propertyInfo.GetGetMethod (true));
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);

      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (getMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      var securableEndPointDefintion = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".NonPublicRelationPropertyWithCustomPermission");

      _extension.RelationReading (_testHelper.Transaction, securableObject, securableEndPointDefintion, ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_WithNonPublicAccessor ()
    {
      var propertyInfo =
          typeof (SecurableObject).GetProperty ("NonPublicRelationPropertyWithCustomPermission", BindingFlags.NonPublic | BindingFlags.Instance);
      var getMethodInformation = MethodInfoAdapter.Create(propertyInfo.GetGetMethod (true));
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (getMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      var securableEndPointDefintion = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".NonPublicRelationPropertyWithCustomPermission");

      _extension.RelationReading (_testHelper.Transaction, securableObject, securableEndPointDefintion, ValueAccess.Current);
    }

    [Test]
    public void Test_AccessGranted_WithMissingAccessor ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject();
      _testHelper.AddExtension (_extension);

      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (new NullMethodInformation());
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Read, true);
      _testHelper.ReplayAll();

      var securableEndPointDefintion = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".RelationPropertyWithMissingGetAccessor");

      _extension.RelationReading (_testHelper.Transaction, securableObject, securableEndPointDefintion, ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_WithMissingAccessor ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (new NullMethodInformation());
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Read, false);
      _testHelper.ReplayAll();

      var securableEndPointDefintion = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".RelationPropertyWithMissingGetAccessor");

      _extension.RelationReading (_testHelper.Transaction, securableObject, securableEndPointDefintion, ValueAccess.Current);
    }

    [Test]
    public void Test_AccessGranted_WithinSecurityFreeSection ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      var securableEndPointDefintion = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".Parent");

      using (new SecurityFreeSection ())
      {
        _extension.RelationReading (_testHelper.Transaction, securableObject, securableEndPointDefintion, ValueAccess.Current);
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithNonSecurableObject ()
    {
      NonSecurableObject nonSecurableObject = _testHelper.CreateNonSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      var nonSecurableEndPointDefintion = nonSecurableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (NonSecurableObject).FullName + ".Parent");


      _extension.RelationReading (_testHelper.Transaction, nonSecurableObject, nonSecurableEndPointDefintion, ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_OneSide_RecursiveSecurity ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.Transaction.ExecuteInScope (() => securableObject.OtherParent = _testHelper.CreateSecurableObject ());
      _testHelper.AddExtension (_extension);

      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_getMethodInformation, TestAccessTypes.First);
      HasAccessDelegate hasAccess = delegate
      {
        Dev.Null = securableObject.OtherParent;
        return true;
      };
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, hasAccess);
      _testHelper.ReplayAll ();

      var securableEndPointDefintion = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".Parent");

      _extension.RelationReading (_testHelper.Transaction, securableObject, securableEndPointDefintion, ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_ManySide_RecursiveSecurity ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.Transaction.ExecuteInScope (() => securableObject.OtherChildren.Add (_testHelper.CreateSecurableObject ()));
      _testHelper.AddExtension (_extension);
      var propertyInfo = typeof (SecurableObject).GetProperty ("Children");
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (MethodInfoAdapter.Create(propertyInfo.GetGetMethod()), TestAccessTypes.First);
      HasAccessDelegate hasAccess = delegate
      {
        Dev.Null = securableObject.OtherChildren[0];
        return true;
      };
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, hasAccess);
      _testHelper.ReplayAll ();

      var securableEndPointDefintion = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".Children");
      
      _extension.RelationReading (_testHelper.Transaction, securableObject, securableEndPointDefintion, ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_OneSide_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.Transaction.ExecuteInScope (() => securableObject.Parent = _testHelper.CreateSecurableObject ());
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_getMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();


      Dev.Null = _testHelper.Transaction.ExecuteInScope (() => securableObject.Parent);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_ManySide_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.Transaction.ExecuteInScope (() => securableObject.Children.Add (_testHelper.CreateSecurableObject ()));
      _testHelper.AddExtension (_extension);
      var propertyInfo = typeof (SecurableObject).GetProperty ("Children");
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (MethodInfoAdapter.Create(propertyInfo.GetGetMethod()), TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      Dev.Null = _testHelper.Transaction.ExecuteInScope (() => securableObject.Children[0]);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithInactiveTransaction ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);

      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_getMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      var securableEndPointDefintion = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".Parent");

      ClientTransactionTestHelper.MakeInactive (_testHelper.Transaction);

      _extension.RelationReading (_testHelper.Transaction, securableObject, securableEndPointDefintion, ValueAccess.Current);

      _testHelper.VerifyAll ();
    }
  }
}
