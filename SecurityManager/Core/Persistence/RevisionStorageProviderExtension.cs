// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.SecurityManager.Domain;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Persistence
{
  public class RevisionStorageProviderExtension
  {
    private readonly IRevisionProvider _revisionProvider;

    public RevisionStorageProviderExtension (IRevisionProvider revisionProvider)
    {
      ArgumentUtility.CheckNotNull ("revisionProvider", revisionProvider);
      
      _revisionProvider = revisionProvider;
    }

    public virtual void Saving (IDbConnection connection, IDbTransaction transaction, IEnumerable<DataContainer> dataContainers)
    {
      ArgumentUtility.CheckNotNull ("connection", connection);
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

      Type securityManagerDomainLayerSuperType = typeof (BaseSecurityManagerObject);
      if (dataContainers.Any (dataContainer => securityManagerDomainLayerSuperType.IsAssignableFrom (dataContainer.DomainObjectType)))
      {
        IncrementRevision (connection, transaction);
        _revisionProvider.InvalidateRevision();
      }
    }

    private void IncrementRevision (IDbConnection connection, IDbTransaction transaction)
    {
      using (IDbCommand command = connection.CreateCommand ())
      {
        var query =  Revision.GetIncrementRevisionQuery();
        Assertion.IsTrue (query.Parameters.Count == 0);
        Assertion.IsTrue (query.QueryType == QueryType.Scalar);

        command.Transaction = transaction;
        command.CommandText = query.Statement;

        command.ExecuteNonQuery ();
      }
    }
  }
}
