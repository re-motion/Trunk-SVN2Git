// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.ExtensibleEnums.Globalization;
using Remotion.ServiceLocation;
using Remotion.UnitTests.ExtensibleEnums.TestDomain;

namespace Remotion.UnitTests.ExtensibleEnums.Globalization
{
  [TestFixture]
  public class ExtensibleEnumerationGlobalizationServiceIntegrationTest
  {
    [Test]
    public void TryGetExtensibleEnumerationValueDisplayName_IntegrationTest ()
    {
      string resourceValue;

      var service = SafeServiceLocator.Current.GetInstance<IExtensibleEnumerationGlobalizationService> ();

      Assert.That (service.TryGetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.Value1 (), out resourceValue), Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Wert1"));
      Assert.That (service.GetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.Value1 ()), Is.EqualTo ("Wert1"));
      Assert.That (service.GetExtensibleEnumerationValueDisplayNameOrDefault (ExtensibleEnumWithResources.Values.Value1 ()), Is.EqualTo ("Wert1"));
      Assert.That (service.ContainsExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.Value1 ()), Is.True);
      
      Assert.That (service.TryGetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.Value2 (), out resourceValue), Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Wert2"));
      Assert.That (service.GetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.Value2 ()), Is.EqualTo ("Wert2"));
      Assert.That (service.GetExtensibleEnumerationValueDisplayNameOrDefault (ExtensibleEnumWithResources.Values.Value2 ()), Is.EqualTo ("Wert2"));
      Assert.That (service.ContainsExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.Value2 ()), Is.True);

      Assert.That (service.TryGetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.ValueWithoutResource (), out resourceValue), Is.False);
      Assert.That (resourceValue, Is.Null);
      Assert.That (service.GetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.ValueWithoutResource ()), Is.EqualTo ("ValueWithoutResource"));
      Assert.That (service.GetExtensibleEnumerationValueDisplayNameOrDefault (ExtensibleEnumWithResources.Values.ValueWithoutResource ()), Is.Null);
      Assert.That (service.ContainsExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.ValueWithoutResource ()), Is.False);

      Assert.That (service.TryGetExtensibleEnumerationValueDisplayName (Color.Values.Red (), out resourceValue), Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Rot"));
      Assert.That (service.GetExtensibleEnumerationValueDisplayName (Color.Values.Red ()), Is.EqualTo ("Rot"));
      Assert.That (service.GetExtensibleEnumerationValueDisplayNameOrDefault (Color.Values.Red ()), Is.EqualTo ("Rot"));
      Assert.That (service.ContainsExtensibleEnumerationValueDisplayName (Color.Values.Red ()), Is.True);

      Assert.That (service.TryGetExtensibleEnumerationValueDisplayName (Color.Values.Green (), out resourceValue), Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Grün"));
      Assert.That (service.GetExtensibleEnumerationValueDisplayName (Color.Values.Green ()), Is.EqualTo ("Grün"));
      Assert.That (service.GetExtensibleEnumerationValueDisplayNameOrDefault (Color.Values.Green ()), Is.EqualTo ("Grün"));
      Assert.That (service.ContainsExtensibleEnumerationValueDisplayName (Color.Values.Green ()), Is.True);

      Assert.That (service.TryGetExtensibleEnumerationValueDisplayName (Color.Values.RedMetallic (), out resourceValue), Is.False);
      Assert.That (service.GetExtensibleEnumerationValueDisplayName (Color.Values.RedMetallic ()), Is.EqualTo ("RedMetallic"));
      Assert.That (service.GetExtensibleEnumerationValueDisplayNameOrDefault (Color.Values.RedMetallic ()), Is.Null);
      Assert.That (service.ContainsExtensibleEnumerationValueDisplayName (Color.Values.RedMetallic ()), Is.False);

      Assert.That (service.TryGetExtensibleEnumerationValueDisplayName (Color.Values.LightRed (), out resourceValue), Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Hellrot"));
      Assert.That (service.GetExtensibleEnumerationValueDisplayName (Color.Values.LightRed ()), Is.EqualTo ("Hellrot"));
      Assert.That (service.GetExtensibleEnumerationValueDisplayNameOrDefault (Color.Values.LightRed ()), Is.EqualTo ("Hellrot"));
      Assert.That (service.ContainsExtensibleEnumerationValueDisplayName (Color.Values.LightRed ()), Is.True);

      Assert.That (service.TryGetExtensibleEnumerationValueDisplayName (Color.Values.LightBlue (), out resourceValue), Is.False);
      Assert.That (resourceValue, Is.Null);
      Assert.That (service.GetExtensibleEnumerationValueDisplayName (Color.Values.LightBlue ()), Is.EqualTo ("LightBlue"));
      Assert.That (service.GetExtensibleEnumerationValueDisplayNameOrDefault (Color.Values.LightBlue ()), Is.Null);
      Assert.That (service.ContainsExtensibleEnumerationValueDisplayName (Color.Values.LightBlue ()), Is.False);
    }
  }
}