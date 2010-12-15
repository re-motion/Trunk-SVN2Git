// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// <see cref="TableDefinition"/> defines a table in a relational database.
  /// </summary>
  public class TableDefinition : IEntityDefinition
  {
    private readonly StorageProviderDefinition _storageProviderDefinition;
    private readonly string _tableName;
    private readonly string _viewName;
    private readonly ReadOnlyCollection<IColumnDefinition> _columns;

    public TableDefinition (StorageProviderDefinition storageProviderDefinition, string tableName, string viewName, IEnumerable<IColumnDefinition> columns)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("tableName", tableName);
      ArgumentUtility.CheckNotEmpty ("viewName", viewName);
      ArgumentUtility.CheckNotNull ("columns", columns);

      _storageProviderDefinition = storageProviderDefinition;
      _tableName = tableName;
      _viewName = viewName;
      _columns = columns.ToList().AsReadOnly();
    }

    public string StorageProviderID
    {
      get { return StorageProviderDefinition.Name; }
    }

    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
    }

    public string TableName
    {
      get { return _tableName; }
    }

    public string ViewName
    {
      get { return _viewName; }
    }

    public string LegacyEntityName
    {
      get { return _tableName; }
    }

    public string LegacyViewName
    {
      get { return _viewName; }
    }

    public ReadOnlyCollection<IColumnDefinition> GetColumns ()
    {
      return _columns;
    }

    public void Accept (IEntityDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.VisitTableDefinition (this);
    }
  }
}