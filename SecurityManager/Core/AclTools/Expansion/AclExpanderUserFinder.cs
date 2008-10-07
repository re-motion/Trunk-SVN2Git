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
using Remotion.Data.DomainObjects.Queries;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpanderUserFinder : IAclExpanderUserFinder
  {
    public List<User> FindUsers ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope ())
      {
        //var findAllUsersQuery = from u in DataContext.Entity<User> ()
        //                        orderby u.LastName, u.FirstName
        //                        select u;
        //return findAllUsersQuery.ToList ();
        var findAllUsersQuery = from u in QueryFactory.CreateQueryable<User>()
                                orderby u.LastName , u.FirstName
                                select u;
        return findAllUsersQuery.ToList();
      }
    }
  }
}