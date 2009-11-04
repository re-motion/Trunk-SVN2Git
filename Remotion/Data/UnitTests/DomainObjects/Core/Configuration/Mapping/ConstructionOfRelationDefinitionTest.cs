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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class ConstructionOfRelationDefinitionTest : StandardMappingTest
  {
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Relation 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer' cannot have two virtual end points.")]
    public void TwoVirtualRelationEndPointDefinitions ()
    {
      ClassDefinition customerDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Customer", "Customer", "TestDomain", typeof (Customer), false);

      VirtualRelationEndPointDefinition endPointDefinition1 = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(customerDefinition, "Orders", true, CardinalityType.One, typeof (Order));

      ClassDefinition orderDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false);

      VirtualRelationEndPointDefinition endPointDefinition2 = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(orderDefinition, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", true, CardinalityType.One, typeof (Customer));

      new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", endPointDefinition1, endPointDefinition2);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Relation 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson' cannot have two non-virtual end points.")]
    public void TwoRelationEndPointDefinitions ()
    {
      ReflectionBasedClassDefinition partnerDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Partner", "Partner", "TestDomain", typeof (Partner), false);
      partnerDefinition.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(partnerDefinition, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson", "ContactPersonID", typeof (ObjectID)));

      RelationEndPointDefinition endPointDefinition1 = new RelationEndPointDefinition (
          partnerDefinition, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson", false);

      ReflectionBasedClassDefinition personDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", "Person", "TestDomain", typeof (Person), false);
      personDefinition.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(personDefinition, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Person.AssociatedPartnerCompany", "AssociatedPartnerCompanyID", typeof (ObjectID)));

      RelationEndPointDefinition endPointDefinition2 = new RelationEndPointDefinition (
          personDefinition, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Person.AssociatedPartnerCompany", false);

      new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson", endPointDefinition1, endPointDefinition2);
    }
  }
}
