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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>
  /// Provides a common interface for classes defining the specifics of a SQL dialect. Used by <see cref="RdbmsProvider"/>.
  /// </summary>
  public interface ISqlDialect
  {
    /// <summary> A delimiter to end a SQL statement if the database requires one, an empty string otherwise. </summary>
    string StatementDelimiter { get; }

    string GetParameterName (string name);

    /// <summary> Surrounds an identifier with delimiters according to the database's syntax. </summary>
    string DelimitIdentifier (string identifier);


    
    
    /// <summary> A delimiter to end a SQL batch if the database. </summary>
    string BatchDelimiter { get; }

    /// <summary> Inserts the USE 'database'-statement at the beginning of the script. </summary>
    void AdjustForConnectionString (List<ScriptStatement> script, string connectionString);

  }
}