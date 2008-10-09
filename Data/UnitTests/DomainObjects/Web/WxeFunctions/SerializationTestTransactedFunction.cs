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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeFunctions
{
  [Serializable]
  public class SerializationTestTransactedFunction: WxeFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SerializationTestTransactedFunction ()
        : base (WxeTransactionMode<ClientTransactionFactory>.CreateRootWithAutoCommit)
    {
    }

    public bool FirstStepExecuted;
    public bool SecondStepExecuted;
    public ClientTransaction TransactionBeforeSerialization;

    public byte[] SerializedSelf;

    // methods and properties

    private void Step1()
    {
      Assert.IsFalse (FirstStepExecuted);
      FirstStepExecuted = true;

      TransactionBeforeSerialization = ClientTransactionScope.CurrentTransaction;
    }

    private void Step2 ()
    {
      Assert.IsTrue (FirstStepExecuted);
      Assert.IsFalse (SecondStepExecuted);

      SerializedSelf = Serializer.Serialize (this); // freeze at this point of time

      SecondStepExecuted = true;
      Assert.AreSame (TransactionBeforeSerialization, ClientTransactionScope.CurrentTransaction);
    }
  }
}
