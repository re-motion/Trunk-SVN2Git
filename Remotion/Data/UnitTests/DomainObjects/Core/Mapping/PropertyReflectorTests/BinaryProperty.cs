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
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class BinaryProperty: BaseTest
  {
    
    [Test]
    public void GetMetadata_NoDomainObjectAndNoValueType_NullableTrue()
    {
      var propertyReflector = CreatePropertyReflector<ClassWithBinaryProperties> ("NoAttribute", DomainModelConstraintProviderMock);

      DomainModelConstraintProviderMock
         .Expect (mock => mock.IsNullable (propertyReflector.PropertyInfo))
         .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (propertyReflector.PropertyInfo))
          .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      var actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithBinaryProperties.NoAttribute",
          actual.PropertyName);
      Assert.AreSame (typeof (byte[]), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_NoDomainObjectAndNoValueType_NullableFalse ()
    {
      var propertyReflector = CreatePropertyReflector<ClassWithBinaryProperties> ("NoAttribute", DomainModelConstraintProviderMock);

      DomainModelConstraintProviderMock
         .Expect (mock => mock.IsNullable (propertyReflector.PropertyInfo))
         .Return (false);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (propertyReflector.PropertyInfo))
          .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      var actual = propertyReflector.GetMetadata ();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithBinaryProperties.NoAttribute",
          actual.PropertyName);
      Assert.AreSame (typeof (byte[]), actual.PropertyType);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (new byte[0], actual.DefaultValue);
      DomainModelConstraintProviderMock.VerifyAllExpectations ();
    }
    
    [Test]
    public void GetMetadata_WithMaximumLength()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithBinaryProperties> ("MaximumLength", DomainModelConstraintProviderMock);

      DomainModelConstraintProviderMock
         .Expect (mock => mock.IsNullable (propertyReflector.PropertyInfo))
         .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (propertyReflector.PropertyInfo))
          .Return (100);
      DomainModelConstraintProviderMock.Replay ();

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithBinaryProperties.MaximumLength",
          actual.PropertyName);
      Assert.AreSame (typeof (byte[]), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (100, actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [BinaryProperty]
    private int Int32Property
    {
      get { throw new NotImplementedException(); }
    }
  }
}
