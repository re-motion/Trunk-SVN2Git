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
  public abstract class ConstraintScriptBuilderBase : IScriptBuilder, IEntityDefinitionVisitor
  {
    private readonly StringBuilder _createConstraintStringBuilder;
    private readonly StringBuilder _dropConstraintStringBuilder;
    private readonly ISqlDialect _sqlDialect;

    protected ConstraintScriptBuilderBase (ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);

      _sqlDialect = sqlDialect;
      _createConstraintStringBuilder = new StringBuilder();
      _dropConstraintStringBuilder = new StringBuilder();
    }

    public abstract void AddToCreateConstraintScript (TableDefinition tableDefinition, StringBuilder createConstraintStringBuilder);
    public abstract void AddToDropConstraintScript (TableDefinition tableDefinition, StringBuilder dropConstraintStringBuilder);

    public string GetCreateScript ()
    {
      return _createConstraintStringBuilder.ToString();
    }

    public string GetDropScript ()
    {
      return _dropConstraintStringBuilder.ToString();
    }

    public void AddEntityDefinition (IEntityDefinition entityDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);

      entityDefinition.Accept (this);
    }

    protected string GetForeignKeyConstraintStatement (TableDefinition tableDefinition)
    {
      var visitor = new ForeignKeyDeclarationTableConstraintDefinitionVisitor (_sqlDialect);

      foreach (var constraint in tableDefinition.Constraints)
        constraint.Accept (visitor);

      return visitor.GetConstraintStatement ();
    }

    private void AddToCreateConstraintScript (TableDefinition tableDefinition)
    {
      if (_createConstraintStringBuilder.Length != 0)
        _createConstraintStringBuilder.Append ("\r\n");
      var length = _createConstraintStringBuilder.Length;

      AddToCreateConstraintScript (tableDefinition, _createConstraintStringBuilder);

      if (_createConstraintStringBuilder.Length == length && length > 1)
        _createConstraintStringBuilder.Remove (length - 2, 2);
    }

    private void AddToDropConstraintScript (TableDefinition tableDefinition)
    {
      if (_dropConstraintStringBuilder.Length != 0)
        _dropConstraintStringBuilder.Append ("\r\n");
      var length = _dropConstraintStringBuilder.Length;

      AddToDropConstraintScript (tableDefinition, _dropConstraintStringBuilder);

      if (_dropConstraintStringBuilder.Length == length && length > 1)
        _dropConstraintStringBuilder.Remove (length - 2, 2);
    }

    void IEntityDefinitionVisitor.VisitTableDefinition (TableDefinition tableDefinition)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);
      
      AddToCreateConstraintScript (tableDefinition);
      AddToDropConstraintScript (tableDefinition);
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