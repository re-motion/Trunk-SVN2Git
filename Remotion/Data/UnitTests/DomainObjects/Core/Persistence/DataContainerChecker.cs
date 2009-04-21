// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence
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
