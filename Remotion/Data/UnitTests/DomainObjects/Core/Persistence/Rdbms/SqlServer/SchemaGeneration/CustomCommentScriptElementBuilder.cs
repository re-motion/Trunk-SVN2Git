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

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  public class CustomCommentScriptElementBuilder : IScriptBuilder
  {
    private readonly IScriptBuilder _innerScriptBuilder;
    private readonly string _customCreateBeginningScript;
    private readonly string _customCreateEndScript;
    private readonly string _customDropBeginningScript;
    private readonly string _customDropEndScript;

    public CustomCommentScriptElementBuilder (
        IScriptBuilder innerScriptBuilder,
        string customCreateBeginningScript,
        string customCreateEndScript,
        string customDropBeginningScript,
        string customDropEndScript)
    {
      ArgumentUtility.CheckNotNull ("innerScriptBuilder", innerScriptBuilder);
      ArgumentUtility.CheckNotNull ("customCreateBeginningScript", customCreateBeginningScript);
      ArgumentUtility.CheckNotNull ("customCreateEndScript", customCreateEndScript);
      ArgumentUtility.CheckNotNull ("customDropBeginningScript", customCreateBeginningScript);
      ArgumentUtility.CheckNotNull ("customDropEndScript", customCreateEndScript);

      _innerScriptBuilder = innerScriptBuilder;
      _customCreateBeginningScript = customCreateBeginningScript;
      _customCreateEndScript = customCreateEndScript;
      _customDropBeginningScript = customDropBeginningScript;
      _customDropEndScript = customDropEndScript;
    }

    public void AddEntityDefinition (IEntityDefinition entityDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);

      _innerScriptBuilder.AddEntityDefinition (entityDefinition);
    }

    public ScriptElementCollection GetCreateScript ()
    {
      var createScriptElements = new ScriptElementCollection();
      createScriptElements.AddElement (new ScriptStatement (_customCreateBeginningScript));
      foreach (var scriptElement in _innerScriptBuilder.GetCreateScript().Elements)
        createScriptElements.AddElement (scriptElement);
      createScriptElements.AddElement (new ScriptStatement (_customDropEndScript));

      return createScriptElements;
    }

    public ScriptElementCollection GetDropScript ()
    {
      var dropScriptElements = new ScriptElementCollection();
      dropScriptElements.AddElement (new ScriptStatement (_customCreateEndScript));
      foreach (var scriptElement in _innerScriptBuilder.GetDropScript().Elements)
        dropScriptElements.AddElement (scriptElement);
      dropScriptElements.AddElement (new ScriptStatement (_customDropEndScript));

      return dropScriptElements;
    }
  }
}