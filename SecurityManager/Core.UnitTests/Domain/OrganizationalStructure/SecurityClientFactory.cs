/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Linq;
using System.Security.Principal;
using Remotion.Security;
using Remotion.Security.Metadata;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  public class SecurityClientFactory
  {
    public SecurityClient CreatedStubbedSecurityClient<T> (params Enum[] accessTypes)
        where T: ISecurableObject
    {
      ArgumentUtility.CheckNotNull ("accessTypes", accessTypes);

      var principalStub = CreatePrincipalStub();

      return new SecurityClient (
          CreateSecurityProviderStub (typeof (T), principalStub, accessTypes),
          new PermissionReflector(),
          CreateUserProviderStub (principalStub),
          MockRepository.GenerateStub<IFunctionalSecurityStrategy>());
    }

    private ISecurityProvider CreateSecurityProviderStub (Type securableClassType, IPrincipal principal, Enum[] returnedAccessTypes)
    {
      var securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();
      securityProviderStub.Stub (
          stub => stub.GetAccess (
                      Arg<ISecurityContext>.Matches (sc => TypeUtility.GetType (sc.Class) == securableClassType),
                      Arg.Is (principal)))
          .Return (returnedAccessTypes.Select (accessType => AccessType.Get (accessType)).ToArray());
      
      return securityProviderStub;
    }

    private IUserProvider CreateUserProviderStub (IPrincipal principal)
    {
      var userProviderStub = MockRepository.GenerateStub<IUserProvider>();
      userProviderStub.Stub (stub => stub.GetUser()).Return (principal);
      
      return userProviderStub;
    }

    private static IPrincipal CreatePrincipalStub ()
    {
      var principalStub = MockRepository.GenerateStub<IPrincipal>();
      var identityStub = MockRepository.GenerateStub<IIdentity>();

      principalStub.Stub (stub => stub.Identity).Return (identityStub);
      identityStub.Stub (stub => stub.Name).Return ("user");

      return principalStub;
    }
  }
}