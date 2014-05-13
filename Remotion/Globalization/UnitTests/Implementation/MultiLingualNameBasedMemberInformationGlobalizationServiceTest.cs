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
  public class MultiLingualNameBasedMemberInformationGlobalizationServiceTest
  {
    [Test]
    public void TryGetTypeDisplayName_WithMultiLingualNameAttribute_ReturnsTheName ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name", "")
              });

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      string multiLingualName;

      var result = service.TryGetTypeDisplayName (typeInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

      Assert.That (result, Is.True);
      Assert.That (multiLingualName, Is.EqualTo ("The Name"));
    }

    [Test]
    public void TryGetTypeDisplayName_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureMatchesSpecificCulture_ReturnsForTheSpecificCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name fr-FR", "fr-FR"),
                  new MultiLingualNameAttribute ("The Name en", "en"),
                  new MultiLingualNameAttribute ("The Name en-US", "en-US")
              });

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();


      using (new CultureScope ("it-IT", "en-US"))
      {
        string multiLingualName;

        var result = service.TryGetTypeDisplayName (typeInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en-US"));
      }
    }


    [Test]
    public void TryGetTypeDisplayName_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureOnlyMatchesNeutralCulture_ReturnsForTheNeutralCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name fr-FR", "fr-FR"),
                  new MultiLingualNameAttribute ("The Name en", "en"),
                  new MultiLingualNameAttribute ("The Name en-GB", "en-GB")
              });

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();


      using (new CultureScope ("it-IT", "en-US"))
      {
        string multiLingualName;

        var result = service.TryGetTypeDisplayName (typeInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en"));
      }
    }

    [Test]
    public void TryGetTypeDisplayName_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureOnlyMatchesInvariantCulture_ReturnsForTheInvariantCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name fr-FR", "fr-FR"),
                  new MultiLingualNameAttribute ("The Name invariant", ""),
                  new MultiLingualNameAttribute ("The Name en-GB", "en-GB")
              });

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();


      using (new CultureScope ("it-IT", "en-US"))
      {
        string multiLingualName;

        var result = service.TryGetTypeDisplayName (typeInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name invariant"));
      }
    }

    [Test]
    public void TryGetTypeDisplayName_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureDoesNotMatchAnyCulture_ThrowsMissingLocalizationException ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name fr-FR", "fr-FR"),
                  new MultiLingualNameAttribute ("The Name en-GB", "en-GB")
              });
      typeInformationStub.Stub(_ =>_.FullName).Return("The.Full.Type.Name");

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      using (new CultureScope ("it-IT", "en-US"))
      {
        string multiLingualName;

        Assert.That (
            () => service.TryGetTypeDisplayName (typeInformationStub, typeInformationForResourceResolutionStub, out multiLingualName),
            Throws.TypeOf<MissingLocalizationException>().With.Message.EqualTo (
                "The type 'The.Full.Type.Name' has one or more MultiLingualNameAttributes applied "
                + "but does not define a localization for the current UI culture 'en-US' or a valid fallback culture "
                + "(i.e. there is no localization defined for the invariant culture)."));
      }
    }
    
    [Test]
    public void TryGetTypeDisplayName_WithMultipleMultiLingualNameAttributesForSameCulture_ThrowsInvalidOperationException ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

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

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      using (new CultureScope ("it-IT", "en-US"))
      {
        string multiLingualName;
        
        Assert.That (
            () => service.TryGetTypeDisplayName (typeInformationStub, typeInformationForResourceResolutionStub, out multiLingualName),
            Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo (
                "The type 'The.Full.Type.Name' has more than one MultiLingualNameAttribute for the culture 'fr-FR' applied. "
                + "The used cultures must be unique within the set of MultiLingualNameAttributes for a type."));
      }
    }

    [Test]
    public void TryGetTypeDisplayName_WithoutMultiLingualNameAttribute_ReturnsNull ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (new MultiLingualNameAttribute[0]);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      string multiLingualName;

      var result = service.TryGetTypeDisplayName (typeInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

      Assert.That (result, Is.False);
      Assert.That (multiLingualName, Is.Null);
    }
  }
}