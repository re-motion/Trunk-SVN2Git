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
using Remotion.Data;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public class WxeTransactedFunctionEventArgs<TTransaction> : EventArgs
    where TTransaction : class, ITransaction
  {
    // types

    // static members

    // member fields

    private TTransaction _transaction;

    // construction and disposing



    public WxeTransactedFunctionEventArgs (TTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      _transaction = transaction;
    }

    // methods and properties

    public TTransaction Transaction
    {
      get { return _transaction; }
    }
  }
}
