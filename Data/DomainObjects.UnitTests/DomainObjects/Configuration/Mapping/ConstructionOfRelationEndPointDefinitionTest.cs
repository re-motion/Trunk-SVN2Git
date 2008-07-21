/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping
{
  [TestFixture]
  public class ConstructionOfRelationEndPointDefinitionTest: StandardMappingTest
  {
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "Relation definition error: Property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Name' of class 'Company' is of type "
        + "'System.String', but non-virtual properties must be of type 'Remotion.Data.DomainObjects.ObjectID'.")]
    public void PropertyOfWrongType()
    {
      ClassDefinition companyDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (Company)];

      new RelationEndPointDefinition (companyDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Name", false);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Relation definition error for end point: Class 'Company' has no property 'UndefinedProperty'.")]
    public void UndefinedProperty()
    {
      ClassDefinition companyDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (Company)];

      new RelationEndPointDefinition (companyDefinition, "UndefinedProperty", false);
    }
  }
}
