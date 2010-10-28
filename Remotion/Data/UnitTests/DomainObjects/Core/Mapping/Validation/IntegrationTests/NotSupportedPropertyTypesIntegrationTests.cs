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
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.IntegrationTests
{
  [TestFixture]
  public class NotSupportedPropertyTypesIntegrationTests : ValidationIntegrationTestBase
  {
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "The property type System.Object is not supported.\r\n"
      + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPropertyTypes.PropertyTypeOfObjectWithoutStorageClassNone.ClassWithInvalidPropertyType, property: InvalidProperty")]
    public void PropertyTypeOfObjectWithoutStorageClassNone ()
    {
      ValidateMapping ("NotSupportedPropertyTypes.PropertyTypeOfObjectWithoutStorageClassNone");
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Mapping does not contain class 'Remotion.Data.DomainObjects.DomainObject'.\r\n"
      + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPropertyTypes.PropertyTypeOfDomainObjectWithoutStorageClassNone.ClassWithInvalidPropertyType, property: InvalidProperty")]
    public void PropertyTypeOfDomainObjectWithoutStorageClassNone ()
    {
      ValidateMapping ("NotSupportedPropertyTypes.PropertyTypeOfDomainObjectWithoutStorageClassNone");
    }

    [Test]
    [ExpectedException (typeof (MappingException), 
      ExpectedMessage = "Mapping does not contain class 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPropertyTypes.PropertyTypeToDomainObjectAboveTheInheritanceRoot.ClassAboveInheritanceRoot'.\r\n"
      + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPropertyTypes.PropertyTypeToDomainObjectAboveTheInheritanceRoot.InheritanceRootClass, property: InvalidProperty")]
    public void PropertyTypeToDomainObjectAboveTheInheritanceRoot ()
    {
      ValidateMapping ("NotSupportedPropertyTypes.PropertyTypeToDomainObjectAboveTheInheritanceRoot");
    }

    [Test]
    [ExpectedException (typeof (MappingException), 
      ExpectedMessage = "The property type Remotion.Data.DomainObjects.ObjectList`1[Remotion.Data.DomainObjects.DomainObject] is not supported.\r\n"
      + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPropertyTypes.PropertyTypeOfObjectList_DomainObject.ClassWithInvalidProperty, property: InvalidProperty1")]
    public void PropertyTypeOfObjectList_DomainObject ()
    {
      ValidateMapping ("NotSupportedPropertyTypes.PropertyTypeOfObjectList_DomainObject");
    }

    [Test]
    [ExpectedException (typeof (MappingException),
      ExpectedMessage = "The property type Remotion.Data.DomainObjects.ObjectList`1[Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPropertyTypes.PropertyTypeOfObjectList_DomainObjectAboveInheritanceRoot.ClassAboveInheritanceRoot] is not supported.\r\n"
      + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPropertyTypes.PropertyTypeOfObjectList_DomainObjectAboveInheritanceRoot.ClassWithInvalidProperty, property: InvalidProperty")]
    public void PropertyTypeOfObjectList_DomainObjectAboveInheritanceRoot ()
    {
      ValidateMapping ("NotSupportedPropertyTypes.PropertyTypeOfObjectList_DomainObjectAboveInheritanceRoot");
    }
  }
}