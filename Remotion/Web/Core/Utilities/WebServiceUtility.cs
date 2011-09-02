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
using System.Reflection;
using System.Web.Script.Services;
using System.Web.Services;
using Remotion.Utilities;

namespace Remotion.Web.Utilities
{
  /// <summary>
  /// Provides utility methods for common web service operations.
  /// </summary>
  public static class WebServiceUtility
  {
    /// <summary>
    /// Checks that <paramref name="type"/> and <paramref name="method"/> declare a valid web service.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of the web service. Must not be <see langword="null" />.</param>
    /// <param name="method">The service method of the web service. Must not be <see langword="null" /> or empty.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if the required attributes for a web service are not set or the web service declaration itself is invalid.
    /// </exception>
    public static void CheckWebService (Type type, string method)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("method", method);

      CheckType (type);
      GetCheckedAttribute<WebServiceAttribute> (type);
      var methodinfo = GetCheckedMethodInfo (type, method);
      GetCheckedAttribute<WebMethodAttribute> (type, methodinfo);
    }

    /// <summary>
    /// Checks that <paramref name="type"/> and <paramref name="method"/> declare a valid script web service.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of the script web service. Must not be <see langword="null" />.</param>
    /// <param name="method">The service method of the script web service. Must not be <see langword="null" /> or empty.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if the required attributes for a script web service are not set or the web service declaration itself is invalid.
    /// </exception>
    public static void CheckScriptService (Type type, string method)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("method", method);

      CheckWebService (type, method);

      GetCheckedAttribute<ScriptServiceAttribute> (type);
      var methodInfo = GetCheckedMethodInfo (type, method);
      GetCheckedAttribute<ScriptMethodAttribute> (type, methodInfo);
    }

    /// <summary>
    /// Checks that <paramref name="type"/> and <paramref name="method"/> declare a valid JSON web service.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of the JSON web service. Must not be <see langword="null" />.</param>
    /// <param name="method">The service method of the JSON web service. Must not be <see langword="null" /> or empty.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if the required attributes for a JSON web service are not set or the web service declaration itself is invalid.
    /// </exception>
    public static void CheckJsonService (Type type, string method)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("method", method);

      CheckScriptService (type, method);

      var methodInfo = GetCheckedMethodInfo (type, method);
      var scriptMethodAttribute = GetCheckedAttribute<ScriptMethodAttribute> (type, methodInfo);
      if (scriptMethodAttribute.ResponseFormat != ResponseFormat.Json)
      {
        throw CreateArgumentException (
            "Web method '{0}' on web service type '{1}' does not have the ResponseFormat property of the {2} set to Json.",
            method,
            type.FullName,
            typeof (ScriptMethodAttribute).Name);
      }
    }

    private static void CheckType (Type type)
    {
      if (!typeof (WebService).IsAssignableFrom (type))
        throw CreateArgumentException ("Web service type '{0}' does not derive from '{1}'.", type.FullName, typeof (WebService).FullName);
    }

    private static MethodInfo GetCheckedMethodInfo (Type type, string method)
    {
      var methodInfo = type.GetMethod (method, BindingFlags.Instance | BindingFlags.Public);
      if (methodInfo == null)
      {
        throw CreateArgumentException (
            "Web method '{0}' was not found on the public API of web service type '{1}'.",
            method,
            type.FullName);
      }
      return methodInfo;
    }

    private static T GetCheckedAttribute<T> (Type type)
        where T: Attribute
    {
      var attribute = AttributeUtility.GetCustomAttribute<T> (type, true);
      if (attribute == null)
      {
        throw CreateArgumentException (
            "Web service type '{0}' does not have the '{1}' applied.",
            type.FullName,
            typeof (T).FullName);
      }

      return attribute;
    }

    private static T GetCheckedAttribute<T> (Type type, MethodInfo methodInfo)
        where T: Attribute
    {
      var attribute = AttributeUtility.GetCustomAttribute<T> (methodInfo, true);
      if (attribute == null)
      {
        throw CreateArgumentException (
            "Web method '{0}' on web service type '{1}' does not have the '{2}' applied.",
            methodInfo.Name,
            type.FullName,
            typeof (T).FullName);
      }
      return attribute;
    }

    private static ArgumentException CreateArgumentException (string message, params object[] args)
    {
      return new ArgumentException (string.Format (message, args));
    }
  }
}