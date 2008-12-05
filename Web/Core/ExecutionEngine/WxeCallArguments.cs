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
using System.Web.UI;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// Use an instance <see cref="WxeCallArguments"/> type to control the execution of a <see cref="WxeFunction"/>. This type always requires a 
  /// <b>sender</b>-object and can be parameterized with <see cref="WxeCallOptions"/> to specify if the <see cref="WxeFunction"/> should execute
  /// within the same window, as a new root-function, with or without a perma-URL and so on.
  /// </summary>
  [Serializable]
  public sealed class WxeCallArguments : WxeCallArgumentsBase
  {
    /// <summary>
    /// The default arguments. Use this instance to execute a <see cref="WxeFunction"/> as a sub-function within the same window.
    /// </summary>
    public static readonly IWxeCallArguments Default = new WxeCallArgumentsBase (new WxeCallOptions (WxePermaUrlOptions.Null));

    private readonly Control _sender;

    public WxeCallArguments (Control sender, WxeCallOptions options)
        : base (options)
    {
      ArgumentUtility.CheckNotNull ("sender", sender);

      _sender = sender;
    }

    public Control Sender
    {
      get { return _sender; }
    }

    protected override void Dispatch (IWxeExecutor executor, WxeFunction function)
    {
      ArgumentUtility.CheckNotNull ("executor", executor);
      ArgumentUtility.CheckNotNull ("function", function);

      Options.Dispatch (executor, function, this);
    }
  }
}
