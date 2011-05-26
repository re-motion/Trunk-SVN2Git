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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class ValueTypes : BaseTest
  {
    [Test]
    public void GetMetadata_WithBasicType ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithAllDataTypes> ("BooleanProperty", DomainModelConstraintProviderMock);

      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (propertyReflector.PropertyInfo))
          .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithAllDataTypes.BooleanProperty", actual.PropertyName);
      Assert.AreSame (typeof (bool), actual.PropertyType);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (false, actual.DefaultValue);
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_WithNullableBasicType ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithAllDataTypes> ("NaBooleanProperty", DomainModelConstraintProviderMock);

      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (propertyReflector.PropertyInfo))
          .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithAllDataTypes.NaBooleanProperty", actual.PropertyName);
      Assert.AreSame (typeof (bool?), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.IsNull (actual.DefaultValue);
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_WithEnumProperty ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithAllDataTypes> ("EnumProperty", DomainModelConstraintProviderMock);

      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (propertyReflector.PropertyInfo))
          .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithAllDataTypes.EnumProperty", actual.PropertyName);
      Assert.AreSame (typeof (ClassWithAllDataTypes.EnumType), actual.PropertyType);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, actual.DefaultValue);
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_WithExtensibleEnumProperty ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithAllDataTypes> (
          "ExtensibleEnumProperty", DomainModelConstraintProviderMock);

      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (propertyReflector.PropertyInfo))
          .Return (false);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (propertyReflector.PropertyInfo))
          .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithAllDataTypes.ExtensibleEnumProperty",
          actual.PropertyName);
      Assert.AreSame (typeof (Color), actual.PropertyType);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (Color.Values.Blue(), actual.DefaultValue);
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_WithOptionalRelationProperty ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithGuidKey> (
          "ClassWithValidRelationsOptional", DomainModelConstraintProviderMock);

      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (propertyReflector.PropertyInfo))
          .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithValidRelationsOptional",
          actual.PropertyName);
      Assert.AreSame (typeof (ObjectID), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    public object ObjectProperty
    {
      get { return null; }
      set { }
    }
  }
}