using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Core.EventReceiver
{
  public abstract class PropertyValueCollectionMockEventReceiver
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public PropertyValueCollectionMockEventReceiver (PropertyValueCollection PropertyValueCollection)
    {
      ArgumentUtility.CheckNotNull ("propertyValueCollection", PropertyValueCollection);

      PropertyValueCollection.PropertyChanged += new PropertyChangeEventHandler (PropertyChanged);
      PropertyValueCollection.PropertyChanging += new PropertyChangeEventHandler (PropertyChanging);

    }

    // abstract methods and properties

    public abstract void PropertyChanging (object sender, PropertyChangeEventArgs args);
    public abstract void PropertyChanged (object sender, PropertyChangeEventArgs args);

  }
}
