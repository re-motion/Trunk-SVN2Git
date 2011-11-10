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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class StringProperty : BaseTest
  {
    [Test]
    public void GetMetadata_WithNullableFalse ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithStringProperties> ("NoAttribute", DomainModelConstraintProviderStub);

      DomainModelConstraintProviderStub
         .Stub(stub => stub.IsNullable (propertyReflector.PropertyInfo))
         .Return (false);

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithStringProperties.NoAttribute",
          actual.PropertyName);
      Assert.AreSame (typeof (string), actual.PropertyType);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (string.Empty, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNullableTrue ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithStringProperties> (
          "NullableFromAttribute", DomainModelConstraintProviderStub);

      DomainModelConstraintProviderStub
         .Stub(stub => stub.IsNullable (propertyReflector.PropertyInfo))
         .Return (true);

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithStringProperties.NullableFromAttribute",
          actual.PropertyName);
      Assert.AreSame (typeof (string), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithMaximumLength ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithStringProperties> ("MaximumLength", DomainModelConstraintProviderStub);

      DomainModelConstraintProviderStub
         .Stub (stub => stub.IsNullable (propertyReflector.PropertyInfo))
         .Return (true);
      DomainModelConstraintProviderStub
          .Stub (stub => stub.GetMaxLength (propertyReflector.PropertyInfo))
          .Return (100);

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithStringProperties.MaximumLength",
          actual.PropertyName);
      Assert.AreSame (typeof (string), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (100, actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNotNullableAndMaximumLength ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithStringProperties> (
          "NotNullableAndMaximumLength", DomainModelConstraintProviderStub);

      DomainModelConstraintProviderStub
         .Stub(stub => stub.IsNullable (propertyReflector.PropertyInfo))
         .Return (false);
      DomainModelConstraintProviderStub
          .Stub (stub => stub.GetMaxLength (propertyReflector.PropertyInfo))
          .Return (100);

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithStringProperties.NotNullableAndMaximumLength",
          actual.PropertyName);
      Assert.AreSame (typeof (string), actual.PropertyType);
      Assert.IsFalse (actual.IsNullable);
      Assert.AreEqual (100, actual.MaxLength);
      Assert.AreEqual (string.Empty, actual.DefaultValue);
    }

    [StringProperty]
    private int Int32Property
    {
      get { throw new NotImplementedException(); }
    }
  }
}