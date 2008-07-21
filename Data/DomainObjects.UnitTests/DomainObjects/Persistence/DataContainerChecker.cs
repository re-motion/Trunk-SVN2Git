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

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Persistence
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
