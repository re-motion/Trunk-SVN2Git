// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;
using Remotion.Utilities;


namespace Remotion.Scripting
{
  public class ScriptingHost
  {
    // Note: Languages must be registered in  App.config <microsoft.scripting>-section of scriptable application.
    // e.g.
    // <languages>
    //   <language names="IronPython;Python;py" extensions=".py" displayName="IronPython 2.0" type="IronPython.Runtime.PythonContext, IronPython, Version=2.0.0.0000, Culture=neutral" />
    // </languages>
    public enum ScriptLanguageType
    {
      Python,
      None
    }

    [ThreadStatic]
    private static ScriptingHost s_scriptingHost;

    private ScriptRuntime _scriptRuntime;
    private Dictionary<ScriptLanguageType, ScriptEngine> _scriptEngines;


    public static ScriptingHost GetScriptingHost ()
    {
      if (s_scriptingHost == null)
      {
        s_scriptingHost = new ScriptingHost ();
      }
      return s_scriptingHost;
    }


    public static ScriptEngine GetScriptEngine (ScriptLanguageType languageType)
    {
      ArgumentUtility.CheckNotNull ("languageType", languageType);
      return GetScriptingHost().GetEngine (languageType);
    }


    private ScriptingHost () {}


    public ScriptRuntime GetScriptRuntime ()
    {
      if (_scriptRuntime == null)
      {
        _scriptRuntime = new ScriptRuntime (ScriptRuntimeSetup.ReadConfiguration());
      }
      return _scriptRuntime;
    }

    public Dictionary<ScriptLanguageType, ScriptEngine> GetScriptEngines ()
    {
      if (_scriptEngines == null)
      {
        _scriptEngines = FindScriptEngines ();
      }
      return _scriptEngines;
    }

    public Dictionary<ScriptLanguageType, ScriptEngine> FindScriptEngines ()
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
      return scriptEngines;
    }


    public ScriptEngine GetEngine (ScriptLanguageType languageType)
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