// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  #region Obsoletes

  [Obsolete ("Use Remotion.Web.ExecutionEngine.WxeFunction instead. (Version 1.11.7)", true)]
  public abstract class WxeScopedTransactedFunction<TTransaction, TScope, TTransactionScopeManager>
  {
    private WxeScopedTransactedFunction ()
    {
      throw new NotImplementedException ("Use Remotion.Web.ExecutionEngine.WxeFunctions instead. (Version 1.11.7)");
    }
  }

  #endregion

  /// <summary>
  /// The new <see cref="WxeFunction"/>. Will replace the <see cref="WxeFunction"/> type once implemtation is completed.
  /// </summary>
  [Serializable]
  public abstract class WxeFunction : WxeStepList, IWxeFunctionExecutionContext
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

    [Obsolete ("Supply a custom ITransactionFactory to the WxeTransactionMode. (Version 1.11.7)", true)]
    protected object CreateRootTransaction ()
    {
      throw new NotImplementedException ("Supply a custom ITransactionFactory to the WxeTransactionMode. (Version 1.11.7)");
    }

    [Obsolete ("Supply a custom ITransactionFactory to the WxeTransactionMode. (Version 1.11.7)", true)]
    protected void OnTransactionCreated (object transaction)
    {
      throw new NotImplementedException ("Use ExceptionHandler.GetCatchExceptionTypes instead. (Version 1.11.7)");
    }

    [Obsolete ("Suppy an auto-commiting WxeTransactionMode to the constructor instead. (Version 1.11.7)", true)]
    protected virtual bool AutoCommit
    {
      get { throw new NotImplementedException ("Suppy an auto-commiting WxeTransactionMode to the constructor instead. (Version 1.11.7)"); }
    }

    [Obsolete ("Use Transaction.Reset() instead. (Version 1.11.7)", true)]
    public void ResetTransaction ()
    {
      throw new NotImplementedException ("Use Transaction.Reset() instead. (Version 1.11.7)");
    }

    #endregion

    public static bool HasAccess (Type functionType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("functionType", functionType, typeof (WxeFunction));

      IWxeSecurityAdapter wxeSecurityAdapter = AdapterRegistry.Instance.GetAdapter<IWxeSecurityAdapter> ();
      if (wxeSecurityAdapter == null)
        return true;

      return wxeSecurityAdapter.HasStatelessAccess (functionType);
    }

    private IWxeFunctionExecutionListener _executionListener = NullExecutionListener.Null;
    private TransactionStrategyBase _transactionStrategy;
    private ITransactionMode _transactionMode;
    private readonly WxeVariablesContainer _variablesContainer;
    private readonly WxeExceptionHandler _exceptionHandler = new WxeExceptionHandler ();
    private string _functionToken;
    private string _returnUrl;

    protected WxeFunction (ITransactionMode transactionMode, params object[] actualParameters)
    {
      ArgumentUtility.CheckNotNull ("transactionMode", transactionMode);
      ArgumentUtility.CheckNotNull ("actualParameters", actualParameters);

      _transactionMode = transactionMode;
      _variablesContainer = new WxeVariablesContainer (this, actualParameters);
    }

    protected WxeFunction (ITransactionMode transactionMode, WxeParameterDeclaration[] parameterDeclarations, object[] actualParameters)
    {
      ArgumentUtility.CheckNotNull ("transactionMode", transactionMode);
      ArgumentUtility.CheckNotNull ("parameterDeclarations", parameterDeclarations);
      ArgumentUtility.CheckNotNull ("actualParameters", actualParameters);

      _transactionMode = transactionMode;
      _variablesContainer = new WxeVariablesContainer (this, actualParameters, parameterDeclarations);
    }

    public override void Execute (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      Assertion.IsNotNull (_executionListener);

      if (!IsExecutionStarted)
      {
        _variablesContainer.EnsureParametersInitialized (null);
        _executionListener = new SecurityExecutionListener (this, _executionListener);

        _transactionStrategy = _transactionMode.CreateTransactionStrategy (this, context);
        Assertion.IsNotNull (_transactionStrategy);

        _executionListener = _transactionStrategy.CreateExecutionListener (_executionListener);
        Assertion.IsNotNull (_executionListener);
      }

      try
      {
        _executionListener.OnExecutionPlay (context);
        base.Execute (context);
        _executionListener.OnExecutionStop (context);
      }
      catch (WxeFatalExecutionException)
      {
        // bubble up
        throw;
      }
      catch (ThreadAbortException)
      {
        _executionListener.OnExecutionPause (context);
        throw;
      }
      catch (Exception stepException)
      {
        Exception unwrappedException = PageUtility.GetUnwrappedExceptionFromHttpException (stepException) ?? stepException;

        try
        {
          _executionListener.OnExecutionFail (context, unwrappedException);
        }
        catch (Exception listenerException)
        {
          throw new WxeFatalExecutionException (unwrappedException, listenerException);
        }

        if (!_exceptionHandler.Catch (unwrappedException))
        {
          if (unwrappedException is WxeUnhandledException)
            throw unwrappedException.PreserveStackTrace();

          throw new WxeUnhandledException (
              string.Format ("An unhandled exception ocured while executing WxeFunction '{0}':\r\n{1}", GetType().FullName, unwrappedException.Message),
              unwrappedException);
        }
      }

      if (_exceptionHandler.Exception == null && ParentStep != null)
        _variablesContainer.ReturnParametersToCaller ();
    }

    public IWxeFunctionExecutionListener ExecutionListener
    {
      get { return _executionListener; }
      set { _executionListener = ArgumentUtility.CheckNotNull ("value", value); }
    }

    public ITransactionStrategy Transaction
    {
      get { return TransactionStrategy; }
    }

    protected internal TransactionStrategyBase TransactionStrategy
    {
      get { return _transactionStrategy ?? NullTransactionStrategy.Null; }
    }

    protected ITransactionMode TransactionMode
    {
      get { return _transactionMode; }
    }

    protected void SetTransactionMode (ITransactionMode transactionMode)
    {
      ArgumentUtility.CheckNotNull ("transactionMode", transactionMode);

      if (_transactionStrategy != null)
        throw new InvalidOperationException ("The TransactionMode cannot be set after the TransactionStrategy has been initialized.");
      _transactionMode = transactionMode;
    }

    object[] IWxeFunctionExecutionContext.GetInParameters ()
    {
      return _variablesContainer.ParameterDeclarations.Where (p => p.IsIn).Select (p => p.GetValue (_variablesContainer.Variables)).ToArray();
    }

    object[] IWxeFunctionExecutionContext.GetOutParameters ()
    {
      return _variablesContainer.ParameterDeclarations.Where (p => p.IsOut).Select (p => p.GetValue (_variablesContainer.Variables)).ToArray ();
    }

    object[] IWxeFunctionExecutionContext.GetVariables ()
    {
      return _variablesContainer.Variables.Values.Cast<object>().ToArray();
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
      StringBuilder sb = new StringBuilder ();
      sb.Append ("WxeFunction: ");
      sb.Append (GetType ().Name);
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
      return sb.ToString ();
    }
  }
}
