// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Data;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.SecurityManager.Domain;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Persistence
{
  public class RevisionStorageProviderExtension
  {
    // constants

    // types

    // static members

    // member fields

    // construction and disposing

    public RevisionStorageProviderExtension ()
    {
    }

    // methods and properties


    public virtual void Saving (IDbConnection connection, IDbTransaction transaction, DataContainerCollection dataContainers)
    {
      ArgumentUtility.CheckNotNull ("connection", connection);
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNullOrItemsNull ("datacContainers", dataContainers);

      Type securityManagerDomainLayerSuperType = typeof (BaseSecurityManagerObject);
      foreach (DataContainer dataContainer in dataContainers)
      {
        if (securityManagerDomainLayerSuperType.IsAssignableFrom (dataContainer.DomainObjectType))
        {
          IncrementRevision (connection, transaction);
          return;
        }
      }
    }

    private void IncrementRevision (IDbConnection connection, IDbTransaction transaction)
    {
      using (IDbCommand command = connection.CreateCommand ())
      {
        command.Transaction = transaction;
        QueryDefinition queryDefintion = DomainObjectsConfiguration.Current.Query.QueryDefinitions.GetMandatory ("Remotion.SecurityManager.Domain.Revision.IncrementRevision");
        command.CommandText = queryDefintion.Statement;

        command.ExecuteNonQuery ();
      }
    }
  }
}
