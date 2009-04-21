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
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  [Serializable]
  public class WxeVariablesContainer
  {
    /// <summary> Hashtable&lt;Type, WxeParameterDeclaration[]&gt; </summary>
    private static readonly Hashtable s_parameterDeclarations = new Hashtable();

    public static WxeParameterDeclaration[] GetParameterDeclarations (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      if (!typeof (WxeFunction).IsAssignableFrom (type))
        throw new ArgumentException ("Type " + type.FullName + " is not derived from WxeFunction.", "type");

      return GetParameterDeclarationsUnchecked (type);
    }

    private static WxeParameterDeclaration[] GetParameterDeclarationsUnchecked (Type type)
    {
      WxeParameterDeclaration[] declarations = (WxeParameterDeclaration[]) s_parameterDeclarations[type];
      if (declarations == null)
      {
        lock (type)
        {
          declarations = (WxeParameterDeclaration[]) s_parameterDeclarations[type];
          if (declarations == null)
          {
            PropertyInfo[] properties = type.GetProperties (BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            ArrayList parameters = new ArrayList (properties.Length); // ArrayList<WxeParameterDeclaration>
            ArrayList indices = new ArrayList (properties.Length); // ArrayList<int>
            foreach (PropertyInfo property in properties)
            {
              WxeParameterAttribute parameterAttribute = WxeParameterAttribute.GetAttribute (property);
              if (parameterAttribute != null)
              {
                parameters.Add (
                    new WxeParameterDeclaration (
                        property.Name, parameterAttribute.Required, parameterAttribute.Direction, property.PropertyType));
                indices.Add (parameterAttribute.Index);
              }
            }

            declarations = (WxeParameterDeclaration[]) parameters.ToArray (typeof (WxeParameterDeclaration));
            int[] numberArray = (int[]) indices.ToArray (typeof (int));
            Array.Sort (numberArray, declarations);

            s_parameterDeclarations.Add (type, declarations);
          }
        }
      }
      return declarations;
    }

    /// <summary>
    ///   Parses a string of comma separated actual parameters.
    /// </summary>
    /// <param name="parameterDeclarations">
    ///  The <see cref="WxeParameterDeclaration"/> list used for parsing the <paramref name="actualParameters"/>.
    ///  Must not be <see langword="null"/> or contain items that are <see langword="null"/>.
    /// </param>
    /// <param name="actualParameters"> 
    ///   The comma separated list of parameters. Must contain an entry for each required parameter.
    ///   Must not be <see langword="null"/>.
    /// </param>
    /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
    /// <returns> An array of parameter values. </returns>
    /// <remarks>
    ///   <list type="table">
    ///     <listheader>
    ///       <term> Type </term>
    ///       <description> Syntax </description>
    ///     </listheader>
    ///     <item>
    ///       <term> <see cref="String"/> </term>
    ///       <description> A quoted string. Escape quotes and line breaks using the backslash character.</description>
    ///     </item>
    ///     <item>
    ///       <term> Any type that has a <see langword="static"/> <b>Parse</b> method. </term>
    ///       <description> A quoted string that can be passed to the type's <b>Parse</b> method. For boolean constants 
    ///         (&quot;true&quot;, &quot;false&quot;) and numeric constants, quotes are optional.  </description>
    ///     </item>
    ///     <item>
    ///       <term> Variable Reference </term>
    ///       <description> An unquoted variable name. </description>
    ///     </item>
    ///   </list>
    /// </remarks>
    /// <example>
    ///   "the first \"string\" argument, containing quotes and a comma", "true", "12/30/04 12:00", variableName
    /// </example>
    public static object[] ParseActualParameters (WxeParameterDeclaration[] parameterDeclarations, string actualParameters, CultureInfo culture)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("parameterDeclarations", parameterDeclarations);
      ArgumentUtility.CheckNotNull ("actualParameters", actualParameters);

      StringUtility.ParsedItem[] parsedItems = StringUtility.ParseSeparatedList (actualParameters, ',');

      if (parsedItems.Length > parameterDeclarations.Length)
        throw new ApplicationException ("Number of actual parameters exceeds number of declared parameters.");

      ArrayList arguments = new ArrayList();
      for (int i = 0; i < parsedItems.Length; ++i)
      {
        StringUtility.ParsedItem item = parsedItems[i];
        WxeParameterDeclaration paramDecl = parameterDeclarations[i];

        try
        {
          if (item.IsQuoted)
          {
            if (paramDecl.Type == typeof (string)) // string constant
              arguments.Add (item.Value);
            else // parse constant
              arguments.Add (TypeConversionProvider.Current.Convert (null, culture, typeof (string), paramDecl.Type, item.Value));
          }
          else
          {
            if (string.CompareOrdinal (item.Value, "true") == 0) // true
              arguments.Add (true);
            else if (string.CompareOrdinal (item.Value, "false") == 0) // false
              arguments.Add (false);
            else if (item.Value.Length > 0 && char.IsDigit (item.Value[0])) // starts with digit -> parse constant
              arguments.Add (TypeConversionProvider.Current.Convert (null, culture, typeof (string), paramDecl.Type, item.Value));
            else // variable name
              arguments.Add (new WxeVariableReference (item.Value));
          }
        }
        catch (ArgumentException e)
        {
          throw new ApplicationException ("Parameter " + paramDecl.Name + ": " + e.Message, e);
        }
        catch (ParseException e)
        {
          throw new ApplicationException ("Parameter " + paramDecl.Name + ": " + e.Message, e);
        }
      }

      return arguments.ToArray();
    }

    /// <summary>
    ///   Converts a list of parameter values into a <see cref="NameValueCollection"/>.
    /// </summary>
    /// <param name="parameterDeclarations">
    ///  The <see cref="WxeParameterDeclaration"/> list used for serializing the <paramref name="parameterValues"/>.
    ///  Must not be <see langword="null"/> or contain items that are <see langword="null"/>.
    /// </param>
    /// <param name="parameterValues"> 
    ///   The list parameter values. Must not be <see langword="null"/>.
    /// </param>
    /// <returns> 
    ///   A <see cref="NameValueCollection"/> containing the serialized <paramref name="parameterValues"/>.
    ///   The names of the parameters are used as keys.
    /// </returns>
    public static NameValueCollection SerializeParametersForQueryString (WxeParameterDeclaration[] parameterDeclarations, object[] parameterValues)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("parameterDeclarations", parameterDeclarations);
      ArgumentUtility.CheckNotNull ("parameterValues", parameterValues);

      NameValueCollection serializedParameters = new NameValueCollection();

      for (int i = 0; i < parameterDeclarations.Length; i++)
      {
        WxeParameterDeclaration parameterDeclaration = parameterDeclarations[i];
        object parameterValue = null;
        if (i < parameterValues.Length)
          parameterValue = parameterValues[i];
        string serializedValue = parameterDeclaration.Converter.ConvertToString (parameterValue, null);
        if (serializedValue != null)
          serializedParameters.Add (parameterDeclaration.Name, serializedValue);
      }
      return serializedParameters;
    }

    private readonly WxeFunction _function;
    private readonly WxeParameterDeclaration[] _parameterDeclarations;
    private readonly NameObjectCollection _variables;
    private object[] _actualParameters;
    private bool _parametersInitialized;

    public WxeVariablesContainer (WxeFunction function, object[] actualParameters)
        : this (ArgumentUtility.CheckNotNull ("function", function), actualParameters, GetParameterDeclarations (function.GetType()))
    {
    }

    public WxeVariablesContainer (WxeFunction function, object[] actualParameters, WxeParameterDeclaration[] parameterDeclarations)
    {
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("actualParameters", actualParameters);
      ArgumentUtility.CheckNotNullOrItemsNull ("parameterDeclarations", parameterDeclarations);

      _function = function;
      _variables = new NameObjectCollection();
      _parameterDeclarations = parameterDeclarations;
      _actualParameters = actualParameters;
    }

    public NameObjectCollection Variables
    {
      get { return _variables; }
    }

    public object[] ActualParameters
    {
      get { return _actualParameters; }
    }

    public WxeParameterDeclaration[] ParameterDeclarations
    {
      get { return _parameterDeclarations; }
    }

    /// <summary> Initalizes parameters by name. </summary>
    /// <param name="parameters"> 
    ///   The list of parameter. Must contain an entry for each required parameter. Must not be <see langword="null"/>. 
    /// </param>
    public void InitializeParameters (NameValueCollection parameters)
    {
      ArgumentUtility.CheckNotNull ("parameters", parameters);
      CheckParametersNotInitialized();

      for (int i = 0; i < _parameterDeclarations.Length; ++i)
      {
        WxeParameterDeclaration paramDeclaration = _parameterDeclarations[i];
        string strval = parameters[paramDeclaration.Name];
        if (strval != null)
        {
          try
          {
            _variables[paramDeclaration.Name] = TypeConversionProvider.Current.Convert (
                null, CultureInfo.InvariantCulture, typeof (string), paramDeclaration.Type, strval);
          }
          catch (Exception e)
          {
            throw new ApplicationException ("Parameter " + paramDeclaration.Name + ": " + e.Message, e);
          }
        }
        else if (paramDeclaration.Required)
          throw new ApplicationException ("Parameter '" + paramDeclaration.Name + "' is missing.");
      }

      _parametersInitialized = true; // since parameterString may not contain variable references, initialization is done right away
    }

    public void InitializeParameters (string parameterString, bool delayInitialization)
    {
      InitializeParameters (parameterString, null, delayInitialization);
    }

    /// <summary> Initializes the <see cref="WxeFunction"/> with the supplied parameters. </summary>
    /// <param name="parameterString"> 
    ///   The comma separated list of parameters. Must contain an entry for each required parameter.
    ///   Must not be <see langword="null"/>.
    /// </param>
    /// <param name="additionalParameters"> 
    ///   The parameters passed to the <see cref="WxeFunction"/> in addition to the executing function's variables.
    ///   Use <see langword="null"/> or an empty collection if all parameters are supplied by the 
    ///   <see cref="Command.WxeFunctionCommandInfo.Parameters"/> string and the function stack.
    /// </param>
    /// <exception cref="InvalidOperationException"> 
    ///   Thrown if the <see cref="WxeFunction"/>'s parameters have already been initialized, either because 
    ///   execution has started or <b>InitializeParameters</b> has been called before.
    /// </exception>
    public void InitializeParameters (string parameterString, NameObjectCollection additionalParameters)
    {
      InitializeParameters (parameterString, additionalParameters, false);
    }

    private void InitializeParameters (string parameterString, NameObjectCollection additionalParameters, bool delayInitialization)
    {
      CheckParametersNotInitialized();

      _actualParameters = ParseActualParameters (ParameterDeclarations, parameterString, CultureInfo.InvariantCulture);

      if (!delayInitialization)
        EnsureParametersInitialized (additionalParameters);
    }

    /// <summary> Pass actualParameters to Variables. </summary>
    public void EnsureParametersInitialized (NameObjectCollection additionalParameters)
    {
      if (_parametersInitialized)
        return;

      NameObjectCollection callerVariables = (_function.ParentStep != null) ? _function.ParentStep.Variables : null;
      callerVariables = NameObjectCollection.Merge (callerVariables, additionalParameters);

      if (_actualParameters.Length > _parameterDeclarations.Length)
      {
        throw new ApplicationException (
            string.Format ("{0} parameters provided but only {1} were expected.", _actualParameters.Length, _parameterDeclarations.Length));
      }

      for (int i = 0; i < _parameterDeclarations.Length; ++i)
      {
        if (i < _actualParameters.Length && _actualParameters[i] != null)
        {
          WxeVariableReference varRef = _actualParameters[i] as WxeVariableReference;
          if (callerVariables != null && varRef != null)
            _parameterDeclarations[i].CopyToCallee (varRef.Name, callerVariables, _variables);
          else
            _parameterDeclarations[i].CopyToCallee (_actualParameters[i], _variables);
        }
        else if (_parameterDeclarations[i].Required)
          throw new ApplicationException ("Parameter '" + _parameterDeclarations[i].Name + "' is missing.");
      }

      _parametersInitialized = true;
    }

    internal void ReturnParametersToCaller ()
    {
      NameObjectCollection callerVariables = _function.ParentStep.Variables;

      for (int i = 0; i < _parameterDeclarations.Length; ++i)
      {
        if (i < _actualParameters.Length)
        {
          WxeVariableReference varRef = _actualParameters[i] as WxeVariableReference;
          if (varRef != null)
            _parameterDeclarations[i].CopyToCaller (varRef.Name, _variables, callerVariables);
        }
      }
    }

    private void CheckParametersNotInitialized ()
    {
      if (_parametersInitialized)
        throw new InvalidOperationException ("Parameters are already initialized.");
    }

    public NameValueCollection SerializeParametersForQueryString ()
    {
      NameValueCollection serializedParameters = new NameValueCollection();
      WxeParameterDeclaration[] parameterDeclarations = ParameterDeclarations;
      NameObjectCollection callerVariables = null;
      if (_function.ParentStep != null)
        callerVariables = _function.ParentStep.Variables;

      bool hasActualParameters = _actualParameters.Length > 0;
      for (int i = 0; i < parameterDeclarations.Length; i++)
      {
        WxeParameterDeclaration parameterDeclaration = parameterDeclarations[i];
        object parameterValue = null;
        if (hasActualParameters)
        {
          if (i < _actualParameters.Length)
            parameterValue = _actualParameters[i];
        }
        else
          parameterValue = _variables[parameterDeclaration.Name];
        string serializedValue = parameterDeclaration.Converter.ConvertToString (parameterValue, callerVariables);
        if (serializedValue != null)
          serializedParameters.Add (parameterDeclaration.Name, serializedValue);
      }
      return serializedParameters;
    }
  }
}
