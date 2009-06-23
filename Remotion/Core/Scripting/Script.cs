// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using Remotion.Utilities;

namespace Remotion.Scripting
{
  /// <summary>
  /// Represents a re-motion script which knows its <see cref="ScriptContext"/> and <see cref="ScriptingHost.ScriptLanguageType"/>.
  /// </summary>
  public class Script
  {
    private string _scriptText;
    private readonly ScriptingHost.ScriptLanguageType _scriptLanguageType;
    private readonly ScriptContext _scriptContext;

    public Script (ScriptContext scriptContext, ScriptingHost.ScriptLanguageType scriptLanguageType, string scriptText)
    {
      ArgumentUtility.CheckNotNull ("scriptContext", scriptContext);
      // Note: null/empty script text is allowed. 
      _scriptContext = scriptContext;
      _scriptLanguageType = scriptLanguageType;
      _scriptText = scriptText;
    }


    public string ScriptText
    {
      get { return _scriptText; }
      set { _scriptText = value; }
    }

    public ScriptingHost.ScriptLanguageType ScriptLanguageType
    {
      get { return _scriptLanguageType; }
    }

    public ScriptContext ScriptContext
    {
      get { return _scriptContext; }
    }
  }
}