//------------------------------------------------------------------------------
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if 
// the code is regenerated.
//
//------------------------------------------------------------------------------
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
using Remotion.Utilities;

namespace Remotion.Scripting
{
   
  public partial class Script<TResult> : ScriptBase
  {
    private readonly Func<TResult> _func;

    public Script (ScriptContext scriptContext, ScriptingHost.ScriptLanguageType scriptLanguageType, string scriptText, 
      ScriptScope scope, string scriptFunctionName)
      : base (scriptContext, scriptLanguageType, scriptText)
    {
      ArgumentUtility.CheckNotNull ("scriptContext", scriptContext);
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNullOrEmpty ("scriptFunctionName", scriptFunctionName);
      // Note: null/empty script text is allowed. 

      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptSource = engine.CreateScriptSourceFromString (scriptText, SourceCodeKind.Statements);
      scriptSource.Execute (scope);

      _func = scope.GetVariable<Func<TResult>> (scriptFunctionName);
    }

    public TResult Execute ()
    {
      ScriptContext.SwitchAndHoldScriptContext (ScriptContext);
      TResult result;
      try
      {
        result = _func ();
      }
      finally
      {
        ScriptContext.ReleaseScriptContext (ScriptContext);
      }
      return result;
    }
  }
   
  public partial class Script<TFixedArg1, TFixedArg2, TResult> : ScriptBase
  {
    private readonly Func<TFixedArg1, TFixedArg2, TResult> _func;

    public Script (ScriptContext scriptContext, ScriptingHost.ScriptLanguageType scriptLanguageType, string scriptText, 
      ScriptScope scope, string scriptFunctionName)
      : base (scriptContext, scriptLanguageType, scriptText)
    {
      ArgumentUtility.CheckNotNull ("scriptContext", scriptContext);
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNullOrEmpty ("scriptFunctionName", scriptFunctionName);
      // Note: null/empty script text is allowed. 

      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptSource = engine.CreateScriptSourceFromString (scriptText, SourceCodeKind.Statements);
      scriptSource.Execute (scope);

      _func = scope.GetVariable<Func<TFixedArg1, TFixedArg2, TResult>> (scriptFunctionName);
    }

    public TResult Execute (TFixedArg1 a1, TFixedArg2 a2)
    {
      ScriptContext.SwitchAndHoldScriptContext (ScriptContext);
      TResult result;
      try
      {
        result = _func (a1, a2);
      }
      finally
      {
        ScriptContext.ReleaseScriptContext (ScriptContext);
      }
      return result;
    }
  }
   
  public partial class Script<TFixedArg1, TFixedArg2, TFixedArg3, TResult> : ScriptBase
  {
    private readonly Func<TFixedArg1, TFixedArg2, TFixedArg3, TResult> _func;

    public Script (ScriptContext scriptContext, ScriptingHost.ScriptLanguageType scriptLanguageType, string scriptText, 
      ScriptScope scope, string scriptFunctionName)
      : base (scriptContext, scriptLanguageType, scriptText)
    {
      ArgumentUtility.CheckNotNull ("scriptContext", scriptContext);
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNullOrEmpty ("scriptFunctionName", scriptFunctionName);
      // Note: null/empty script text is allowed. 

      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptSource = engine.CreateScriptSourceFromString (scriptText, SourceCodeKind.Statements);
      scriptSource.Execute (scope);

      _func = scope.GetVariable<Func<TFixedArg1, TFixedArg2, TFixedArg3, TResult>> (scriptFunctionName);
    }

    public TResult Execute (TFixedArg1 a1, TFixedArg2 a2, TFixedArg3 a3)
    {
      ScriptContext.SwitchAndHoldScriptContext (ScriptContext);
      TResult result;
      try
      {
        result = _func (a1, a2, a3);
      }
      finally
      {
        ScriptContext.ReleaseScriptContext (ScriptContext);
      }
      return result;
    }
  }
   
  public partial class Script<TFixedArg1, TFixedArg2, TFixedArg3, TFixedArg4, TResult> : ScriptBase
  {
    private readonly Func<TFixedArg1, TFixedArg2, TFixedArg3, TFixedArg4, TResult> _func;

    public Script (ScriptContext scriptContext, ScriptingHost.ScriptLanguageType scriptLanguageType, string scriptText, 
      ScriptScope scope, string scriptFunctionName)
      : base (scriptContext, scriptLanguageType, scriptText)
    {
      ArgumentUtility.CheckNotNull ("scriptContext", scriptContext);
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNullOrEmpty ("scriptFunctionName", scriptFunctionName);
      // Note: null/empty script text is allowed. 

      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptSource = engine.CreateScriptSourceFromString (scriptText, SourceCodeKind.Statements);
      scriptSource.Execute (scope);

      _func = scope.GetVariable<Func<TFixedArg1, TFixedArg2, TFixedArg3, TFixedArg4, TResult>> (scriptFunctionName);
    }

