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
  ///   Encapsulates an exception when it is re-thrown by a <see cref="WxeFunction"/>.
  /// </summary>
  [Serializable]
  public class WxeUnhandledException : WxeException
  {

    public WxeUnhandledException (string message)
      : base (message)
    {
    }

    public WxeUnhandledException (string message, Exception innerException)
      : base (message, innerException)
    {
    }

    public WxeUnhandledException (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
    }
  }

}
