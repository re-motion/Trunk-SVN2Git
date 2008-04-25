using System;
using Rhino.Mocks;
using Remotion.Utilities;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;
using Mocks_Property = Rhino.Mocks.Constraints.Property;

namespace Remotion.Data.DomainObjects.UnitTests.EventReceiver
{
  public abstract class DomainObjectCollectionMockEventReceiver
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public DomainObjectCollectionMockEventReceiver (DomainObjectCollection domainObjectCollection)
    {
      ArgumentUtility.CheckNotNull ("domainObjectCollection", domainObjectCollection);

      domainObjectCollection.Added += new DomainObjectCollectionChangeEventHandler (Added);
      domainObjectCollection.Adding += new DomainObjectCollectionChangeEventHandler (Adding);
      domainObjectCollection.Removed += new DomainObjectCollectionChangeEventHandler (Removed);
      domainObjectCollection.Removing += new DomainObjectCollectionChangeEventHandler (Removing);
    }

    // abstract methods and properties

    protected abstract void Added (object sender, DomainObjectCollectionChangeEventArgs args);
    protected abstract void Adding (object sender, DomainObjectCollectionChangeEventArgs args);
    protected abstract void Removed (object sender, DomainObjectCollectionChangeEventArgs args);
    protected abstract void Removing (object sender, DomainObjectCollectionChangeEventArgs args);

    public void Adding (object sender, DomainObject domainObject)
    {
      Adding (null, (DomainObjectCollectionChangeEventArgs) null);
      LastCall.Constraints (Mocks_Is.Same (sender), Mocks_Property.Value ("DomainObject", domainObject));
    }

    public void Added (object sender, DomainObject domainObject)
    {
      Added (null, (DomainObjectCollectionChangeEventArgs) null);
      LastCall.Constraints (Mocks_Is.Same (sender), Mocks_Property.Value ("DomainObject", domainObject));
    }

    public void Removing (object sender, DomainObject domainObject)
    {
      Removing (null, (DomainObjectCollectionChangeEventArgs) null);
      LastCall.Constraints (Mocks_Is.Same (sender), Mocks_Property.Value ("DomainObject", domainObject));
    }

    public void Removed (object sender, DomainObject domainObject)
    {
      Removed (null, (DomainObjectCollectionChangeEventArgs) null);
      LastCall.Constraints (Mocks_Is.Same (sender), Mocks_Property.Value ("DomainObject", domainObject));
    }
  }
}
