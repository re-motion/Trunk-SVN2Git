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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  //TODO: Doc
  [Serializable]
  public class NullTransactionMode : ITransactionMode
  {
    public NullTransactionMode ()
    {
    }

    public virtual ITransactionStrategy CreateTransactionStrategy (WxeFunction2 function, IWxeFunctionExecutionListener executionListener)
    {
      ArgumentUtility.CheckNotNull ("executionListener", executionListener);
      return new NullTransactionStrategy (executionListener);
    }

    public bool AutoCommit
    {
      get { return false; }
    }
  }
}