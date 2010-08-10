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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Security;
using Remotion.Data.UnitTests.DomainObjects.Security.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.Security.Configuration;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Security.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class SecurityClientTransactionExtensionIntegrationTest
  {
    private ISecurityProvider _securityProviderStub;
    private IPrincipalProvider _principalProviderStub;
    private ClientTransaction _clientTransaction;
    private ISecurityPrincipal _securityPrincipalStub;

    [SetUp]
    public void SetUp ()
    {
      _securityProviderStub = MockRepository.GenerateStub<ISecurityProvider> ();
      _principalProviderStub = MockRepository.GenerateStub<IPrincipalProvider> ();
      _securityPrincipalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();

      _principalProviderStub.Stub (stub => stub.GetPrincipal ()).Return (_securityPrincipalStub);

      _clientTransaction = ClientTransaction.CreateRootTransaction ();
      _clientTransaction.Extensions.Add ("SCE", new SecurityClientTransactionExtension());

      SecurityConfiguration.Current.SecurityProvider = _securityProviderStub;
      SecurityConfiguration.Current.PrincipalProvider = _principalProviderStub;

      _clientTransaction.EnterNonDiscardingScope();
    }

    [TearDown]
    public void TearDown ()
    {
      ClientTransactionScope.ResetActiveScope();
      SecurityConfiguration.Current.SecurityProvider = null;
      SecurityConfiguration.Current.PrincipalProvider = null;
    }


    [Test]
    public void AccessGranted_PropertyWithDefaultPermission ()
    {
        var securityContextStub = MockRepository.GenerateStub<ISecurityContext> ();
        var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory> ();

        securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext ()).Return (securityContextStub);
        _securityProviderStub.Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub)).Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

        SecurableObject securableObject;
        using (new SecurityFreeSection ())
        {
          securableObject = SecurableObject.NewObject (_clientTransaction, new ObjectSecurityStrategy (securityContextFactoryStub));
        }

        Dev.Null = securableObject.PropertyWithDefaultPermission;
    }

    [Test]
    public void AccessGranted_PropertyWithCustomPermission ()
    {
        var securityContextStub = MockRepository.GenerateStub<ISecurityContext> ();
        var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory> ();

        securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext ()).Return (securityContextStub);
        _securityProviderStub.Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub)).Return (new[] { AccessType.Get (TestAccessTypes.First) });

        SecurableObject securableObject;
        using (new SecurityFreeSection ())
        {
          securableObject = SecurableObject.NewObject (_clientTransaction, new ObjectSecurityStrategy (securityContextFactoryStub));
        }

        Dev.Null = securableObject.PropertyWithCustomPermission;
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException), 
      ExpectedMessage = "Access to get-accessor of property 'PropertyWithDefaultPermission' on type 'Remotion.Data.UnitTests.DomainObjects.Security.TestDomain.SecurableObject' has been denied.")]
    public void AccessDenied_PropertyWithDefaultPermission ()
    {
        var securityContextStub = MockRepository.GenerateStub<ISecurityContext> ();
        var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory> ();

        securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext ()).Return (securityContextStub);

        SecurableObject securableObject;
        using (new SecurityFreeSection ())
        {
          securableObject = SecurableObject.NewObject (_clientTransaction, new ObjectSecurityStrategy (securityContextFactoryStub));
        }

        Dev.Null = securableObject.PropertyWithDefaultPermission;
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException),
      ExpectedMessage = "Access to get-accessor of property 'PropertyWithCustomPermission' on type 'Remotion.Data.UnitTests.DomainObjects.Security.TestDomain.SecurableObject' has been denied.")]
    public void AccessDenied_PropertyWithCustomPermission ()
    {
      {
        var securityContextStub = MockRepository.GenerateStub<ISecurityContext> ();
        var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory> ();

        securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext ()).Return (securityContextStub);

        SecurableObject securableObject;
        using (new SecurityFreeSection ())
        {
          securableObject = SecurableObject.NewObject (_clientTransaction, new ObjectSecurityStrategy (securityContextFactoryStub));
        }

        Dev.Null = securableObject.PropertyWithCustomPermission;
      }
    }
    
    [Test]
    public void AccessGranted_MixedPropertyWithDefaultPermission ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext> ();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory> ();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext ()).Return (securityContextStub);
      _securityProviderStub.Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub)).Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      SecurableObject securableObject;
      using (new SecurityFreeSection ())
      {
        securableObject = SecurableObject.NewObject (_clientTransaction, new ObjectSecurityStrategy (securityContextFactoryStub));
      }

      Dev.Null = ((ISecurableObjectMixin) securableObject).MixedPropertyWithDefaultPermission;
    }

    [Test]
    public void AccessGranted_MixedPropertyWithCustomPermission ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext> ();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory> ();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext ()).Return (securityContextStub);
      _securityProviderStub.Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub)).Return (new[] { AccessType.Get (TestAccessTypes.First) });

      SecurableObject securableObject;
      using (new SecurityFreeSection ())
      {
        securableObject = SecurableObject.NewObject (_clientTransaction, new ObjectSecurityStrategy (securityContextFactoryStub));
      }

      Dev.Null = ((ISecurableObjectMixin) securableObject).MixedPropertyWithCustomPermission;
    }


    [Test]
    [ExpectedException (typeof (PermissionDeniedException), 
      ExpectedMessage = "Access to get-accessor of property 'MixedPropertyWithDefaultPermission' on type 'Remotion.Data.UnitTests.DomainObjects.Security.TestDomain.SecurableObject' has been denied.")]
    public void AccessDenied_MixedPropertyWithDefaultPermission ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext> ();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory> ();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext ()).Return (securityContextStub);
      
      SecurableObject securableObject;
      using (new SecurityFreeSection ())
      {
        securableObject = SecurableObject.NewObject (_clientTransaction, new ObjectSecurityStrategy (securityContextFactoryStub));
      }

      Dev.Null = ((ISecurableObjectMixin) securableObject).MixedPropertyWithDefaultPermission;
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException),
      ExpectedMessage = "Access to get-accessor of property 'MixedPropertyWithCustomPermission' on type 'Remotion.Data.UnitTests.DomainObjects.Security.TestDomain.SecurableObject' has been denied.")]
    public void AccessDenied_MixedPropertyWithCustomPermission ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext> ();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory> ();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext ()).Return (securityContextStub);
      
      SecurableObject securableObject;
      using (new SecurityFreeSection ())
      {
        securableObject = SecurableObject.NewObject (_clientTransaction, new ObjectSecurityStrategy (securityContextFactoryStub));
      }

      Dev.Null = ((ISecurableObjectMixin) securableObject).MixedPropertyWithCustomPermission;
    }
  }
}