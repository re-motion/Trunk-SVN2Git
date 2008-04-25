using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public class ClientTransactionBaseTest : StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    private ClientTransactionMock _clientTransactionMock;
    private ClientTransactionScope _transactionScope;
    private TestDataContainerFactory _testDataContainerFactory;

    // construction and disposing

    protected ClientTransactionBaseTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      ReInitializeTransaction ();
    }

    public override void TearDown ()
    {
      base.TearDown();
      _testDataContainerFactory = null;
      DisposeTransaction ();
    }

    protected ClientTransactionMock ClientTransactionMock
    {
      get { return _clientTransactionMock; }
    }

    protected TestDataContainerFactory TestDataContainerFactory
    {
      get { return _testDataContainerFactory; }
    }

    private void DisposeTransaction ()
    {
      if (_transactionScope != null)
      {
        if (ClientTransactionScope.ActiveScope == _transactionScope)
          _transactionScope.Leave ();
        else
          ClientTransactionScope.ResetActiveScope ();
        _transactionScope = null;
        _clientTransactionMock = null;
      }
    }

    protected void ReInitializeTransaction ()
    {
      DisposeTransaction ();

      _clientTransactionMock = new ClientTransactionMock ();
      _transactionScope = _clientTransactionMock.EnterDiscardingScope();
      _testDataContainerFactory = new TestDataContainerFactory (_clientTransactionMock);
    }

    protected void CheckIfObjectIsDeleted (ObjectID id)
    {
      try
      {
        DomainObject domainObject = TestDomainBase.GetObject (id, true);
        Assert.IsNull (domainObject, string.Format ("Object '{0}' was not deleted.", id));
      }
      catch (ObjectNotFoundException)
      {
      }
    }
  }
}
