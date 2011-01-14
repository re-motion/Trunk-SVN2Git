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
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  public abstract class ConstraintBuilderBase
  {
    private readonly StringBuilder _createConstraintStringBuilder;
    private readonly List<string> _entityNamesForDropConstraintScript;
    private readonly ISqlDialect _sqlDialect;

    protected ConstraintBuilderBase (ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);

      _sqlDialect = sqlDialect;
      _createConstraintStringBuilder = new StringBuilder();
      _entityNamesForDropConstraintScript = new List<string>();
    }

    public abstract void AddToCreateConstraintScript (TableDefinition tableDefinition, StringBuilder createConstraintStringBuilder);
    public abstract void AddToDropConstraintScript (List<string> entityNamesForDropConstraintScript, StringBuilder dropConstraintStringBuilder);

    public string GetAddConstraintScript ()
    {
      return _createConstraintStringBuilder.ToString();
    }

    public string GetDropConstraintScript ()
    {
      if (_entityNamesForDropConstraintScript.Count == 0)
        return string.Empty;

      StringBuilder dropConstraintStringBuilder = new StringBuilder();
      AddToDropConstraintScript (_entityNamesForDropConstraintScript, dropConstraintStringBuilder);
      return dropConstraintStringBuilder.ToString();
    }

    public void AddConstraints (ClassDefinitionCollection classes)
    {
      ArgumentUtility.CheckNotNull ("classes", classes);

      foreach (ClassDefinition currentClass in classes)
        AddConstraint (currentClass);
    }

    public void AddConstraint (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var tableDefinition = classDefinition.StorageEntityDefinition as TableDefinition;
      if (tableDefinition!=null)
      {
        AddToCreateConstraintScript (tableDefinition);
        _entityNamesForDropConstraintScript.Add (tableDefinition.TableName);
      }
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
      int length = _createConstraintStringBuilder.Length;

      AddToCreateConstraintScript (tableDefinition, _createConstraintStringBuilder);

      if (_createConstraintStringBuilder.Length == length && length > 1)
        _createConstraintStringBuilder.Remove (length - 2, 2);
    }
    
  }
}