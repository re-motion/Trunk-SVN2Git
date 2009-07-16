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
    /// <see cref="ScriptEnvironment"/> factory method.
    /// </summary>
    public static ScriptEnvironment Create ()
    {
      return new ScriptEnvironment (ScriptingHost.GetScriptEngine (ScriptLanguageType.Python).CreateScope ());
    }

 
    /// <summary>
    /// Wraps the passed <see cref="ScriptScope"/> in a <see cref="ScriptEnvironment"/>. 
    /// </summary>
    /// <param name="scriptScope"></param>
    private ScriptEnvironment (ScriptScope scriptScope)
    {
      ArgumentUtility.CheckNotNull ("scriptScope", scriptScope);
      _scriptScope = scriptScope;
    }


    public ScriptScope ScriptScope
    {
      get { return _scriptScope; }
    }


    /// <summary>
    /// Makes the <see cref="ScriptEnvironment"/> CLR aware. 
    /// </summary>
    public void ImportClr ()
    {
      const string scriptText = "import clr";

      var engine = ScriptingHost.GetScriptEngine (ScriptLanguageType.Python);
      var scriptSource = engine.CreateScriptSourceFromString (scriptText, SourceCodeKind.Statements);
      scriptSource.Execute (_scriptScope);
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

      const ScriptLanguageType scriptLanguageType = ScriptLanguageType.Python;
      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptSource = engine.CreateScriptSourceFromString (scriptText, SourceCodeKind.Statements);
      scriptSource.Execute (_scriptScope);
    }

    /// <summary>
    /// Sets a variable with the passe <paramref name="name"/> to the passed <paramref name="value"/> within the <see cref="ScriptEnvironment"/>.
    /// </summary>
    public void SetVariable (string name, Object value)
    {
      _scriptScope.SetVariable (name, value);
    }

    /// <summary>
    /// Gets the value of the variable with the passe <paramref name="name"/> as a <see cref="Variable{T}"/> struct.
    /// If the variable does not exist within the <see cref="ScriptEnvironment"/>, the 
    /// <see cref="Variable{T}"/>.<see cref="Variable{T}.IsValid"/>-property is <see langword="false" />.
    /// </summary> 
    public Variable<T> GetVariable<T> (string name)
    {
      T value;
      bool isValid = _scriptScope.TryGetVariable<T> (name, out value);
      return new Variable<T>(value,isValid);
    }

    /// <summary>
    /// Contains the value of a variable retrieved from a <see cref="ScriptEnvironment"/>, together with the information 
    /// whether the value is valid (i.e. the variable existed).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Variable<T>
    {
      private readonly T _value;
      private readonly bool _isValid;

      public Variable (T value, bool isValid)
      {
        _value = value;
        _isValid = isValid;
      }

      /// <summary>
      /// Value of the variable. Defined only if <see cref="IsValid"/> is <see langword="true"/>.
      /// </summary>
      public T Value
      {
        get { return _value; }
      }

      /// <summary>
      /// <see langword="true"/> if the variable existed, <see langword="false"/> otherwise.
      /// </summary>
      public bool IsValid
      {
        get { return _isValid; }
      }
    }
  }
}