using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.EventReceiver
{
  public abstract class DataContainerMockEventReceiver
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public DataContainerMockEventReceiver (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      dataContainer.PropertyChanged += new PropertyChangeEventHandler (PropertyChanged);
      dataContainer.PropertyChanging += new PropertyChangeEventHandler (PropertyChanging);
    }

    // abstract methods and properties

    public abstract void PropertyChanging (object sender, PropertyChangeEventArgs args);
    public abstract void PropertyChanged (object sender, PropertyChangeEventArgs args);

  }
}
