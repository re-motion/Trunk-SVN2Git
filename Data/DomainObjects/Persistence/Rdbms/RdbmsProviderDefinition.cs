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
