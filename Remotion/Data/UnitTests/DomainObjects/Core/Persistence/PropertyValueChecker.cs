// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.UnitTests.DomainObjects.Core.Resources;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence
{
  public class PropertyValueChecker
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public PropertyValueChecker ()
    {
    }

    // methods and properties

    public void Check (PropertyValue expectedValue, PropertyValue actualValue)
    {
      Assert.IsNotNull (actualValue, expectedValue.Name);
      Assert.AreEqual (expectedValue.Name, actualValue.Name, "Name");

      AreValuesEqual (expectedValue.Value, actualValue.Value, string.Format ("Value, expected property name: '{0}'", expectedValue.Name));

      if (expectedValue.Value != null)
      {
        Assert.AreEqual (expectedValue.Value.GetType (), actualValue.Value.GetType (),
            string.Format ("Type of Value, expected property name: '{0}'", expectedValue.Name));
      }

      AreValuesEqual (expectedValue.OriginalValue, actualValue.OriginalValue, string.Format ("OriginalValue, expected property name: '{0}'", expectedValue.Name));

      if (expectedValue.OriginalValue != null)
      {
        Assert.AreEqual (expectedValue.OriginalValue.GetType (), actualValue.OriginalValue.GetType (),
            string.Format ("Type of OriginalValue, expected property name: '{0}'", expectedValue.Name));
      }

      Assert.AreEqual (expectedValue.HasChanged, actualValue.HasChanged,
          string.Format ("HasChanged, expected property name: '{0}'", expectedValue.Name));

      Assert.AreEqual (expectedValue.HasBeenTouched, actualValue.HasBeenTouched,
          string.Format ("HasBeenTouched, expected property name: '{0}'", expectedValue.Name));
    }

    private void AreValuesEqual (object expected, object actual, string message)
    {
      if (expected == actual)
        return;

      if (expected == null || expected.GetType () != typeof (byte[]))
        Assert.AreEqual (expected, actual, message);
      else
        ResourceManager.AreEqual ((byte[]) expected, (byte[]) actual, message);
    }
  }
}
