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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  public class SqlCompositeScriptBuilder : ScriptBuilderBase, IScriptBuilder
  {
    private readonly IScriptBuilder[] _scriptBuilders;
    public const string DefaultSchema = "dbo";

    // TODO: Add tests using mocks as soon as interfaces can be used for partial script builders
    public SqlCompositeScriptBuilder (
        RdbmsProviderDefinition rdbmsProviderDefinition,
        params IScriptBuilder[] scriptBuilders)
      : base (ArgumentUtility.CheckNotNull ("rdbmsProviderDefinition", rdbmsProviderDefinition), SqlServer.SqlDialect.Instance)
    {
      ArgumentUtility.CheckNotNull ("scriptBuilders", scriptBuilders);
     
      _scriptBuilders = scriptBuilders;
    }

    public IScriptBuilder[] ScriptBuilders
    {
      get { return _scriptBuilders; }
    }

    public string GetDatabaseName ()
    {
      //TODO improve this logic
      var initialCatalogMarker = "Initial Catalog=";
      var startIndex = RdbmsProviderDefinition.ConnectionString.IndexOf (initialCatalogMarker) + initialCatalogMarker.Length;
      string temp = RdbmsProviderDefinition.ConnectionString.Substring (startIndex);
      return temp.Substring (0, temp.IndexOf (";"));
    }

    public override void AddEntityDefinition (IEntityDefinition entityDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);

      foreach (var scriptBuilder in ScriptBuilders)
        scriptBuilder.AddEntityDefinition (entityDefinition);
    }

    public override string GetCreateScript ()
    {
      var createScript = new StringBuilder();
      createScript.AppendFormat ("USE {0}\r\n", GetDatabaseName());
      SqlDialect.AddBatchForScript(createScript);

      foreach (var scriptBuilder in ScriptBuilders)
      {
        createScript.Append(scriptBuilder.GetCreateScript());
        SqlDialect.AddBatchForScript (createScript);
      }

      return createScript.ToString();
    }

    public override string GetDropScript ()
    {
      var dropScript = new StringBuilder ();
      dropScript.AppendFormat ("USE {0}\r\n", GetDatabaseName ());
      SqlDialect.AddBatchForScript (dropScript);

      foreach (var scriptBuilder in ScriptBuilders.Reverse())
      {
        dropScript.Append (scriptBuilder.GetDropScript ());
        SqlDialect.AddBatchForScript (dropScript);
      }

      return dropScript.ToString ();
    }
  }
}