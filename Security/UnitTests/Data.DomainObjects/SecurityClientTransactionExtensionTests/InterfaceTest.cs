using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Security.Data.DomainObjects;

namespace Remotion.Security.UnitTests.Data.DomainObjects.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class InterfaceTest
  {

    [Test]
    public void TestInterfaceMembers ()
    {
      IClientTransactionExtension extension = new SecurityClientTransactionExtension ();
      extension.ObjectsLoaded (null, null);
      extension.ObjectDeleted (null, null);
      extension.PropertyValueRead (null, null, null, null, ValueAccess.Current);
      extension.PropertyValueChanged (null, null, null, null, null);
      extension.RelationRead (null, null, null, (DomainObjectCollection) null, ValueAccess.Current);
      extension.RelationRead (null, null, null, (DomainObject) null, ValueAccess.Current);
      extension.RelationChanged (null, null, null);
      extension.Committing (null, null);
      extension.Committed (null, null);
      extension.RollingBack (null, null);
      extension.RolledBack (null, null);
    }
  }
}