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
using System.Collections;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Linq;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpander
  {

    private IAclExpanderUserFinder _userFinder = new AclExpanderUserFinder ();
    private IAclExpanderAclFinder _accessControlListFinder = new AclExpanderAclFinder ();

    public IAclExpanderUserFinder UserFinder
    {
      get { return _userFinder; }
      set { _userFinder = value; }
    }

    public IAclExpanderAclFinder AccessControlListFinder
    {
      get { return _accessControlListFinder; }
      set { _accessControlListFinder = value; }
    }

    //public static List<User> FindAllUsers ()
    //{
    //  var result = from u in DataContext.Entity<User> ()
    //               orderby u.LastName, u.FirstName
    //               select u;

    //  return result.ToList ();
    //}

    //public static List<AccessControlList> FindAllAccessControlLists ()
    //{
    //  var acls = new List<AccessControlList>();
    //  foreach (SecurableClassDefinition securableClassDefinition in SecurableClassDefinition.FindAll ())
    //  {
    //    acls.AddRange (securableClassDefinition.AccessControlLists);
    //  }

    //  return acls;
    //}


    List<AclExpansionEntry> GetAclExpansion ()
    {
      //var aclExpansion = new AclExpansion();
      //var aclExpansionEntries = new Dictionary<UserWithRole,AclExpansionEntry> (); //
      var aclExpansionEntries = new List<AclExpansionEntry> ();

      var users = _userFinder.Users;
      var acls = _accessControlListFinder.AccessControlLists;

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
      return aclExpansionEntries;
    }
  }


  public class AclExpansion : IEnumerable<AclExpansionEntry>
  {
    private readonly AclExpansionEntry[] _aclExpansionEntries;

    public AclExpansion (AclExpansionEntry[] aclExpansionEntries)
    {
      ArgumentUtility.CheckNotNull ("aclExpansionEntries", aclExpansionEntries);
      _aclExpansionEntries = aclExpansionEntries;
    }

    public IEnumerator<AclExpansionEntry> GetEnumerator ()
    {
      return ((IEnumerable<AclExpansionEntry>) _aclExpansionEntries).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}