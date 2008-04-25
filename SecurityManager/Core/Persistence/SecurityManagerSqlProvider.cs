using System;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms;

namespace Remotion.SecurityManager.Persistence
{
  public class SecurityManagerSqlProvider : SqlProvider
  {
    // constants

    // types

    // static members

    // member fields

    private RevisionStorageProviderExtension _revisionExtension;

    // construction and disposing

    public SecurityManagerSqlProvider (RdbmsProviderDefinition definition) 
      : base (definition)
    {
      _revisionExtension = new RevisionStorageProviderExtension ();
    }

    // methods and properties

    public override void Save (DataContainerCollection dataContainers)
    {
      _revisionExtension.Saving (Connection, Transaction , dataContainers);
      base.Save (dataContainers);
    }
  }
}