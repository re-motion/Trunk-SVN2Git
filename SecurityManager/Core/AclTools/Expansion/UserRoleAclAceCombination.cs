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
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class UserRoleAclAceCombination
  {
    public UserRoleAclAceCombination (Role role, AccessControlEntry ace)
    {
      Role = role;
      Ace = ace;
    }

    public Role Role { get; private set; }
    public User User { get { return Role.User; } }
    public AccessControlEntry Ace { get; private set; }
    public AccessControlList Acl { get { return Ace.AccessControlList; } }
  }
}