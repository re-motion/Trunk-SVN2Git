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
    //PropertyTypeIsSupportedValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
      "The property type System.Object is not supported.\r\n\r\n"
      +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPropertyTypes."
      +"PropertyTypeOfObjectWithoutStorageClassNone.ClassWithInvalidPropertyType\r\n"
      +"Property: InvalidProperty")]
    public void PropertyTypeOfObjectWithoutStorageClassNone ()
    {
      ValidateMapping ("NotSupportedPropertyTypes.PropertyTypeOfObjectWithoutStorageClassNone");
    }

    //Exception is thrown in MappingConfiguration (DomainObject class is not added to the mapping) 
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Mapping does not contain class 'Remotion.Data.DomainObjects.DomainObject'.\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPropertyTypes."
        +"PropertyTypeOfDomainObjectWithoutStorageClassNone.ClassWithInvalidPropertyType\r\n"
        +"Property: InvalidProperty"
        )]
    public void PropertyTypeOfDomainObjectWithoutStorageClassNone ()
    {
      ValidateMapping ("NotSupportedPropertyTypes.PropertyTypeOfDomainObjectWithoutStorageClassNone");
    }

    //Exception is thrown in MappingConfiguration (classes above inheritance hierarchy are not added to the mapping) 
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Mapping does not contain class 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration."
        +"NotSupportedPropertyTypes.PropertyTypeToDomainObjectAboveTheInheritanceRoot.ClassAboveInheritanceRoot'.\r\n"
        +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPropertyTypes."+
        "PropertyTypeToDomainObjectAboveTheInheritanceRoot.InheritanceRootClass\r\n"
        +"Property: InvalidProperty"
        )]
    public void PropertyTypeToDomainObjectAboveTheInheritanceRoot ()
    {
      ValidateMapping ("NotSupportedPropertyTypes.PropertyTypeToDomainObjectAboveTheInheritanceRoot");
    }

    //PropertyTypeIsSupportedValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
      "The property type Remotion.Data.DomainObjects.ObjectList`1[Remotion.Data.DomainObjects.DomainObject] is not supported.\r\n\r\n"
      +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPropertyTypes."
      +"PropertyTypeOfObjectList_DomainObject.ClassWithInvalidProperty\r\n"
      +"Property: InvalidProperty1\r\n"
      +"----------\r\n"
      +"The property type Remotion.Data.DomainObjects.ObjectList`1[Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors."
      +"ValidationIntegration.NotSupportedPropertyTypes.PropertyTypeOfObjectList_DomainObjectAboveInheritanceRoot.ClassAboveInheritanceRoot] "
      +"is not supported.\r\n\r\n"
      +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPropertyTypes."
      +"PropertyTypeOfObjectList_DomainObjectAboveInheritanceRoot.ClassWithInvalidProperty\r\n"
      +"Property: InvalidProperty")]
    public void PropertyTypeOfObjectList_DomainObject ()
    {
      ValidateMapping ("NotSupportedPropertyTypes.PropertyTypeOfObjectList_DomainObject");
    }

    //PropertyTypeIsSupportedValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
      "The property type Remotion.Data.DomainObjects.ObjectList`1[Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors."
      +"ValidationIntegration.NotSupportedPropertyTypes.PropertyTypeOfObjectList_DomainObjectAboveInheritanceRoot.ClassAboveInheritanceRoot] is not "
      +"supported.\r\n\r\n"
      +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPropertyTypes."
      +"PropertyTypeOfObjectList_DomainObjectAboveInheritanceRoot.ClassWithInvalidProperty\r\n"
      +"Property: InvalidProperty")]
    public void PropertyTypeOfObjectList_DomainObjectAboveInheritanceRoot ()
    {
      ValidateMapping ("NotSupportedPropertyTypes.PropertyTypeOfObjectList_DomainObjectAboveInheritanceRoot");
    }

    //PropertyTypeIsSupportedValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
       "The property type Remotion.Data.DomainObjects.ObjectList`1[Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors."
       +"ValidationIntegration.NotSupportedPropertyTypes.PropertyTypeOfObjectList_DerivedDomainObject_Unidirectional.DerivedDomainObjectClass] "
       +"is not supported.\r\n\r\n"
       +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPropertyTypes."
       +"PropertyTypeOfObjectList_DerivedDomainObject_Unidirectional.ClassWithInvalidPropertyType\r\n"
       +"Property: InvalidProperty")]
    public void PropertyTypeOfObjectList_DerivedDomainObject_Unidirectional ()
    {
      ValidateMapping ("NotSupportedPropertyTypes.PropertyTypeOfObjectList_DerivedDomainObject_Unidirectional");
    }
  }
}