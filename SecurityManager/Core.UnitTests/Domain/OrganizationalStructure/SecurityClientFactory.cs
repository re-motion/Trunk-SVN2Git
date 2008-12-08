// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
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
