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
using Remotion.Globalization.Implementation;
using Remotion.Reflection;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Globalization.UnitTests.Implementation
{
  [TestFixture]
  public class MultiLingualNameBasedEnumerationGlobalizationServiceTest
  {
    private enum TestEnum
    {
      ValueWithoutMultiLingualNameAttribute,

      [MultiLingualNameAttribute ("The Name", "")]
      ValueWithMultiLingualNameAttributeForInvariantCulture,

      [MultiLingualNameAttribute ("The Name invariant", "")]
      [MultiLingualNameAttribute ("The Name fr-FR", "fr-FR")]
      [MultiLingualNameAttribute ("The Name en", "en")]
      [MultiLingualNameAttribute ("The Name en-US", "en-US")]
      ValueWithMultiLingualNameAttributes,

      [MultiLingualNameAttribute ("The Name fr-FR", "fr-FR")]
      ValueWithoutInvariantCulture,

      [MultiLingualNameAttribute ("The Name invariant", "")]
      [MultiLingualNameAttribute ("The Name fr-FR", "fr-FR")]
      [MultiLingualNameAttribute ("The Name fr-FR", "fr-FR")]
      [MultiLingualNameAttribute ("The Name en-US", "en-US")]
      ValueWithDuplicateMultiLingualNameAttributes
    }

    [Flags]
    private enum FlagsEnum
    {
      [MultiLingualNameAttribute ("The Value 1", "")]
      Value1 = 1,

      [MultiLingualNameAttribute ("The Value 2", "")]
      Value2 = 2
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultiLingualNameAttribute_ReturnsTheName ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      string multiLingualName;

      var result = service.TryGetEnumerationValueDisplayName (TestEnum.ValueWithMultiLingualNameAttributeForInvariantCulture, out multiLingualName);

      Assert.That (result, Is.True);
      Assert.That (multiLingualName, Is.EqualTo ("The Name"));
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureMatchesSpecificCulture_ReturnsForTheSpecificCulture ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      using (new CultureScope ("it-IT", "en-US"))
      {
        string multiLingualName;

        var result = service.TryGetEnumerationValueDisplayName (TestEnum.ValueWithMultiLingualNameAttributes, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en-US"));
      }
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureOnlyMatchesNeutralCulture_ReturnsForTheNeutralCulture ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      using (new CultureScope ("it-IT", "en-GB"))
      {
        string multiLingualName;

        var result = service.TryGetEnumerationValueDisplayName (TestEnum.ValueWithMultiLingualNameAttributes, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en"));
      }
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureOnlyMatchesInvariantCulture_ReturnsForTheInvariantCulture ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      using (new CultureScope ("it-IT", "de-AT"))
      {
        string multiLingualName;

        var result = service.TryGetEnumerationValueDisplayName (TestEnum.ValueWithMultiLingualNameAttributes, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name invariant"));
      }
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureDoesNotMatchAnyCulture_ThrowsMissingLocalizationException ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      using (new CultureScope ("it-IT", "en-GB"))
      {
        string multiLingualName;

        Assert.That (
            () => service.TryGetEnumerationValueDisplayName (TestEnum.ValueWithoutInvariantCulture, out multiLingualName),
            Throws.TypeOf<MissingLocalizationException>().With.Message.EqualTo (
                "The enum value 'ValueWithoutInvariantCulture' declared on type "
                + "'Remotion.Globalization.UnitTests.Implementation.MultiLingualNameBasedEnumerationGlobalizationServiceTest+TestEnum' "
                + "has one or more MultiLingualNameAttributes applied "
                + "but does not define a localization for the current UI culture 'en-GB' or a valid fallback culture "
                + "(i.e. there is no localization defined for the invariant culture)."));
      }
    }
    
    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultipleMultiLingualNameAttributesForSameCulture_ThrowsInvalidOperationException ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name fr-FR", "fr-FR"),
                  new MultiLingualNameAttribute ("The Name fr-FR", "fr-FR"),
                  new MultiLingualNameAttribute ("The Name en-GB", "en-GB")
              });
      typeInformationStub.Stub(_ =>_.FullName).Return("The.Full.Type.Name");

      using (new CultureScope ("it-IT", "en-US"))
      {
        string multiLingualName;

        Assert.That (
            () => service.TryGetEnumerationValueDisplayName (TestEnum.ValueWithDuplicateMultiLingualNameAttributes, out multiLingualName),
            Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo (
                "The enum value 'ValueWithDuplicateMultiLingualNameAttributes' declared on type "
                + "'Remotion.Globalization.UnitTests.Implementation.MultiLingualNameBasedEnumerationGlobalizationServiceTest+TestEnum' "
                + "has more than one MultiLingualNameAttribute for the culture 'fr-FR' applied. "
                + "The used cultures must be unique within the set of MultiLingualNameAttributes for a type."));
      }
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithoutMultiLingualNameAttribute_ReturnsNull ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (new MultiLingualNameAttribute[0]);
      typeInformationStub.Stub (_ => _.BaseType).Return (null);

      string multiLingualName;

      var result = service.TryGetEnumerationValueDisplayName (TestEnum.ValueWithoutMultiLingualNameAttribute, out multiLingualName);

      Assert.That (result, Is.False);
      Assert.That (multiLingualName, Is.Null);
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithUnknownEnumValue_ReturnsNull ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      string multiLingualName;

      var result = service.TryGetEnumerationValueDisplayName ((TestEnum) 100, out multiLingualName);

      Assert.That (result, Is.False);
      Assert.That (multiLingualName, Is.Null);
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithCombinedFlagsEnumValue_ReturnsNull ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      string multiLingualName;

      var result = service.TryGetEnumerationValueDisplayName (FlagsEnum.Value1 | FlagsEnum.Value2, out multiLingualName);

      Assert.That (result, Is.False);
      Assert.That (multiLingualName, Is.Null);
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultipleCalls_UsesCacheToRetrieveTheLocalizedName ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      using (new CultureScope ("", "en-US"))
      {
        string multiLingualName;
        var result = service.TryGetEnumerationValueDisplayName (TestEnum.ValueWithMultiLingualNameAttributes, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en-US"));
      }

      using (new CultureScope ("", "fr-FR"))
      {
        string multiLingualName;
        var result = service.TryGetEnumerationValueDisplayName (TestEnum.ValueWithMultiLingualNameAttributes, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name fr-FR"));
      }
    }
  }
}