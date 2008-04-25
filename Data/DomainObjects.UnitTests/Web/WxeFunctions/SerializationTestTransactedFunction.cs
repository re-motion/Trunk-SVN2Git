using System;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine;
using Remotion.Development.UnitTesting;
using NUnit.Framework;

namespace Remotion.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
  [Serializable]
  public class SerializationTestTransactedFunction: WxeTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SerializationTestTransactedFunction ()
        : base ()
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