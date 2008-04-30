using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping
{
  [TestFixture]
  public class MappingConfigurationTest: StandardMappingTest
  {
    private MockRepository _mockRepository;
    private IMappingLoader _mockMappingLoader;
    private ClassDefinitionCollection _classDefinitionCollection = new ClassDefinitionCollection();
    private RelationDefinitionCollection _relationDefinitionCollection = new RelationDefinitionCollection();

    public override void SetUp()
    {
      base.SetUp();

      _mockRepository = new MockRepository();
      _mockMappingLoader = _mockRepository.CreateMock<IMappingLoader>();
    }

    [Test]
    public void Initialize()
    {
      Expect.Call (_mockMappingLoader.GetClassDefinitions()).Return (_classDefinitionCollection);
      Expect.Call (_mockMappingLoader.GetRelationDefinitions (_classDefinitionCollection)).Return (_relationDefinitionCollection);
      Expect.Call (_mockMappingLoader.ResolveTypes).Return (true);

      _mockRepository.ReplayAll();

      MappingConfiguration configuration = new MappingConfiguration (_mockMappingLoader);
      ClassDefinitionCollection actualClassDefinitionCollection = configuration.ClassDefinitions;
      RelationDefinitionCollection actualRelationDefinitionCollection = configuration.RelationDefinitions;

      _mockRepository.VerifyAll();

      Assert.That (actualClassDefinitionCollection, Is.SameAs (_classDefinitionCollection));
      Assert.That (actualRelationDefinitionCollection, Is.SameAs (_relationDefinitionCollection));
      Assert.That (configuration.ResolveTypes, Is.True);
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
              TestMappingConfiguration.Current.ClassDefinitions[typeof (Order)]["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"]));
      Assert.IsTrue (
          MappingConfiguration.Current.Contains (
              MappingConfiguration.Current.ClassDefinitions[typeof (Order)]["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"]));
    }

    [Test]
    public void ContainsRelationDefinition()
    {
      Assert.IsFalse (
          MappingConfiguration.Current.Contains (
              TestMappingConfiguration.Current.RelationDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"]));
      Assert.IsTrue (
          MappingConfiguration.Current.Contains (
              MappingConfiguration.Current.RelationDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"]));
    }

    [Test]
    public void ContainsRelationEndPointDefinition()
    {
      Assert.IsFalse (
          MappingConfiguration.Current.Contains (
              TestMappingConfiguration.Current.RelationDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"].
                  EndPointDefinitions[0]));
      Assert.IsFalse (
          MappingConfiguration.Current.Contains (
              TestMappingConfiguration.Current.RelationDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"].
                  EndPointDefinitions[1]));

      Assert.IsTrue (
          MappingConfiguration.Current.Contains (
              MappingConfiguration.Current.RelationDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"].EndPointDefinitions[
                  0]));
      Assert.IsTrue (
          MappingConfiguration.Current.Contains (
              MappingConfiguration.Current.RelationDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"].EndPointDefinitions[
                  1]));
    }

    [Test]
    public void ContainsRelationEndPointDefinitionNotInMapping()
    {
      ReflectionBasedClassDefinition orderDefinition = new ReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false, new List<Type> ());
      ReflectionBasedClassDefinition orderTicketDefinition =
          new ReflectionBasedClassDefinition ("OrderTicket", "OrderTicket", "TestDomain", typeof (OrderTicket), false, new List<Type> ());
      orderTicketDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(orderTicketDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", "OrderID", typeof (ObjectID), false));

      VirtualRelationEndPointDefinition orderEndPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(orderDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

      RelationEndPointDefinition orderTicketEndPointdefinition =
          new RelationEndPointDefinition (orderTicketDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", true);

      new RelationDefinition ("RelationIDNotInMapping", orderEndPointDefinition, orderTicketEndPointdefinition);

      Assert.IsFalse (MappingConfiguration.Current.Contains (orderEndPointDefinition));
    }
  }
}