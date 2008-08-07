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
      ClassDefinition customerDefinition = new ReflectionBasedClassDefinition ("Customer", "Customer", "TestDomain", typeof (Customer), false, new PersistentMixinFinderMock());

      VirtualRelationEndPointDefinition endPointDefinition1 = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(customerDefinition, "Orders", true, CardinalityType.One, typeof (Order));

      ClassDefinition orderDefinition = new ReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false, new PersistentMixinFinderMock());

      VirtualRelationEndPointDefinition endPointDefinition2 = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(orderDefinition, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", true, CardinalityType.One, typeof (Customer));

      new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", endPointDefinition1, endPointDefinition2);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Relation 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson' cannot have two non-virtual end points.")]
    public void TwoRelationEndPointDefinitions ()
    {
      ReflectionBasedClassDefinition partnerDefinition = new ReflectionBasedClassDefinition ("Partner", "Partner", "TestDomain", typeof (Partner), false, new PersistentMixinFinderMock());
      partnerDefinition.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(partnerDefinition, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson", "ContactPersonID", typeof (ObjectID)));

      RelationEndPointDefinition endPointDefinition1 = new RelationEndPointDefinition (
          partnerDefinition, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson", false);

      ReflectionBasedClassDefinition personDefinition = new ReflectionBasedClassDefinition ("Person", "Person", "TestDomain", typeof (Person), false, new PersistentMixinFinderMock());
      personDefinition.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(personDefinition, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Person.AssociatedPartnerCompany", "AssociatedPartnerCompanyID", typeof (ObjectID)));

      RelationEndPointDefinition endPointDefinition2 = new RelationEndPointDefinition (
          personDefinition, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Person.AssociatedPartnerCompany", false);

      new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson", endPointDefinition1, endPointDefinition2);
    }
  }
}
