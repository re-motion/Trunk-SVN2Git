// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
// 
using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// Supplies enumeration over all <see cref="Role"/>|s of <see cref="User"/>|s (taken from <see cref="IAclExpanderUserFinder"/>) and 
  /// <see cref="AccessControlEntry"/>|s of <see cref="AccessControlList"/>|s (taken from <see cref="IAclExpanderAclFinder"/>)
  /// combinations. 
  /// </summary>
  public class UserRoleAclAceCombinations : IUserRoleAclAceCombinations
  {
    private readonly IAclExpanderUserFinder _userFinder;
    private readonly IAclExpanderAclFinder _accessControlListFinder;


    public UserRoleAclAceCombinations (IAclExpanderUserFinder userFinder, IAclExpanderAclFinder accessControlListFinder)
    {
      _userFinder = userFinder;
      _accessControlListFinder = accessControlListFinder;
    }

    public IEnumerator<UserRoleAclAceCombination> GetEnumerator ()
    {
      var users = _userFinder.FindUsers ();
      var acls = _accessControlListFinder.FindAccessControlLists ();

      foreach (var user in users)
      {
        //To.ConsoleLine.e (() => user);
        foreach (var role in user.Roles)
        {
          //To.ConsoleLine.s ("\t").e (() => role);
          foreach (var acl in acls)
          {
            //To.ConsoleLine.s ("\t\t").e (() => acl);
            foreach (var ace in acl.AccessControlEntries)
            {
              //To.ConsoleLine.s ("\t\t\t").e (() => ace);
              yield return new UserRoleAclAceCombination (role, ace);
            }
          }
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}