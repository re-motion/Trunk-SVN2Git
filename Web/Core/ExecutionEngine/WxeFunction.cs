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
using System.Globalization;
using System.Text;
using System.Threading;
using Remotion.Collections;
using Remotion.Logging;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  ///   Performs a sequence of operations in a web application using named arguments.
  /// </summary>
  [Serializable]
  public abstract class WxeFunction : WxeStepList
  {
    #region Obsoletes

    [Obsolete ("Use Remotion.Web.ExecutionEngine.Infrastructure.WxeVariablesContainer.GetParameterDeclarations instead. (Version 1.11.5)", true)]
    public static WxeParameterDeclaration[] GetParameterDeclarations (Type type)
    {
      throw new NotImplementedException ("Use WxeVariablesContainer.GetParameterDeclarations instead. (Version 1.11.5)");
    }

    [Obsolete ("Use Remotion.Web.ExecutionEngine.Infrastructure.WxeVariablesContainer.ParseActualParameters instead. (Version 1.11.5)", true)]
    public static object[] ParseActualParameters (WxeParameterDeclaration[] parameterDeclarations, string actualParameters, CultureInfo culture)
    {
      throw new NotImplementedException ("Use WxeVariablesContainer.ParseActualParameters instead. (Version 1.11.5)");
    }

    [Obsolete ("Use Remotion.Web.ExecutionEngine.Infrastructure.WxeVariablesContainer.SerializeParametersForQueryString instead. (Version 1.11.5)", true)]
    public static NameValueCollection SerializeParametersForQueryString (WxeParameterDeclaration[] parameterDeclarations, object[] parameterValues)
    {
      throw new NotImplementedException ("Use WxeVariablesContainer.SerializeParametersForQueryString instead. (Version 1.11.5)");
    }

    [Obsolete ("Use VariablesContainer.ParameterDeclarations instead. (Version 1.11.5)", true)]
    public WxeParameterDeclaration[] ParameterDeclarations
    {
      get { throw new NotImplementedException ("Use VariablesContainer.ParameterDeclarations instead. (Version 1.11.5)"); }
    }

    [Obsolete ("Use VariablesContainer.GetParameterDeclarations instead. (Version 1.11.5)", true)]
    public void InitializeParameters (NameValueCollection parameters)
    {
      throw new NotImplementedException ("Use VariablesContainer.InitializeParameters instead. (Version 1.11.5)");
    }

    [Obsolete ("Use VariablesContainer.InitializeParameters instead. (Version 1.11.5)", true)]
    public void InitializeParameters (string parameterString, bool delayInitialization)
    {
      throw new NotImplementedException ("Use VariablesContainer.InitializeParameters instead. (Version 1.11.5)");
    }

    [Obsolete ("Use VariablesContainer.InitializeParameters instead. (Version 1.11.5)", true)]
    public void InitializeParameters (string parameterString, NameObjectCollection additionalParameters)
    {
      throw new NotImplementedException ("Use VariablesContainer.InitializeParameters instead. (Version 1.11.5)");
    }

    [Obsolete ("Use VariablesContainer.SerializeParametersForQueryString instead. (Version 1.11.5)", true)]
    public NameValueCollection SerializeParametersForQueryString ()
    {
      throw new NotImplementedException ("Use VariablesContainer.SerializeParametersForQueryString instead. (Version 1.11.5)");
    }

    [Obsolete ("Use ExceptionHandler.CatchExceptions instead. (Version 1.11.7)", true)]
    public bool CatchExceptions
    {
      get { throw new NotImplementedException ("Use ExceptionHandler.CatchExceptions instead. (Version 1.11.7)"); }
      set { throw new NotImplementedException ("Use ExceptionHandler.CatchExceptions instead. (Version 1.11.7)"); }
    }

    [Obsolete ("Use ExceptionHandler.SetCatchExceptionTypes instead. (Version 1.11.7)", true)]
    public void SetCatchExceptionTypes (params Type[] exceptionTypes)
    {
      throw new NotImplementedException ("Use ExceptionHandler.SetCatchExceptionTypes instead. (Version 1.11.7)");
    }

    [Obsolete ("Use ExceptionHandler.AppendCatchExceptionTypes instead. (Version 1.11.7)", true)]
    public void AppendCatchExceptionTypes (params Type[] exceptionTypes)
    {
      throw new NotImplementedException ("Use ExceptionHandler.AppendCatchExceptionTypes instead. (Version 1.11.7)");
    }

    [Obsolete ("Use ExceptionHandler.GetCatchExceptionTypes instead. (Version 1.11.7)", true)]
    public Type[] GetCatchExceptionTypes ()
    {
      throw new NotImplementedException ("Use ExceptionHandler.GetCatchExceptionTypes instead. (Version 1.11.7)");
    }

    [Obsolete ("Use ExceptionHandler.Exception instead. (Version 1.11.7)", true)]
    public Exception Exception
    {
      get { throw new NotImplementedException ("Use ExceptionHandler.Exception instead. (Version 1.11.7)"); }
    }

    #endregion

    public static bool HasAccess (Type functionType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("functionType", functionType, typeof (WxeFunction));

      IWxeSecurityAdapter wxeSecurityAdapter = AdapterRegistry.Instance.GetAdapter<IWxeSecurityAdapter>();
      if (wxeSecurityAdapter == null)
        return true;

      return wxeSecurityAdapter.HasStatelessAccess (functionType);
    }

    private static readonly ILog s_log = LogManager.GetLogger (typeof (WxeFunction));

    private readonly WxeVariablesContainer _variablesContainer;
    private readonly WxeExceptionHandler _exceptionHandler;
    private string _functionToken;
    private string _returnUrl;

    protected WxeFunction (WxeParameterDeclaration[] parameterDeclarations,  params object[] actualParameters)
    {
      _variablesContainer = new WxeVariablesContainer (this, actualParameters, parameterDeclarations);
      _exceptionHandler = new WxeExceptionHandler();

      Insert (0, new WxeMethodStep (CheckPermissions));
    }

    protected WxeFunction (params object[] actualParameters)
    {
      _variablesContainer = new WxeVariablesContainer (this, actualParameters);
      _exceptionHandler = new WxeExceptionHandler ();

      Insert (0, new WxeMethodStep (CheckPermissions));
    }

    /// <summary> Take the actual parameters without any conversion. </summary>
    public override void Execute (WxeContext context)
    {
      if (!IsExecutionStarted)
      {
        s_log.Debug ("Initializing execution of " + GetType().FullName + ".");

        _variablesContainer.EnsureParametersInitialized (null);
      }
      else
        s_log.Debug (string.Format ("Resuming execution of " + GetType().FullName + "."));

      try
      {
        base.Execute (context);
      }
      catch (ThreadAbortException)
      {
        throw;
      }
      catch (WxeExecuteUserControlStepException)
      {
        throw;
      }
      catch (WxeExecuteUserControlNextStepException)
      {
      }
      catch (Exception e)
      {
        Exception unwrappedException = PageUtility.GetUnwrappedExceptionFromHttpException (e) ?? e;
        if (!_exceptionHandler.Catch (unwrappedException))
        {
          if (unwrappedException is WxeUnhandledException)
            throw unwrappedException;

          throw new WxeUnhandledException (
              string.Format ("An unhandled exception ocured while executing WxeFunction  '{0}': {1}", GetType().FullName, unwrappedException.Message),
              unwrappedException);
        }
      }

      if (_exceptionHandler.Exception == null && ParentStep != null)
        _variablesContainer.ReturnParametersToCaller();

      s_log.Debug ("Ending execution of " + GetType().FullName + ".");
    }


    public string ReturnUrl
    {
      get { return _returnUrl; }
      set { _returnUrl = value; }
    }

    public override NameObjectCollection Variables
    {
      get { return _variablesContainer.Variables; }
    }

    public WxeVariablesContainer VariablesContainer
    {
      get { return _variablesContainer; }
    }

    public WxeExceptionHandler ExceptionHandler
    {
      get { return _exceptionHandler; }
    }

    public string FunctionToken
    {
      get
      {
        if (_functionToken != null)
          return _functionToken;
        WxeFunction rootFunction = RootFunction;
        if (rootFunction != null && rootFunction != this)
          return rootFunction.FunctionToken;
        throw new InvalidOperationException (
            "The WxeFunction does not have a RootFunction, i.e. the top-most WxeFunction does not have a FunctionToken.");
      }
    }

    internal void SetFunctionToken (string functionToken)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("functionToken", functionToken);
      _functionToken = functionToken;
    }

    public override string ToString ()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append ("WxeFunction: ");
      sb.Append (GetType().Name);
      sb.Append (" (");
      for (int i = 0; i < _variablesContainer.ActualParameters.Length; ++i)
      {
        if (i > 0)
          sb.Append (", ");
        object value = _variablesContainer.ActualParameters[i];
        if (value is WxeVariableReference)
          sb.Append ("@" + ((WxeVariableReference) value).Name);
        else if (value is string)
          sb.AppendFormat ("\"{0}\"", value);
        else
          sb.Append (value);
      }
      sb.Append (")");
      return sb.ToString();
    }

    protected virtual void CheckPermissions (WxeContext context)
    {
      IWxeSecurityAdapter wxeSecurityAdapter = AdapterRegistry.Instance.GetAdapter<IWxeSecurityAdapter>();
      if (wxeSecurityAdapter == null)
        return;

      wxeSecurityAdapter.CheckAccess (this);
    }
  }
}