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
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Linq.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer
{
  /// <summary>
  /// Defines the <see cref="ISqlDialect"/> for MS SQL Server.
  /// </summary>
  public class SqlDialect : ISqlDialect
  {
    public static readonly SqlDialect Instance = new SqlDialect ();

    protected SqlDialect ()
    {
    }

    public virtual string StatementDelimiter
    {
      get { return ";"; }
    }

    public string BatchDelimiter
    {
      get { return "GO"; }
    }

    public virtual string GetParameterName (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      if (name.StartsWith ("@"))
        return name;
      else
        return "@" + name;
    }

    public virtual string DelimitIdentifier (string identifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("identifier", identifier);

      return "[" + identifier + "]";
    }

    public void AdjustForConnectionString (List<ScriptStatement> script, string connectionString)
    {
      var initialCatalogMarker = "Initial Catalog=";

      if (!connectionString.Contains (initialCatalogMarker))
        throw new InvalidOperationException ("No database-name could be found in the given connection-string.");

      var startIndex = connectionString.IndexOf (initialCatalogMarker) + initialCatalogMarker.Length;
      var temp = connectionString.Substring (startIndex);
      var databaseName = temp.Substring (0, temp.IndexOf (";"));

      script.Insert (0, new ScriptStatement ("USE " + databaseName));
    }
    
  }
}