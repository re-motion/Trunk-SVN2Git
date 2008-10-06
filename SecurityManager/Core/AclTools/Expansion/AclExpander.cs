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
using System.Linq;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Diagnostics.ToText;
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


    // Spike implementation to test algorithm. 
    public List<AclExpansionEntry> GetAclExpansionEntryList_Spike ()
    {
      var aclExpansionEntries = new List<AclExpansionEntry> ();

      var users = _userFinder.FindUsers();
      var acls = _accessControlListFinder.FindAccessControlLists();

      foreach (var user in users)
      {
        To.ConsoleLine.e (() => user);
        foreach (var role in user.Roles)
        {
          To.ConsoleLine.s ("\t").e (() => role);
          foreach (var acl in acls)
          {
            To.ConsoleLine.s ("\t\t").e (() => acl);
            foreach (var ace in acl.AccessControlEntries)
            {
              To.ConsoleLine.s ("\t\t\t").e (() => ace);
              AclProbe aclProbe = AclProbe.CreateAclProbe (user, role, ace);
              To.ConsoleLine.s ("\t\t\t").e (() => aclProbe);

              // TODO(?): Check if we already queried with an identical token.
              AccessTypeDefinition[] accessTypeDefinitions = acl.GetAccessTypes (aclProbe.SecurityToken);

              if (accessTypeDefinitions.Length > 0)
              {
                var aclExpansionEntry = new AclExpansionEntry (user, role, aclProbe.AccessConditions, accessTypeDefinitions);
                To.ConsoleLine.s ("\t\t\t").e (() => aclExpansionEntry);
                aclExpansionEntries.Add (aclExpansionEntry);
              }
            }
          }
        }
      }

      List<AclExpansionEntry> aclExpansionEntriesSortedAndDistinct = 
        (from AclExpansionEntry aclExpansionEntry in aclExpansionEntries
         //orderby aclExpansionEntry.User.DisplayName, aclExpansionEntry.Role.DisplayName
         select aclExpansionEntry ).Distinct().ToList();

      return aclExpansionEntriesSortedAndDistinct;
    }
  }
}