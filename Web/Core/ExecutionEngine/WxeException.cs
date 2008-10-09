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

/// <summary> This exception is thrown by the web execution engine. </summary>
[Serializable]
public class WxeException: Exception
{
	public WxeException()
    : base ("The execution engine encountered an error.")
	{
  }

  public WxeException (string message)
    : base (message)
  {
  }

  public WxeException (string message, Exception innerException)
    : base (message, innerException)
  {
  }

  public WxeException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}
}
