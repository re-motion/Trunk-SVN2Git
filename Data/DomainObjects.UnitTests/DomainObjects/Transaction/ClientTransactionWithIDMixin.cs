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
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Transaction
{
  public interface IClientTransactionWithID
  {
    Guid ID { get; }
    void Commit ();
    void Rollback ();
    ClientTransaction AsClientTransaction { get; }
  }

  public class ClientTransactionWithIDMixin : Mixin<ClientTransaction>, IClientTransactionWithID
  {
    private readonly Guid _id;

    public ClientTransactionWithIDMixin ()
    {
      _id = Guid.NewGuid ();
    }

    public Guid ID
    {
      get { return _id; }
    }

    public void Commit ()
    {
      This.Commit ();
    }

    public void Rollback ()
    {
      This.Rollback ();
    }

    public ClientTransaction AsClientTransaction
    {
      get { return This; }
    }

    [OverrideTarget]
    public new string ToString ()
    {
      return ID.ToString ();
    }
  }
}
