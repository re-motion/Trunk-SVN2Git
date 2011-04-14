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
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// Contains database-independent code for generating indexes for the persistence model.
  /// </summary>
  public abstract class IndexBuilderBase : IIndexDefinitionVisitor
  {
    private readonly StringBuilder _createIndexStringBuilder;
    private readonly StringBuilder _dropIndexStringBuilder;
    private readonly ISqlDialect _sqlDialect;

    protected IndexBuilderBase (ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);

      _sqlDialect = sqlDialect;
      _createIndexStringBuilder = new StringBuilder();
      _dropIndexStringBuilder = new StringBuilder();
    }

    public abstract string IndexStatementSeparator { get; }
    
    protected StringBuilder CreateIndexStringBuilder
    {
      get { return _createIndexStringBuilder; }
    }

    protected StringBuilder DropIndexStringBuilder
    {
      get { return _dropIndexStringBuilder; }
    }

    protected ISqlDialect SqlDialect
    {
      get { return _sqlDialect; }
    }

    public string GetCreateIndexScript ()
    {
      return _createIndexStringBuilder.ToString ();
    }

    public string GetDropIndexScript ()
    {
      return _dropIndexStringBuilder.ToString ();
    }

    public void AddIndexes (IEntityDefinition entityDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);

      foreach (var indexDefinition in entityDefinition.Indexes)
      {
        indexDefinition.Accept (this);
      }
    }

    protected string GetColumnList (IEnumerable<IColumnDefinition> columnDefinitions, bool allowNulls)
    {
      return NameListColumnDefinitionVisitor.GetNameList (columnDefinitions, allowNulls, _sqlDialect);
    }

    protected void AppendSeparator ()
    {
      if (_createIndexStringBuilder.Length != 0)
        _createIndexStringBuilder.Append (IndexStatementSeparator);
      if (_dropIndexStringBuilder.Length != 0)
        _dropIndexStringBuilder.Append (IndexStatementSeparator);
    }
    
  }
}