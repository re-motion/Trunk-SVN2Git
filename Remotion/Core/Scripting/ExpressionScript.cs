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
  /// <summary>
  /// An <see cref="ExpressionScript{TResult}"/> is a script which contains only a single expression. During script execution this 
  /// expression is evaluated and the result returned. 
  /// </summary>
  /// <remarks>
  /// Under IronPython <see cref="ExpressionScript{TResult}"/>|s are more safe than 
  /// full blown <see cref="ScriptFunction{TResult}"/>|s, since they cannot contain import statements, i.e. they can only access the objects
  /// which can be reached from within their <see cref="ScriptScope"/>.
  /// </remarks>
  /// <typeparam name="TResult">The expression result type.</typeparam>
  public class ExpressionScript<TResult> : ScriptBase
  {
    private readonly ScriptSource _scriptSource;
    private readonly ScriptEnvironment _scriptEnvironment;

    public ExpressionScript (
        ScriptContext scriptContext,
        ScriptLanguageType scriptLanguageType,
        string scriptText,
        ScriptEnvironment scriptEnvironment)
        : base (scriptContext, scriptLanguageType, scriptText)
    {
      ArgumentUtility.CheckNotNull ("scriptContext", scriptContext);
      ArgumentUtility.CheckNotNull ("scriptEnvironment", scriptEnvironment);
      ArgumentUtility.CheckNotNullOrEmpty ("scriptText", scriptText);

      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      _scriptSource = engine.CreateScriptSourceFromString (scriptText, SourceCodeKind.Expression);
      _scriptEnvironment = scriptEnvironment;
    }

    public TResult Execute ()
    {
      ScriptContext.SwitchAndHoldScriptContext (ScriptContext);
      TResult result;
      try
      {
        result = _scriptSource.Execute<TResult> (_scriptEnvironment.ScriptScope);
      }
      finally
      {
        ScriptContext.ReleaseScriptContext (ScriptContext);
      }
      return result;
    }
  }
}