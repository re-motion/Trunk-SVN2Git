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
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;
using Remotion.Collections;
using Remotion.Context;


namespace Remotion.Scripting
{
  /// <summary>
  /// Provides access to Dynamic Language Runtime <see cref="ScriptEngine"/>|s through its static members. 
  /// Returned <see cref="ScriptEngine"/>|s are local to the calling thread.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Note: Script Languages must be registered in the "App.config" &lt;microsoft.scripting&gt;-section of the scriptable application
  /// with the string representation of the respective <see cref="ScriptLanguageType"/> given under the &lt;languages&gt;-tag
  /// names-attribute. e.g.: 
  /// <code lang="XML"><![CDATA[
  /// <microsoft.scripting>
  ///   <languages>
  ///     <language names="Python" extensions=".py" displayName="IronPython 2.0" type="IronPython.Runtime.PythonContext, IronPython, Version=2.0.0.0000, Culture=neutral" />
  ///   </languages>  
  /// </microsoft.scripting>
  /// ]]></code>
  /// </para>
  /// <para>
  /// <example>
  /// <code  escaped="true" lang="C#">
  /// // Retrieve IronPython DLR ScriptEngine
  /// var pythonEngine = ScriptingHost.GetScriptEngine (ScriptLanguageType.Python);
  /// </code>
  /// </example>
  /// </para>
  /// </remarks>
  public class ScriptingHost
  {
    //// ScriptingHost encapsulates Microsoft.Scripting.Hosting.ScriptRuntime, which is not thread safe. We therefore supply a seperate 
    //// singleton instance to every thread through a thread static member.
    //[ThreadStatic]
    //private static ScriptingHost s_scriptingHost;

    private const string scriptingHostCurrentSafeContextTag = "Remotion.Scripting.ScriptingHost.Current";

    private ScriptRuntime _scriptRuntime;
    private ReadOnlyDictionarySpecific<ScriptLanguageType, ScriptEngine> _scriptEngines;


    // ScriptingHost encapsulates Microsoft.Scripting.Hosting.ScriptRuntime, which is not thread safe. We therefore supply a seperate 
    // singleton instance to every thread through a thread static member.
    public static ScriptingHost Current
    {
      get
      {
        var scriptingHost = (ScriptingHost) SafeContext.Instance.GetData (scriptingHostCurrentSafeContextTag);
        if (scriptingHost == null)
        {
          scriptingHost = new ScriptingHost ();
          SafeContext.Instance.SetData (scriptingHostCurrentSafeContextTag, scriptingHost);
        }
        return scriptingHost;
      }
    }

    /// <summary>
    /// Retrieves the ScriptEngine given by the <paramref name="languageType"/> parameter. Throws if requested engine is not available on system.
    /// </summary>
    /// <remarks>
    /// Note: Executing scripts directly through a <see cref="ScriptEngine"/> will bypass re-motion's Extension Module separation
    /// (see <see cref="ScriptContext"/>). For guaranteed re-motion mixin stable binding use re-motion  
    /// <see cref="ScriptFunction{TFixedArg1,TResult}"/>.<see cref="ScriptFunction{TFixedArg1,TResult}.Execute"/> etc instead.
    /// </remarks>
    public static ScriptEngine GetScriptEngine (ScriptLanguageType languageType)
    {
      return Current.GetEngine (languageType);
    }



    private ScriptingHost () {}

    private ScriptRuntime GetScriptRuntime ()
    {
      if (_scriptRuntime == null)
      {
        _scriptRuntime = new ScriptRuntime (ScriptRuntimeSetup.ReadConfiguration());
      }
      return _scriptRuntime;
    }

    private ReadOnlyDictionarySpecific<ScriptLanguageType, ScriptEngine> GetScriptEngines ()
    {
      if (_scriptEngines == null)
      {
        _scriptEngines = FindScriptEngines ();
      }
      return _scriptEngines;
    }

    private ReadOnlyDictionarySpecific<ScriptLanguageType, ScriptEngine> FindScriptEngines ()
    {
      var scriptEngines = new Dictionary<ScriptLanguageType, ScriptEngine> ();
      foreach (ScriptLanguageType languageType in Enum.GetValues (typeof (ScriptLanguageType)))
      {
        ScriptEngine engine;
        var engineAvailable = GetScriptRuntime().TryGetEngine (languageType.ToString(), out engine);
        if (engineAvailable)
        {
          scriptEngines[languageType] = engine;
        }
      }
      return new ReadOnlyDictionarySpecific<ScriptLanguageType, ScriptEngine> (scriptEngines);
    }


    private ScriptEngine GetEngine (ScriptLanguageType languageType)
    {
      ScriptEngine scriptEngine;
      bool engineAvailable = GetScriptEngines().TryGetValue (languageType, out scriptEngine);
      if (!engineAvailable)
      {
        throw new NotSupportedException (String.Format ("ScriptEngine for ScriptLanguageType {0} cannot be supplied. Check App.config <microsoft.scripting>-section.",languageType));
      }
      return scriptEngine;
    }
  }
}