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
using Remotion.Web.ExecutionEngine;
using Remotion.Development.UnitTesting;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

  /// <summary> Provides a test implementation of the <see langword="abstract"/> <see cref="WxeTransactedFunctionBase{TTransaction}"/> type. </summary>
  [Serializable]
  public class WxeTransactedFunctionMock : WxeTransactedFunctionBase<ITransaction>
  {
    private ProxyWxeTransaction _storedWxeTransaction;
    public event Proc OnStep1;

    public WxeTransactedFunctionMock (ProxyWxeTransaction wxeTransaction)
      : base ()
    {
      _storedWxeTransaction = wxeTransaction;
    }

    protected override WxeTransactionBase<ITransaction> CreateWxeTransaction ()
    {
      return _storedWxeTransaction;
    }

    protected override ITransaction CreateRootTransaction ()
    {
      throw new NotImplementedException ();
    }

    public new ITransaction MyTransaction
    {
      get { return base.MyTransaction; }
    }

    public new ITransaction Transaction
    {
      get { return base.Transaction; }
    }

    public void InitiateCreateTransaction ()
    {
      PrivateInvoke.SetNonPublicField (this, "_wxeTransaction", CreateWxeTransaction ());
    }

    private void Step1 ()
    {
      if (OnStep1 != null)
        OnStep1();
    }
  }
}
