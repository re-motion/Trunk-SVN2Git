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
using System.Collections;
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Data;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public class WxeTransactedFunctionParameterEnlister<TTransaction, TScope, TTransactionScopeManager>
    where TTransaction : class, ITransaction
    where TScope : class, ITransactionScope<TTransaction>
    where TTransactionScopeManager : ITransactionScopeManager<TTransaction, TScope>
  {
    private readonly TTransactionScopeManager _scopeManager;
    private readonly TTransaction _transaction;

    private readonly Set<Tuple<WxeParameterDeclaration, object>> _enlistedObjects = new Set<Tuple<WxeParameterDeclaration, object>> ();

    public WxeTransactedFunctionParameterEnlister (TTransaction transaction, TTransactionScopeManager scopeManager)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNull ("scopeManager", scopeManager);
      _transaction = transaction;
      _scopeManager = scopeManager;
    }

    public IEnumerable<Tuple<WxeParameterDeclaration, object>> EnlistedObjects
    {
      get { return _enlistedObjects; }
    }

    public void EnlistParameters (IEnumerable<WxeParameterDeclaration> parameterDeclarations, NameObjectCollection variables)
    {
      ArgumentUtility.CheckNotNull ("parameterDeclarations", parameterDeclarations);
      ArgumentUtility.CheckNotNull ("variables", variables);

      foreach (WxeParameterDeclaration parameterDeclaration in parameterDeclarations)
      {
        object parameter = parameterDeclaration.GetValue (variables);
        EnlistParameter (parameterDeclaration, parameter);
      }
    }

    public void EnlistParameter (WxeParameterDeclaration parameterDeclaration, object parameter)
    {
      if (!TryEnlistAsSingleObject (parameterDeclaration, parameter))
        TryEnlistAsEnumerable (parameterDeclaration, parameter);
    }

    public bool TryEnlistAsSingleObject (WxeParameterDeclaration parameterDeclaration, object objectToEnlist)
    {
      if (_scopeManager.TryEnlistObject (_transaction, objectToEnlist))
      {
        _enlistedObjects.Add (Tuple.NewTuple (parameterDeclaration, objectToEnlist));
        return true;
      }
      else
        return false;
    }

    public bool TryEnlistAsEnumerable (WxeParameterDeclaration parameterDeclaration, object possibleEnumerable)
    {
      IEnumerable enumerable = possibleEnumerable as IEnumerable;
      if (enumerable != null)
      {
        foreach (object innerParameter in enumerable)
          EnlistParameter (parameterDeclaration, innerParameter);
        return true;
      }
      else
        return false;
    }

    public void LoadAllEnlistedObjects ()
    {
      TScope scope = _scopeManager.EnterScope (_transaction);
      try
      {
        foreach (Tuple<WxeParameterDeclaration, object> objectToLoad in _enlistedObjects)
        {
          try
          {
            _scopeManager.EnsureEnlistedObjectIsLoaded (_transaction, objectToLoad.B);
          }
          catch (Exception ex)
          {
            string message = string.Format (
                "The object '{0}' cannot be enlisted in the function's transaction. Maybe it was newly created "
                    + "and has not yet been committed, or it was deleted.",
                objectToLoad.B);
            throw new ArgumentException (message, objectToLoad.A.Name, ex);
          }
        }
      }
      finally
      {
        scope.Leave ();
      }
    }
  }
}
