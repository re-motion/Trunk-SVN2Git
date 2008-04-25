using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins.Context;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class ConstructionOfVirtualRelationEndPointDefinitionTest : StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public ConstructionOfVirtualRelationEndPointDefinitionTest ()
    {
    }

    // methods and properties

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Relation definition error: Virtual property 'Dummy' of class 'Company' is of type"
            + "'Remotion.Data.DomainObjects.DomainObject',"
            + " but must be derived from 'Remotion.Data.DomainObjects.DomainObject' or "
            + " 'Remotion.Data.DomainObjects.DomainObjectCollection' or must be"
            + " 'Remotion.Data.DomainObjects.DomainObjectCollection'.")]
    public void VirtualEndPointOfDomainObjectType ()
    {
      ReflectionBasedClassDefinition companyDefinition = new ReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false, new List<Type>());

      VirtualRelationEndPointDefinition endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(companyDefinition, "Dummy", false, CardinalityType.One, typeof (DomainObject));
    }

    [Test]
    public void VirtualEndPointOfDomainObjectCollectionType ()
    {
      ReflectionBasedClassDefinition companyDefinition = new ReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false, new List<Type>());

      VirtualRelationEndPointDefinition endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(companyDefinition, "Dummy", false, CardinalityType.Many, typeof (DomainObjectCollection));
    }

    [Test]
    public void VirtualEndPointOfOrderCollectionType ()
    {
      ReflectionBasedClassDefinition companyDefinition = new ReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false, new List<Type>());

      VirtualRelationEndPointDefinition endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(companyDefinition, "Dummy", false, CardinalityType.Many, typeof (OrderCollection));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "The property type of a virtual end point of a one-to-one relation"
        + " must be derived from 'Remotion.Data.DomainObjects.DomainObject'.")]
    public void VirtualEndPointWithCardinalityOneAndWrongPropertyType ()
    {
      ReflectionBasedClassDefinition companyDefinition = new ReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false, new List<Type>());

      VirtualRelationEndPointDefinition endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(companyDefinition, "Dummy", false, CardinalityType.One, typeof (OrderCollection));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "The property type of a virtual end point of a one-to-many relation"
        + " must be or be derived from 'Remotion.Data.DomainObjects.DomainObjectCollection'.")]
    public void VirtualEndPointWithCardinalityManyAndWrongPropertyType ()
    {
      ReflectionBasedClassDefinition companyDefinition = new ReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false, new List<Type>());

      VirtualRelationEndPointDefinition endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(companyDefinition, "Dummy", false, CardinalityType.Many, typeof (Company));
    }

    [Test]
    public void InitializeWithSortExpression ()
    {
      ReflectionBasedClassDefinition customerDefinition = new ReflectionBasedClassDefinition ("Customer", "Customer", "TestDomain", typeof (Customer), false, new List<Type>());

      VirtualRelationEndPointDefinition endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(customerDefinition, "Orders", false, CardinalityType.Many, typeof (OrderCollection), "OrderNumber desc");

      Assert.AreEqual ("OrderNumber desc", endPointDefinition.SortExpression);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Property 'Orders' of class 'Customer' must not specify a SortExpression, because cardinality is equal to 'one'.")]
    public void InitializeWithSortExpressionAndCardinalityOfOne ()
    {
      ReflectionBasedClassDefinition customerDefinition = new ReflectionBasedClassDefinition ("Customer", "Customer", "TestDomain", typeof (Customer), false, new List<Type>());

      VirtualRelationEndPointDefinition endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(customerDefinition, "Orders", false, CardinalityType.One, typeof (Order), "OrderNumber desc");
    }
  }
}
