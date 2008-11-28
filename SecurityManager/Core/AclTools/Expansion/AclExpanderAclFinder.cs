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
using Remotion.SecurityManager.Domain.AccessControl;


namespace Remotion.SecurityManager.AclTools.Expansion
{
  // TODO AE: Test!
  public class AclExpanderAclFinder : IAclExpanderAclFinder
  {
    public List<AccessControlList> FindAccessControlLists ()
    {
      var findAllAclsQuery = from acl in QueryFactory.CreateLinqQuery<AccessControlList> ()
                             select acl;
      return findAllAclsQuery.ToList ();
    }
  }
}