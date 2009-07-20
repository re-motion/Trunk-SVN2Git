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
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Hosting;
using Remotion.Diagnostics.ToText;

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
      if (mi.IsGenericMethod)
      {
        To.ConsoleLine.sb().e (mi.Name).e (mi.ReturnType).e (mi.Attributes).
            e (pis.Select (pi => pi.ParameterType)).e (pis.Select (pi => pi.Attributes)).e (pis.Select (pi => pi.Position)).
            e (pis.Select (pi => pi.ParameterType.IsGenericParameter ? pi.ParameterType.GenericParameterPosition : -1)).e (
            pis.Select (pi => pi.MetadataToken)).e (pis.Select (pi => pi.ToString())).se().
            e (mi.DeclaringType);
      }
      else
      {
        To.ConsoleLine.sb().e (mi.Name).e (mi.ReturnType).e (mi.DeclaringType).e (mi.Attributes).e (
            pis.Select (pi => pi.ParameterType)).e (pis.Select (pi => pi.Attributes)).se();
      }
    }

    public static MethodInfo GetAnyInstanceMethod (Type type, string name)
    {
      return type.GetMethod (name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }

    public static MethodInfo GetAnyInstanceMethod (Type type, string name, Type[] argumentTypes)
    {
      return type.GetMethod (name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, Type.DefaultBinder, argumentTypes, new ParameterModifier[0]);
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
  }
}