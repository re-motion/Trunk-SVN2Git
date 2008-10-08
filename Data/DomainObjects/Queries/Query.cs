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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries
{
/// <summary>
/// Represents a default implementation of <see cref="IQuery"/>.
/// </summary>
[Serializable] 
public class Query : IQuery
{
  // types

  // static members and constants

  // member fields

  private readonly QueryDefinition _definition;
  private readonly QueryParameterCollection _parameters;

  // construction and disposing

  /// <summary>
  /// Initializes a new instance of the <see cref="Query"/> class using a pre-defined query.
  /// </summary>
  /// <param name="queryID">The <paramref name="queryID"/> of the query definition from queries.xml to use.</param>
  /// <exception cref="Configuration.QueryConfigurationException"><paramref name="queryID"/> could not be found in the <see cref="Configuration.QueryConfiguration"/>.</exception>
  public Query (string queryID) : this (queryID, new QueryParameterCollection ()) 
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Query"/> class using a pre-defined query and a given collection of <see cref="QueryParameter"/>s.
  /// </summary>
  /// <param name="queryID">The <paramref name="queryID"/> of the query definition from queries.xml to use.</param>
  /// <param name="parameters">The <see cref="QueryParameter"/>s to use to execute the query. Must not be <see langword="null"/>.</param>
  /// <exception cref="Configuration.QueryConfigurationException"><paramref name="queryID"/> could not be found in the <see cref="Configuration.QueryConfiguration"/>.</exception>
  /// <exception cref="System.ArgumentNullException">
  ///   <paramref name="parameters"/> is <see langword="null"/>.
  /// </exception>
  public Query (string queryID, QueryParameterCollection parameters) 
      : this (DomainObjectsConfiguration.Current.Query.QueryDefinitions.GetMandatory (queryID), parameters)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Query"/> class using a <see cref="Configuration.QueryDefinition"/>.
  /// </summary>
  /// <param name="definition">The <see cref="Configuration.QueryDefinition"/> to use for the query. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <paramref name="definition"/> is <see langword="null"/>.
  /// </exception>
  public Query (QueryDefinition definition) : this (definition, new QueryParameterCollection ())
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Query"/> class using a <see cref="Configuration.QueryDefinition"/> and a given collection of <see cref="QueryParameter"/>s.
  /// </summary>
  /// <param name="definition">The <see cref="Configuration.QueryDefinition"/> to use for the query.</param>
  /// <param name="parameters">The <see cref="QueryParameter"/>s to use for executing the query.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <paramref name="definition"/> is <see langword="null"/>.<br /> -or- <br />
  ///   <paramref name="parameters"/> is <see langword="null"/>.
  /// </exception>
  public Query (QueryDefinition definition, QueryParameterCollection parameters)
  {
    ArgumentUtility.CheckNotNull ("definition", definition);
    ArgumentUtility.CheckNotNull ("parameters", parameters);

    _definition = definition;
    _parameters = parameters;
  }

  // methods and properties

  /// <summary>
  /// Gets the <see cref="Configuration.QueryDefinition"/> that is associated with the query.
  /// </summary>
  public QueryDefinition Definition 
  {
    get { return _definition; }
  }

  /// <summary>
  /// Gets the <see cref="Configuration.QueryDefinition.ID"/> of the associated <see cref="Configuration.QueryDefinition"/>.
  /// </summary>
  public string ID
  {
    get { return _definition.ID; }
  }

  /// <summary>
  /// Gets the <see cref="Configuration.QueryDefinition.CollectionType"/> of the associated <see cref="Configuration.QueryDefinition"/>.
  /// </summary>
  public Type CollectionType 
  {
    get { return _definition.CollectionType; }
  }

  /// <summary>
  /// Gets the <see cref="Configuration.QueryDefinition.QueryType"/> of the associated <see cref="Configuration.QueryDefinition"/>.
  /// </summary>
  public QueryType QueryType 
  {
    get { return _definition.QueryType; }
  }

  /// <summary>
  /// Gets the <see cref="Configuration.QueryDefinition.Statement"/> of the associated <see cref="Configuration.QueryDefinition"/>.
  /// </summary>
  public string Statement 
  {
    get { return _definition.Statement; }
  }

  /// <summary>
  /// Gets the <see cref="Configuration.QueryDefinition.StorageProviderID"/> of the associated <see cref="Configuration.QueryDefinition"/>.
  /// </summary>
  public string StorageProviderID
  {
    get { return _definition.StorageProviderID; }
  }

  /// <summary>
  /// Gets the <see cref="QueryParameter"/>s that are used to execute the <see cref="Query"/>.
  /// </summary>
  public QueryParameterCollection Parameters
  {
    get { return _parameters; }
  }
}
}
