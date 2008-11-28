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
using Remotion.Data.DomainObjects.Queries;
using Remotion.SecurityManager.Domain.OrganizationalStructure;


namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpanderUserFinder : IAclExpanderUserFinder
  {
    private readonly string _firstName;
    private readonly string _lastName;
    private readonly string _userName;

    public AclExpanderUserFinder () : this(null,null,null) {}

    // TODO AE: Don't discuss in code, discuss in person, decide, and remove comments. (Or leave comment with rationale only.)
    // TODO MGI: Only use UserName. This value is authoritive and must be unique
    // MGI Comment: To be able to filter for first and last name is an additional, orthogonal feature. It
    // It was not introduced for convenience and is already implemeted.
    public AclExpanderUserFinder (string firstName, string lastName, string userName)
    {
      _firstName = firstName;
      _lastName = lastName;
      _userName = userName;
    }

    public List<User> FindUsers ()
    {
      var findAllUsersQuery = from u in QueryFactory.CreateLinqQuery<User>()
                              where 
                                (_lastName == null || u.LastName == _lastName) && 
                                (_firstName == null || u.FirstName == _firstName) &&
                                (_userName == null || u.UserName == _userName) 
                              orderby u.LastName , u.FirstName
                              select u;
      return findAllUsersQuery.ToList();
    }
  }
}