    public TResult Execute (TFixedArg1 a1, TFixedArg2 a2, TFixedArg3 a3, TFixedArg4 a4)
    {
      ScriptContext.SwitchAndHoldScriptContext (ScriptContext);
      TResult result;
      try
      {
        result = _func (a1, a2, a3, a4);
      }
      finally
      {
        ScriptContext.ReleaseScriptContext (ScriptContext);
      }
      return result;
    }
  }
   
  public partial class Script<TFixedArg1, TFixedArg2, TFixedArg3, TFixedArg4, TFixedArg5, TResult> : ScriptBase
  {
    private readonly Func<TFixedArg1, TFixedArg2, TFixedArg3, TFixedArg4, TFixedArg5, TResult> _func;

    public Script (ScriptContext scriptContext, ScriptingHost.ScriptLanguageType scriptLanguageType, string scriptText, 
      ScriptScope scope, string scriptFunctionName)
      : base (scriptContext, scriptLanguageType, scriptText)
    {
      ArgumentUtility.CheckNotNull ("scriptContext", scriptContext);
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNullOrEmpty ("scriptFunctionName", scriptFunctionName);
      // Note: null/empty script text is allowed. 

      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptSource = engine.CreateScriptSourceFromString (scriptText, SourceCodeKind.Statements);
      scriptSource.Execute (scope);

      _func = scope.GetVariable<Func<TFixedArg1, TFixedArg2, TFixedArg3, TFixedArg4, TFixedArg5, TResult>> (scriptFunctionName);
    }

    public TResult Execute (TFixedArg1 a1, TFixedArg2 a2, TFixedArg3 a3, TFixedArg4 a4, TFixedArg5 a5)
    {
      ScriptContext.SwitchAndHoldScriptContext (ScriptContext);
      TResult result;
      try
      {
        result = _func (a1, a2, a3, a4, a5);
      }
      finally
      {
        ScriptContext.ReleaseScriptContext (ScriptContext);
      }
      return result;
    }
  }
   
  public partial class Script<TFixedArg1, TFixedArg2, TFixedArg3, TFixedArg4, TFixedArg5, TFixedArg6, TResult> : ScriptBase
  {
    private readonly Func<TFixedArg1, TFixedArg2, TFixedArg3, TFixedArg4, TFixedArg5, TFixedArg6, TResult> _func;

    public Script (ScriptContext scriptContext, ScriptingHost.ScriptLanguageType scriptLanguageType, string scriptText, 
      ScriptScope scope, string scriptFunctionName)
      : base (scriptContext, scriptLanguageType, scriptText)
    {
      ArgumentUtility.CheckNotNull ("scriptContext", scriptContext);
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNullOrEmpty ("scriptFunctionName", scriptFunctionName);
      // Note: null/empty script text is allowed. 

      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptSource = engine.CreateScriptSourceFromString (scriptText, SourceCodeKind.Statements);
      scriptSource.Execute (scope);

      _func = scope.GetVariable<Func<TFixedArg1, TFixedArg2, TFixedArg3, TFixedArg4, TFixedArg5, TFixedArg6, TResult>> (scriptFunctionName);
    }

    public TResult Execute (TFixedArg1 a1, TFixedArg2 a2, TFixedArg3 a3, TFixedArg4 a4, TFixedArg5 a5, TFixedArg6 a6)
    {
      ScriptContext.SwitchAndHoldScriptContext (ScriptContext);
      TResult result;
      try
      {
        result = _func (a1, a2, a3, a4, a5, a6);
      }
      finally
      {
        ScriptContext.ReleaseScriptContext (ScriptContext);
      }
      return result;
    }
  }
   
  public partial class Script<TFixedArg1, TFixedArg2, TFixedArg3, TFixedArg4, TFixedArg5, TFixedArg6, TFixedArg7, TResult> : ScriptBase
  {
    private readonly Func<TFixedArg1, TFixedArg2, TFixedArg3, TFixedArg4, TFixedArg5, TFixedArg6, TFixedArg7, TResult> _func;

    public Script (ScriptContext scriptContext, ScriptingHost.ScriptLanguageType scriptLanguageType, string scriptText, 
      ScriptScope scope, string scriptFunctionName)
      : base (scriptContext, scriptLanguageType, scriptText)
    {
      ArgumentUtility.CheckNotNull ("scriptContext", scriptContext);
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNullOrEmpty ("scriptFunctionName", scriptFunctionName);
      // Note: null/empty script text is allowed. 

      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptSource = engine.CreateScriptSourceFromString (scriptText, SourceCodeKind.Statements);
      scriptSource.Execute (scope);

      _func = scope.GetVariable<Func<TFixedArg1, TFixedArg2, TFixedArg3, TFixedArg4, TFixedArg5, TFixedArg6, TFixedArg7, TResult>> (scriptFunctionName);
    }

