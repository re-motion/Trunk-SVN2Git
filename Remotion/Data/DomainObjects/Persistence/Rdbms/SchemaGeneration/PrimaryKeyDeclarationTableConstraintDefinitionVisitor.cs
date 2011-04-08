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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// The <see cref="PrimaryKeyDeclarationTableConstraintDefinitionVisitor"/> visits the <see cref="PrimaryKeyConstraintDefinition"/> and generates 
  /// the  corresponding constraint statement for it.
  /// </summary>
  public class PrimaryKeyDeclarationTableConstraintDefinitionVisitor : ITableConstraintDefinitionVisitor
  {
    private string _constraintStatement;
    private readonly ISqlDialect _sqlDialect;

    public PrimaryKeyDeclarationTableConstraintDefinitionVisitor (ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);

      _sqlDialect = sqlDialect;
    }

    public void VisitPrimaryKeyConstraintDefinition (PrimaryKeyConstraintDefinition primaryKeyConstraintDefinition)
    {
      if (!string.IsNullOrEmpty (_constraintStatement))
        throw new InvalidOperationException ("Only one primary key constraint is allowed.");

      _constraintStatement = string.Format (
          "CONSTRAINT {0} PRIMARY KEY CLUSTERED ({1})",
          _sqlDialect.DelimitIdentifier (primaryKeyConstraintDefinition.ConstraintName),
          NameListColumnDefinitionVisitor.GetNameList (primaryKeyConstraintDefinition.Columns.Cast<IColumnDefinition>(), false, _sqlDialect));
    }

    public void VisitForeignKeyConstraintDefinition (ForeignKeyConstraintDefinition foreignKeyConstraintDefinition)
    {
      ArgumentUtility.CheckNotNull ("foreignKeyConstraintDefinition", foreignKeyConstraintDefinition);

      //Nothing to do here
    }

    public string GetConstraintStatement ()
    {
      return _constraintStatement;
    }
  }
}