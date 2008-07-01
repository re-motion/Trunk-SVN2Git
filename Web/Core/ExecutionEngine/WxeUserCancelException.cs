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
  ///   Throw this exception to cancel the execution of a <see cref="WxeFunction"/> while executing a 
  ///   <see cref="WxePageStep"/>. 
  /// </summary>
  /// <remarks>
  ///   In event handlers that call other <see cref="WxeFunction"/>s, this exception should be caught and ignored.
  ///   (Consider catching its base type <see cref="WxeIgnorableException"/> instead.)
  /// </remarks>
  [Serializable]
  public class WxeUserCancelException : WxeIgnorableException
  {
    public WxeUserCancelException ()
      : this ("User cancelled this step.")
    {
    }

    public WxeUserCancelException (string message)
      : base (message)
    {
    }
    public WxeUserCancelException (string message, Exception innerException)
      : base (message, innerException)
    {
    }

    protected WxeUserCancelException (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
    }
  }

}
