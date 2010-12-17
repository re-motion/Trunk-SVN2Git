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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  public abstract class TableBuilderBase
  {
    private readonly StringBuilder _createTableStringBuilder;
    private readonly StringBuilder _dropTableStringBuilder;

    protected TableBuilderBase ()
    {
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

    public void AddTables (ClassDefinitionCollection classes)
    {
      ArgumentUtility.CheckNotNull ("classes", classes);

      foreach (ClassDefinition currentClass in classes)
        AddTable (currentClass);
    }

    public void AddTable (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var tableDefinition = classDefinition.StorageEntityDefinition as TableDefinition;
      if (tableDefinition!=null)
      {
        AddToCreateTableScript (tableDefinition);
        AddToDropTableScript (tableDefinition);
      }
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

  }
}