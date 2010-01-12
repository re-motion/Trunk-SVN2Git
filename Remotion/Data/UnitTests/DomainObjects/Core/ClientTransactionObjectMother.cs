using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Rhino.Mocks;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  public static class ClientTransactionObjectMother
  {
    public static ClientTransaction CreatePartialMock ()
    {
      return new MockRepository ().PartialMock<ClientTransaction> (
          new Dictionary<Enum, object> (), 
          new ClientTransactionExtensionCollection (), 
          new RootCollectionEndPointChangeDetectionStrategy (), 
          new DictionaryBasedEnlistedDomainObjectManager ());
    }
  }
}