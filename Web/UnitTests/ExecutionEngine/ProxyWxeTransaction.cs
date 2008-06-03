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

  /// <summary> Provides a test implementation of the <see langword="abstract"/> <see cref="WxeTransactionBase{TTransaction}"/> type. </summary>
  [Serializable]
  public abstract class ProxyWxeTransaction : WxeTransactionBase<ITransaction>
  {
    public ProxyWxeTransaction ()
      : base (null, true, false)
    {
    }

    public new ITransaction Transaction
    {
      get { return base.Transaction; }
      set { PrivateInvoke.SetNonPublicField (this, "_transaction", value); }
    }

    protected override ITransaction GetRootTransactionFromFunction ()
    {
      return Proxy_CreateRootTransaction ();
    }

    public abstract ITransaction Proxy_CreateRootTransaction ();

    protected override ITransaction CurrentTransaction
    {
      get { return Proxy_CurrentTransaction; }
    }

    public abstract ITransaction Proxy_CurrentTransaction { get;}

    protected override void SetCurrentTransaction (ITransaction transaction)
    {
      Proxy_SetCurrentTransaction (transaction);
    }

    public abstract void Proxy_SetCurrentTransaction (ITransaction transaction);

    protected override void SetPreviousCurrentTransaction(ITransaction transaction)
    {
      Proxy_SetPreviousCurrentTransaction (transaction);
    }

    public abstract void Proxy_SetPreviousCurrentTransaction (ITransaction transaction);

    public override void Execute (WxeContext context)
    {
      base.Execute (context);
    }

    protected override void OnTransactionCreating ()
    {
      Proxy_OnTransactionCreating ();
    }

    public virtual void Proxy_OnTransactionCreating ()
    {
      base.OnTransactionCreating ();
    }

    protected override void OnTransactionCreated (ITransaction transaction)
    {
      Proxy_OnTransactionCreated (transaction);
    }

    public virtual void Proxy_OnTransactionCreated (ITransaction transaction)
    {
      base.OnTransactionCreated (transaction);
    }

    protected override void OnTransactionCommitting ()
    {
      Proxy_OnTransactionCommitting ();
    }

    public virtual void Proxy_OnTransactionCommitting ()
    {
      base.OnTransactionCommitting ();
    }

    protected override void OnTransactionCommitted ()
    {
      Proxy_OnTransactionCommitted ();
    }

    public virtual void Proxy_OnTransactionCommitted ()
    {
      base.OnTransactionCommitted ();
    }

    protected override void OnTransactionRollingBack ()
    {
      Proxy_OnTransactionRollingBack ();
    }

    public virtual void Proxy_OnTransactionRollingBack ()
    {
      base.OnTransactionRollingBack ();
    }

    protected override void OnTransactionRolledBack ()
    {
      Proxy_OnTransactionRolledBack ();
    }

    public virtual void Proxy_OnTransactionRolledBack ()
    {
      base.OnTransactionRolledBack ();
    }
  }

}
