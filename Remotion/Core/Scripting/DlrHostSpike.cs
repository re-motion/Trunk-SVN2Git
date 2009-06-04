// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;


// For build test only !
namespace Remotion.Scripting
{
  public class DlrHostSpike
  {
    private readonly ScriptRuntime _scriptRuntime;

    readonly Dictionary<EngineType, ScriptEngine> _engines = new Dictionary<EngineType, ScriptEngine> ();

    readonly MemoryStream _output;
    readonly MemoryStream _error;
    private string _lastScriptOutput;

    public string LastScriptOutput
    {
      get { return _lastScriptOutput; }
    }

    public string ErrorFromLastExecution { get; set; }

    public ScriptRuntime ScriptRuntime
    {
      get { return _scriptRuntime; }
    }


    public enum EngineType
    {
      // TODO: Enable as soon as Managed JSscript is released by MS to work outside of Silverlight.
      // Don't forget to add "JavaScript" to the JS language-atribute entry, e.g.:
      // <language names="ManagedJScript;JScript;js;JavaScript" extensions=".jsx;.js" displayName="Managed JScript" type="Microsoft.JScript.Runtime.JSContext, Microsoft.JScript.Runtime, Version=1.0.0.0, Culture=neutral" />
#if(false)
        JavaScript, 
#endif
      Python,
      Ruby
    }

    public DlrHostSpike ()
    {
      _output = new MemoryStream ();
      _error = new MemoryStream ();

      //string configFilePath = Path.GetFullPath (Uri.UnescapeDataString (new Uri (typeof (DlrHost).Assembly.CodeBase).AbsolutePath)) + ".config";
      _scriptRuntime = new ScriptRuntime (ScriptRuntimeSetup.ReadConfiguration ());

      foreach (EngineType engineType in Enum.GetValues (typeof (EngineType)))
      {
        ScriptEngine engine;
        var engineAvailable = ScriptRuntime.TryGetEngine (engineType.ToString (), out engine);
        if (engineAvailable)
        {
          _engines[engineType] = engine;
        }
      }

      ScriptRuntime.Globals.SetVariable ("prompt", "ThePrompt");

      //_engine = _runtime.GetEngine ("py");
      //_theScope = _engine.CreateScope ();

      ScriptRuntime.IO.SetOutput (_output, new StreamWriter (_output));
      ScriptRuntime.IO.SetErrorOutput (_error, new StreamWriter (_error));

      // Load assembly types into dynamic context
      //Runtime.LoadAssembly (Assembly.GetAssembly (typeof (Window1)));
      //Runtime.LoadAssembly (Assembly.GetAssembly (typeof (Application)));
    }



    public T ExecuteWithRetval<T> (EngineType engineType, string scriptText, params object[] scriptParameters)
    {
      ScriptEngine scriptEngine = GetEngine (engineType);
      ScriptSource scriptSource = scriptEngine.CreateScriptSourceFromString (
          scriptText,
          SourceCodeKind.Statements);

      var scriptScope = scriptEngine.CreateScope ();

      int iParam = 0;
      foreach (var scriptParameter in scriptParameters)
      {
        scriptScope.SetVariable ("param" + iParam, scriptParameter);
        ++iParam;
      }

      //To.ConsoleLine.e ("scriptScope.GetVariableNames()", scriptScope.GetVariableNames ());

      // To.ConsoleLine.e (scriptScope.GetVariableNames());
      scriptSource.Execute (scriptScope);
      T result = scriptScope.GetVariable<T> ("retval");

      _lastScriptOutput = ReadOutput ();
      return result;
    }


    public string ExecuteInCurrentScope (EngineType engineType, string scriptText, params object[] scriptParameters)
    {
      ScriptEngine engine = GetEngine (engineType);
      ScriptSource src = engine.CreateScriptSourceFromString (
          scriptText,
          SourceCodeKind.Statements);
      try
      {
        var scope = engine.CreateScope ();

        int iParam = 0;
        foreach (var scriptParameter in scriptParameters)
        {
          scope.SetVariable ("param" + iParam, scriptParameter);
          ++iParam;
        }
        //_theScope.SetVariable ("theScriptCode", snippet);
        object o = src.Execute (scope);
      }
      catch (Exception ex)
      {
        ErrorFromLastExecution = ex.Message;
        return null;
      }
      return ReadOutput ();
    }

    public ScriptEngine GetEngine (EngineType engineType)
    {
      return _engines[engineType];
    }

    private string ReadOutput ()
    {
      return ReadFromStream (_output);
    }

    private string ReadError ()
    {
      return ReadFromStream (_error);
    }

    private string ReadFromStream (MemoryStream ms)
    {
      int length = (int) ms.Length;
      Byte[] bytes = new Byte[length];

      ms.Seek (0, SeekOrigin.Begin);
      ms.Read (bytes, 0, (int) ms.Length);

      ms.SetLength (0);

      return Encoding.GetEncoding ("utf-8").GetString (bytes, 0, (int) bytes.Length);
    }

    public static DlrHostSpike New ()
    {
      return new DlrHostSpike ();
    }

    public void LoadAssembly (Assembly assembly)
    {
      ScriptRuntime.LoadAssembly (assembly);
    }
  }
}