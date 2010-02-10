using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  public static class ClientTransactionObjectMother
  {
    public static ClientTransaction CreatePartialMock ()
    {
      return CreatePartialMock(new MockRepository ());
    }

    public static ClientTransaction CreatePartialMock (MockRepository mockRepository)
    {
      return mockRepository.PartialMock<ClientTransaction> (
          new Dictionary<Enum, object> (), 
          new ClientTransactionExtensionCollection (), 
          new RootCollectionEndPointChangeDetectionStrategy (), 
          new DictionaryBasedEnlistedDomainObjectManager ());
    }

    public static ClientTransaction CreateStrictMock ()
    {
      return CreateStrictMock(new MockRepository ());
    }

    public static ClientTransaction CreateStrictMock (MockRepository mockRepository)
    {
      return mockRepository.StrictMock<ClientTransaction> (
          new Dictionary<Enum, object> (),
          new ClientTransactionExtensionCollection (),
          new RootCollectionEndPointChangeDetectionStrategy (),
          new DictionaryBasedEnlistedDomainObjectManager ());
    }
  }
}