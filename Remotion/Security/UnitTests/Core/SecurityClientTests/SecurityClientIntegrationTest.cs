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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Reflection;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.Core.SampleDomain;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.Core.SecurityClientTests
{
  [TestFixture]
  public class SecurityClientIntegrationTest
  {
    private SecurityClient _securityClient;
    private ISecurityPrincipal _securityPrincipalStub;
    private IFunctionalSecurityStrategy _functionalSecurityStrategyStub;
    private ISecurityProvider _securityProviderStub;
    private IPrincipalProvider _principalProviderStub;

    [SetUp]
    public void SetUp ()
    {
      _securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();
      _securityPrincipalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      _functionalSecurityStrategyStub = MockRepository.GenerateStub<IFunctionalSecurityStrategy> ();
      _principalProviderStub = MockRepository.GenerateStub<IPrincipalProvider> ();
      
      _principalProviderStub.Stub (stub => stub.GetPrincipal()).Return (_securityPrincipalStub); 

      _securityClient = new SecurityClient (
          _securityProviderStub, 
          new PermissionReflector (), 
          _principalProviderStub, 
          _functionalSecurityStrategyStub, 
          new ReflectionBasedMemberResolver ());
    }

    [Test]
    public void HasAccess_InstanceMethod ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext> ();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory> ();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext()).Return (securityContextStub);
      _securityProviderStub.Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub)).Return (new[] { AccessType.Get (GeneralAccessTypes.Delete) });

      ISecurableObject securableObject = new SecurableObject (new ObjectSecurityStrategy (securityContextFactoryStub));
      var methodInfo = typeof (SecurableObject).GetMethod ("Delete", new Type[0]);
      
      var hasMethodAccess = _securityClient.HasMethodAccess (securableObject, methodInfo);

      Assert.That (hasMethodAccess, Is.True);
    }

    [Test]
    public void HasAccess_MethodInDerivedClass ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext> ();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory> ();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext ()).Return (securityContextStub);
      _securityProviderStub.Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub)).Return (new[] { AccessType.Get (GeneralAccessTypes.Create) });

      ISecurableObject securableObject = new DerivedSecurableObject (new ObjectSecurityStrategy (securityContextFactoryStub));
      var methodInfo = typeof (DerivedSecurableObject).GetMethod ("Make");

      var hasMethodAccess = _securityClient.HasMethodAccess (securableObject, methodInfo);

      Assert.That (hasMethodAccess, Is.True);
    }

    [Test]
    public void HasAccess_StaticMethod ()
    {
      var securityContext = SecurityContext.CreateStateless (typeof (SecurableObject));
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory> ();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext ()).Return (securityContext);
      _securityProviderStub.Stub (mock => mock.GetAccess (securityContext, _securityPrincipalStub)).Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });
      
      var securityClient = new SecurityClient (
          _securityProviderStub,
          new PermissionReflector (),
          _principalProviderStub,
          new FunctionalSecurityStrategy (),
          new ReflectionBasedMemberResolver ());

      var methodInfo = typeof (SecurableObject).GetMethod ("IsValid", new Type[]{typeof(SecurableObject)});

      var hasMethodAccess = securityClient.HasStaticMethodAccess (typeof(SecurableObject), methodInfo);

      Assert.That (hasMethodAccess, Is.True);
    }

    [Test]
    public void HasAccess_StatelessMethod ()
    {
      var securityContext = SecurityContext.CreateStateless (typeof (SecurableObject));
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory> ();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext ()).Return (securityContext);
      _securityProviderStub.Stub (mock => mock.GetAccess (securityContext, _securityPrincipalStub)).Return (new[] { AccessType.Get (GeneralAccessTypes.Delete) });

      var securityClient = new SecurityClient (
          _securityProviderStub,
          new PermissionReflector (),
          _principalProviderStub,
          new FunctionalSecurityStrategy (),
          new ReflectionBasedMemberResolver ());

      var methodInfo = typeof (SecurableObject).GetMethod ("Delete", new Type[0]);
     
      var hasMethodAccess = securityClient.HasStatelessMethodAccess (typeof (SecurableObject), methodInfo);

      Assert.That (hasMethodAccess, Is.True);
    }

    [Test]
    public void HasAccess_Property ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext> ();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory> ();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext ()).Return (securityContextStub);
      _securityProviderStub.Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub)).Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      ISecurableObject securableObject = new SecurableObject (new ObjectSecurityStrategy (securityContextFactoryStub));
      var propertyInfo = typeof (SecurableObject).GetProperty ("IsVisible");

      var hasMethodAccess = _securityClient.HasPropertyReadAccess (securableObject, propertyInfo);

      Assert.That (hasMethodAccess, Is.True);
    }
  }
}