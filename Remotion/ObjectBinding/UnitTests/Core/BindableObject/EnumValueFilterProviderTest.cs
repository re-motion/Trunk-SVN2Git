// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class EnumValueFilterProviderTest
  {
    private DisableEnumValuesAttribute _attribute;

    [SetUp]
    public void SetUp ()
    {
      _attribute = new DisableEnumValuesAttribute (typeof (StubEnumerationValueFilter));
    }

    [Test]
    public void GetEnumerationValueFilter_FromPropertyInformation ()
    {
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation> ();
      propertyInformationStub.Stub (stub => stub.GetCustomAttribute<DisableEnumValuesAttribute> (true)).Return (_attribute);

      var provider = new EnumValueFilterProvider<DisableEnumValuesAttribute> (
          propertyInformationStub,
          delegate
          {
            Assert.Fail ("Must not be called");
            return null;
          });
      Assert.That (provider.GetEnumerationValueFilter (), Is.TypeOf (typeof (StubEnumerationValueFilter)));
    }

    [Test]
    public void GetEnumerationValueFilter_FromAttributeProvider ()
    {
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation> ();
      propertyInformationStub.Stub (stub => stub.GetCustomAttribute<DisableEnumValuesAttribute> (true)).Return (null);
      propertyInformationStub.Stub (stub => stub.PropertyType).Return (typeof (int));

      var provider = new EnumValueFilterProvider<DisableEnumValuesAttribute> (
          propertyInformationStub,
          delegate (Type type)
          {
            Assert.That (type, Is.SameAs (typeof (int)));
            return new[] { _attribute };
          });
      Assert.That (provider.GetEnumerationValueFilter (), Is.TypeOf (typeof (StubEnumerationValueFilter)));
    }

    [Test]
    public void GetEnumerationValueFilter_FromAttributeProvider_Multiple ()
    {
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation> ();
      propertyInformationStub.Stub (stub => stub.GetCustomAttribute<IDisableEnumValuesAttribute> (true)).Return (null);
      propertyInformationStub.Stub (stub => stub.PropertyType).Return (typeof (int));

      var filterStub = MockRepository.GenerateStub<IEnumerationValueFilter> ();
      
      var additionalAttributeStub = MockRepository.GenerateStub<IDisableEnumValuesAttribute> ();
      additionalAttributeStub.Stub (stub => stub.GetEnumerationValueFilter ()).Return (filterStub);

      var provider = new EnumValueFilterProvider<IDisableEnumValuesAttribute> (
          propertyInformationStub,
          delegate (Type type)
          {
            Assert.That (type, Is.SameAs (typeof (int)));
            return new[] { _attribute, additionalAttributeStub };
          });

      var actualFilter = provider.GetEnumerationValueFilter ();

      Assert.That (actualFilter, Is.TypeOf (typeof (CompositeEnumerationValueFilter)));

      var compositeFilter = ((CompositeEnumerationValueFilter) actualFilter);
      Assert.That (compositeFilter.Filters.Count, Is.EqualTo (2));
      Assert.That (compositeFilter.Filters[0], Is.TypeOf (typeof (StubEnumerationValueFilter)));
      Assert.That (compositeFilter.Filters[1], Is.SameAs (filterStub));
    }

    [Test]
    public void GetEnumerationValueFilter_None ()
    {
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation> ();
      propertyInformationStub.Stub (stub => stub.GetCustomAttribute<DisableEnumValuesAttribute> (true)).Return (null);
      propertyInformationStub.Stub (stub => stub.PropertyType).Return (typeof (int));

      var provider = new EnumValueFilterProvider<DisableEnumValuesAttribute> (
          propertyInformationStub,
          delegate (Type type)
          {
            Assert.That (type, Is.SameAs (typeof (int)));
            return new DisableEnumValuesAttribute[0];
          });
      Assert.That (provider.GetEnumerationValueFilter (), Is.TypeOf (typeof (NullEnumerationValueFilter)));
    }
  }
}