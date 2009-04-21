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
