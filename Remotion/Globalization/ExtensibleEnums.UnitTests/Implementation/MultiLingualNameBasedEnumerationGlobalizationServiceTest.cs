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
using System.Reflection;
using NUnit.Framework;
using Remotion.ExtensibleEnums;
using Remotion.Globalization.ExtensibleEnums.Implementation;
using Remotion.Globalization.Implementation;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Globalization.ExtensibleEnums.UnitTests.Implementation
{
  [TestFixture]
  public class MultiLingualNameBasedExtensibleEnumGlobalizationServiceTest
  {
    private class TestEnum : ExtensibleEnum<TestEnum>
    {
      public TestEnum (string declarationSpace, string valueName)
          : base (declarationSpace, valueName)
      {
      }
    }

    private static class TestEnumExtensions
    {
      public static TestEnum ValueWithoutMultiLingualNameAttribute (ExtensibleEnumDefinition<TestEnum> definition)
      {
        throw new NotImplementedException();
      }

      [MultiLingualName ("The Name", "")]
      public static TestEnum ValueWithMultiLingualNameAttributeForInvariantCulture (ExtensibleEnumDefinition<TestEnum> definition)
      {
        throw new NotImplementedException();
      }

      [MultiLingualName ("The Name invariant", "")]
      [MultiLingualName ("The Name fr-FR", "fr-FR")]
      [MultiLingualName ("The Name en", "en")]
      [MultiLingualName ("The Name en-US", "en-US")]
      public static TestEnum ValueWithMultiLingualNameAttributes (ExtensibleEnumDefinition<TestEnum> definition)
      {
        throw new NotImplementedException();
      }

      [MultiLingualName ("The Name fr-FR", "fr-FR")]
      public static TestEnum ValueWithoutInvariantCulture (ExtensibleEnumDefinition<TestEnum> definition)
      {
        throw new NotImplementedException();
      }

      [MultiLingualName ("The Name invariant", "")]
      [MultiLingualName ("The Name fr-FR", "fr-FR")]
      [MultiLingualName ("The Name fr-FR", "fr-FR")]
      [MultiLingualName ("The Name en-US", "en-US")]
      public static TestEnum ValueWithDuplicateMultiLingualNameAttributes (ExtensibleEnumDefinition<TestEnum> definition)
      {
        throw new NotImplementedException();
      }
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultiLingualNameAttribute_ReturnsTheName ()
    {
      var service = new MultiLingualNameBasedExtensibleEnumGlobalizationService();

      var extensibleEnumInfo = MockRepository.GenerateStub<IExtensibleEnumInfo>();
      extensibleEnumInfo.Stub (_ => _.DefiningMethod).Return (GetMethod ("ValueWithMultiLingualNameAttributeForInvariantCulture"));
      var extensibleEnumStub = MockRepository.GenerateStub<IExtensibleEnum>();
      extensibleEnumStub.Stub (_ => _.GetValueInfo()).Return (extensibleEnumInfo);
      
      string multiLingualName;

      var result = service.TryGetExtensibleEnumValueDisplayName (extensibleEnumStub, out multiLingualName);

      Assert.That (result, Is.True);
      Assert.That (multiLingualName, Is.EqualTo ("The Name"));
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureMatchesSpecificCulture_ReturnsForTheSpecificCulture ()
    {
      var service = new MultiLingualNameBasedExtensibleEnumGlobalizationService();
      
      var extensibleEnumInfo = MockRepository.GenerateStub<IExtensibleEnumInfo>();
      extensibleEnumInfo.Stub (_ => _.DefiningMethod).Return (GetMethod ("ValueWithMultiLingualNameAttributes"));
      var extensibleEnumStub = MockRepository.GenerateStub<IExtensibleEnum>();
      extensibleEnumStub.Stub (_ => _.GetValueInfo()).Return (extensibleEnumInfo);

      using (new CultureScope ("it-IT", "en-US"))
      {
        string multiLingualName;

      var result = service.TryGetExtensibleEnumValueDisplayName (extensibleEnumStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en-US"));
      }
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureOnlyMatchesNeutralCulture_ReturnsForTheNeutralCulture ()
    {
      var service = new MultiLingualNameBasedExtensibleEnumGlobalizationService();
      
      var extensibleEnumInfo = MockRepository.GenerateStub<IExtensibleEnumInfo>();
      extensibleEnumInfo.Stub (_ => _.DefiningMethod).Return (GetMethod ("ValueWithMultiLingualNameAttributes"));
      var extensibleEnumStub = MockRepository.GenerateStub<IExtensibleEnum>();
      extensibleEnumStub.Stub (_ => _.GetValueInfo()).Return (extensibleEnumInfo);

      using (new CultureScope ("it-IT", "en-GB"))
      {
        string multiLingualName;

      var result = service.TryGetExtensibleEnumValueDisplayName (extensibleEnumStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en"));
      }
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureOnlyMatchesInvariantCulture_ReturnsForTheInvariantCulture ()
    {
      var service = new MultiLingualNameBasedExtensibleEnumGlobalizationService();
      
      var extensibleEnumInfo = MockRepository.GenerateStub<IExtensibleEnumInfo>();
      extensibleEnumInfo.Stub (_ => _.DefiningMethod).Return (GetMethod ("ValueWithMultiLingualNameAttributes"));
      var extensibleEnumStub = MockRepository.GenerateStub<IExtensibleEnum>();
      extensibleEnumStub.Stub (_ => _.GetValueInfo()).Return (extensibleEnumInfo);

      using (new CultureScope ("it-IT", "de-AT"))
      {
        string multiLingualName;

      var result = service.TryGetExtensibleEnumValueDisplayName (extensibleEnumStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name invariant"));
      }
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureDoesNotMatchAnyCulture_ThrowsMissingLocalizationException ()
    {
      var service = new MultiLingualNameBasedExtensibleEnumGlobalizationService();
      
      var extensibleEnumInfo = MockRepository.GenerateStub<IExtensibleEnumInfo>();
      extensibleEnumInfo.Stub (_ => _.DefiningMethod).Return (GetMethod ("ValueWithoutInvariantCulture"));
      var extensibleEnumStub = MockRepository.GenerateStub<IExtensibleEnum>();
      extensibleEnumStub.Stub (_ => _.GetValueInfo()).Return (extensibleEnumInfo);

      using (new CultureScope ("it-IT", "en-GB"))
      {
        string multiLingualName;

        Assert.That (
            () => service.TryGetExtensibleEnumValueDisplayName (extensibleEnumStub, out multiLingualName),
            Throws.TypeOf<MissingLocalizationException>().With.Message.EqualTo (
                "The extensible enum value 'ValueWithoutInvariantCulture' declared on type "
                + "'Remotion.Globalization.ExtensibleEnums.UnitTests.Implementation.MultiLingualNameBasedExtensibleEnumGlobalizationServiceTest+TestEnumExtensions' "
                + "has one or more MultiLingualNameAttributes applied "
                + "but does not define a localization for the current UI culture 'en-GB' or a valid fallback culture "
                + "(i.e. there is no localization defined for the invariant culture)."));
      }
    }
    
    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultipleMultiLingualNameAttributesForSameCulture_ThrowsInvalidOperationException ()
    {
      var service = new MultiLingualNameBasedExtensibleEnumGlobalizationService();

      var extensibleEnumInfo = MockRepository.GenerateStub<IExtensibleEnumInfo>();
      extensibleEnumInfo.Stub (_ => _.DefiningMethod).Return (GetMethod ("ValueWithDuplicateMultiLingualNameAttributes"));
      var extensibleEnumStub = MockRepository.GenerateStub<IExtensibleEnum>();
      extensibleEnumStub.Stub (_ => _.GetValueInfo()).Return (extensibleEnumInfo);

      using (new CultureScope ("it-IT", "en-US"))
      {
        string multiLingualName;

        Assert.That (
            () => service.TryGetExtensibleEnumValueDisplayName (extensibleEnumStub, out multiLingualName),
            Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo (
                "The extensible enum value 'ValueWithDuplicateMultiLingualNameAttributes' declared on type "
                + "'Remotion.Globalization.ExtensibleEnums.UnitTests.Implementation.MultiLingualNameBasedExtensibleEnumGlobalizationServiceTest+TestEnumExtensions' "
                + "has more than one MultiLingualNameAttribute for the culture 'fr-FR' applied. "
                + "The used cultures must be unique within the set of MultiLingualNameAttributes for a type."));
      }
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithoutMultiLingualNameAttribute_ReturnsNull ()
    {
      var service = new MultiLingualNameBasedExtensibleEnumGlobalizationService();

      var extensibleEnumInfo = MockRepository.GenerateStub<IExtensibleEnumInfo>();
      extensibleEnumInfo.Stub (_ => _.DefiningMethod).Return (GetMethod ("ValueWithoutMultiLingualNameAttribute"));
      var extensibleEnumStub = MockRepository.GenerateStub<IExtensibleEnum>();
      extensibleEnumStub.Stub (_ => _.GetValueInfo()).Return (extensibleEnumInfo);

      string multiLingualName;

      var result = service.TryGetExtensibleEnumValueDisplayName (extensibleEnumStub, out multiLingualName);

      Assert.That (result, Is.False);
      Assert.That (multiLingualName, Is.Null);
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultipleCalls_UsesCacheToRetrieveTheLocalizedName ()
    {
      var service = new MultiLingualNameBasedExtensibleEnumGlobalizationService();

      var extensibleEnumInfo = MockRepository.GenerateStub<IExtensibleEnumInfo>();
      extensibleEnumInfo.Stub (_ => _.DefiningMethod).Return (GetMethod ("ValueWithMultiLingualNameAttributes"));
      var extensibleEnumStub = MockRepository.GenerateStub<IExtensibleEnum>();
      extensibleEnumStub.Stub (_ => _.GetValueInfo()).Return (extensibleEnumInfo);

      using (new CultureScope ("", "en-US"))
      {
        string multiLingualName;
        var result = service.TryGetExtensibleEnumValueDisplayName (extensibleEnumStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en-US"));
      }

      using (new CultureScope ("", "fr-FR"))
      {
        string multiLingualName;
        var result = service.TryGetExtensibleEnumValueDisplayName (extensibleEnumStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name fr-FR"));
      }
    }

    private MethodInfo GetMethod (string name)
    {
      var methodInfo = typeof (TestEnumExtensions).GetMethod (name);
      Assert.That (methodInfo, Is.Not.Null);
      return methodInfo;
    }
  }
}