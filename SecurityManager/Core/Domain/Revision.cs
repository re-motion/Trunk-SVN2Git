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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  public static class Revision
  {
    public static int GetRevision ()
    {
      Query query = new Query ("Remotion.SecurityManager.Domain.Revision.GetRevision");
      return (int) ClientTransactionScope.CurrentTransaction.QueryManager.GetScalar (query);
    }

    public static void IncrementRevision ()
    {
      Query query = new Query ("Remotion.SecurityManager.Domain.Revision.IncrementRevision");
      ClientTransactionScope.CurrentTransaction.QueryManager.GetScalar (query);
    }
  }
}
