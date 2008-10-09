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
using System.ComponentModel;
using System.Threading;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// The new <see cref="WxeFunction"/>. Will replace the <see cref="WxeFunction"/> type once implemtation is completed.
  /// </summary>
  [Serializable]
  public abstract class WxeFunction2 : WxeStepList, IWxeFunctionExecutionContext
  {

    //public static bool HasAccess (Type functionType)
    //{
    //  ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("functionType", functionType, typeof (WxeFunction));

    //  IWxeSecurityAdapter wxeSecurityAdapter = AdapterRegistry.Instance.GetAdapter<IWxeSecurityAdapter> ();
    //  if (wxeSecurityAdapter == null)
    //    return true;

    //  return wxeSecurityAdapter.HasStatelessAccess (functionType);
    //}

    private IWxeFunctionExecutionListener _executionListener = NullExecutionListener.Null;
    private TransactionStrategyBase _transactionStrategy = NullTransactionStrategy.Null;
    private readonly ITransactionMode _transactionMode;
    //private readonly WxeVariablesContainer _variablesContainer;
    //private string _functionToken;
    //private string _returnUrl;

    protected WxeFunction2 (ITransactionMode transactionMode, WxeParameterDeclaration[] parameterDeclarations, object[] actualParameters)
    {
      ArgumentUtility.CheckNotNull ("transactionMode", transactionMode);
      ArgumentUtility.CheckNotNull ("parameterDeclarations", parameterDeclarations);
      ArgumentUtility.CheckNotNull ("actualParameters", actualParameters);

      _transactionMode = transactionMode;
      //_variablesContainer = new WxeVariablesContainer (this, actualParameters, parameterDeclarations);
    }

    public override void Execute (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      Assertion.IsNotNull (_executionListener);

      if (!IsExecutionStarted)
      {
        _transactionStrategy = _transactionMode.CreateTransactionStrategy (this, context);
        _executionListener = _transactionStrategy.CreateExecutionListener (_executionListener);
        //_variablesContainer.EnsureParametersInitialized (null);
      }

      _executionListener.OnExecutionPlay (context);

      try
      {
        base.Execute (context);
        _executionListener.OnExecutionStop (context);
      }
      catch (ThreadAbortException)
      {
        _executionListener.OnExecutionPause (context);
        throw;
      }
      catch (Exception stepException)
      {
        try
        {
          _executionListener.OnExecutionFail (context, stepException);
        }
        catch (Exception listenerException)
        {
          throw new WxeFatalExecutionException (stepException, listenerException);
        }
        throw;
      }
      
      //if (_exception == null && ParentStep != null)
      //  _variablesContainer.ReturnParametersToCaller ();
    }

    public IWxeFunctionExecutionListener ExecutionListener
    {
      get { return _executionListener; }
      set { _executionListener = ArgumentUtility.CheckNotNull ("value", value); }
    }

    //TODO: Remove when WxeFunction2 merged to WxeFunction
    public new WxeFunction2 ParentFunction
    {
      get { return WxeStep.GetStepByType<WxeFunction2> (ParentStep); }
    }

    public ITransactionStrategy Transaction
    {
      get { return _transactionStrategy; }
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public TransactionStrategyBase TransactionStrategy
    {
      get { return _transactionStrategy; }
    }

    public ITransactionMode TransactionMode
    {
      get { return _transactionMode; }
    }

    object[] IWxeFunctionExecutionContext.GetInParameters ()
    {
      return new object[0];
    }

    object[] IWxeFunctionExecutionContext.GetOutParameters ()
    {
      return new object[0];
    }

    //public string ReturnUrl
    //{
    //  get { return _returnUrl; }
    //  set { _returnUrl = value; }
    //}

    //public string FunctionToken
    //{
    //  get
    //  {
    //    if (_functionToken != null)
    //      return _functionToken;
    //    WxeFunction rootFunction = RootFunction;
    //    if (rootFunction != null && rootFunction != this)
    //      return rootFunction.FunctionToken;
    //    throw new InvalidOperationException (
    //        "The WxeFunction does not have a RootFunction, i.e. the top-most WxeFunction does not have a FunctionToken.");
    //  }
    //}

    //internal void SetFunctionToken (string functionToken)
    //{
    //  ArgumentUtility.CheckNotNullOrEmpty ("functionToken", functionToken);
    //  _functionToken = functionToken;
    //}

    //public override string ToString ()
    //{
    //  StringBuilder sb = new StringBuilder ();
    //  sb.Append ("WxeFunction: ");
    //  sb.Append (GetType ().Name);
    //  sb.Append (" (");
    //  for (int i = 0; i < _variablesContainer.ActualParameters.Length; ++i)
    //  {
    //    if (i > 0)
    //      sb.Append (", ");
    //    object value = _variablesContainer.ActualParameters[i];
    //    if (value is WxeVariableReference)
    //      sb.Append ("@" + ((WxeVariableReference) value).Name);
    //    else if (value is string)
    //      sb.AppendFormat ("\"{0}\"", value);
    //    else
    //      sb.Append (value);
    //  }
    //  sb.Append (")");
    //  return sb.ToString ();
    //}

    //protected virtual void CheckPermissions (WxeContext context)
    //{
    //  IWxeSecurityAdapter wxeSecurityAdapter = AdapterRegistry.Instance.GetAdapter<IWxeSecurityAdapter> ();
    //  if (wxeSecurityAdapter == null)
    //    return;

    //  wxeSecurityAdapter.CheckAccess (this);
    //}

  }
}