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
using System;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Remotion.Diagnostics.ToText;
using Remotion.Utilities;

namespace Remotion.Scripting
{
  /// <summary>
  /// Wrapper around a DLR <see cref="Microsoft.Scripting.Hosting.ScriptScope"/>. 
  /// A <see cref="ScriptEnvironment"/> contains the symbols (DLR: objects) a script which runs in this environment can see.
  /// </summary>
  public class ScriptEnvironment
  {
    private readonly ScriptScope _scriptScope;

    /// <summary>
    /// Default ctor. Recommended way to create a <see cref="ScriptEnvironment"/>.
    /// </summary>
    public ScriptEnvironment ()
      : this (ScriptingHost.GetScriptEngine (ScriptingHost.ScriptLanguageType.Python).CreateScope ())
    {
    }
    
    /// <summary>
    /// Wraps the passed <see cref="ScriptScope"/> in a <see cref="ScriptEnvironment"/>. Only use this ctor if you know what you are doing.
    /// </summary>
    /// <param name="scriptScope"></param>
    public ScriptEnvironment (ScriptScope scriptScope)
    {
      ArgumentUtility.CheckNotNull ("scriptScope", scriptScope);
      _scriptScope = scriptScope;
    }


    public ScriptScope ScriptScope
    {
      get { return _scriptScope; }
    }


    /// <summary>
    /// Imports the passed symbols from the given namespace in the given assembly into the <see cref="ScriptEnvironment"/>. 
    /// </summary>
    /// <param name="assembly">Partial name of the assembly to import from (e.g. "Remotion")</param>
    /// <param name="nameSpace">Namespace name in assembly to import from (e.g. "Remotion.Diagnostics.ToText")</param>
    /// <param name="symbols">array of symbol names to import (e.g. "To", "ToTextBuilder", ...)</param>
    public void Import (string assembly, string nameSpace, params string[] symbols)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("assembly", assembly);
      ArgumentUtility.CheckNotNullOrEmpty ("nameSpace", nameSpace);
      ArgumentUtility.CheckNotNullOrEmpty ("symbols", symbols);

      // TODO: Implement Stringify(sb,ss,se,elementToStringFunc) etc extension methods for IEnumerable instead
      var toString = To.String;
      toString.ToTextProvider.Settings.UseAutomaticStringEnclosing = false;

      string scriptText = @"
import clr
clr.AddReferenceByPartialName('" + assembly + "')" +
 @"
from " + nameSpace + " import " + toString.sbLiteral ("", ",", "").elements (symbols).se ();

      const ScriptingHost.ScriptLanguageType scriptLanguageType = ScriptingHost.ScriptLanguageType.Python;
      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptSource = engine.CreateScriptSourceFromString (scriptText, SourceCodeKind.Statements);
      scriptSource.Execute (_scriptScope);
    }

    public void SetVariable (string name, Object value)
    {
      _scriptScope.SetVariable (name, value);
    }

    public Variable<T> GetVariable<T> (string name)
    {
      T value;
      bool isValid = _scriptScope.TryGetVariable<T> (name, out value);
      return new Variable<T>(value,isValid);
    }

    public struct Variable<T>
    {
      private readonly T _value;
      private readonly bool _isValid;

      public Variable (T value, bool isValid)
      {
        _value = value;
        _isValid = isValid;
      }

      public T Value
      {
        get { return _value; }
      }

      public bool IsValid
      {
        get { return _isValid; }
      }
    }
  }
}