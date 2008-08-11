/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using Remotion.Collections;
using Remotion.Logging;
using Remotion.Security;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.ExecutionEngine
{

  /// <summary>
  ///   Performs a sequence of operations in a web application using named arguments.
  /// </summary>
  [Serializable]
  public abstract class WxeFunction : WxeStepList
  {
    public static WxeParameterDeclaration[] GetParameterDeclarations (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      if (!typeof (WxeFunction).IsAssignableFrom (type))
        throw new ArgumentException ("Type " + type.FullName + " is not derived from WxeFunction.", "type");

      return WxeFunction.GetParameterDeclarationsUnchecked (type);
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
                parameters.Add (new WxeParameterDeclaration (
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
    public static object[] ParseActualParameters (
        WxeParameterDeclaration[] parameterDeclarations, string actualParameters, CultureInfo culture)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("parameterDeclarations", parameterDeclarations);
      ArgumentUtility.CheckNotNull ("actualParameters", actualParameters);

      StringUtility.ParsedItem[] parsedItems = StringUtility.ParseSeparatedList (actualParameters, ',');

      if (parsedItems.Length > parameterDeclarations.Length)
        throw new ApplicationException ("Number of actual parameters exceeds number of declared parameters.");

      ArrayList arguments = new ArrayList ();
      for (int i = 0; i < parsedItems.Length; ++i)
      {
        StringUtility.ParsedItem item = parsedItems[i];
        WxeParameterDeclaration paramDecl = parameterDeclarations[i];

        try
        {
          if (item.IsQuoted)
          {
            if (paramDecl.Type == typeof (string))                              // string constant
              arguments.Add (item.Value);
            else                                                                // parse constant
              arguments.Add (TypeConversionProvider.Current.Convert (null, culture, typeof (string), paramDecl.Type, item.Value));
          }
          else
          {
            if (string.CompareOrdinal (item.Value, "true") == 0)                // true
              arguments.Add (true);
            else if (string.CompareOrdinal (item.Value, "false") == 0)          // false
              arguments.Add (false);
            else if (item.Value.Length > 0 && char.IsDigit (item.Value[0]))     // starts with digit -> parse constant
              arguments.Add (TypeConversionProvider.Current.Convert (null, culture, typeof (string), paramDecl.Type, item.Value));
            else                                                                // variable name
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

      return arguments.ToArray ();
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
    public static NameValueCollection SerializeParametersForQueryString (
        WxeParameterDeclaration[] parameterDeclarations, object[] parameterValues)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("parameterDeclarations", parameterDeclarations);
      ArgumentUtility.CheckNotNull ("parameterValues", parameterValues);

      NameValueCollection serializedParameters = new NameValueCollection ();

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

    public static bool HasAccess (Type functionType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("functionType", functionType, typeof (WxeFunction));

      IWxeSecurityAdapter wxeSecurityAdapter = AdapterRegistry.Instance.GetAdapter<IWxeSecurityAdapter> ();
      if (wxeSecurityAdapter == null)
        return true;

      return wxeSecurityAdapter.HasStatelessAccess (functionType);
    }

    /// <summary> Hashtable&lt;Type, WxeParameterDeclaration[]&gt; </summary>
    private static Hashtable s_parameterDeclarations = new Hashtable ();

    private static ILog s_log = LogManager.GetLogger (typeof (WxeFunction));

    private NameObjectCollection _variables;

    private string _functionToken;
    private string _returnUrl;
    private object[] _actualParameters;
    private bool _parametersInitialized = false;
    private bool _catchExceptions = false;
    private Type[] _catchExceptionTypes = null;
    private Exception _exception = null;

    public WxeFunction (params object[] actualParameters)
    {
      _variables = new NameObjectCollection ();
      _returnUrl = null;
      _actualParameters = actualParameters;

      Insert (0, new WxeMethodStep (CheckPermissions));
    }

    /// <summary> 
    ///   If this is <c>true</c>, exceptions are caught and returned in the <see cref="Exception"/> property.
    /// </summary>
    public bool CatchExceptions
    {
      get { return _catchExceptions; }
      set { _catchExceptions = value; }
    }

    /// <summary>
    ///   Sets <see cref="CatchExceptions"/> to <c>true</c> and limits the types of exceptions that are caught.
    /// </summary>
    /// <param name="exceptionTypes"> Exceptions of these types or sub classes will be caught, all other
    ///     exceptions will be rethrown. </param>
    public void SetCatchExceptionTypes (params Type[] exceptionTypes)
    {
      _catchExceptions = true;
      _catchExceptionTypes = exceptionTypes;
    }

    /// <summary>
    ///   Joins the passed exceptions types with those already assigned.
    /// </summary>
    /// <param name="exceptionTypes"> 
    ///   Exceptions of these types or sub classes will be caught, all other exceptions will be rethrown. 
    /// </param>
    public void AppendCatchExceptionTypes (params Type[] exceptionTypes)
    {
      if (_catchExceptionTypes != null)
      {
        ArrayList exceptionTypeList = new ArrayList (_catchExceptionTypes);
        for (int idxNewTypes = 0; idxNewTypes < exceptionTypes.Length; idxNewTypes++)
        {
          bool isRegistered = false;
          for (int idxRegisteredTypes = 0; idxRegisteredTypes < _catchExceptionTypes.Length; idxRegisteredTypes++)
          {
            if (_catchExceptionTypes[idxRegisteredTypes] == exceptionTypes[idxNewTypes])
            {
              isRegistered = true;
              break;
            }
          }
          if (!isRegistered)
            exceptionTypeList.Add (exceptionTypes[idxNewTypes]);
        }
        exceptionTypes = (Type[]) exceptionTypeList.ToArray (typeof (Type));
      }
      SetCatchExceptionTypes (exceptionTypes);
    }

    public Type[] GetCatchExceptionTypes ()
    {
      return (Type[]) _catchExceptionTypes.Clone ();
    }

    /// <summary>
    ///   Contains any exception that occured during execution (only if <see cref="CatchExceptions"/> is <c>true</c>).
    /// </summary>
    public Exception Exception
    {
      get { return _exception; }
    }

    /// <summary> Take the actual parameters without any conversion. </summary>
    public override void Execute (WxeContext context)
    {
      if (!ExecutionStarted)
      {
        s_log.Debug ("Initializing execution of " + this.GetType ().FullName + ".");
        NameObjectCollection parentVariables = (ParentStep != null) ? ParentStep.Variables : null;
        EnsureParametersInitialized (null);
      }
      else
      {
        s_log.Debug (string.Format ("Resuming execution of " + this.GetType ().FullName + "."));
      }

      try
      {
        base.Execute (context);
      }
      catch (Exception e)
      {
        if (e is ThreadAbortException)
          throw;

        if (e is HttpException && e.InnerException != null)
          e = e.InnerException;
        if (e is HttpUnhandledException && e.InnerException != null)
          e = e.InnerException;

        bool match = false;
        if (_catchExceptions && _catchExceptionTypes != null)
        {
          foreach (Type exceptionType in _catchExceptionTypes)
          {
            if (exceptionType.IsAssignableFrom (e.GetType ()))
            {
              match = true;
              break;
            }
          }
        }

        if (!_catchExceptions || !match)
        {
          if (e is WxeUnhandledException)
            throw e;
          throw new WxeUnhandledException (string.Format ("An unhandled exception ocured while executing WxeFunction  '{0}': {1}", GetType ().FullName, e.Message), e);
        }

        _exception = e;
      }

      if (_exception == null && ParentStep != null)
        ReturnParametersToCaller ();

      s_log.Debug ("Ending execution of " + this.GetType ().FullName + ".");
    }

    public string ReturnUrl
    {
      get { return _returnUrl; }
      set { _returnUrl = value; }
    }

    public override NameObjectCollection Variables
    {
      get { return _variables; }
    }

    public virtual WxeParameterDeclaration[] ParameterDeclarations
    {
      get { return WxeFunction.GetParameterDeclarations (this.GetType ()); }
    }

    public string FunctionToken
    {
      get
      {
        if (_functionToken != null)
          return _functionToken;
        WxeFunction rootFunction = RootFunction;
        if (rootFunction != null)
          return rootFunction.FunctionToken;
        throw new InvalidOperationException ("The WxeFunction does not have a RootFunction.");
      }      
    }

    internal void SetFunctionToken (string functionToken)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("functionToken", functionToken);
      _functionToken = functionToken;
    }

    public override string ToString ()
    {
      StringBuilder sb = new StringBuilder ();
      sb.Append (this.GetType ().Name);
      sb.Append (" (");
      for (int i = 0; i < _actualParameters.Length; ++i)
      {
        if (i > 0)
          sb.Append (", ");
        object value = _actualParameters[i];
        if (value is WxeVariableReference)
          sb.Append ("@" + ((WxeVariableReference) value).Name);
        else if (value is string)
          sb.AppendFormat ("\"{0}\"", value);
        else
          sb.Append (value);
      }
      sb.Append (")");
      return sb.ToString ();
    }

    /// <summary> Initalizes parameters by name. </summary>
    /// <param name="parameters"> 
    ///   The list of parameter. Must contain an entry for each required parameter. Must not be <see langword="null"/>. 
    /// </param>
    public void InitializeParameters (NameValueCollection parameters)
    {
      ArgumentUtility.CheckNotNull ("parameters", parameters);
      CheckParametersNotInitialized ();

      WxeParameterDeclaration[] parameterDeclarations = ParameterDeclarations;

      for (int i = 0; i < parameterDeclarations.Length; ++i)
      {
        WxeParameterDeclaration paramDeclaration = parameterDeclarations[i];
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
        {
          throw new ApplicationException ("Parameter '" + paramDeclaration.Name + "' is missing.");
        }
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

    private void InitializeParameters (
        string parameterString, NameObjectCollection additionalParameters, bool delayInitialization)
    {
      CheckParametersNotInitialized ();

      _actualParameters =
          WxeFunction.ParseActualParameters (ParameterDeclarations, parameterString, CultureInfo.InvariantCulture);

      if (!delayInitialization)
        EnsureParametersInitialized (additionalParameters);
    }

    /// <summary> Pass actualParameters to Variables. </summary>
    private void EnsureParametersInitialized (NameObjectCollection additionalParameters)
    {
      if (_parametersInitialized)
        return;

      WxeParameterDeclaration[] parameterDeclarations = ParameterDeclarations;
      NameObjectCollection callerVariables = (ParentStep != null) ? ParentStep.Variables : null;
      callerVariables = NameObjectCollection.Merge (callerVariables, additionalParameters);

      if (_actualParameters.Length > parameterDeclarations.Length)
        throw new ApplicationException (string.Format ("{0} parameters provided but only {1} were expected.", _actualParameters.Length, parameterDeclarations.Length));

      for (int i = 0; i < parameterDeclarations.Length; ++i)
      {
        if (i < _actualParameters.Length && _actualParameters[i] != null)
        {
          WxeVariableReference varRef = _actualParameters[i] as WxeVariableReference;
          if (callerVariables != null && varRef != null)
            parameterDeclarations[i].CopyToCallee (varRef.Name, callerVariables, _variables);
          else
            parameterDeclarations[i].CopyToCallee (_actualParameters[i], _variables);
        }
        else if (parameterDeclarations[i].Required)
        {
          throw new ApplicationException ("Parameter '" + parameterDeclarations[i].Name + "' is missing.");
        }
      }

      _parametersInitialized = true;
    }

    private void ReturnParametersToCaller ()
    {
      WxeParameterDeclaration[] parameterDeclarations = ParameterDeclarations;
      NameObjectCollection callerVariables = ParentStep.Variables;

      for (int i = 0; i < parameterDeclarations.Length; ++i)
      {
        if (i < _actualParameters.Length)
        {
          WxeVariableReference varRef = _actualParameters[i] as WxeVariableReference;
          if (varRef != null)
            parameterDeclarations[i].CopyToCaller (varRef.Name, _variables, callerVariables);
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
      NameValueCollection serializedParameters = new NameValueCollection ();
      WxeParameterDeclaration[] parameterDeclarations = ParameterDeclarations;
      NameObjectCollection callerVariables = null;
      if (ParentStep != null)
        callerVariables = ParentStep.Variables;

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
        {
          parameterValue = _variables[parameterDeclaration.Name];
        }
        string serializedValue = parameterDeclaration.Converter.ConvertToString (parameterValue, callerVariables);
        if (serializedValue != null)
          serializedParameters.Add (parameterDeclaration.Name, serializedValue);
      }
      return serializedParameters;
    }

    protected virtual void CheckPermissions (WxeContext context)
    {
      IWxeSecurityAdapter wxeSecurityAdapter = AdapterRegistry.Instance.GetAdapter<IWxeSecurityAdapter> ();
      if (wxeSecurityAdapter == null)
        return;

      wxeSecurityAdapter.CheckAccess (this);
    }
  }

  [AttributeUsage (AttributeTargets.Property, AllowMultiple = false)]
  public class WxeParameterAttribute : Attribute
  {
    private int _index;
    private bool? _required;
    private WxeParameterDirection _direction;

    public WxeParameterAttribute (int index, WxeParameterDirection direction)
      : this (index, null, direction)
    {
    }

    public WxeParameterAttribute (int index, bool required)
      : this (index, required, WxeParameterDirection.In )
    {
    }

    public WxeParameterAttribute (int index)
      : this (index, null, WxeParameterDirection.In)
    {
    }

    /// <summary>
    /// Declares a property as WXE function parameter.
    /// </summary>
    /// <param name="index"> Index of the parameter within the function's parameter list. </param>
    /// <param name="required"> Speficies whether this parameter must be specified (an not 
    ///     be <see langword="null"/>). Default is <see langword="true"/> for value types
    ///     and <see langword="false"/> for reference types. </param>
    /// <param name="direction"> Declares the parameter as input or output parameter, or both. </param>
    public WxeParameterAttribute (int index , bool required, WxeParameterDirection direction)
      : this (index, (bool?) required, direction)
    {
    }

    private WxeParameterAttribute (int index, bool? required, WxeParameterDirection direction)
    {
      _index = index;
      _required = required;
      _direction = direction;
    }

    public int Index
    {
      get { return _index; }
    }

    public bool Required
    {
      get { return _required.Value; }
    }

    public WxeParameterDirection Direction
    {
      get { return _direction; }
    }

    public static WxeParameterAttribute GetAttribute (PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      WxeParameterAttribute[] attributes = (WxeParameterAttribute[]) property.GetCustomAttributes (typeof (WxeParameterAttribute), false);
      if (attributes == null || attributes.Length == 0)
        return null;

      WxeParameterAttribute attribute = attributes[0];
      if (!attribute._required.HasValue)
        attribute._required = property.PropertyType.IsValueType;
      return attribute;
    }
  }

}
