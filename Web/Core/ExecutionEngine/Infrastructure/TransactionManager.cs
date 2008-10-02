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
using System.Collections.Generic;
using Remotion.Data;
using Remotion.Utilities;
using System.Collections;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  public class TransactionManager : ITransactionManager
  {
    private readonly ITransactionScopeManager _scopeManager;
    private ITransaction _transaction;

    public TransactionManager (ITransactionScopeManager transactionScopeManager)
    {
      ArgumentUtility.CheckNotNull ("transactionScopeManager", transactionScopeManager);
      _scopeManager = transactionScopeManager;
    }

    public ITransaction Transaction
    {
      get { return _transaction; }
    }

    public void InitializeTransaction ()
    {
      var transaction = _scopeManager.CreateRootTransaction ();
      Assertion.IsNotNull (transaction);
      _transaction = transaction;
    }

    public void Release ()
    {
      throw new System.NotImplementedException();
    }

    public void RegisterObjects (IEnumerable objects)
    {
      ArgumentUtility.CheckNotNull ("objects", objects);

      _transaction.RegisterObjects (FlattenList (objects));
    }

    private IEnumerable<object> FlattenList (IEnumerable objects)
    {
      var list = new List<object> ();
      foreach (var obj in objects)
      {
        if (obj is IEnumerable)
          list.AddRange (FlattenList ((IEnumerable) obj));
        else if (obj != null)
          list.Add (obj);
      }

      return list;
    }
  }
}