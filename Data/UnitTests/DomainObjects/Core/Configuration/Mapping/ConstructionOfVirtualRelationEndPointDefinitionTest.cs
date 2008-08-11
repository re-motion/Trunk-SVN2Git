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
      ReflectionBasedClassDefinition companyDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false);

      VirtualRelationEndPointDefinition endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(companyDefinition, "Dummy", false, CardinalityType.One, typeof (DomainObject));
    }

    [Test]
    public void VirtualEndPointOfDomainObjectCollectionType ()
    {
      ReflectionBasedClassDefinition companyDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false);

      VirtualRelationEndPointDefinition endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(companyDefinition, "Dummy", false, CardinalityType.Many, typeof (DomainObjectCollection));
    }

    [Test]
    public void VirtualEndPointOfOrderCollectionType ()
    {
      ReflectionBasedClassDefinition companyDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false);

      VirtualRelationEndPointDefinition endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(companyDefinition, "Dummy", false, CardinalityType.Many, typeof (OrderCollection));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "The property type of a virtual end point of a one-to-one relation"
        + " must be derived from 'Remotion.Data.DomainObjects.DomainObject'.")]
    public void VirtualEndPointWithCardinalityOneAndWrongPropertyType ()
    {
      ReflectionBasedClassDefinition companyDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false);

      VirtualRelationEndPointDefinition endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(companyDefinition, "Dummy", false, CardinalityType.One, typeof (OrderCollection));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "The property type of a virtual end point of a one-to-many relation"
        + " must be or be derived from 'Remotion.Data.DomainObjects.DomainObjectCollection'.")]
    public void VirtualEndPointWithCardinalityManyAndWrongPropertyType ()
    {
      ReflectionBasedClassDefinition companyDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false);

      VirtualRelationEndPointDefinition endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(companyDefinition, "Dummy", false, CardinalityType.Many, typeof (Company));
    }

    [Test]
    public void InitializeWithSortExpression ()
    {
      ReflectionBasedClassDefinition customerDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Customer", "Customer", "TestDomain", typeof (Customer), false);

      VirtualRelationEndPointDefinition endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(customerDefinition, "Orders", false, CardinalityType.Many, typeof (OrderCollection), "OrderNumber desc");

      Assert.AreEqual ("OrderNumber desc", endPointDefinition.SortExpression);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Property 'Orders' of class 'Customer' must not specify a SortExpression, because cardinality is equal to 'one'.")]
    public void InitializeWithSortExpressionAndCardinalityOfOne ()
    {
      ReflectionBasedClassDefinition customerDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Customer", "Customer", "TestDomain", typeof (Customer), false);

      VirtualRelationEndPointDefinition endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(customerDefinition, "Orders", false, CardinalityType.One, typeof (Order), "OrderNumber desc");
    }
  }
}
