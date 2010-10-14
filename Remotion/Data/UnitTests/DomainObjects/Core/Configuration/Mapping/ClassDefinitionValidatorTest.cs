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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Integration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class ClassDefinitionValidatorTest : MappingReflectionTestBase
  {
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Name' of class 'Partner' must not define storage specific name 'Name', because class 'Company' in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithDuplicatePropertyBaseClass ()
    {
      ReflectionBasedClassDefinition companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false);

      ReflectionBasedClassDefinition partnerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Partner", "Company", "TestDomain", typeof (Partner), false, companyClass);

      partnerClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (partnerClass, "Name", "Name"));
      companyClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (companyClass, "Name", "Name"));

      companyClass.SetReadOnly ();
      partnerClass.SetReadOnly ();
      
      new ClassDefinitionValidator (companyClass).ValidateInheritanceHierarchy ();
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Name' of class 'Supplier' must not define storage specific name 'Name', because class 'Company' in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithDuplicatePropertyBaseOfBaseClass ()
    {
      ReflectionBasedClassDefinition companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false);

      ReflectionBasedClassDefinition partnerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Partner", "Company", "TestDomain", typeof (Partner), false, companyClass);

      ReflectionBasedClassDefinition supplierClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Supplier", "Company", "TestDomain", typeof (Supplier), false, partnerClass);

      supplierClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (supplierClass, "Name", "Name"));
      companyClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (companyClass, "Name", "Name"));

      companyClass.SetReadOnly ();
      partnerClass.SetReadOnly ();
      supplierClass.SetReadOnly ();

      new ClassDefinitionValidator (companyClass).ValidateInheritanceHierarchy ();
    }
  }
}
