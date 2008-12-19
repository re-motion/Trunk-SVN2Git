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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// The <see cref="WxeCallArgumentsBase"/> type is the default implementation of <see cref="IWxeCallArguments"/> and acts as the base type for
  /// the <see cref="WxeCallArguments"/> and <see cref="WxePermaUrlCallArguments"/> types.
  /// </summary>
  /// <remarks>
  /// <note type="inotes">Override the <see cref="Dispatch"/> method to control the execution of the <see cref="WxeFunction"/>.</note>
  /// </remarks>
  [Serializable]
  public class WxeCallArgumentsBase : IWxeCallArguments
  {
    private readonly WxeCallOptions _options;

    protected internal WxeCallArgumentsBase (WxeCallOptions options)
    {
      ArgumentUtility.CheckNotNull ("options", options);

      _options = options;
    }

    public WxeCallOptions Options
    {
      get { return _options; }
    }

    protected virtual void Dispatch (IWxeExecutor executor, WxeFunction function)
    {
      ArgumentUtility.CheckNotNull ("executor", executor);
      ArgumentUtility.CheckNotNull ("function", function);

      _options.Dispatch (executor, function, null);
    }

    void IWxeCallArguments.Dispatch (IWxeExecutor executor, WxeFunction function)
    {
      Dispatch (executor, function);
    }
  }
}
