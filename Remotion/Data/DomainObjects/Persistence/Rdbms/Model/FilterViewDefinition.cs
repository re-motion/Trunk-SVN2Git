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
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// <see cref="FilterViewDefinition"/> defines a filtered view in a relational database.
  /// </summary>
  public class FilterViewDefinition : IEntityDefinition
  {
    private readonly string _viewName;
    private readonly IEntityDefinition _baseEntity;
    private readonly string _classID;
    private readonly ReadOnlyCollection<ColumnDefinition> _columns;

    public FilterViewDefinition (string viewName, IEntityDefinition baseEntity, string classID, IEnumerable<string> includedColumnNames)
    {
      ArgumentUtility.CheckNotNull ("baseEntity", baseEntity);
      ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);
      ArgumentUtility.CheckNotNull ("includedColumnNames", includedColumnNames);

      _viewName = viewName;
      _baseEntity = baseEntity;
      _classID = classID;

      var includedColumnNamesSet = new HashSet<string> (includedColumnNames);
      _columns = _baseEntity.GetColumns().Where (column => includedColumnNamesSet.Contains (column.Name)).ToList().AsReadOnly();

      if (_columns.Count < includedColumnNamesSet.Count)
      {
        var columnsNotFound = string.Join(", ", includedColumnNamesSet.Where (ic => !_baseEntity.GetColumns().Select (cd => cd.Name).Contains (ic)).ToArray());
        throw new ArgumentException (string.Format ("Following column names could not be found: '{0}'", columnsNotFound));
      }
    }

    public string ViewName
    {
      get { return _viewName; }
    }

    public IEntityDefinition BaseEntity
    {
      get { return _baseEntity; }
    }

    public string ClassID
    {
      get { return _classID; }
    }

    public string LegacyEntityName
    {
      get { return null; }
    }

    public ReadOnlyCollection<ColumnDefinition> GetColumns ()
    {
      return _columns;
    }
  }
}