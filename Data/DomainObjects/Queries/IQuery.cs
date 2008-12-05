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
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.Queries
{
/// <summary>
/// Represents an executable query.
/// </summary>
public interface IQuery
{
  /// <summary>
  /// Gets a unique identifier for the query.
  /// </summary>
  string ID { get; }

  /// <summary>
  /// Gets the statement of the query.
  /// </summary>
  /// <remarks>The statement must be understood by the <see cref="Remotion.Data.DomainObjects.Persistence.StorageProvider"/> responsible for executing the query.</remarks>
  string Statement { get; }

  /// <summary>
  /// Gets the unique ID of the <see cref="Remotion.Data.DomainObjects.Persistence.StorageProvider"/> responsible for executing the query.
  /// </summary>
  string StorageProviderID { get; }

  /// <summary>
  /// Gets the type of the collection if the query returns a collection of <see cref="Remotion.Data.DomainObjects.DomainObject"/>s.
  /// </summary> 
  Type CollectionType { get; }

  /// <summary>
  /// Gets the <see cref="Configuration.QueryType"/> of the query.
  /// </summary>
  QueryType QueryType { get; }

  /// <summary>
  /// Gets the <see cref="QueryParameter"/>s that are used to execute the query.
  /// </summary>
  QueryParameterCollection Parameters { get; }
}
}
