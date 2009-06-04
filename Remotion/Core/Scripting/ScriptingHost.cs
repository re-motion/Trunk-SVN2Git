// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using Microsoft.Scripting.Hosting;

namespace Remtion.Scripting
{
  public class ScriptingHost
  {
    private readonly ScriptRuntime _scriptRuntime;

    public enum ScriptLanguageType
    {
      // Note: Languages must be registered in  App.config <microsoft.scripting>-section of scriptable application.
      // e.g.
      // <languages>
      //    <language names="IronPython;Python;py" extensions=".py" displayName="IronPython 2.0" type="IronPython.Runtime.PythonContext, IronPython, Version=2.0.0.0000, Culture=neutral" />
      //  </languages>
      Python
    }

    public ScriptRuntime ScriptRuntime
    {
      get { return _scriptRuntime; }
    }

    public ScriptingHost ()
    {
      _scriptRuntime = new ScriptRuntime (ScriptRuntimeSetup.ReadConfiguration ());
    }


  }
}