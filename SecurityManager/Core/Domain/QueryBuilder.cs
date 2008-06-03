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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  public abstract class QueryBuilder
  {
    // types

    // static members and constants

    // member fields

    private string _queryName;
    private Type _domainObjectResultType;
    private QueryType _queryType = QueryType.Collection;
    private Type _collectionType = typeof (DomainObjectCollection);
    private QueryParameterCollection _parameters;

    // construction and disposing

    protected QueryBuilder (string queryName, Type domainObjectResultType)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("queryName", queryName);
      ArgumentUtility.CheckNotNull ("domainObjectResultType", domainObjectResultType);

      _queryName = queryName;
      _domainObjectResultType = domainObjectResultType;
      _parameters = new QueryParameterCollection ();
    }

    // methods and properties

    public QueryType QueryType
    {
      get { return _queryType; }
    }

    public Type CollectionType
    {
      get { return _collectionType; }
      set { _collectionType = value; }
    }

    protected QueryParameterCollection Parameters
    {
      get { return _parameters; }
    }

    protected virtual Query CreateQueryFromStatement (string statement)
    {
      string storageProviderID = GetStorageProviderID ();
      QueryDefinition queryDefinition = new QueryDefinition (_queryName, storageProviderID, statement, _queryType, _collectionType);
      return new Query (queryDefinition, _parameters);
    }

    protected string GetStorageProviderID ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (_domainObjectResultType);
      return classDefinition.StorageProviderID;
    }
  }
}
