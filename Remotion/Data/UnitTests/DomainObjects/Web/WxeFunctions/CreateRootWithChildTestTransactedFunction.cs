// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeFunctions
{
  [Serializable]
  public class CreateRootWithChildTestTransactedFunction : CreateRootWithChildTestTransactedFunctionBase
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public CreateRootWithChildTestTransactedFunction (ClientTransaction previousClientTransaction, WxeFunction childFunction)
        : base (WxeTransactionMode<ClientTransactionFactory>.CreateRootWithAutoCommit, childFunction, previousClientTransaction)
    {
      Add (
          new WxeMethodStep (
              delegate ()
              {
                TransactionAfterChild = ClientTransactionScope.CurrentTransaction;
                Assert.AreSame (TransactionBeforeChild, TransactionAfterChild);
              }));
    }

    public ClientTransaction TransactionBeforeChild;
    public ClientTransaction TransactionAfterChild;

    // methods and properties

    [WxeParameter (1, true, WxeParameterDirection.In)]
    public ClientTransaction PreviousClientTransaction
    {
      get { return (ClientTransaction) Variables["PreviousClientTransaction"]; }
      set { Variables["PreviousClientTransaction"] = value; }
    }

    private void Step1 ()
    {
      Assert.AreNotSame (PreviousClientTransaction, ClientTransactionScope.CurrentTransaction);
      TransactionBeforeChild = ClientTransactionScope.CurrentTransaction;
    }
  }
}
