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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class MappingConfigurationTest: StandardMappingTest
  {
    private MockRepository _mockRepository;
    private IMappingLoader _mockMappingLoader;
    private ClassDefinitionCollection _classDefinitionCollection = new ClassDefinitionCollection();
    private RelationDefinitionCollection _relationDefinitionCollection = new RelationDefinitionCollection();
    private ReflectionBasedNameResolver _nameResolver;

    public override void SetUp()
    {
      base.SetUp();

      _nameResolver = new ReflectionBasedNameResolver ();
      _mockRepository = new MockRepository ();
      _mockMappingLoader = _mockRepository.CreateMock<IMappingLoader>();
    }

    [Test]
    public void Initialize()
    {
      Expect.Call (_mockMappingLoader.GetClassDefinitions ()).Return (_classDefinitionCollection);
      Expect.Call (_mockMappingLoader.GetRelationDefinitions (_classDefinitionCollection)).Return (_relationDefinitionCollection);
      Expect.Call (_mockMappingLoader.ResolveTypes).Return (true);
      Expect.Call (_mockMappingLoader.NameResolver).Return (_nameResolver);

      _mockRepository.ReplayAll();

      MappingConfiguration configuration = new MappingConfiguration (_mockMappingLoader);
      ClassDefinitionCollection actualClassDefinitionCollection = configuration.ClassDefinitions;
      RelationDefinitionCollection actualRelationDefinitionCollection = configuration.RelationDefinitions;

      _mockRepository.VerifyAll();

      Assert.That (actualClassDefinitionCollection, Is.SameAs (_classDefinitionCollection));
      Assert.That (actualRelationDefinitionCollection, Is.SameAs (_relationDefinitionCollection));
      Assert.That (configuration.ResolveTypes, Is.True);
      Assert.That (configuration.NameResolver, Is.SameAs (_nameResolver));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "IMappingLoader.GetClassDefinitions() evaluated and returned null.")]
    public void Initialize_WithGetClassDefinitionsEvaluatesNull()
    {
      SetupResult.For (_mockMappingLoader.GetClassDefinitions()).Return (null);

      _mockRepository.ReplayAll();

      new MappingConfiguration (_mockMappingLoader);
    
      _mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "IMappingLoader.GetRelationDefinitions (ClassDefinitionCollection) evaluated and returned null.")]
    public void Initialize_WithGetRelationDefinitionsEvaluatesNull ()
    {
      SetupResult.For (_mockMappingLoader.GetClassDefinitions ()).Return (_classDefinitionCollection);
      SetupResult.For (_mockMappingLoader.GetRelationDefinitions (_classDefinitionCollection)).Return (null);

      _mockRepository.ReplayAll ();

      new MappingConfiguration (_mockMappingLoader);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void SetCurrent()
    {
      try
      {
        SetupResult.For (_mockMappingLoader.GetClassDefinitions()).Return (_classDefinitionCollection);
        SetupResult.For (_mockMappingLoader.GetRelationDefinitions (_classDefinitionCollection)).Return (_relationDefinitionCollection);
        SetupResult.For (_mockMappingLoader.ResolveTypes).Return (true);
        SetupResult.For (_mockMappingLoader.NameResolver).Return (_nameResolver);

        _mockRepository.ReplayAll();

        MappingConfiguration configuration = new MappingConfiguration (_mockMappingLoader);
        MappingConfiguration.SetCurrent (configuration);

        Assert.AreSame (configuration, MappingConfiguration.Current);
      }
      finally
      {
        MappingConfiguration.SetCurrent (null);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "Argument 'mappingConfiguration' must have property 'ResolveTypes' set.\r\nParameter name: mappingConfiguration")]
    public void SetCurrentRejectsUnresolvedTypes()
    {
      SetupResult.For (_mockMappingLoader.GetClassDefinitions()).Return (_classDefinitionCollection);
      SetupResult.For (_mockMappingLoader.GetRelationDefinitions (_classDefinitionCollection)).Return (_relationDefinitionCollection);
      SetupResult.For (_mockMappingLoader.ResolveTypes).Return (false);
      SetupResult.For (_mockMappingLoader.NameResolver).Return (_nameResolver);

      _mockRepository.ReplayAll();

      MappingConfiguration configuration = new MappingConfiguration (_mockMappingLoader);

      _mockRepository.VerifyAll();

      MappingConfiguration.SetCurrent (configuration);
    }

    [Test]
    public void ContainsClassDefinition()
    {
      Assert.IsFalse (MappingConfiguration.Current.Contains (TestMappingConfiguration.Current.ClassDefinitions[typeof (Order)]));
      Assert.IsTrue (MappingConfiguration.Current.Contains (MappingConfiguration.Current.ClassDefinitions[typeof (Order)]));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsNull()
    {
      MappingConfiguration.Current.Contains ((ClassDefinition) null);
    }

    [Test]
    public void ContainsPropertyDefinition()
    {
      Assert.IsFalse (
          MappingConfiguration.Current.Contains (
              TestMappingConfiguration.Current.ClassDefinitions[typeof (Order)]["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]));
      Assert.IsTrue (
          MappingConfiguration.Current.Contains (
              MappingConfiguration.Current.ClassDefinitions[typeof (Order)]["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]));
    }

    [Test]
    public void ContainsRelationDefinition()
    {
      Assert.IsFalse (
          MappingConfiguration.Current.Contains (
              TestMappingConfiguration.Current.RelationDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order"]));
      Assert.IsTrue (
          MappingConfiguration.Current.Contains (
              MappingConfiguration.Current.RelationDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order"]));
    }

    [Test]
    public void ContainsRelationEndPointDefinition()
    {
      Assert.IsFalse (
          MappingConfiguration.Current.Contains (
              TestMappingConfiguration.Current.RelationDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order"].
                  EndPointDefinitions[0]));
      Assert.IsFalse (
          MappingConfiguration.Current.Contains (
              TestMappingConfiguration.Current.RelationDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order"].
                  EndPointDefinitions[1]));

      Assert.IsTrue (
          MappingConfiguration.Current.Contains (
              MappingConfiguration.Current.RelationDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order"].EndPointDefinitions[
                  0]));
      Assert.IsTrue (
          MappingConfiguration.Current.Contains (
              MappingConfiguration.Current.RelationDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order"].EndPointDefinitions[
                  1]));
    }

    [Test]
    public void ContainsRelationEndPointDefinitionNotInMapping()
    {
      ReflectionBasedClassDefinition orderDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false);
      ReflectionBasedClassDefinition orderTicketDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("OrderTicket", "OrderTicket", "TestDomain", typeof (OrderTicket), false);
      orderTicketDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(orderTicketDefinition, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", "OrderID", typeof (ObjectID), false));

      VirtualRelationEndPointDefinition orderEndPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(orderDefinition, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

      RelationEndPointDefinition orderTicketEndPointdefinition =
          new RelationEndPointDefinition (orderTicketDefinition, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", true);

      new RelationDefinition ("RelationIDNotInMapping", orderEndPointDefinition, orderTicketEndPointdefinition);

      Assert.IsFalse (MappingConfiguration.Current.Contains (orderEndPointDefinition));
    }
  }
}
