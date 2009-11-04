// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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

namespace Remotion.Scripting
{
  /// <summary>
  /// Mother of all re-motion classes representing scripts (<see cref="ExpressionScript{TResult}"/>, <see cref="ScriptFunction{TResult}"/>, <see cref="ScriptFunction{TFixedArg1,TResult}"/>, etc).
  /// Each script knows its <see cref="ScriptContext"/> and <see cref="Scripting.ScriptLanguageType"/>.
  /// </summary>
  public abstract class ScriptBase
  {
    private string _scriptText;
    private readonly ScriptLanguageType _scriptLanguageType;
    private readonly ScriptContext _scriptContext;

    protected ScriptBase (ScriptContext scriptContext, ScriptLanguageType scriptLanguageType, string scriptText)
    {
      _scriptContext = scriptContext;
      _scriptLanguageType = scriptLanguageType;
      _scriptText = scriptText;
    }


    public string ScriptText
    {
      get { return _scriptText; }
      set { _scriptText = value; }
    }

    public ScriptLanguageType ScriptLanguageType
    {
      get { return _scriptLanguageType; }
    }

    public ScriptContext ScriptContext
    {
      get { return _scriptContext; }
    }
  }


}
