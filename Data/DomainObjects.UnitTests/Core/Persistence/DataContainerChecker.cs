using System;
using NUnit.Framework;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Persistence
{
  public class DataContainerChecker
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public DataContainerChecker ()
    {
    }

    // methods and properties

    public void Check (DataContainer expectedContainer, DataContainer actualContainer)
    {
      Assert.IsNotNull (actualContainer, "actualContainer");
      Assert.AreEqual (expectedContainer.ID.Value, actualContainer.ID.Value, "ID");
      Assert.AreEqual (expectedContainer.DomainObjectType, actualContainer.DomainObjectType, "DomainObjectType");
      Assert.AreEqual (expectedContainer.State, actualContainer.State, "State");

      Assert.AreEqual (expectedContainer.PropertyValues.Count, actualContainer.PropertyValues.Count,
          "PropertyValues.Count");

      PropertyValueChecker valueChecker = new PropertyValueChecker ();
      foreach (PropertyValue expectedPropertyValue in expectedContainer.PropertyValues)
      {
        valueChecker.Check (expectedPropertyValue, actualContainer.PropertyValues[expectedPropertyValue.Name]);
      }
    }
  }
}
