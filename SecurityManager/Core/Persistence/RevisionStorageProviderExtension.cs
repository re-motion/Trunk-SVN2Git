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