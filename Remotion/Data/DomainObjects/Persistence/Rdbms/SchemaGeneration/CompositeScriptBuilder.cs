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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// <see cref="CompositeScriptBuilder"/> contains database-independent code to generate database-scripts for a relational database.
  /// </summary>
  public class CompositeScriptBuilder : IScriptBuilder
  {
    private readonly IScriptBuilder[] _scriptBuilders;
    private readonly RdbmsProviderDefinition _rdbmsProviderDefinition;
    private readonly ISqlDialect _sqlDialect;

    public CompositeScriptBuilder (RdbmsProviderDefinition rdbmsProviderDefinition, ISqlDialect sqlDialect, params IScriptBuilder[] scriptBuilders)
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

    public virtual void AddEntityDefinition (IEntityDefinition entityDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);

      foreach (var scriptBuilder in _scriptBuilders)
        scriptBuilder.AddEntityDefinition (entityDefinition);
    }

    public virtual ScriptElementCollection GetCreateScript ()
    {
      return GetFullScriptCollection (_scriptBuilders.Select (builder => builder.GetCreateScript ()));
    }

    public virtual ScriptElementCollection GetDropScript ()
    {
      return GetFullScriptCollection (_scriptBuilders.Reverse().Select (builder => builder.GetDropScript()));
    }

    private ScriptElementCollection GetFullScriptCollection (IEnumerable<ScriptElementCollection> elementCollection)
    {
      var script = new List<ScriptStatement> ();
      foreach (var partialScript in elementCollection)
        partialScript.AppendToScript (script);

      _sqlDialect.AdjustForConnectionString (script, _rdbmsProviderDefinition.ConnectionString);

      var scriptElementCollection = new ScriptElementCollection();
      foreach (var statement in script)
        scriptElementCollection.AddElement (statement);

      return scriptElementCollection;
    }
  }
}