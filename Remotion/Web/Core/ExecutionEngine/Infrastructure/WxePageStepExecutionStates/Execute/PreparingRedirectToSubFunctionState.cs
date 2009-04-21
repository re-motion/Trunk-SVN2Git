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
using System.Collections.Specialized;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute
{
  /// <summary>
  /// The <see cref="PreparingRedirectToSubFunctionState"/> is responsible for evaluating the perma-URL information required for a sub-function.
  /// Executing this state will transition the <see cref="IExecutionStateContext"/> into the <see cref="RedirectingToSubFunctionState"/>.
  /// </summary>
  [Serializable]
  public class PreparingRedirectToSubFunctionState : ExecutionStateBase<PreparingRedirectToSubFunctionStateParameters>
  {
    public PreparingRedirectToSubFunctionState (IExecutionStateContext executionStateContext, PreparingRedirectToSubFunctionStateParameters parameters)
        : base (executionStateContext, parameters)
    {
      if (!Parameters.PermaUrlOptions.UsePermaUrl)
      {
        throw new ArgumentException (
            string.Format ("The '{0}' type only supports WxePermaUrlOptions with the UsePermaUrl-flag set to true.", GetType().Name), "parameters");
      }
    }

    public override void ExecuteSubFunction (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      string destinationUrl = GetDestinationPermanentUrl (context);
      string resumeUrl = context.GetResumePath();

      ExecutionStateContext.SetExecutionState (
          new RedirectingToSubFunctionState (
              ExecutionStateContext,
              new RedirectingToSubFunctionStateParameters (Parameters.SubFunction, Parameters.PostBackCollection, destinationUrl, resumeUrl)));
    }

    private string GetDestinationPermanentUrl (WxeContext context)
    {
      NameValueCollection urlParameters;
      if (Parameters.PermaUrlOptions.UrlParameters == null)
        urlParameters = Parameters.SubFunction.VariablesContainer.SerializeParametersForQueryString();
      else
        urlParameters = Parameters.PermaUrlOptions.UrlParameters.Clone();

      urlParameters.Set (WxeHandler.Parameters.WxeFunctionToken, context.FunctionToken);

      return context.GetPermanentUrl (Parameters.SubFunction.GetType(), urlParameters, Parameters.PermaUrlOptions.UseParentPermaUrl);
    }
  }
}
