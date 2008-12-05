// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Threading;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeFunctions
{
  [Serializable]
  public class ThreadAbortTestTransactedFunction : WxeFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public ThreadAbortTestTransactedFunction ()
      : base (WxeTransactionMode<ClientTransactionFactory>.CreateRootWithAutoCommit)
    {
    }

    public bool FirstStepExecuted;
    public bool SecondStepExecuted;
    public bool ThreadAborted;

    public ClientTransactionScope TransactionScopeInFirstStep;
    public ClientTransactionScope TransactionScopeInSecondStepBeforeException;
    public ClientTransactionScope TransactionScopeInSecondStepAfterException;

    // methods and properties

    private void Step1 ()
    {
      Assert.IsFalse (FirstStepExecuted);
      Assert.IsFalse (SecondStepExecuted);
      Assert.IsFalse (ThreadAborted);
      FirstStepExecuted = true;
      TransactionScopeInFirstStep = ClientTransactionScope.ActiveScope;
    }

    private void Step2 ()
    {
      Assert.IsTrue (FirstStepExecuted);
      Assert.IsFalse (SecondStepExecuted);

      if (!ThreadAborted)
      {
        TransactionScopeInSecondStepBeforeException = ClientTransactionScope.ActiveScope;
        Assert.AreSame (TransactionScopeInFirstStep, TransactionScopeInSecondStepBeforeException);
        ThreadAborted = true;
        Thread.CurrentThread.Abort();
      }
      TransactionScopeInSecondStepAfterException = ClientTransactionScope.ActiveScope;
      Assert.AreNotSame (TransactionScopeInFirstStep, TransactionScopeInSecondStepAfterException);
      Assert.AreSame (TransactionScopeInFirstStep.ScopedTransaction, TransactionScopeInSecondStepAfterException.ScopedTransaction);
      SecondStepExecuted = true;
    }
  }
}
