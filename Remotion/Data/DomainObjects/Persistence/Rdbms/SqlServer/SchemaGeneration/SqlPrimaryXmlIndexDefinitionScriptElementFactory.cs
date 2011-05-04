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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// The <see cref="SqlPrimaryXmlIndexDefinitionScriptElementFactory"/> is responsible to create script-elements for primary-xml indexes in a 
  /// sql-server database.
  /// </summary>
  public class SqlPrimaryXmlIndexDefinitionScriptElementFactory : SqlIndexScriptElementFactoryBase<SqlPrimaryXmlIndexDefinition>
  {
    public override IScriptElement GetCreateElement (SqlPrimaryXmlIndexDefinition indexDefinition)
    {
      ArgumentUtility.CheckNotNull ("indexDefinition", indexDefinition);

      return new ScriptStatement(
      string.Format (
          "CREATE PRIMARY XML INDEX [{0}]\r\n"
          + "  ON [{1}].[{2}] ([{3}]){4}",
          indexDefinition.IndexName,
          indexDefinition.ObjectName.SchemaName ?? CompositeScriptBuilder2.DefaultSchema,
          indexDefinition.ObjectName.EntityName,
          indexDefinition.XmlColumn.Name,
          GetCreateIndexOptions (GetCreateIndexOptionItems (indexDefinition))));
    }
  }
}