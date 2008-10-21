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
using System.IO;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using PreProcessingSubFunctionState = Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute.PreProcessingSubFunctionState;
using ExecuteByRedirect_PreProcessingSubFunctionState = 
  Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.ExecuteExternalByRedirect.PreProcessingSubFunctionState;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary> This step interrupts the server side execution to display a page to the user. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Class/*' />
  [Serializable]
  public class WxePageStep : WxeStep, IExecutionStateContext
  {
    private IWxePageExecutor _pageExecutor = new WxePageExecutor();
    private readonly ResourceObjectBase _page;
    private readonly string _pageToken;
    private string _pageState;

    [NonSerialized]
    private WxeHandler _wxeHandler;

    private IExecutionState _executionState = NullExecutionState.Null;
    private IUserControlExecutor _userControlExecutor = NullUserControlExecutor.Null;

    /// <summary> Initializes a new instance of the <b>WxePageStep</b> type. </summary>
    /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Ctor/param[@name="page"]' />
    public WxePageStep (string page)
        : this (new ResourceObject(null, page))
    {
    }

    /// <summary> Initializes a new instance of the <b>WxePageStep</b> type. </summary>
    /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Ctor/param[@name="pageref"]' />
    public WxePageStep (WxeVariableReference pageref)
        : this (new ResourceObjectWithVarRef (null, pageref))
    {
    }

    protected WxePageStep (ResourceObjectBase page)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      _page = page;
      _pageToken = Guid.NewGuid().ToString();
   }

    /// <summary> The URL of the page to be displayed by this <see cref="WxePageStep"/>. </summary>
    public string Page
    {
      get { return _page.GetResourcePath (Variables); }
    }

    /// <summary> Gets the currently executing <see cref="WxeStep"/>. </summary>
    /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/ExecutingStep/*' />
    public override WxeStep ExecutingStep
    {
      get
      {
        if (_executionState.IsExecuting)
          return _executionState.Parameters.SubFunction.ExecutingStep;
        else
          return this;
      }
    }

    /// <summary> 
    ///   Displays the <see cref="WxePageStep"/>'s page or the sub-function that has been invoked by the 
    ///   <see cref="ExecuteFunction"/> method.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Execute/*' />
    public override void Execute (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      if (_wxeHandler != null)
      {
        context.HttpContext.Handler = _wxeHandler;
        _wxeHandler = null;
      }

      if (!_executionState.IsExecuting)
      {
        //  Use the Page's postback data
        context.PostBackCollection = null;
        context.SetIsReturningPostBack (false);

        _userControlExecutor.Execute (context);
      }
      else
      {
        while (_executionState.IsExecuting)
          _executionState.ExecuteSubFunction (context);
      }

      try
      {
        _pageExecutor.ExecutePage (context, Page);
      }
      catch (WxeExecuteUserControlNextStepException)
      {
        _userControlExecutor.Return (context);

        try
        {
          _pageExecutor.ExecutePage (context, Page);
        }
        finally
        {
          _userControlExecutor = NullUserControlExecutor.Null;
        }
      }
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ExecuteFunction (PreProcessingSubFunctionStateParameters parameters, WxeRepostOptions repostOptions)
    {
      ArgumentUtility.CheckNotNull ("parameters", parameters);
      ArgumentUtility.CheckNotNull ("repostOptions", repostOptions);

      if (_executionState.IsExecuting)
        throw new InvalidOperationException ("Cannot execute function while another function executes.");

      _wxeHandler = parameters.Page.WxeHandler;

      _executionState = new PreProcessingSubFunctionState (this, parameters, repostOptions);
      Execute();
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ExecuteFunctionExternalByRedirect (PreProcessingSubFunctionStateParameters parameters, WxeReturnOptions returnOptions)
    {
      ArgumentUtility.CheckNotNull ("parameters", parameters);
      ArgumentUtility.CheckNotNull ("returnOptions", returnOptions);

      if (_executionState.IsExecuting)
        throw new InvalidOperationException ("Cannot execute function while another function executes.");

      _wxeHandler = parameters.Page.WxeHandler;

      _executionState = new ExecuteByRedirect_PreProcessingSubFunctionState (this, parameters, returnOptions);
      Execute();
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ExecuteFunction (WxeUserControl2 userControl, WxeFunction subFunction, Control sender, bool usesEventTarget)
    {
      ArgumentUtility.CheckNotNull ("userControl", userControl);
      ArgumentUtility.CheckNotNull ("subFunction", subFunction);
      ArgumentUtility.CheckNotNull ("sender", sender);

      _wxeHandler = userControl.WxePage.WxeHandler;
      
      _userControlExecutor = new UserControlExecutor (this, userControl, subFunction, sender, usesEventTarget);
      Execute ();
    }

    /// <summary> Gets the token for this page step. </summary>
    /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/PageToken/*' />
    public string PageToken
    {
      get { return _pageToken; }
    }

    public override string ToString ()
    {
      return "WxePageStep: " + Page;
    }

    /// <summary> Saves the passed <paramref name="state"/> object into the <see cref="WxePageStep"/>. </summary>
    /// <param name="state"> An <b>ASP.NET</b> viewstate object. </param>
    public void SavePageStateToPersistenceMedium (object state)
    {
      LosFormatter formatter = new LosFormatter();
      StringWriter writer = new StringWriter();
      formatter.Serialize (writer, state);
      _pageState = writer.ToString();
    }

    /// <summary> 
    ///   Returns the viewstate previsously saved through the <see cref="SavePageStateToPersistenceMedium"/> method. 
    /// </summary>
    /// <returns> An <b>ASP.NET</b> viewstate object. </returns>
    public object LoadPageStateFromPersistenceMedium ()
    {
      LosFormatter formatter = new LosFormatter();
      return formatter.Deserialize (_pageState);
    }

    /// <summary> 
    ///   Aborts the <see cref="WxePageStep"/>. Aborting will cascade to any <see cref="WxeFunction"/> executed
    ///   in the scope of this step if they are part of the same hierarchy, i.e. share a common <see cref="WxeStep.RootFunction"/>.
    /// </summary>
    protected override void AbortRecursive ()
    {
      base.AbortRecursive();
      if (_executionState.IsExecuting && _executionState.Parameters.SubFunction.RootFunction == this.RootFunction)
        _executionState.Parameters.SubFunction.Abort();
    }

    public IWxePageExecutor PageExecutor
    {
      get { return _pageExecutor; }
    }

    public IUserControlExecutor UserControlExecutor
    {
      get { return _userControlExecutor; }
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void SetPageExecutor (IWxePageExecutor pageExecutor)
    {
      ArgumentUtility.CheckNotNull ("pageExecutor", pageExecutor);
      _pageExecutor = pageExecutor;
    }

    WxeStep IExecutionStateContext.CurrentStep
    {
      get { return this; }
    }

    WxeFunction IExecutionStateContext.CurrentFunction
    {
      get { return ParentFunction; }
    }

    IExecutionState IExecutionStateContext.ExecutionState
    {
      get { return _executionState; }
    }

    void IExecutionStateContext.SetExecutionState (IExecutionState executionState)
    {
      ArgumentUtility.CheckNotNull ("executionState", executionState);

      _executionState = executionState;
    }
  }
}