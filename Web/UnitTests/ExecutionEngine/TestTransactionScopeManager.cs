/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Data;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  public class TestTransactionScopeManager : ITransactionScopeManager<TestTransaction, TestTransactionScope>
  {
    public TestTransaction RootTransactionToCreate = new TestTransaction();
    public MultiDictionary<TestTransaction, object> EnlistedObjects = new MultiDictionary<TestTransaction, object> ();
    public MultiDictionary<TestTransaction, object> LoadedObjects = new MultiDictionary<TestTransaction, object> ();
    public Dictionary<TestTransaction, Tuple<TestTransaction, bool>> EnlistedSameObjects = new Dictionary<TestTransaction, Tuple<TestTransaction, bool>>();
    public Dictionary<TestTransaction, TestTransaction> CopiedTransactionEventHandlers = new Dictionary<TestTransaction, TestTransaction> ();

    public TestTransactionScope ActiveScope
    {
      get { return TestTransactionScope.CurrentScope; }
    }

    public TestTransactionScope EnterScope (TestTransaction transaction)
    {
      return new TestTransactionScope (transaction);
    }

    public TestTransaction CreateRootTransaction ()
    {
      return RootTransactionToCreate;
    }

    public bool TryEnlistObject (TestTransaction transaction, object objectToBeEnlisted)
    {
      EnlistedObjects.Add (transaction, objectToBeEnlisted);
      return true;
    }

    public void EnlistSameObjects (TestTransaction sourceTransaction, TestTransaction destinationTransaction, bool copyCollectionEventHandlers)
    {
      EnlistedSameObjects.Add (destinationTransaction, Tuple.NewTuple (sourceTransaction, copyCollectionEventHandlers));
    }

    public void CopyTransactionEventHandlers (TestTransaction sourceTransaction, TestTransaction destinationTransaction)
    {
      CopiedTransactionEventHandlers.Add (destinationTransaction, sourceTransaction);
    }

    public void EnsureEnlistedObjectIsLoaded (TestTransaction transaction, object objectEnlistedInTransaction)
    {
      LoadedObjects.Add (transaction, objectEnlistedInTransaction);
    }
  }
}
