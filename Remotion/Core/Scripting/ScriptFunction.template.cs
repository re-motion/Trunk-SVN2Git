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
using Remotion.Utilities;

namespace Remotion.Scripting
{
  // @begin-template first=1 template=1 generate=0..9 suppressTemplate=true
   
  // @replace "TFixedArg<n>, "
  public partial class ScriptFunction<TFixedArg1, TResult> : ScriptBase
  {
    // @replace "TFixedArg<n>, "
    private readonly Func<TFixedArg1, TResult> _func;

    public ScriptFunction (ScriptContext scriptContext, ScriptLanguageType scriptLanguageType, string scriptText, 
      ScriptEnvironment scriptEnvironment, string scriptFunctionName)
      : base (scriptContext, scriptLanguageType, scriptText)
    {
      ArgumentUtility.CheckNotNull ("scriptContext", scriptContext);
      ArgumentUtility.CheckNotNull ("scriptEnvironment", scriptEnvironment);
      ArgumentUtility.CheckNotNullOrEmpty ("scriptFunctionName", scriptFunctionName);
      ArgumentUtility.CheckNotNullOrEmpty ("scriptText", scriptText);

      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptSource = engine.CreateScriptSourceFromString (scriptText, SourceCodeKind.Statements);
      scriptSource.Execute (scriptEnvironment.ScriptScope);

      // @replace "TFixedArg<n>, "
      _func = scriptEnvironment.ScriptScope.GetVariable<Func<TFixedArg1, TResult>> (scriptFunctionName);
    }

    // @replace "TFixedArg<n> a<n>" ", "
    public TResult Execute (TFixedArg1 a1)
    {
      ScriptContext.SwitchAndHoldScriptContext (ScriptContext);
      TResult result;
      try
      {
        // @replace "a<n>" ", "
        result = _func (a1);
      }
      finally
      {
        ScriptContext.ReleaseScriptContext (ScriptContext);
      }
      return result;
    }
  }
  // @end-template
}