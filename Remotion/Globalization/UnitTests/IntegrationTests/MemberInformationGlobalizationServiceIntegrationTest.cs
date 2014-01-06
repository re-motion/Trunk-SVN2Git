﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Globalization.UnitTests.TestDomain;
using Remotion.Reflection;
using Remotion.ServiceLocation;

namespace Remotion.Globalization.UnitTests.IntegrationTests
{
  [TestFixture]
  public class MemberInformationGlobalizationServiceIntegrationTest
  {
    [Test]
    public void TryGetTypeDisplayName ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IMemberInformationGlobalizationService>();

      string resourceValue;
      Assert.That (
          service.TryGetTypeDisplayName (
              TypeAdapter.Create (typeof (ClassWithShortResourceIdentifier)),
              TypeAdapter.Create (typeof (ClassWithResources)),
              out resourceValue),
          Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Short Type ID"));

      Assert.That (
          service.TryGetTypeDisplayName (
              TypeAdapter.Create (typeof (ClassWithLongResourceIdentifier)),
              TypeAdapter.Create (typeof (ClassWithResources)),
              out resourceValue),
          Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Long Type ID"));

      Assert.That (
          service.TryGetTypeDisplayName (
              TypeAdapter.Create (typeof (ClassWithLongResourceIdentifier)),
              TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes)),
              out resourceValue),
          Is.False);
      Assert.That (resourceValue, Is.Null);
    }

    [Test]
    public void GetTypeDisplayName ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IMemberInformationGlobalizationService>();

      Assert.That (
          service.GetTypeDisplayName (
              TypeAdapter.Create (typeof (ClassWithShortResourceIdentifier)),
              TypeAdapter.Create (typeof (ClassWithResources))),
          Is.EqualTo ("Short Type ID"));

      Assert.That (
          service.GetTypeDisplayName (
              TypeAdapter.Create (typeof (ClassWithLongResourceIdentifier)),
              TypeAdapter.Create (typeof (ClassWithResources))),
          Is.EqualTo ("Long Type ID"));

      Assert.That (
          service.GetTypeDisplayName (
              TypeAdapter.Create (typeof (ClassWithLongResourceIdentifier)),
              TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes))),
          Is.EqualTo ("ClassWithLongResourceIdentifier"));
    }

    [Test]
    public void ContainsTypeDisplayName ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IMemberInformationGlobalizationService>();

      Assert.That (
          service.ContainsTypeDisplayName (
              TypeAdapter.Create (typeof (ClassWithShortResourceIdentifier)),
              TypeAdapter.Create (typeof (ClassWithResources))),
          Is.True);

      Assert.That (
          service.ContainsTypeDisplayName (
              TypeAdapter.Create (typeof (ClassWithLongResourceIdentifier)),
              TypeAdapter.Create (typeof (ClassWithResources))),
          Is.True);

      Assert.That (
          service.ContainsTypeDisplayName (
              TypeAdapter.Create (typeof (ClassWithLongResourceIdentifier)),
              TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes))),
          Is.False);
    }

    [Test]
    public void TryGetPropertyDisplayName ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IMemberInformationGlobalizationService>();

      string resourceValue;
      Assert.That (
          service.TryGetPropertyDisplayName (
              PropertyInfoAdapter.Create (typeof (ClassWithProperties).GetProperty ("PropertyWithShortIdentifier")),
              TypeAdapter.Create (typeof (ClassWithResources)),
              out resourceValue),
          Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Short Property ID"));

      Assert.That (
          service.TryGetPropertyDisplayName (
              PropertyInfoAdapter.Create (typeof (ClassWithProperties).GetProperty ("PropertyWithLongIdentifier")),
              TypeAdapter.Create (typeof (ClassWithResources)),
              out resourceValue),
          Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Long Property ID"));

      Assert.That (
          service.TryGetPropertyDisplayName (
              PropertyInfoAdapter.Create (typeof (ClassWithProperties).GetProperty ("PropertyWithoutResources")),
              TypeAdapter.Create (typeof (ClassWithResources)),
              out resourceValue),
          Is.False);
      Assert.That (resourceValue, Is.Null);

      Assert.That (
          service.TryGetPropertyDisplayName (
              PropertyInfoAdapter.Create (typeof (ClassWithProperties).GetProperty ("PropertyWithLongIdentifier")),
              TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes)),
              out resourceValue),
          Is.False);
      Assert.That (resourceValue, Is.Null);
    }

    [Test]
    public void GetPropertyDisplayName ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IMemberInformationGlobalizationService>();

      Assert.That (
          service.GetPropertyDisplayName (
              PropertyInfoAdapter.Create (typeof (ClassWithProperties).GetProperty ("PropertyWithShortIdentifier")),
              TypeAdapter.Create (typeof (ClassWithResources))),
          Is.EqualTo ("Short Property ID"));

      Assert.That (
          service.GetPropertyDisplayName (
              PropertyInfoAdapter.Create (typeof (ClassWithProperties).GetProperty ("PropertyWithLongIdentifier")),
              TypeAdapter.Create (typeof (ClassWithResources))),
          Is.EqualTo ("Long Property ID"));

      Assert.That (
          service.GetPropertyDisplayName (
              PropertyInfoAdapter.Create (typeof (ClassWithProperties).GetProperty ("PropertyWithoutResources")),
              TypeAdapter.Create (typeof (ClassWithResources))),
          Is.EqualTo ("PropertyWithoutResources"));

      Assert.That (
          service.GetPropertyDisplayName (
              PropertyInfoAdapter.Create (typeof (ClassWithProperties).GetProperty ("PropertyWithLongIdentifier")),
              TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes))),
          Is.EqualTo ("PropertyWithLongIdentifier"));
    }

    [Test]
    public void ContainsPropertyDisplayName ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IMemberInformationGlobalizationService>();

      Assert.That (
          service.ContainsPropertyDisplayName (
              PropertyInfoAdapter.Create (typeof (ClassWithProperties).GetProperty ("PropertyWithShortIdentifier")),
              TypeAdapter.Create (typeof (ClassWithResources))),
          Is.True);

      Assert.That (
          service.ContainsPropertyDisplayName (
              PropertyInfoAdapter.Create (typeof (ClassWithProperties).GetProperty ("PropertyWithLongIdentifier")),
              TypeAdapter.Create (typeof (ClassWithResources))),
          Is.True);

      Assert.That (
          service.ContainsPropertyDisplayName (
              PropertyInfoAdapter.Create (typeof (ClassWithProperties).GetProperty ("PropertyWithoutResources")),
              TypeAdapter.Create (typeof (ClassWithResources))),
          Is.False);

      Assert.That (
          service.ContainsPropertyDisplayName (
              PropertyInfoAdapter.Create (typeof (ClassWithProperties).GetProperty ("PropertyWithLongIdentifier")),
              TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes))),
          Is.False);
    }
  }
}