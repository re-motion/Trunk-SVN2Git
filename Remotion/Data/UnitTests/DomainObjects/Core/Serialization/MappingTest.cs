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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class MappingTest : SerializationBaseTest
  {
    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = 
        "The PropertyDefinition 'PropertyName' cannot be serialized because is is not part of the current mapping.")]
    public void PropertyDefinitionWithoutClassDefinition_NotInMapping ()
    {
      ClassDefinition classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "ClassID", "EntityName", DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition, typeof (Order), false);
      PropertyDefinition propertyDefinition = PropertyDefinitionFactory.CreateForFakePropertyInfo (
          classDefinition, "PropertyName", "ColumnName", typeof (string), true, 100, StorageClass.Persistent);

      SerializeAndDeserialize (propertyDefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = 
        "The PropertyDefinition 'OrderNumber' cannot be serialized because is is not part of the current mapping.")]
    public void PropertyDefinitionWithClassDefinition_NotInMapping ()
    {
      ClassDefinition classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "ClassID", "EntityName", DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition, typeof (Order), false);
      PropertyDefinition propertyDefinition = PropertyDefinitionFactory.CreateForFakePropertyInfo (
          classDefinition, "OrderNumber", "OrderNo", typeof (int), StorageClass.Persistent);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));

      SerializeAndDeserialize (propertyDefinition);
    }

    [Test]
    public void PropertyDefinition_InMapping ()
    {
      ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions["Order"];
      PropertyDefinition orderNumberDefinition = orderDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"];

      var deserializedOrderNumberDefinition = (PropertyDefinition) SerializeAndDeserialize (orderNumberDefinition);
      Assert.That (deserializedOrderNumberDefinition, Is.SameAs (orderNumberDefinition));
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = 
        "The PropertyDefinition 'PropertyName' cannot be serialized because is is not part of the current mapping.")]
    public void PropertyDefinitionWithUnresolvedNativePropertyType_NotInMapping ()
    {
      ClassDefinition classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Order", "Order", DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition, typeof (Order), false);
      PropertyDefinition propertyDefinition = PropertyDefinitionFactory.CreateForFakePropertyInfo (
          classDefinition, "PropertyName", "ColumnName", typeof (int), StorageClass.Persistent);

      SerializeAndDeserialize (propertyDefinition);
    }

    [Test]
    public void SimplePropertyDefinitionCollection ()
    {
      var definitions = new PropertyDefinitionCollection();
      definitions.Add (MappingConfiguration.Current.ClassDefinitions["Order"].MyPropertyDefinitions[0]);
      definitions.Add (MappingConfiguration.Current.ClassDefinitions["Order"].MyPropertyDefinitions[1]);

      var deserializedDefinitions = (PropertyDefinitionCollection) SerializeAndDeserialize (definitions);

      Assert.That (ReferenceEquals (definitions, deserializedDefinitions), Is.False);
      Assert.That (deserializedDefinitions.Count, Is.EqualTo (definitions.Count));
      Assert.That (deserializedDefinitions[0], Is.SameAs (definitions[0]));
      Assert.That (deserializedDefinitions[1], Is.SameAs (definitions[1]));
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = 
        "The RelationEndPointDefinition 'Order' cannot be serialized because is is not part of the current mapping.")]
    public void RelationEndPointDefinitionWithoutRelationDefinition_NotInMapping ()
    {
      ClassDefinition classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "OrderTicket", "OrderTicket", DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition, typeof (OrderTicket), false);
      var propertyDefinition = PropertyDefinitionFactory.CreateForFakePropertyInfo (
              classDefinition, "Order", "OrderID", typeof (ObjectID), false, StorageClass.Persistent);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
      var endPointdefinition = new RelationEndPointDefinition (propertyDefinition, true);

      SerializeAndDeserialize (endPointdefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The RelationEndPointDefinition 'Order' cannot be serialized because is is not part of the current mapping.")]
    public void RelationEndPointDefinitionWithRelationDefinition_NotInMapping ()
    {
      ClassDefinition orderDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Order", "Order", DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition, typeof (Order), false);
      ClassDefinition orderTicketDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "OrderTicket", "OrderTicket", DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition, typeof (OrderTicket), false);

      var orderTicketPropertyDefinition = PropertyDefinitionFactory.CreateForFakePropertyInfo (
              orderTicketDefinition, "Order", "OrderID", typeof (ObjectID), false, StorageClass.Persistent);
      orderTicketDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { orderTicketPropertyDefinition }, true));

      VirtualRelationEndPointDefinition orderEndPointDefinition =
          VirtualRelationEndPointDefinitionFactory.CreateVirtualRelationEndPointDefinition (
              orderDefinition, "OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

      var orderTicketEndPointdefinition = new RelationEndPointDefinition (orderTicketPropertyDefinition, true);

      new RelationDefinition ("OrderToOrderTicket", orderEndPointDefinition, orderTicketEndPointdefinition);

      SerializeAndDeserialize (orderTicketEndPointdefinition);
    }

    [Test]
    public void RelationEndPointDefinition_InMapping ()
    {
      RelationDefinition relationDefinition = MappingConfiguration.Current.RelationDefinitions[
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket:"
          + "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order->Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"
        ];
      var endPointDefinition = (RelationEndPointDefinition) relationDefinition.GetEndPointDefinition (
          "OrderTicket",
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");

      var deserializedEndPointDefinition = (RelationEndPointDefinition) SerializeAndDeserialize (endPointDefinition);
      Assert.That (deserializedEndPointDefinition, Is.SameAs (endPointDefinition));
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage =
        "The VirtualRelationEndPointDefinition 'OrderTicket' cannot be serialized because is is not part of the current mapping.")]
    public void VirtualRelationEndPointDefinitionWithoutRelationDefinition_NotInMapping ()
    {
      ClassDefinition classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Order", "Order", DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition, typeof (Order), false);

      VirtualRelationEndPointDefinition endPointdefinition =
          VirtualRelationEndPointDefinitionFactory.CreateVirtualRelationEndPointDefinition (
              classDefinition, "OrderTicket", true, CardinalityType.One, typeof (Order));

      SerializeAndDeserialize (endPointdefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage =
        "The VirtualRelationEndPointDefinition 'OrderTicket' cannot be serialized because is is not part of the current mapping.")]
    public void VirtualRelationEndPointDefinitionWithRelationDefinition_NotInMapping ()
    {
      ClassDefinition orderDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Order", "Order", DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition, typeof (Order), false);
      ClassDefinition orderTicketDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "OrderTicket", "OrderTicket", DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition, typeof (OrderTicket), false);

      var orderTicketPropertyDefinition = PropertyDefinitionFactory.CreateForFakePropertyInfo (
              orderTicketDefinition, "Order", "OrderID", typeof (ObjectID), false, StorageClass.Persistent);
      orderTicketDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { orderTicketPropertyDefinition }, true));

      VirtualRelationEndPointDefinition orderEndPointDefinition =
          VirtualRelationEndPointDefinitionFactory.CreateVirtualRelationEndPointDefinition (
              orderDefinition, "OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

      var orderTicketEndPointdefinition = new RelationEndPointDefinition (orderTicketPropertyDefinition, true);

      new RelationDefinition ("OrderToOrderTicket", orderEndPointDefinition, orderTicketEndPointdefinition);

      SerializeAndDeserialize (orderEndPointDefinition);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionInMapping ()
    {
      RelationDefinition relationDefinition = MappingConfiguration.Current.RelationDefinitions[
        "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket:"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order->Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"
      ];
      var endPointDefinition = (VirtualRelationEndPointDefinition) relationDefinition.GetEndPointDefinition (
          "Order",
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");

      var deserializedEndPointDefinition =
          (VirtualRelationEndPointDefinition) SerializeAndDeserialize (endPointDefinition);
      Assert.That (deserializedEndPointDefinition, Is.SameAs (endPointDefinition));
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The AnonymousRelationEndPointDefinition '<anonymous>' cannot be serialized because is is not part of the current mapping.")
    ]
    public void AnonymousRelationEndPointDefinitionWithoutRelationDefinition_NotInMapping ()
    {
      ClassDefinition classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Client", "Client", DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition, typeof (Client), false);

      var endPointdefinition = new AnonymousRelationEndPointDefinition (classDefinition);

      SerializeAndDeserialize (endPointdefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The AnonymousRelationEndPointDefinition '<anonymous>' cannot be serialized because is is not part of the current mapping.")
    ]
    public void AnonymousRelationEndPointDefinitionWithRelationDefinition_NotInMapping ()
    {
      ClassDefinition clientDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Client", "Client", DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition, typeof (Client), false);
      ClassDefinition locationDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Location", "Location", DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition, typeof (Location), false);

      var locationPropertyDefinition = PropertyDefinitionFactory.CreateForFakePropertyInfo (
              locationDefinition, "Client", "ClientID", typeof (ObjectID), false, StorageClass.Persistent);
      locationDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { locationPropertyDefinition }, true));

      var clientEndPointDefinition = new AnonymousRelationEndPointDefinition (clientDefinition);
      var locationEndPointDefinition = new RelationEndPointDefinition (locationPropertyDefinition, true);

      new RelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client", clientEndPointDefinition, locationEndPointDefinition);

      SerializeAndDeserialize (clientEndPointDefinition);
    }

    [Test]
    public void AnonymousRelationEndPointDefinition_InMapping ()
    {
      RelationDefinition relationDefinition = MappingConfiguration.Current.RelationDefinitions[
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location:"
          + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client"];
      var endPointDefinition =
          (AnonymousRelationEndPointDefinition) relationDefinition.GetOppositeEndPointDefinition (
              "Location",
              "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");

      var deserializedEndPointDefinition =
          (AnonymousRelationEndPointDefinition) SerializeAndDeserialize (endPointDefinition);
      Assert.That (deserializedEndPointDefinition, Is.SameAs (endPointDefinition));
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = 
        "The RelationDefinition 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket:"
        + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket.Order->"
        + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderTicket' cannot be serialized because is is not part of "
        + "the current mapping.")]
    public void RelationDefinition_NotInMapping ()
    {
      RelationDefinition relationDefinition = FakeMappingConfiguration.Current.RelationDefinitions[
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
          + "TestDomain.Integration.OrderTicket.Order->Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
          + "TestDomain.Integration.Order.OrderTicket"];
      SerializeAndDeserialize (relationDefinition);
    }

    [Test]
    public void RelationDefinition_InMapping ()
    {
      var relationDefinition =
          MappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket:"
              + "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order->Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"];
      var deserializedRelationDefinition = (RelationDefinition) SerializeAndDeserialize (relationDefinition);

      Assert.That (deserializedRelationDefinition, Is.SameAs (relationDefinition));
    }

    [Test]
    public void SimpleClassDefinitionCollection ()
    {
      var definitions = new ClassDefinitionCollection();
      definitions.Add (MappingConfiguration.Current.ClassDefinitions[0]);
      definitions.Add (MappingConfiguration.Current.ClassDefinitions[1]);

      var deserializedDefinitions = (ClassDefinitionCollection) SerializeAndDeserialize (definitions);

      Assert.That (ReferenceEquals (definitions, deserializedDefinitions), Is.False);
      Assert.That (deserializedDefinitions[0], Is.SameAs (definitions[0]));
      Assert.That (deserializedDefinitions[1], Is.SameAs (definitions[1]));
      Assert.That (deserializedDefinitions.Contains (definitions[0].ID), Is.True);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The ClassDefinition 'Partner' cannot be serialized because is is not part of the current mapping.")]
    public void ClassDefinition_NotInMapping ()
    {
      ClassDefinition classDefinition = FakeMappingConfiguration.Current.ClassDefinitions.GetMandatory ("Partner");

      SerializeAndDeserialize (classDefinition);
    }

    [Test]
    public void ClassDefinition_InMapping ()
    {
      // Note: Partner has a base class and several derived classes.
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Partner");

      var deserializedClassDefinition = (ClassDefinition) SerializeAndDeserialize (classDefinition);

      Assert.That (deserializedClassDefinition, Is.SameAs (classDefinition));
    }

    [Test]
    public void PropertyDefinitionCollectionInMapping ()
    {
      PropertyDefinitionCollection definitions = MappingConfiguration.Current.ClassDefinitions["Order"].MyPropertyDefinitions;
      var deserializedDefinitions = (PropertyDefinitionCollection) SerializeAndDeserialize (definitions);

      Assert.That (definitions, Is.Not.SameAs (deserializedDefinitions));
    }

    [Test]
    public void PropertyDefinitionWithEnumType ()
    {
      PropertyDefinition enumPropertyDefinition =
          MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Customer").MyPropertyDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Type"];

      var deserializedEnumPropertyDefinition = (PropertyDefinition) SerializeAndDeserialize (enumPropertyDefinition);

      Assert.That (deserializedEnumPropertyDefinition, Is.SameAs (enumPropertyDefinition));
    }
  }
}