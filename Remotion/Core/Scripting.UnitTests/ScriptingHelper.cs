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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Remotion.Diagnostics.ToText;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  public class ScriptingHelper
  {
    public static ScriptScope CreateScriptScope (ScriptLanguageType scriptLanguageType)
    {
      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      return engine.CreateScope ();
    }

    public static void ToConsoleLine (MethodInfo mi)
    {
      var pis = mi.GetParameters ();

      To.ConsoleLine.sb().e (mi.Name).e (mi.MetadataToken).e (mi.GetBaseDefinition().MetadataToken).
      e(mi.MemberType.GetTypeCode()).e(mi.MethodHandle.Value).
        e (mi.ReturnType).e (mi.Attributes).e (pis.Select (pi => pi.ParameterType)).e (pis.Select (pi => pi.Attributes));
      if (mi.IsGenericMethod)
      {
        To.Console.e (pis.Select (pi => pi.Position)).
            e (pis.Select (pi => pi.ParameterType.IsGenericParameter ? pi.ParameterType.GenericParameterPosition : -1));
      }
      To.Console.e (pis.Select (pi => pi.MetadataToken)).e (pis.Select (pi => pi.ToString ())).se ().
          e (mi.DeclaringType);
    }


    public static void ToConsoleLine (string methodName, params Type[] types)
    {
      To.ConsoleLine.nl (2).s ("Method: ").e (methodName);
      foreach (var type in types)
      {
        To.ConsoleLine.nl ().e (type.Name).s(": ");
        ScriptingHelper.GetAnyInstanceMethodArray (type, methodName).Process (mi => ScriptingHelper.ToConsoleLine (mi));
      }
    }


    public static ModuleScope CreateModuleScope (string namePostfix)
    {
      string name = "Remotion.Scripting.CodeGeneration.Generated.Test." + namePostfix;
      string nameSigned = name + ".Signed";
      string nameUnsigned = name + ".Unsigned";
      const string ext = ".dll";
      return new ModuleScope (true, nameSigned, nameSigned + ext, nameUnsigned, nameUnsigned + ext);
    }


    public static MethodInfo GetAnyInstanceMethod (Type type, string name)
    {
      return type.GetMethod (name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }

    public static MethodInfo GetAnyInstanceMethod (Type type, string name, Type[] argumentTypes)
    {
      return type.GetMethod (name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, Type.DefaultBinder, argumentTypes, new ParameterModifier[0]);
    }

    public static MethodInfo[] GetAnyInstanceMethodArray (Type type, string name)
    {
      return type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where (
        mi => (mi.Name == name)).ToArray ();
    }



    public static MethodInfo[] GetAnyPublicInstanceMethodArray (Type type, string name)
    {
      return type.GetMethods (BindingFlags.Instance | BindingFlags.Public).Where (
        mi => (mi.Name == name)).ToArray ();
    } 

    public static MethodInfo GetAnyGenericInstanceMethod (Type type, string name, int numberGenericParameters)
    {
      return type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where (
        mi => (mi.IsGenericMethodDefinition && mi.Name == name && mi.GetGenericArguments ().Length == numberGenericParameters)).Single ();
    }

    public static MethodInfo[] GetAnyGenericInstanceMethodArray (Type type, string name, int numberGenericParameters)
    {
      return type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where (
        mi => (mi.IsGenericMethodDefinition && mi.Name == name && mi.GetGenericArguments ().Length == numberGenericParameters)).ToArray ();
    }


    public static MethodInfo[] GetAnyExplicitInterfaceMethodArray (Type type, string name, Type[] argumentTypes)
    {
      return type.GetMethods (BindingFlags.Instance | BindingFlags.NonPublic).Where (
        mi => (mi.Name.EndsWith (name) && mi.GetParameters ().Select (pi => pi.ParameterType).SequenceEqual (argumentTypes))).ToArray ();
    }

    public static MethodInfo GetExplicitInterfaceMethod (Type interfaceType, Type type, string name, Type[] argumentTypes)
    {
      return type.GetMethod (interfaceType.FullName + "." + name, BindingFlags.Instance | BindingFlags.NonPublic, Type.DefaultBinder, argumentTypes, new ParameterModifier[0]);
    }

    public static TResult ExecuteScriptExpression<TResult> (string expressionScriptSourceCode, params object[] scriptParameter)
    {
      const ScriptLanguageType scriptLanguageType = ScriptLanguageType.Python;
      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptSource = engine.CreateScriptSourceFromString (expressionScriptSourceCode, SourceCodeKind.Expression);
      var compiledScript = scriptSource.Compile ();
      var scriptScope = ScriptingHost.GetScriptEngine (scriptLanguageType).CreateScope ();

      for (int i = 0; i < scriptParameter.Length; i++)
      {
        scriptScope.SetVariable ("p" + i, scriptParameter[i]);
      }
      return compiledScript.Execute<TResult> (scriptScope);
    }


    public static long ExecuteAndTime (int nrLoop, Func<Object> func)
    {
      var timings = ExecuteAndTime (new[] { nrLoop}, func);
      return timings.Single();
    }


    public static long[] ExecuteAndTime (int[] nrLoopsArray, Func<Object> func)
    {
      return ExecuteAndTimeStable (nrLoopsArray, 10, func);
    }


    public static long[] ExecuteAndTimeFast (int[] nrLoopsArray, Func<Object> func)
    {
      var timings = new System.Collections.Generic.List<long> ();

      foreach (var nrLoops in nrLoopsArray)
      {
        System.GC.Collect (2);
        System.GC.WaitForPendingFinalizers ();

        Stopwatch stopwatch = new Stopwatch ();
        stopwatch.Start ();

        for (int i = 0; i < nrLoops; i++)
        {
          func ();
        }

        stopwatch.Stop ();
        timings.Add (stopwatch.ElapsedMilliseconds);
      }

      return timings.ToArray();
    }


    /// <summary>
    /// Timing method which takes the fastest timing from <paramref name="nrRuns"/> timing runs, 
    /// thereby making the timing results more stable.
    /// </summary>
    public static long[] ExecuteAndTimeStable (int[] nrLoopsArray, int nrRuns ,Func<Object> func)
    {
      var nrLoopsArrayLength = nrLoopsArray.Length;
      var timings = new long[nrLoopsArrayLength];
      for (int iLoop = 0; iLoop < nrLoopsArrayLength; iLoop++)
      {
        timings[iLoop] = long.MaxValue;
      }

      for (int iRun = 0; iRun < nrRuns; iRun++)
      {
        for (int iLoop = 0; iLoop < nrLoopsArrayLength; iLoop++)
        {
          System.GC.Collect (2);
          System.GC.WaitForPendingFinalizers();

          Stopwatch stopwatch = new Stopwatch ();
          stopwatch.Start ();

          for (int i = 0; i < nrLoopsArray[iLoop]; i++)
          {
            func();
          }

          stopwatch.Stop();
          timings[iLoop] = Math.Min (timings[iLoop], stopwatch.ElapsedMilliseconds);
        }
      }

      return timings.ToArray ();
    }


    public static void ExecuteAndTime (string testName, int[] nrLoopsArray, Func<Object> func)
    {
      var timings = ExecuteAndTime (nrLoopsArray, func);

      To.ConsoleLine.s ("Timings ").e (testName).s (",").e (() => nrLoopsArray).s (": ").nl ().sb ();
      foreach (var timing in timings)
      {
        To.Console.e (timing);
      }
      To.Console.se ();
    }


    public static FieldInfo GetProxiedField (object proxy)
    {
      Type proxyType = GetActualType (proxy);
      return proxyType.GetField ("_proxied", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    public static object GetProxiedFieldValue (object proxy)
    {
      var proxiedField = GetProxiedField (proxy);
      return proxiedField.GetValue (proxy);
    }

    public static void SetProxiedFieldValue (object proxy, object value)
    {
      var proxiedField = GetProxiedField (proxy);
      proxiedField.SetValue (proxy, value);
    }

    public static Type GetActualType (object proxy)
    {
      var objectGetType = typeof (object).GetMethod ("GetType");
      return (Type) objectGetType.Invoke (proxy, new object[0]);
    }

  }
}