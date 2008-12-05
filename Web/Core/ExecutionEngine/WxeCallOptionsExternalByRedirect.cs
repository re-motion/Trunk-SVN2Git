// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// Use the <see cref="WxeCallOptionsExternalByRedirect"/> type if you whish to execute a <see cref="WxeFunction"/> as a new root function in the
  /// same window.The <see cref="WxeFunction"/> will be initialized on the server and then opened via a HTTP-redirect request.
  /// </summary>
  [Serializable]
  public class WxeCallOptionsExternalByRedirect : WxeCallOptions
  {
    private readonly bool _returnToCaller;
    private readonly NameValueCollection _callerUrlParameters;

    public WxeCallOptionsExternalByRedirect ()
        : this (new WxePermaUrlOptions (false, null), true, null)
    {
    }

    public WxeCallOptionsExternalByRedirect (NameValueCollection urlParameters)
        : this (new WxePermaUrlOptions (false, urlParameters), true, null)
    {
    }

    public WxeCallOptionsExternalByRedirect (WxePermaUrlOptions permaUrlOptions, bool returnToCaller, NameValueCollection callerUrlParameters)
        : base (permaUrlOptions)
    {
      _returnToCaller = returnToCaller;
      _callerUrlParameters = callerUrlParameters;
    }

    public override void Dispatch (IWxeExecutor executor, WxeFunction function, WxeCallArguments handler)
    {
      ArgumentUtility.CheckNotNull ("executor", executor);
      ArgumentUtility.CheckNotNull ("function", function);

      executor.ExecuteFunctionExternalByRedirect (function, this);

      throw new WxeCallExternalException();
    }

    public bool ReturnToCaller
    {
      get { return _returnToCaller; }
    }

    public NameValueCollection CallerUrlParameters
    {
      get { return _callerUrlParameters; }
    }
  }
}
