// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using Microsoft.Scripting.Hosting;

// For build test only !
namespace Remtion.Scripting
{

  namespace IronPython_Research.Helper
  {
    public static class PythonScriptEngine
    {
      private static DlrHost s_dlrHost;
      private static ScriptEngine s_scriptEngine;

      public static ScriptEngine ScriptEngine
      {
        get
        {
          if (s_scriptEngine == null)
          {
            s_dlrHost = DlrHost.New ();
            s_scriptEngine = s_dlrHost.GetEngine (DlrHost.EngineType.Python);
          }
          return s_scriptEngine;
        }
      }
    }
  }
}