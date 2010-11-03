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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class MappingTest : SerializationBaseTest
  {
    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The ReflectionBasedPropertyDefinition 'PropertyName' cannot be serialized because is is not part of the current mapping.")]
    public void PropertyDefinitionWithoutClassDefinition_NotInMapping ()
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("ClassID", "EntityName", "TestDomain", typeof (Order), false);
      PropertyDefinition propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo(classDefinition, "PropertyName", "ColumnName", typeof (string), true, 100, StorageClass.Persistent);

      SerializeAndDeserialize (propertyDefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The ReflectionBasedPropertyDefinition 'OrderNumber' cannot be serialized because is is not part of the current mapping.")]
    public void PropertyDefinitionWithClassDefinition_NotInMapping ()
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("ClassID", "EntityName", "TestDomain", typeof (Order), false);
      PropertyDefinition propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo(classDefinition, "OrderNumber", "OrderNo", typeof (int), StorageClass.Persistent);
      classDefinition.MyPropertyDefinitions.Add (propertyDefinition);

      PropertyDefinition deserializedPropertyDefinition = (PropertyDefinition) SerializeAndDeserialize (propertyDefinition);

      Assert.IsFalse (ReferenceEquals (propertyDefinition, deserializedPropertyDefinition));
      Assert.IsFalse (ReferenceEquals (propertyDefinition.ClassDefinition, deserializedPropertyDefinition.ClassDefinition));
      AreEqual (propertyDefinition, deserializedPropertyDefinition);

    }

    [Test]
    public void PropertyDefinition_InMapping ()
    {
      ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions["Order"];
      PropertyDefinition orderNumberDefinition = orderDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"];

      PropertyDefinition deserializedOrderNumberDefinition = (PropertyDefinition) SerializeAndDeserialize (orderNumberDefinition);
      Assert.AreSame (orderNumberDefinition, deserializedOrderNumberDefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The ReflectionBasedPropertyDefinition 'PropertyName' cannot be serialized because is is not part of the current mapping.")]
    public void PropertyDefinitionWithUnresolvedNativePropertyType_NotInMapping ()
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false);
      PropertyDefinition propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo(classDefinition, "PropertyName", "ColumnName", typeof (int), StorageClass.Persistent);

      PropertyDefinition deserializedPropertyDefinition = (PropertyDefinition) SerializeAndDeserialize (propertyDefinition);
      AreEqual (propertyDefinition, deserializedPropertyDefinition);
    }

    [Test]
    public void SimplePropertyDefinitionCollection ()
    {
      PropertyDefinitionCollection definitions = new PropertyDefinitionCollection ();
      definitions.Add (MappingConfiguration.Current.ClassDefinitions["Order"].MyPropertyDefinitions[0]);
      definitions.Add (MappingConfiguration.Current.ClassDefinitions["Order"].MyPropertyDefinitions[1]);

      PropertyDefinitionCollection deserializedDefinitions = (PropertyDefinitionCollection) SerializeAndDeserialize (definitions);

      Assert.IsFalse (ReferenceEquals (definitions, deserializedDefinitions));
      Assert.AreEqual (definitions.Count, deserializedDefinitions.Count);
      Assert.AreSame (definitions[0], deserializedDefinitions[0]);
      Assert.AreSame (definitions[1], deserializedDefinitions[1]);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The RelationEndPointDefinition 'Order' cannot be serialized because is is not part of the current mapping.")]
    public void RelationEndPointDefinitionWithoutRelationDefinition_NotInMapping ()
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("OrderTicket", "OrderTicket", "TestDomain", typeof (OrderTicket), false);
      classDefinition.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (classDefinition, "Order", "OrderID", typeof (ObjectID), false, StorageClass.Persistent));
      RelationEndPointDefinition endPointdefinition = new RelationEndPointDefinition (classDefinition, "Order", true);

      SerializeAndDeserialize (endPointdefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The RelationEndPointDefinition 'Order' cannot be serialized because is is not part of the current mapping.")]
    public void RelationEndPointDefinitionWithRelationDefinition_NotInMapping ()
    {
      ReflectionBasedClassDefinition orderDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false);
      ReflectionBasedClassDefinition orderTicketDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("OrderTicket", "OrderTicket", "TestDomain", typeof (OrderTicket), false);

      orderTicketDefinition.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo(orderTicketDefinition, "Order", "OrderID", typeof (ObjectID), false,StorageClass.Persistent));

      VirtualRelationEndPointDefinition orderEndPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(orderDefinition, "OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

      RelationEndPointDefinition orderTicketEndPointdefinition = new RelationEndPointDefinition (orderTicketDefinition, "Order", true);

      new RelationDefinition ("OrderToOrderTicket", orderEndPointDefinition, orderTicketEndPointdefinition);

      RelationEndPointDefinition deserializedOrderTicketEndPointdefinition = (RelationEndPointDefinition) SerializeAndDeserialize (orderTicketEndPointdefinition);

      Assert.IsFalse (ReferenceEquals (orderTicketEndPointdefinition, deserializedOrderTicketEndPointdefinition));
      AreEqual (orderTicketEndPointdefinition, deserializedOrderTicketEndPointdefinition);
    }

    [Test]
    public void RelationEndPointDefinition_InMapping ()
    {
      RelationDefinition relationDefinition = 
        MappingConfiguration.Current.RelationDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket->"
        +"Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order"];
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) relationDefinition.GetEndPointDefinition ("OrderTicket", 
        "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");

      RelationEndPointDefinition deserializedEndPointDefinition = (RelationEndPointDefinition) SerializeAndDeserialize (endPointDefinition);
      Assert.AreSame (endPointDefinition, deserializedEndPointDefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The ReflectionBasedVirtualRelationEndPointDefinition 'OrderTicket' cannot be serialized because is is not part of the current mapping.")]
    public void VirtualRelationEndPointDefinitionWithoutRelationDefinition_NotInMapping ()
    {
      ClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false);

      VirtualRelationEndPointDefinition endPointdefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(classDefinition, "OrderTicket", true, CardinalityType.One, typeof (Order));

      SerializeAndDeserialize (endPointdefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The ReflectionBasedVirtualRelationEndPointDefinition 'OrderTicket' cannot be serialized because is is not part of the current mapping.")]
    public void VirtualRelationEndPointDefinitionWithRelationDefinition_NotInMapping ()
    {
      ReflectionBasedClassDefinition orderDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false);
      ReflectionBasedClassDefinition orderTicketDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("OrderTicket", "OrderTicket", "TestDomain", typeof (OrderTicket), false);

      orderTicketDefinition.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (orderTicketDefinition, "Order", "OrderID", typeof (ObjectID), false, StorageClass.Persistent));

      VirtualRelationEndPointDefinition orderEndPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(orderDefinition, "OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

      RelationEndPointDefinition orderTicketEndPointdefinition = new RelationEndPointDefinition (orderTicketDefinition, "Order", true);

      new RelationDefinition ("OrderToOrderTicket", orderEndPointDefinition, orderTicketEndPointdefinition);

      SerializeAndDeserialize (orderEndPointDefinition);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionInMapping ()
    {
      RelationDefinition relationDefinition = 
        MappingConfiguration.Current.RelationDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket->"
       +"Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order"];
      VirtualRelationEndPointDefinition endPointDefinition = (VirtualRelationEndPointDefinition) relationDefinition.GetEndPointDefinition ("Order", 
        "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");

      VirtualRelationEndPointDefinition deserializedEndPointDefinition = (VirtualRelationEndPointDefinition) SerializeAndDeserialize (endPointDefinition);
      Assert.AreSame (endPointDefinition, deserializedEndPointDefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The AnonymousRelationEndPointDefinition '<anonymous>' cannot be serialized because is is not part of the current mapping.")]
    public void AnonymousRelationEndPointDefinitionWithoutRelationDefinition_NotInMapping ()
    {
      ClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Client", "Client", "TestDomain", typeof (Client), false);

      AnonymousRelationEndPointDefinition endPointdefinition = new AnonymousRelationEndPointDefinition (classDefinition);

      SerializeAndDeserialize (endPointdefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The AnonymousRelationEndPointDefinition '<anonymous>' cannot be serialized because is is not part of the current mapping.")]
    public void AnonymousRelationEndPointDefinitionWithRelationDefinition_NotInMapping ()
    {
      ReflectionBasedClassDefinition clientDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Client", "Client", "TestDomain", typeof (Client), false);
      ReflectionBasedClassDefinition locationDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Location", "Location", "TestDomain", typeof (Location), false);

      locationDefinition.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (locationDefinition, "Client", "ClientID", typeof (ObjectID), false, StorageClass.Persistent));

      AnonymousRelationEndPointDefinition clientEndPointDefinition = new AnonymousRelationEndPointDefinition (clientDefinition);
      RelationEndPointDefinition locationEndPointDefinition = new RelationEndPointDefinition (locationDefinition, "Client", true);

      new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client", clientEndPointDefinition, locationEndPointDefinition);

      SerializeAndDeserialize (clientEndPointDefinition);
    }

    [Test]
    public void AnonymousRelationEndPointDefinition_InMapping ()
    {
      RelationDefinition relationDefinition = 
        MappingConfiguration.Current.RelationDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Location->"
        +"Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client"];
      AnonymousRelationEndPointDefinition endPointDefinition = (AnonymousRelationEndPointDefinition) relationDefinition.GetOppositeEndPointDefinition ("Location", 
        "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");

      AnonymousRelationEndPointDefinition deserializedEndPointDefinition = (AnonymousRelationEndPointDefinition) SerializeAndDeserialize (endPointDefinition);
      Assert.AreSame (endPointDefinition, deserializedEndPointDefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException), 
      ExpectedMessage = "The RelationDefinition 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket"
      +"->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket.Order' cannot be serialized because is is not part of the current mapping.")]
    public void RelationDefinition_NotInMapping ()
    {
      RelationDefinition relationDefinition = FakeMappingConfiguration.Current.RelationDefinitions.GetMandatory (
        "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket.Order");
      RelationDefinition deserializedRelationDefinition = (RelationDefinition) SerializeAndDeserialize (relationDefinition);

      Assert.IsFalse (ReferenceEquals (relationDefinition, deserializedRelationDefinition));
      AreEqual (relationDefinition, deserializedRelationDefinition);
    }

    [Test]
    public void RelationDefinition_InMapping ()
    {
      RelationDefinition relationDefinition = 
        MappingConfiguration.Current.RelationDefinitions.GetMandatory ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket->"
        +"Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");
      RelationDefinition deserializedRelationDefinition = (RelationDefinition) SerializeAndDeserialize (relationDefinition);

      Assert.AreSame (relationDefinition, deserializedRelationDefinition);
      AreEqual (relationDefinition, deserializedRelationDefinition);
    }

    [Test]
    public void SimpleRelationDefinitionCollection ()
    {
      RelationDefinitionCollection definitions = new RelationDefinitionCollection ();
      definitions.Add (MappingConfiguration.Current.RelationDefinitions[0]);
      definitions.Add (MappingConfiguration.Current.RelationDefinitions[1]);

      RelationDefinitionCollection deserializedDefinitions = (RelationDefinitionCollection) SerializeAndDeserialize (definitions);

      Assert.IsFalse (ReferenceEquals (definitions, deserializedDefinitions));
      Assert.AreSame (definitions[0], deserializedDefinitions[0]);
      Assert.AreSame (definitions[1], deserializedDefinitions[1]);
    }

    [Test]
    public void SimpleClassDefinitionCollection ()
    {
      ClassDefinitionCollection definitions = new ClassDefinitionCollection ();
      definitions.Add (MappingConfiguration.Current.ClassDefinitions[0]);
      definitions.Add (MappingConfiguration.Current.ClassDefinitions[1]);

      ClassDefinitionCollection deserializedDefinitions = (ClassDefinitionCollection) SerializeAndDeserialize (definitions);

      Assert.IsFalse (ReferenceEquals (definitions, deserializedDefinitions));
      AreEqual (definitions[0], deserializedDefinitions[0]);
      AreEqual (definitions[1], deserializedDefinitions[1]);
      Assert.IsTrue (deserializedDefinitions.Contains (definitions[0].ID));
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The ReflectionBasedClassDefinition 'Partner' cannot be serialized because is is not part of the current mapping.")]
    public void ClassDefinition_NotInMapping ()
    {
      ClassDefinition classDefinition = FakeMappingConfiguration.Current.ClassDefinitions.GetMandatory ("Partner");

      ClassDefinition deserializedClassDefinition = (ClassDefinition) SerializeAndDeserialize (classDefinition);

      Assert.IsFalse (ReferenceEquals (classDefinition, deserializedClassDefinition));
      AreEqual (classDefinition, deserializedClassDefinition);
    }

    [Test]
    public void ClassDefinition_InMapping ()
    {
      // Note: Partner has a base class and several derived classes.
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Partner");

      ClassDefinition deserializedClassDefinition = (ClassDefinition) SerializeAndDeserialize (classDefinition);

      Assert.AreSame (classDefinition, deserializedClassDefinition);
    }

    [Test]
    public void PropertyDefinitionCollectionInMapping ()
    {
      PropertyDefinitionCollection definitions = MappingConfiguration.Current.ClassDefinitions["Order"].MyPropertyDefinitions;
      PropertyDefinitionCollection deserializedDefinitions = (PropertyDefinitionCollection) SerializeAndDeserialize (definitions);

      Assert.IsFalse (ReferenceEquals (definitions, deserializedDefinitions));
      Assert.AreSame (definitions.ClassDefinition, deserializedDefinitions.ClassDefinition);
    }

    [Test]
    public void PropertyDefinitionWithEnumType ()
    {
      PropertyDefinition enumPropertyDefinition =
          MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Customer").MyPropertyDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Type"];

      PropertyDefinition deserializedEnumPropertyDefinition = (PropertyDefinition) SerializeAndDeserialize (enumPropertyDefinition);

      AreEqual (enumPropertyDefinition, deserializedEnumPropertyDefinition);
    }

    private void AreEqual (ClassDefinition expected, ClassDefinition actual)
    {
      Assert.AreEqual (expected.ID, actual.ID);

      if (expected.BaseClass != null)
        Assert.AreEqual (expected.BaseClass.ID, actual.BaseClass.ID);
      else
        Assert.IsNull (actual.BaseClass);

      Assert.AreEqual (expected.ClassType, actual.ClassType);
      Assert.AreEqual (expected.MyEntityName, actual.MyEntityName);
      Assert.AreEqual (expected.StorageProviderID, actual.StorageProviderID);

      Assert.AreEqual (expected.DerivedClasses.Count, actual.DerivedClasses.Count);
      for (int i = 0; i < expected.DerivedClasses.Count; i++)
        AreEqual (expected.DerivedClasses[i], actual.DerivedClasses[i]);

      Assert.AreEqual (expected.MyPropertyDefinitions.Count, actual.MyPropertyDefinitions.Count);
      for (int i = 0; i < expected.MyPropertyDefinitions.Count; i++)
        AreEqual (expected.MyPropertyDefinitions[i], actual.MyPropertyDefinitions[i]);

      Assert.AreEqual (expected.MyRelationDefinitions.Count, actual.MyRelationDefinitions.Count);
      for (int i = 0; i < expected.MyRelationDefinitions.Count; i++)
        AreEqual (expected.MyRelationDefinitions[i], actual.MyRelationDefinitions[i]);
    }

    private void AreEqual (RelationDefinition expected, RelationDefinition actual)
    {
      Assert.AreEqual (expected.ID, actual.ID);
      Assert.AreEqual (expected.EndPointDefinitions.Count, actual.EndPointDefinitions.Count);

      for (int i = 0; i < expected.EndPointDefinitions.Count; i++)
        AreEqual (expected.EndPointDefinitions[i], actual.EndPointDefinitions[i]);
    }

    private void AreEqual (IRelationEndPointDefinition expected, IRelationEndPointDefinition actual)
    {
      Assert.AreEqual (expected.GetType (), actual.GetType ());

      Assert.AreEqual (expected.Cardinality, actual.Cardinality);
      Assert.AreEqual (expected.ClassDefinition.ID, actual.ClassDefinition.ID);
      Assert.AreEqual (expected.IsMandatory, actual.IsMandatory);
      Assert.AreEqual (expected.IsAnonymous, actual.IsAnonymous);
      Assert.AreEqual (expected.IsVirtual, actual.IsVirtual);
      Assert.AreEqual (expected.PropertyName, actual.PropertyName);
      Assert.AreEqual (expected.PropertyType, actual.PropertyType);

      if (expected.RelationDefinition != null)
        Assert.AreEqual (expected.RelationDefinition.ID, actual.RelationDefinition.ID);
      else
        Assert.IsNull (actual.RelationDefinition);

      if (expected.GetType () == typeof (VirtualRelationEndPointDefinition))
        Assert.AreEqual (((VirtualRelationEndPointDefinition) expected).GetSortExpression(), ((VirtualRelationEndPointDefinition) actual).GetSortExpression());
    }

    private void AreEqual (PropertyDefinition expected, PropertyDefinition actual)
    {
      if (expected.ClassDefinition != null)
        Assert.AreEqual (expected.ClassDefinition.ID, actual.ClassDefinition.ID);
      else
        Assert.IsNull (actual.ClassDefinition);

      Assert.AreEqual (expected.StorageProperty.Name, actual.StorageProperty.Name);
      Assert.AreEqual (expected.DefaultValue, actual.DefaultValue);
      Assert.AreEqual (expected.IsNullable, actual.IsNullable);
      Assert.AreEqual (expected.MaxLength, actual.MaxLength);
      Assert.AreEqual (expected.PropertyName, actual.PropertyName);
      Assert.AreEqual (expected.PropertyType, actual.PropertyType);
      Assert.AreEqual (expected.StorageClass, actual.StorageClass);
    }
  }
}
