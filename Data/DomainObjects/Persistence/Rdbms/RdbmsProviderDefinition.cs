using System;
using System.Collections.Specialized;
using System.Configuration;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class RdbmsProviderDefinition: StorageProviderDefinition
  {
    private string _connectionString;

    public RdbmsProviderDefinition (
        string name,
        Type storageProviderType,
        string connectionString)
        : base (name, storageProviderType)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("connectionString", connectionString);
      _connectionString = connectionString;
    }

    public RdbmsProviderDefinition (string name, NameValueCollection config)
        : base (name, config)
    {
      ArgumentUtility.CheckNotNull ("config", config);

      string connectionStringName = GetAndRemoveNonEmptyStringAttribute (config, "connectionString", name, true);
      _connectionString = ConfigurationWrapper.Current.GetConnectionString (connectionStringName, true).ConnectionString;
    }

    public string ConnectionString
    {
      get { return _connectionString; }
    }

    public override bool IsIdentityTypeSupported (Type identityType)
    {
      ArgumentUtility.CheckNotNull ("identityType", identityType);

      return (identityType == typeof (Guid));
    }
  }
}