// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using Microsoft.Scripting.Hosting;
using Remotion.Diagnostics.ToText;

namespace Remtion.Scripting
{
  public static class ScriptingHost
  {
    [ThreadStatic]
    private static ScriptRuntime s_scriptRuntime;

    public enum ScriptLanguageType
    {
      // Note: Languages must be registered in  App.config <microsoft.scripting>-section of scriptable application.
      // e.g.
      // <languages>
      //    <language names="IronPython;Python;py" extensions=".py" displayName="IronPython 2.0" type="IronPython.Runtime.PythonContext, IronPython, Version=2.0.0.0000, Culture=neutral" />
      //  </languages>
      Python
    }

    public static ScriptRuntime ScriptRuntime
    {
      get
      {
        if (s_scriptRuntime == null)
        {
          s_scriptRuntime = new ScriptRuntime (ScriptRuntimeSetup.ReadConfiguration ());
        }
          return s_scriptRuntime; }
      }

 
  }
}