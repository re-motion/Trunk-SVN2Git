// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
