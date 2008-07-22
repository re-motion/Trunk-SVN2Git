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
using Remotion.Data.DomainObjects;
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
