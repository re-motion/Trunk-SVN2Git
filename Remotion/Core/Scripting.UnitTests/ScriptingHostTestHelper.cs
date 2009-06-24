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
using Remotion.Collections;
using Remotion.Development.UnitTesting;

namespace Remotion.Scripting.UnitTests
{
  static class ScriptingHostTestHelper 
  {
    //public static ScriptEngine GetScriptEngine (ScriptingHost.ScriptLanguageType languageType)
    //{
    //  return (ScriptEngine) PrivateInvoke.InvokeNonPublicStaticMethod (typeof (ScriptingHost), "GetScriptEngine", languageType);
    //}

    public static ScriptRuntime GetScriptRuntime (this ScriptingHost scriptingHost)
    {
      return (ScriptRuntime) PrivateInvoke.InvokeNonPublicMethod (scriptingHost, "GetScriptRuntime");
    }

    public static ReadOnlyDictionarySpecific<ScriptingHost.ScriptLanguageType, ScriptEngine> GetScriptEngines (this ScriptingHost scriptingHost)
    {
      return (ReadOnlyDictionarySpecific<ScriptingHost.ScriptLanguageType, ScriptEngine>)
             PrivateInvoke.InvokeNonPublicMethod (scriptingHost, "GetScriptEngines");
    }

    public static ReadOnlyDictionarySpecific<ScriptingHost.ScriptLanguageType, ScriptEngine> FindScriptEngines (this ScriptingHost scriptingHost)
    {
      return (ReadOnlyDictionarySpecific<ScriptingHost.ScriptLanguageType, ScriptEngine>) 
             PrivateInvoke.InvokeNonPublicMethod (scriptingHost, "FindScriptEngines");
    }

    public static ScriptEngine GetEngine (this ScriptingHost scriptingHost, ScriptingHost.ScriptLanguageType languageType)
    {
      return (ScriptEngine) PrivateInvoke.InvokeNonPublicMethod (scriptingHost, "GetEngine", languageType);
    }
  }
}