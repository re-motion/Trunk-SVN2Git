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
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// The <see cref="ForeignKeyDeclarationTableConstraintDefinitionVisitor"/> visits the <see cref="ForeignKeyConstraintDefinition"/>s and generates 
  /// the  corresponding constraint statement for it.
  /// </summary>
  public class ForeignKeyDeclarationTableConstraintDefinitionVisitor : ITableConstraintDefinitionVisitor
  {
    private readonly ISqlDialect _sqlDialect;
    private readonly StringBuilder _constraints;

    public ForeignKeyDeclarationTableConstraintDefinitionVisitor (ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);

      _sqlDialect = sqlDialect;
      _constraints = new StringBuilder(0);
    }

    public void VisitPrimaryKeyConstraintDefinition (PrimaryKeyConstraintDefinition primaryKeyConstraintDefinition)
    {
      ArgumentUtility.CheckNotNull ("primaryKeyConstraintDefinition", primaryKeyConstraintDefinition);

      //Nothing to do here
    }

    public void VisitForeignKeyConstraintDefinition (ForeignKeyConstraintDefinition foreignKeyConstraintDefinition)
    {
      ArgumentUtility.CheckNotNull ("foreignKeyConstraintDefinition", foreignKeyConstraintDefinition);

      // TODO Review 3627: Rename Referenced <-> Referencing in all classes using ForeignKeyConstraintDefinition
      string constraint = GetConstraintDeclaration (foreignKeyConstraintDefinition);

      if (_constraints.Length > 0)
      {
        _constraints.Append (_sqlDialect.ConstraintDelimiter);
        _constraints.AppendLine();
        _constraints.Append (" ");
      }
      _constraints.Append (constraint);
    }

    private string GetConstraintDeclaration (ForeignKeyConstraintDefinition foreignKeyConstraintDefinition)
    {
      string referencedColumnNameList = GetColumnNameList (foreignKeyConstraintDefinition.ReferencedColumns);
      string referencingColumnNameList = GetColumnNameList (foreignKeyConstraintDefinition.ReferencingColumns);

      return string.Format (
          " CONSTRAINT {0} FOREIGN KEY ({1}) REFERENCES {2}.{3} ({4})",
          _sqlDialect.DelimitIdentifier (foreignKeyConstraintDefinition.ConstraintName),
          referencedColumnNameList,
          _sqlDialect.DelimitIdentifier (foreignKeyConstraintDefinition.ReferencedTableName.SchemaName ?? ScriptBuilder.DefaultSchema),
          _sqlDialect.DelimitIdentifier (foreignKeyConstraintDefinition.ReferencedTableName.EntityName),
          referencingColumnNameList);
    }

    private string GetColumnNameList (IEnumerable<SimpleColumnDefinition> columnDefinitions)
    {
      return NameListColumnDefinitionVisitor.GetNameList (columnDefinitions.Cast<IColumnDefinition>(), false, _sqlDialect);
    }

    public string GetConstraintStatement ()
    {
      return _constraints.ToString();
    }
  }
}