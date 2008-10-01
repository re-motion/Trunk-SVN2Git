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
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Diagnostics;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpander
  {
    private readonly IAclExpanderUserFinder _userFinder;
    private readonly IAclExpanderAclFinder _accessControlListFinder;


    public AclExpander (IAclExpanderUserFinder userFinder, IAclExpanderAclFinder accessControlListFinder)
    {
      _userFinder = userFinder;
      _accessControlListFinder = accessControlListFinder;
    }

    public AclExpander () : this (new AclExpanderUserFinder (), new AclExpanderAclFinder ()) {}


    public List<AclExpansionEntry> GetAclExpansionEntryList ()
    {
      var aclExpansionEntries = new List<AclExpansionEntry> ();

      var users = _userFinder.Users;
      var acls = _accessControlListFinder.AccessControlLists;

      foreach (var user in users)
      {
        foreach (var role in user.Roles)
        {
          foreach (var acl in acls)
          {
            To.ConsoleLine.e (acl);
            foreach (var ace in acl.AccessControlEntries)
            {
              To.ConsoleLine.e (ace);
              AclProbe aclProbe = AclProbe.CreateAclProbe (user, role, ace);
              To.ConsoleLine.e (aclProbe);

              // TODO(?): Check if we already queried with an identical token.
              AccessTypeDefinition[] accessTypeDefinitions = acl.GetAccessTypes (aclProbe.SecurityToken);

              if (accessTypeDefinitions != null && accessTypeDefinitions.Length > 0)
              {
                aclExpansionEntries.Add (new AclExpansionEntry (user, role, aclProbe.AccessConditions, accessTypeDefinitions));
              }
            }
          }
        }
      }
      return aclExpansionEntries;
    }
  }
}