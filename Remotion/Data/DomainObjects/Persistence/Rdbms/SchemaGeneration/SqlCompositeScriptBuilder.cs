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
using System.Linq;
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// Acts as an abstract base class for implementations generating a setup script for a list of entity definitions.
  /// </summary>
  public class SqlCompositeScriptBuilder : IScriptBuilder
  {
    public const string DefaultSchema = "dbo";
    
    private readonly RdbmsProviderDefinition _rdbmsProviderDefinition;
    private readonly ISqlDialect _sqlDialect;
    private readonly IScriptBuilder[] _scriptBuilders;
    
    public SqlCompositeScriptBuilder (
        RdbmsProviderDefinition rdbmsProviderDefinition, ISqlDialect sqlDialect, params IScriptBuilder[] scriptBuilders)
    {
      ArgumentUtility.CheckNotNull ("rdbmsProviderDefinition", rdbmsProviderDefinition);
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);
      ArgumentUtility.CheckNotNull ("scriptBuilders", scriptBuilders);

      _rdbmsProviderDefinition = rdbmsProviderDefinition;
      _sqlDialect = sqlDialect;
      _scriptBuilders = scriptBuilders;
    }

    public RdbmsProviderDefinition RdbmsProviderDefinition
    {
      get { return _rdbmsProviderDefinition; }
    }

    public ISqlDialect SqlDialect
    {
      get { return _sqlDialect; }
    }

    public IScriptBuilder[] ScriptBuilders
    {
      get { return _scriptBuilders; }
    }

    public void AddEntityDefinition (IEntityDefinition entityDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);

      foreach (var scriptBuilder in ScriptBuilders)
        scriptBuilder.AddEntityDefinition (entityDefinition);
    }

    public string GetCreateScript ()
    {
      var createScript = new StringBuilder();
      SqlDialect.CreateScriptForConnectionString (createScript, RdbmsProviderDefinition.ConnectionString);
      SqlDialect.AddBatchForScript (createScript);

      foreach (var scriptBuilder in ScriptBuilders)
      {
        createScript.Append (scriptBuilder.GetCreateScript());
        SqlDialect.AddBatchForScript (createScript);
      }

      return createScript.ToString();
    }

    public string GetDropScript ()
    {
      var dropScript = new StringBuilder();
      SqlDialect.CreateScriptForConnectionString (dropScript, RdbmsProviderDefinition.ConnectionString);
      SqlDialect.AddBatchForScript (dropScript);

      foreach (var scriptBuilder in ScriptBuilders.Reverse())
      {
        dropScript.Append (scriptBuilder.GetDropScript());
        SqlDialect.AddBatchForScript (dropScript);
      }

      return dropScript.ToString();
    }
  }
}