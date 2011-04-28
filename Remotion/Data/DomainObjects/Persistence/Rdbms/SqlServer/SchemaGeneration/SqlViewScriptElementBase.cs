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
using System.Linq;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// <see cref="SqlViewScriptElementBase{T}"/> represents the base-class for all factory classes that are responsible to create new script elements 
  /// for a  relational database.
  /// </summary>
  public abstract class SqlViewScriptElementBase<T> : IScriptElementFactory<T> where T : IEntityDefinition
  {
    public abstract IScriptElement GetCreateElement (T entityDefinition);

    public virtual IScriptElement GetDropElement (T entityDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);

      return new ScriptStatement (
        string.Format (
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = '{1}' AND TABLE_SCHEMA = '{0}')\r\n"
          + "  DROP VIEW [{0}].[{1}]",
          entityDefinition.ViewName.SchemaName ?? CompositeScriptBuilder.DefaultSchema,
          entityDefinition.ViewName.EntityName
          ));
    }

    protected virtual bool UseSchemaBinding (T entityDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);
      return true;
    }

    protected virtual string GetColumnList (IEnumerable<SimpleColumnDefinition> columns)
    {
      return SeparatedStringBuilder.Build (", ", columns, cd => cd != null ? ("["+ cd.Name + "]") : "NULL");
    }
  }
}