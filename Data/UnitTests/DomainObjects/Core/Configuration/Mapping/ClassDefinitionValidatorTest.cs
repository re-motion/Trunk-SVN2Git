// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class ClassDefinitionValidatorTest : StandardMappingTest
  {
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Class 'Partner' must not define property 'Name', because base class 'Company' already defines a property with the same name.")]
    public void ValidateMappingWithDuplicatePropertyBaseClass ()
    {
      ReflectionBasedClassDefinition companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false);

      ReflectionBasedClassDefinition partnerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Partner", "Company", "TestDomain", typeof (Partner), false, companyClass);

      partnerClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (partnerClass, "Name", "Name", typeof (string), 100));
      companyClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (companyClass, "Name", "Name", typeof (string), 100));

      new ClassDefinitionValidator (companyClass).ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Class 'Supplier' must not define property 'Name', because base class 'Company' already defines a property with the same name.")]
    public void ValidateMappingWithDuplicatePropertyBaseOfBaseClass ()
    {
      ReflectionBasedClassDefinition companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false);

      ReflectionBasedClassDefinition partnerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Partner", "Company", "TestDomain", typeof (Partner), false, companyClass);

      ReflectionBasedClassDefinition supplierClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Supplier", "Company", "TestDomain", typeof (Supplier), false, partnerClass);

      supplierClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (supplierClass, "Name", "Name", typeof (string), 100));
      companyClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (companyClass, "Name", "Name", typeof (string), 100));

      new ClassDefinitionValidator (companyClass).ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }
  }
}
