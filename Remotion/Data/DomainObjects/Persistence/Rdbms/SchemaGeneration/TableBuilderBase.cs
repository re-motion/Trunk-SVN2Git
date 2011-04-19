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
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// Contains database-independent code for generating tables for the persistence model.
  /// </summary>
  public abstract class TableBuilderBase : IEntityDefinitionVisitor
  {
    private readonly StringBuilder _createTableStringBuilder;
    private readonly StringBuilder _dropTableStringBuilder;
    private readonly ISqlDialect _sqlDialect;

    protected TableBuilderBase (ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);

      _sqlDialect = sqlDialect;
      _createTableStringBuilder = new StringBuilder();
      _dropTableStringBuilder = new StringBuilder();
    }

    public abstract void AddToCreateTableScript (TableDefinition tableDefinition, StringBuilder createTableStringBuilder);
    public abstract void AddToDropTableScript (TableDefinition tableDefinition, StringBuilder dropTableStringBuilder);

    public string GetCreateTableScript ()
    {
      return _createTableStringBuilder.ToString();
    }

    public string GetDropTableScript ()
    {
      return _dropTableStringBuilder.ToString();
    }

    public void AddTable (IEntityDefinition entityDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);

      entityDefinition.Accept (this);
    }

    private void AddToCreateTableScript (TableDefinition tableDefinition)
    {
      if (_createTableStringBuilder.Length != 0)
        _createTableStringBuilder.Append ("\r\n");

      AddToCreateTableScript (tableDefinition, _createTableStringBuilder);
    }

    private void AddToDropTableScript (TableDefinition tableDefinition)
    {
      if (_dropTableStringBuilder.Length != 0)
        _dropTableStringBuilder.Append ("\r\n");

      AddToDropTableScript (tableDefinition, _dropTableStringBuilder);
    }

    protected string GetColumnList (TableDefinition tableDefinition)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);

      return DeclarationListColumnDefinitionVisitor.GetDeclarationList (tableDefinition.Columns, _sqlDialect);
    }

    protected string GetPrimaryKeyConstraintStatement (TableDefinition tableDefinition)
    {
      var visitor = new PrimaryKeyDeclarationTableConstraintDefinitionVisitor (_sqlDialect);

      foreach (var constraint in tableDefinition.Constraints)
        constraint.Accept (visitor);

      return visitor.GetConstraintStatement();
    }

    void IEntityDefinitionVisitor.VisitTableDefinition (TableDefinition tableDefinition)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);

      AddToCreateTableScript (tableDefinition);
      AddToDropTableScript (tableDefinition);
    }

    void IEntityDefinitionVisitor.VisitUnionViewDefinition (UnionViewDefinition unionViewDefinition)
    {
      ArgumentUtility.CheckNotNull ("unionViewDefinition", unionViewDefinition);

      //Nothing to do here
    }

    void IEntityDefinitionVisitor.VisitFilterViewDefinition (FilterViewDefinition filterViewDefinition)
    {
      ArgumentUtility.CheckNotNull ("filterViewDefinition", filterViewDefinition);

      //Nothing to do here
    }

    void IEntityDefinitionVisitor.VisitNullEntityDefinition (NullEntityDefinition nullEntityDefinition)
    {
      ArgumentUtility.CheckNotNull ("nullEntityDefinition", nullEntityDefinition);

      //Nothing to do here
    }
  }
}