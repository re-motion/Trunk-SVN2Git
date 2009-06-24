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
using Remotion.Reflection;

namespace Remotion.Scripting
{
  // @begin-template first=1 template=1 generate=0..3 suppressTemplate=true
   
  // @replace "TFixedArg<n>, "
  public partial class Script<TFixedArg1, TResult> : ScriptBase
  {
    // @replace "TFixedArg<n>, "
    private readonly Func<TFixedArg1, TResult> _func;

    public Script (ScriptContext scriptContext, ScriptingHost.ScriptLanguageType scriptLanguageType, string scriptText, 
      ScriptScope scope, string scriptFunctionName)
      : base (scriptContext, scriptLanguageType, scriptText)
    {
      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptSource = engine.CreateScriptSourceFromString (scriptText, SourceCodeKind.Statements);
      scriptSource.Execute (scope);

      // @replace "TFixedArg<n>, "
      _func = scope.GetVariable<Func<TFixedArg1, TResult>> (scriptFunctionName);
    }

    // @replace "TFixedArg<n> a<n>" ", "
    public TResult Execute (TFixedArg1 a1)
    {
      // TODO: Switch context !
      // @replace "a<n>" ", "
      return _func (a1);
    }
  }
  // @end-template
}