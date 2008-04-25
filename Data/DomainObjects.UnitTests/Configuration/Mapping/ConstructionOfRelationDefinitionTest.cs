using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins.Context;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class ConstructionOfRelationDefinitionTest : StandardMappingTest
  {
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Relation 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' cannot have two virtual end points.")]
    public void TwoVirtualRelationEndPointDefinitions ()
    {
      ClassDefinition customerDefinition = new ReflectionBasedClassDefinition ("Customer", "Customer", "TestDomain", typeof (Customer), false, new List<Type>());

      VirtualRelationEndPointDefinition endPointDefinition1 = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(customerDefinition, "Orders", true, CardinalityType.One, typeof (Order));

      ClassDefinition orderDefinition = new ReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false, new List<Type>());

      VirtualRelationEndPointDefinition endPointDefinition2 = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(orderDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", true, CardinalityType.One, typeof (Customer));

      new RelationDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", endPointDefinition1, endPointDefinition2);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Relation 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson' cannot have two non-virtual end points.")]
    public void TwoRelationEndPointDefinitions ()
    {
      ReflectionBasedClassDefinition partnerDefinition = new ReflectionBasedClassDefinition ("Partner", "Partner", "TestDomain", typeof (Partner), false, new List<Type>());
      partnerDefinition.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(partnerDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson", "ContactPersonID", typeof (ObjectID)));

      RelationEndPointDefinition endPointDefinition1 = new RelationEndPointDefinition (
          partnerDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson", false);

      ReflectionBasedClassDefinition personDefinition = new ReflectionBasedClassDefinition ("Person", "Person", "TestDomain", typeof (Person), false, new List<Type>());
      personDefinition.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(personDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Person.AssociatedPartnerCompany", "AssociatedPartnerCompanyID", typeof (ObjectID)));

      RelationEndPointDefinition endPointDefinition2 = new RelationEndPointDefinition (
          personDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Person.AssociatedPartnerCompany", false);

      new RelationDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson", endPointDefinition1, endPointDefinition2);
    }
  }
}
