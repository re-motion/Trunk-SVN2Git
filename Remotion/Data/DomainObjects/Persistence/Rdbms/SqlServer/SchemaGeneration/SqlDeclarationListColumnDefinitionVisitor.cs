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

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// Visits <see cref="IColumnDefinition"/> objects and generates a list of SQL declarations for the visited columns.
  /// </summary>
  public class SqlDeclarationListColumnDefinitionVisitor : IColumnDefinitionVisitor
  {
    private readonly StringBuilder _columnList = new StringBuilder();

    public string GetDeclarationList ()
    {
      return _columnList.ToString();
    }

    public void VisitSimpleColumnDefinition (SimpleColumnDefinition simpleColumnDefinition)
    {
      ArgumentUtility.CheckNotNull ("simpleColumnDefinition", simpleColumnDefinition);

      if (_columnList.Length > 0)
        _columnList.Append (",\r\n");
      
      _columnList.AppendFormat (
          "  [{0}] {1}{2}",
          simpleColumnDefinition.Name,
          simpleColumnDefinition.StorageType,
          simpleColumnDefinition.IsNullable ? " NULL" : " NOT NULL");
    }

    public void VisitObjectIDWithClassIDColumnDefinition (ObjectIDWithClassIDColumnDefinition objectIDWithClassIDColumnDefinition)
    {
      ArgumentUtility.CheckNotNull ("objectIDWithClassIDColumnDefinition", objectIDWithClassIDColumnDefinition);

      objectIDWithClassIDColumnDefinition.ObjectIDColumn.Accept (this);
      objectIDWithClassIDColumnDefinition.ClassIDColumn.Accept (this);
    }

    public void VisitNullColumnDefinition (NullColumnDefinition nullColumnDefinition)
    {
      ArgumentUtility.CheckNotNull ("nullColumnDefinition", nullColumnDefinition);

      throw new NotSupportedException ("Cannot declare a non-existing column.");
    }
  }
}