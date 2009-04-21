// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class ConstructionOfRelationEndPointDefinitionTest: StandardMappingTest
  {
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "Relation definition error: Property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.Name' of class 'Company' is of type "
        + "'System.String', but non-virtual properties must be of type 'Remotion.Data.DomainObjects.ObjectID'.")]
    public void PropertyOfWrongType()
    {
      ClassDefinition companyDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (Company)];

      new RelationEndPointDefinition (companyDefinition, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.Name", false);
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
