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
using System.Collections.Specialized;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// Use an instance of the <see cref="WxePermaUrlCallArguments"/> to execute a sub-function within the same window while providing a perma-URL
  /// in the browser's location bar.
  /// </summary>
  [Serializable]
  public sealed class WxePermaUrlCallArguments : WxeCallArgumentsBase
  {
    public WxePermaUrlCallArguments ()
        : base (new WxeCallOptions (new WxePermaUrlOptions (false, null)))
    {
    }

    public WxePermaUrlCallArguments (bool useParentPermaUrl)
        : base (new WxeCallOptions (new WxePermaUrlOptions (useParentPermaUrl, null)))
    {
    }

    public WxePermaUrlCallArguments (NameValueCollection urlParameters)
        : base (new WxeCallOptions (new WxePermaUrlOptions (false, urlParameters)))
    {
    }

    public WxePermaUrlCallArguments (bool useParentPermaUrl, NameValueCollection urlParameters)
        : base (new WxeCallOptions (new WxePermaUrlOptions (useParentPermaUrl, urlParameters)))
    {
    }
  }
}