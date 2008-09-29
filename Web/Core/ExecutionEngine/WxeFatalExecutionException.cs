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
  //TODO: useful exception message
  [Serializable]
  public class WxeFatalExecutionException:WxeException
  {
    private readonly Exception _outerException;

    public WxeFatalExecutionException (Exception innerExcection, Exception outerException)
      :base ("Execution failed", innerExcection)
    {
      _outerException = outerException;
    }

    public WxeFatalExecutionException (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
    }

    public Exception OuterException
    {
      get { return _outerException; }
    }
  }
}