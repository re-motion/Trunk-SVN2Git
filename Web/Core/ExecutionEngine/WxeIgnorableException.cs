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

namespace Remotion.Web.ExecutionEngine
{

  /// <summary> 
  ///   Exceptions derived from <see cref="WxeIgnorableException"/> can safely be ignored in event handlers 
  ///   that call other WxeFunctions.
  ///   <see cref="WxeUserCancelException"/>.
  ///   <see cref="WxeCallExternalException"/>
  /// </summary>
  /// <remarks>
  ///   Consider using a try/catch block that catches all <see cref="WxeIgnorableException"/>s in each event 
  ///   handler that calls another function.
  /// </remarks>
  /// <example >
  /// This example shows how to call a <see cref="WxeFunction"/> from an event handler.
  /// <code>
  /// void Button1_Click (object sender, EventArgs e)
  /// {
  ///   try
  ///   {
  ///     var result = OtherPage.Call (this, "argument1", ...);
  ///     Input1.Text = result;
  ///   }
  ///   catch (WxeIgnorableException) { }
  /// }
  /// </code> 
  /// If the user cancels OtherPage, they will just return to this page without any error messages, and without 
  /// the remaining code (the assignment of the result) being executed.
  /// If <see cref="WxeCallOptionsExternal"/> are used, the event handler can be interrupted (the remaining code 
  /// is not executed) without breaking the rest of the page lifecycle (with other options, ASP.NET would actually 
  /// stop processing the page at all).
  /// </example>
  [Serializable]
  public abstract class WxeIgnorableException : WxeException
  {
    protected WxeIgnorableException (string message)
      : base (message)
    {
    }

    protected WxeIgnorableException (string message, Exception innerException)
      : base (message, innerException)
    {
    }

    protected WxeIgnorableException (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
    }
  }

}
