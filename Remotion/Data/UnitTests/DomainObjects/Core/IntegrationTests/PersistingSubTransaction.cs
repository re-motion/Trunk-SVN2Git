/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Persistence;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  public class PersistingSubTransaction : SubClientTransaction
  {
    public PersistingSubTransaction (ClientTransaction parentTransaction)
        : base(parentTransaction)
    {
    }

    protected override void PersistData (DataContainerCollection changedDataContainers)
    {
      if (changedDataContainers.Count > 0)
      {
        using (var persistenceManager = new PersistenceManager ())
        {
          persistenceManager.Save (changedDataContainers);
        }
      }

      base.PersistData (changedDataContainers);
    }
  }
}