    public TResult Execute (TFixedArg1 a1, TFixedArg2 a2, TFixedArg3 a3, TFixedArg4 a4, TFixedArg5 a5, TFixedArg6 a6, TFixedArg7 a7)
    {
      ScriptContext.SwitchAndHoldScriptContext (ScriptContext);
      TResult result;
      try
      {
        result = _func (a1, a2, a3, a4, a5, a6, a7);
      }
      finally
      {
        ScriptContext.ReleaseScriptContext (ScriptContext);
      }
      return result;
    }
  }
   
  public partial class Script<TFixedArg1, TFixedArg2, TFixedArg3, TFixedArg4, TFixedArg5, TFixedArg6, TFixedArg7, TFixedArg8, TResult> : ScriptBase
  {
    private readonly Func<TFixedArg1, TFixedArg2, TFixedArg3, TFixedArg4, TFixedArg5, TFixedArg6, TFixedArg7, TFixedArg8, TResult> _func;

    public Script (ScriptContext scriptContext, ScriptingHost.ScriptLanguageType scriptLanguageType, string scriptText, 
      ScriptScope scope, string scriptFunctionName)
      : base (scriptContext, scriptLanguageType, scriptText)
    {
      ArgumentUtility.CheckNotNull ("scriptContext", scriptContext);
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNullOrEmpty ("scriptFunctionName", scriptFunctionName);
      // Note: null/empty script text is allowed. 

      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptSource = engine.CreateScriptSourceFromString (scriptText, SourceCodeKind.Statements);
      scriptSource.Execute (scope);

      _func = scope.GetVariable<Func<TFixedArg1, TFixedArg2, TFixedArg3, TFixedArg4, TFixedArg5, TFixedArg6, TFixedArg7, TFixedArg8, TResult>> (scriptFunctionName);
    }

    public TResult Execute (TFixedArg1 a1, TFixedArg2 a2, TFixedArg3 a3, TFixedArg4 a4, TFixedArg5 a5, TFixedArg6 a6, TFixedArg7 a7, TFixedArg8 a8)
    {
      ScriptContext.SwitchAndHoldScriptContext (ScriptContext);
      TResult result;
      try
      {
        result = _func (a1, a2, a3, a4, a5, a6, a7, a8);
      }
      finally
      {
        ScriptContext.ReleaseScriptContext (ScriptContext);
      }
      return result;
    }
  }
   
  public partial class Script<TFixedArg1, TFixedArg2, TFixedArg3, TFixedArg4, TFixedArg5, TFixedArg6, TFixedArg7, TFixedArg8, TFixedArg9, TResult> : ScriptBase
  {
    private readonly Func<TFixedArg1, TFixedArg2, TFixedArg3, TFixedArg4, TFixedArg5, TFixedArg6, TFixedArg7, TFixedArg8, TFixedArg9, TResult> _func;

    public Script (ScriptContext scriptContext, ScriptingHost.ScriptLanguageType scriptLanguageType, string scriptText, 
      ScriptScope scope, string scriptFunctionName)
      : base (scriptContext, scriptLanguageType, scriptText)
    {
      ArgumentUtility.CheckNotNull ("scriptContext", scriptContext);
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNullOrEmpty ("scriptFunctionName", scriptFunctionName);
      // Note: null/empty script text is allowed. 

      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptSource = engine.CreateScriptSourceFromString (scriptText, SourceCodeKind.Statements);
      scriptSource.Execute (scope);

      _func = scope.GetVariable<Func<TFixedArg1, TFixedArg2, TFixedArg3, TFixedArg4, TFixedArg5, TFixedArg6, TFixedArg7, TFixedArg8, TFixedArg9, TResult>> (scriptFunctionName);
    }

    public TResult Execute (TFixedArg1 a1, TFixedArg2 a2, TFixedArg3 a3, TFixedArg4 a4, TFixedArg5 a5, TFixedArg6 a6, TFixedArg7 a7, TFixedArg8 a8, TFixedArg9 a9)
    {
      ScriptContext.SwitchAndHoldScriptContext (ScriptContext);
      TResult result;
      try
      {
        result = _func (a1, a2, a3, a4, a5, a6, a7, a8, a9);
      }
      finally
      {
        ScriptContext.ReleaseScriptContext (ScriptContext);
      }
      return result;
    }
  }
}
