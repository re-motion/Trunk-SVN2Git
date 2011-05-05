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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// <see cref="SqlDatabaseSelectionScriptElementBuilder"/> ist responsible to generate a sql-statement to change the database context to the 
  /// specified  database for a sql-server database.
  /// </summary>
  public class SqlDatabaseSelectionScriptElementBuilder : IScriptBuilder
  {
    private readonly string _connectionString;
    private readonly IScriptBuilder _innerScriptBuilder;

    public SqlDatabaseSelectionScriptElementBuilder (IScriptBuilder innerScriptBuilder, string connectionString)
    {
      ArgumentUtility.CheckNotNull ("innerScriptBuilder", innerScriptBuilder);
      ArgumentUtility.CheckNotNull ("connectionString", connectionString);

      _innerScriptBuilder = innerScriptBuilder;
      _connectionString = connectionString;
    }

    public IScriptBuilder InnerScriptBuilder
    {
      get { return _innerScriptBuilder; }
    }

    public void AddEntityDefinition (IEntityDefinition entityDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);

      _innerScriptBuilder.AddEntityDefinition (entityDefinition);
    }

    public ScriptElementCollection GetCreateScript ()
    {
      var createScriptElements = new ScriptElementCollection ();
      createScriptElements.AddElement (new ScriptStatement ("USE " + GetDatabaseName()));
      foreach (var scriptElement in _innerScriptBuilder.GetCreateScript ().Elements)
        createScriptElements.AddElement (scriptElement);

      return createScriptElements;
    }

    public ScriptElementCollection GetDropScript ()
    {
      var dropScriptElements = new ScriptElementCollection ();
      dropScriptElements.AddElement (new ScriptStatement ("USE " + GetDatabaseName ()));
      foreach (var scriptElement in _innerScriptBuilder.GetDropScript ().Elements)
        dropScriptElements.AddElement (scriptElement);

      return dropScriptElements;
    }

    private string GetDatabaseName ()
    {
      var initialCatalogMarker = "Initial Catalog=";

      if (!_connectionString.Contains (initialCatalogMarker))
        throw new InvalidOperationException ("No database-name could be found in the given connection-string.");

      var startIndex = _connectionString.IndexOf (initialCatalogMarker) + initialCatalogMarker.Length;
      var temp = _connectionString.Substring (startIndex);
      return temp.Substring (0, temp.IndexOf (";"));
    }
  }
}