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
using System.Collections.Specialized;
using System.Security.Principal;
using Remotion.Configuration;
using Remotion.Data.DomainObjects;
using Remotion.Logging;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager
{
  public class SecurityService: ExtendedProviderBase, IRevisionBasedSecurityProvider
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (SecurityService));

    private readonly IAccessControlListFinder _accessControlListFinder;
    private readonly ISecurityTokenBuilder _securityTokenBuilder;

    public SecurityService()
        : this (new AccessControlListFinder(), new SecurityTokenBuilder())
    {
    }

    public SecurityService (IAccessControlListFinder accessControlListFinder, ISecurityTokenBuilder securityTokenBuilder)
        : this ("SecurityManager", new NameValueCollection(), accessControlListFinder, securityTokenBuilder)
    {
    }


    public SecurityService (string name, NameValueCollection config)
        : this (name, config, new AccessControlListFinder(), new SecurityTokenBuilder())
    {
    }

    public SecurityService (
        string name, NameValueCollection config, IAccessControlListFinder accessControlListFinder, ISecurityTokenBuilder securityTokenBuilder)
        : base (name, config)
    {
      ArgumentUtility.CheckNotNull ("accessControlListFinder", accessControlListFinder);
      ArgumentUtility.CheckNotNull ("securityTokenBuilder", securityTokenBuilder);

      _accessControlListFinder = accessControlListFinder;
      _securityTokenBuilder = securityTokenBuilder;
    }

    public AccessType[] GetAccess (ISecurityContext context, IPrincipal user)
    {
      return GetAccess (ClientTransaction.NewRootTransaction(), context, user);
    }

    public AccessType[] GetAccess (ClientTransaction transaction, ISecurityContext context, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("user", user);

      AccessControlList acl;
      SecurityToken token;
      try
      {
        acl = _accessControlListFinder.Find (transaction, context);
        token = _securityTokenBuilder.CreateToken (transaction, user, context);
      }
      catch (AccessControlException e)
      {
        s_log.Error ("Error during evaluation of security query.", e);
        return new AccessType[0];
      }

      using (transaction.EnterNonDiscardingScope())
      {
        AccessTypeDefinition[] accessTypes = acl.GetAccessTypes (token);
        return Array.ConvertAll<AccessTypeDefinition, AccessType> (accessTypes, ConvertToAccessType);
      }
    }

    public int GetRevision ()
    {
      using (ClientTransaction.NewRootTransaction().EnterNonDiscardingScope())
      {
        return Revision.GetRevision ();
      }
    }

    private AccessType ConvertToAccessType (AccessTypeDefinition accessTypeDefinition)
    {
      //TODO: Use new EnumWrapper
      return AccessType.Get (new EnumWrapper (accessTypeDefinition.Name));
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
