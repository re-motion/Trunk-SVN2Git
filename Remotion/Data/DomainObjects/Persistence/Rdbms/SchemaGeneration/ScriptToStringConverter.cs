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
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  public class ScriptToStringConverter : IScriptToStringConverter
  {
    public struct ScriptPair
    {
      private readonly string _createScript;
      private readonly string _dropScript;

      public ScriptPair (string createScript, string dropScript)
      {
        ArgumentUtility.CheckNotNullOrEmpty ("createScript", createScript);
        ArgumentUtility.CheckNotNullOrEmpty ("dropScript", dropScript);

        _createScript = createScript;
        _dropScript = dropScript;
      }

      public string CreateScript
      {
        get { return _createScript; }
      }

      public string DropScript
      {
        get { return _dropScript; }
      }
    }

    public ScriptPair Convert (IScriptBuilder scriptBuilder)
    {
      ArgumentUtility.CheckNotNull ("scriptBuilder", scriptBuilder);

      var createScriptStatements = new List<ScriptStatement> ();
      var dropScriptStatements = new List<ScriptStatement> ();
      var createScriptCollection = scriptBuilder.GetCreateScript ();
      var dropScriptCollection = scriptBuilder.GetDropScript ();

      createScriptCollection.AppendToScript (createScriptStatements);
      dropScriptCollection.AppendToScript (dropScriptStatements);

      return new ScriptPair (
          createScriptStatements.Aggregate (new StringBuilder (), (sb, stmt) => sb.AppendLine (stmt.Statement)).ToString (),
          dropScriptStatements.Aggregate (new StringBuilder (), (sb, stmt) => sb.AppendLine (stmt.Statement)).ToString ());
    }
  }
}