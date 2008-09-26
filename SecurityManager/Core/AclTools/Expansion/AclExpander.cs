/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpander
  {

    public static List<User> FindAllUsers ()
    {
      var result = from u in DataContext.Entity<User> ()
                   orderby u.LastName, u.FirstName
                   select u;

      return result.ToList ();
    }

    public static List<AccessControlList> FindAllAccessControlLists ()
    {
      var acls = new List<AccessControlList>();
      foreach (SecurableClassDefinition securableClassDefinition in SecurableClassDefinition.FindAll ())
      {
        acls.AddRange (securableClassDefinition.AccessControlLists);
      }

      return acls;
    }


    void ExpandSpike ()
    {
      //TODO   var aclExpansion = new AclExpansion();

      var users = FindAllUsers();
      var acls = FindAllAccessControlLists();

      foreach (var user in users)
      {
        foreach (var role in user.Roles)
        {
          foreach (var acl in acls)
          {
            foreach (var ace in acl.AccessControlEntries)
            {
              //TODO   AclProbe aclProbe = AclProbe.CreateAclProbe (user, role, ace);
              //TODO   AccessTypeDefinition[] accessTypeDefinitions = acl.GetAccessTypes (aclProbe.SecurityToken);
              //TODO   aclExpansion.Add (new AclExpansionEntry (user, role, aclProbe.Conditions, accessTypeDefinitions));
            }
          }
        }
      }
    }
  }
}