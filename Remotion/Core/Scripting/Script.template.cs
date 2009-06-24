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
using Microsoft.Scripting.Hosting;
using Remotion.Reflection;

namespace Remotion.Scripting
{
  // @begin-template first=1 template=1 generate=0..3 suppressTemplate=true

  // @replace "TFixedArg<n>, "
  public partial class Script<TFixedArg1, TResult> : ScriptBase
  {

    // @replace "TFixedArg<n>, "
    private Func<TFixedArg1, TResult> _func;

    // TEST ONLY !
    public Script (ScriptContext scriptContext, ScriptingHost.ScriptLanguageType scriptLanguageType, string scriptText)
      : base (scriptContext, scriptLanguageType, scriptText)
    { 
    } 

    public Script (ScriptContext scriptContext, ScriptingHost.ScriptLanguageType scriptLanguageType, string scriptText, 
      ScriptScope scope, string scriptFunctionName)
      : base (scriptContext, scriptLanguageType, scriptText)
    {
      // @replace "TFixedArg<n>, "
      _func = scope.GetVariable<Func<TFixedArg1, TResult>> (scriptFunctionName);
    }


  }
  // @end-template

  
 







  //public Script (ScriptContext scriptContext, ScriptingHost.ScriptLanguageType scriptLanguageType, string scriptText, ScriptScope scope)
  //  : base (scriptContext, scriptLanguageType, scriptText)
  //{
  //  _func scope.GetVariable<Func<TFixedArg1, TResult>> (name);
  //}

  //// @replace ", TFixedArg<n> fixedArg<n>"
  //public Script (DelegateSelector delegateSelector, TFixedArg1 fixedArg1)
  //{
  //  _delegateSelector = delegateSelector;
  //  // @begin-repeat
  //  // @replace-one "fixedArg<n>"
  //  _fixedArg1 = fixedArg1;
  //  // @end-repeat
  //}
  
  
  //public class Script<TR> : ScriptBase
  //{
  //  private readonly Func<TR> _func;

  //  public Script (ScriptContext scriptContext, ScriptingHost.ScriptLanguageType scriptLanguageType, string scriptText)
  //      : base(scriptContext, scriptLanguageType, scriptText)
  //  {
  //    _func = GetFunc<Func<TR>>();
  //  }

  //  public TR Execute ()
  //  {
  //    // TODO: Switch context
  //    return _func();
  //  }
  //}


  //public class Script<TA1, TR> : ScriptBase
  //{
  //  private readonly Func<TA1, TR> _func;

  //  public Script (ScriptContext scriptContext, ScriptingHost.ScriptLanguageType scriptLanguageType, string scriptText)
  //    : base (scriptContext, scriptLanguageType, scriptText)
  //  {
  //    _func = GetFunc<Func<TA1, TR>> ();
  //  }

  //  public TR Execute (TA1 a1)
  //  {
  //    return _func (a1);
  //  }
  //}
}