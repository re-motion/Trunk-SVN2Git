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
using System.Runtime.Serialization;
using System.Web;

namespace Remotion.Web.ExecutionEngine
{

  /// <summary> 
  ///   This exception is thrown to cancel the processing of an event handler that called a <see cref="WxeFunction"/>.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///   When this exception is thrown in an event handler, further execution of that event handler should stop, but the 
  ///   event handler should then return normally.
  ///   </para><para>
  ///   Usually, calls to <see cref="WxeFunction"/>s eventually end up with a <see cref="System.Threading.ThreadAbortException"/> 
  ///   thrown by ASP.NET via <see cref="HttpResponse.Redirect"/> or <see cref="HttpServerUtility.Transfer"/>, so there
  ///   is no need to prevent code below that call from being executed immediately after the call.
  ///   </para><para>
  ///   However, calling them externally results in the current request being completed normally, with a JavaScript 
  ///   window.open() rendered out to be invoked on the client. In this case, this exception is thrown, and should be 
  ///   caught and ignored by the event handler. (Consider catching its base type <see cref="WxeIgnorableException"/>
  ///   instead.)
  ///   </para>
  /// </remarks>
  [Serializable]
  public class WxeCallExternalException : WxeIgnorableException
  {
    public WxeCallExternalException ()
      : base ("This exception does not indicate an error. It is used to cancel the processing of an event handler that used "
              + "WxeCallOptionsExternal. Please catch this exception when invoking a page's Call method.")
    {
    }
  }

}
