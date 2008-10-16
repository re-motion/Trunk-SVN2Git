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

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  //TODO: Doc
  [Serializable]
  public class CreateChildIfParentTransactionMode : ITransactionMode
  {
    private readonly bool _autoCommit;
    private readonly ITransactionFactory _transactionFactory;

    public CreateChildIfParentTransactionMode (bool autoCommit, ITransactionFactory transactionFactory)
    {
      ArgumentUtility.CheckNotNull ("transactionFactory", transactionFactory);

      _autoCommit = autoCommit;
      _transactionFactory = transactionFactory;
    }

    public virtual TransactionStrategyBase CreateTransactionStrategy (WxeFunction function, WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("context", context);

      if (function.ParentFunction != null)
      {
        var childTransactionStrategy = function.ParentFunction.TransactionStrategy.CreateChildTransactionStrategy (_autoCommit, function, context);
        if (childTransactionStrategy != null)
          return childTransactionStrategy;
      }

      return new RootTransactionStrategy (_autoCommit, _transactionFactory.CreateRootTransaction(), NullTransactionStrategy.Null, function);
    }

    public bool AutoCommit
    {
      get { return _autoCommit; }
    }

    public ITransactionFactory TransactionFactory
    {
      get { return _transactionFactory; }
    }
  }
}