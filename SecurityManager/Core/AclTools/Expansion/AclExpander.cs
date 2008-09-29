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
      //var aclExpansion = new AclExpansion();
      //var aclExpansionEntries = new Dictionary<UserWithRole,AclExpansionEntry> (); //
      var aclExpansionEntries = new List<AclExpansionEntry> ();

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
              AclProbe aclProbe = AclProbe.CreateAclProbe (user, role, ace);

              // TODO(?): Check if query with such a token was already done.
              AccessTypeDefinition[] accessTypeDefinitions = acl.GetAccessTypes (aclProbe.SecurityToken);

              if (accessTypeDefinitions != null && accessTypeDefinitions.Length > 0)
              {
                //var userWithRole = new UserWithRole (user, role);
                //AclExpansionEntry aclExpansionEntry;
                //aclExpansionEntries.TryGetValue (userWithRole, out aclExpansionEntry);
                //if(aclExpansionEntry == null)
                //{
                //  aclExpansionEntries[userWithRole] = new AclExpansionEntry (user, role, aclProbe.AccessConditions, accessTypeDefinitions);
                //}

                aclExpansionEntries.Add (new AclExpansionEntry (user, role, aclProbe.AccessConditions, accessTypeDefinitions));
              }
            }
          }
        }
      }
    }
  }


  public class UserWithRole : IEquatable<AclExpansionEntry>
  {
    public User User { get; private set; }
    public Role Role { get; private set; }
    public UserWithRole (User user, Role role)
    {
      User = user;
      Role = role;
    }

    public bool Equals (AclExpansionEntry other)
    {
      return (User == other.User) && (Role == other.Role);
    }
  }

  public class AclExpansion
  {
    public void Add (AclExpansionEntry aclExpansionEntry)
    {
      throw new NotImplementedException();
    }
  }

  public class AclExpansionEntry
  {
    public AclExpansionEntry (User user, Role role, 
      AclExpansionAccessConditions accessConditions, AccessTypeDefinition[] accessTypeDefinitions)
    {
      User = user;
      Role = role;
      AccessConditions = accessConditions;
      AccessTypeDefinitions = accessTypeDefinitions;
    }

    public User User { get; set; }
    public Role Role { get; set; }
    public AclExpansionAccessConditions AccessConditions { get; set; }
    public AccessTypeDefinition[] AccessTypeDefinitions { get; set; }
    

  }
}