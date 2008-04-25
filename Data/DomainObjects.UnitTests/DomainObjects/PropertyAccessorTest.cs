using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class PropertyAccessorTest : ClientTransactionBaseTest
  {
    private static PropertyAccessor CreateAccessor (DomainObject domainObject, string propertyIdentifier)
    {
      ConstructorInfo ctor =
          typeof (PropertyAccessor).GetConstructor (
              BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] {typeof (DomainObject), typeof (string)}, null);
      try
      {
        return
            (PropertyAccessor)
            ctor.Invoke (BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] {domainObject, propertyIdentifier}, null);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    [Test]
    public void ManualPropertyAccessor ()
    {
      IndustrialSector sector = IndustrialSector.NewObject ();

      Company company = Company.NewObject ();
      company.IndustrialSector = sector;
      Assert.AreSame (sector, company.IndustrialSector, "related object");

      Assert.IsTrue (sector.Companies.ContainsObject (company), "related objects");

      sector.Name = "Foo";
      Assert.AreEqual ("Foo", sector.Name, "property value");
    }

    [Test]
    public void GetPropertyObjects()
    {
      CheckPropertyObjects(typeof (IndustrialSector), "Name",
          delegate (Tuple<PropertyDefinition, IRelationEndPointDefinition, ClassDefinition, string> t)
          {
            Assert.IsNotNull (t.A);
            Assert.AreSame (t.C, t.A.ClassDefinition);
            Assert.AreEqual (t.D, t.A.PropertyName);
            Assert.IsNull (t.B);
          });

      CheckPropertyObjects (typeof (IndustrialSector), "Companies",
          delegate (Tuple<PropertyDefinition, IRelationEndPointDefinition, ClassDefinition, string> t)
          {
            Assert.IsNotNull (t.B);
            Assert.AreSame (t.C, t.B.ClassDefinition);
            Assert.AreEqual (t.D, t.B.PropertyName);
          });

      CheckPropertyObjects (typeof (Company), "IndustrialSector",
          delegate (Tuple<PropertyDefinition, IRelationEndPointDefinition, ClassDefinition, string> t)
          {
            Assert.IsNotNull (t.B);
            Assert.AreSame (t.C, t.B.ClassDefinition);
            Assert.AreEqual (t.D, t.B.PropertyName);
          });

      CheckPropertyObjects (typeof (Employee), "Computer",
          delegate (Tuple<PropertyDefinition, IRelationEndPointDefinition, ClassDefinition, string> t)
          {
            Assert.IsNotNull (t.B);
            Assert.AreSame (t.C, t.B.ClassDefinition);
            Assert.AreEqual (t.D, t.B.PropertyName);
          });

      CheckPropertyObjects (typeof (Computer), "Employee",
          delegate (Tuple<PropertyDefinition, IRelationEndPointDefinition, ClassDefinition, string> t)
          {
            Assert.IsNotNull (t.B);
            Assert.AreSame (t.C, t.B.ClassDefinition);
            Assert.AreEqual (t.D, t.B.PropertyName);
          });

      CheckPropertyObjects (typeof (Client), "ParentClient",
          delegate (Tuple<PropertyDefinition, IRelationEndPointDefinition, ClassDefinition, string> t)
          {
            Assert.IsNotNull (t.B);
            Assert.AreSame (t.C, t.B.ClassDefinition);
            Assert.AreEqual (t.D, t.B.PropertyName);
          });
    }

    private static void CheckPropertyObjects (Type type, string shortPropertyName,
        Action<Tuple<PropertyDefinition, IRelationEndPointDefinition, ClassDefinition, string>> checker)
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[type];
      string identifier = type.FullName + "." + shortPropertyName;

      Tuple<PropertyDefinition, IRelationEndPointDefinition> propertyObjects =
          PropertyAccessor.GetPropertyDefinitionObjects (classDefinition, identifier);

      checker (new Tuple<PropertyDefinition, IRelationEndPointDefinition, ClassDefinition, string> (propertyObjects.A, propertyObjects.B,
          classDefinition, identifier));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "does not have a mapping property named", MatchType = MessageMatch.Contains)]
    public void GetPropertyObjectsThrowsOnInvalidPropertyID1()
    {
      PropertyAccessor.GetPropertyDefinitionObjects (
          MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)],
          "Bla");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "does not have a mapping property named", MatchType = MessageMatch.Contains)]
    public void GetPropertyObjectsThrowsOnInvalidPropertyID2 ()
    {
      PropertyAccessor.GetPropertyDefinitionObjects (
          MappingConfiguration.Current.ClassDefinitions[typeof (Company)],
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies");
    }

    [Test]
    public void PropertyAccessorKindStatic ()
    {
      Assert.AreEqual (PropertyKind.PropertyValue,
          PropertyAccessor.GetPropertyKind(MappingConfiguration.Current.ClassDefinitions[typeof(IndustrialSector)],
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name"),
          "Property value type");

      Assert.AreEqual (PropertyKind.RelatedObjectCollection,
          PropertyAccessor.GetPropertyKind (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)],
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies"),
          "Related object collection type - bidirectional relation 1:n, 1 side");

      Assert.AreEqual (PropertyKind.RelatedObject,
          PropertyAccessor.GetPropertyKind(MappingConfiguration.Current.ClassDefinitions[typeof(Company)],
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector"),
          "Related object type - bidirectional relation 1:n, n side");

      Assert.AreEqual (PropertyKind.RelatedObject,
          PropertyAccessor.GetPropertyKind (MappingConfiguration.Current.ClassDefinitions[typeof (Employee)],
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer"),
          "Related object type - bidirectional relation 1:1, referenced side");

      Assert.AreEqual (PropertyKind.RelatedObject,
          PropertyAccessor.GetPropertyKind (MappingConfiguration.Current.ClassDefinitions[typeof (Computer)],
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"),
          "Related object type - bidirectional relation 1:1, foreign key side");

      Assert.AreEqual (PropertyKind.RelatedObject,
          PropertyAccessor.GetPropertyKind (MappingConfiguration.Current.ClassDefinitions[typeof (Client)],
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient"),
          "Related object type - unidirectional relation 1:n, 1 side");
    }

    [Test]
    public void PropertyAccessorKindInstance ()
    {
      IndustrialSector sector = IndustrialSector.NewObject();
      Assert.AreEqual (PropertyKind.PropertyValue,
          CreateAccessor (sector, "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name").Kind, "Property value type");

      Assert.AreEqual (PropertyKind.RelatedObjectCollection,
          CreateAccessor (sector, "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies").Kind,
          "Related object collection type - bidirectional relation 1:n, 1 side");

      Company company = Company.NewObject ();
      Assert.AreEqual (PropertyKind.RelatedObject,
          CreateAccessor (company, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector").Kind,
          "Related object type - bidirectional relation 1:n, n side");

      Employee employee = Employee.NewObject ();
      Assert.AreEqual (PropertyKind.RelatedObject,
          CreateAccessor (employee, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer").Kind,
          "Related object type - bidirectional relation 1:1, referenced side");

      Computer computer = Computer.NewObject ();
      Assert.AreEqual (PropertyKind.RelatedObject,
          CreateAccessor (computer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee").Kind,
          "Related object type - bidirectional relation 1:1, foreign key side");

      Client client = Client.NewObject ();
      Assert.AreEqual (PropertyKind.RelatedObject,
          CreateAccessor (client, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient").Kind,
          "Related object type - unidirectional relation 1:n, 1 side");
    }

    [Test]
    public void PropertyTypeStatic()
    {
      Assert.AreEqual (typeof (string),
          PropertyAccessor.GetPropertyType(MappingConfiguration.Current.ClassDefinitions[typeof(IndustrialSector)],
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name"),
          "Property value type");

      Assert.AreEqual (typeof (ObjectList<Company>),
          PropertyAccessor.GetPropertyType(MappingConfiguration.Current.ClassDefinitions[typeof(IndustrialSector)],
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies"),
          "Related object collection type - bidirectional relation 1:n, 1 side");

      Assert.AreEqual (typeof (IndustrialSector),
          PropertyAccessor.GetPropertyType (MappingConfiguration.Current.ClassDefinitions[typeof (Company)],
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector"),
          "Related object type - bidirectional relation 1:n, n side");

      Assert.AreEqual (typeof (Computer),
          PropertyAccessor.GetPropertyType(MappingConfiguration.Current.ClassDefinitions[typeof(Employee)],
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer"),
          "Related object type - bidirectional relation 1:1, referenced side");

      Assert.AreEqual (typeof (Employee),
          PropertyAccessor.GetPropertyType (MappingConfiguration.Current.ClassDefinitions[typeof (Computer)],
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"),
          "Related object type - bidirectional relation 1:1, foreign key side");

      Assert.AreEqual (typeof (Client),
          PropertyAccessor.GetPropertyType(MappingConfiguration.Current.ClassDefinitions[typeof(Client)],
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient"),
          "Related object type - unidirectional relation 1:n, 1 side");
    }

    [Test]
    public void PropertyTypeInstance ()
    {
      Assert.AreEqual (typeof (string),
          CreateAccessor (IndustrialSector.NewObject(), "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name").PropertyType,
          "Property value type");

      Assert.AreEqual (typeof (ObjectList<Company>),
          CreateAccessor (IndustrialSector.NewObject (), "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies").PropertyType,
          "Related object collection type - bidirectional relation 1:n, 1 side");

      Assert.AreEqual (typeof (IndustrialSector),
          CreateAccessor (Company.NewObject (), "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector").PropertyType,
          "Related object type - bidirectional relation 1:n, n side");

      Assert.AreEqual (typeof (Computer),
          CreateAccessor (Employee.NewObject(), "Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer").PropertyType,
          "Related object type - bidirectional relation 1:1, referenced side");

      Assert.AreEqual (typeof (Employee),
          CreateAccessor (Computer.NewObject (),  "Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee").PropertyType,
          "Related object type - bidirectional relation 1:1, foreign key side");

      Assert.AreEqual (typeof (Client),
          CreateAccessor (Client.NewObject (),  "Remotion.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient").PropertyType,
          "Related object type - unidirectional relation 1:n, 1 side");
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException), ExpectedMessage = "Actual type 'System.String' of property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name' does not match expected type 'System.Int32'.")]
    public void PropertyAccessorGetThrowsIfWrongType ()
    {
      IndustrialSector sector = IndustrialSector.NewObject ();
      CreateAccessor (sector, "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name").GetValue<int>();
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException), ExpectedMessage = "Actual type 'System.String' of property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name' does not match expected type 'System.Int32'.")]
    public void PropertyAccessorSetThrowsIfWrongType ()
    {
      IndustrialSector sector = IndustrialSector.NewObject ();
      CreateAccessor (sector, "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name").SetValue (5);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "does not have a mapping property named", MatchType = MessageMatch.Contains)]
    public void PropertyAccessorThrowsIfWrongIdentifier ()
    {
      IndustrialSector sector = IndustrialSector.NewObject ();
      CreateAccessor (sector, "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.FooBarFredBaz");
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Related object collections cannot be set.",
          MatchType = MessageMatch.Contains)]
    public void PropertyAccessorThrowsIfSettingObjectList ()
    {
      IndustrialSector sector = IndustrialSector.NewObject ();
      CreateAccessor (sector, "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies").SetValue (new ObjectList<Company> ());
    }

    [Test]
    public void IsValidProperty ()
    {
      Assert.IsFalse (PropertyAccessor.IsValidProperty (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)], "Bla"));
      Assert.IsFalse (PropertyAccessor.IsValidProperty (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)], "Companies"));
      Assert.IsTrue (PropertyAccessor.IsValidProperty (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)],
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies"));
    }

    [Test]
    public void PropertyMetadata()
    {
      IndustrialSector sector = IndustrialSector.NewObject();
      PropertyAccessor accessor = CreateAccessor (sector,
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies");

      Assert.AreSame (sector, accessor.DomainObject);
      
      Assert.AreSame (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)], accessor.ClassDefinition);
      Assert.AreSame ("Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies", accessor.PropertyIdentifier);
      Assert.IsNull (accessor.PropertyDefinition);
      Assert.IsNotNull (accessor.RelationEndPointDefinition);
      Assert.AreSame (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)]
          .GetRelationEndPointDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies"),
          accessor.RelationEndPointDefinition);

      accessor = CreateAccessor (IndustrialSector.NewObject(),
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name");

      Assert.IsNotNull (accessor.PropertyDefinition);
      Assert.AreSame (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)]
          .GetPropertyDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name"),
          accessor.PropertyDefinition);

      Assert.IsNull (accessor.RelationEndPointDefinition);

      accessor = CreateAccessor (Computer.NewObject (),
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee");

      Assert.IsNotNull (accessor.PropertyDefinition);
      Assert.AreSame (MappingConfiguration.Current.ClassDefinitions[typeof (Computer)]
          .GetPropertyDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"),
          accessor.PropertyDefinition);

      Assert.IsNotNull (accessor.RelationEndPointDefinition);
      Assert.AreSame (MappingConfiguration.Current.ClassDefinitions[typeof (Computer)]
          .GetRelationEndPointDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"),
          accessor.RelationEndPointDefinition);
    }

    [Test]
    public void HasChangedAndOriginalValueSimple()
    {
      IndustrialSector sector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      PropertyAccessor property = CreateAccessor (sector, "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name");
      Assert.IsFalse (property.HasChanged);
      string originalValue = property.GetValue<string>();
      Assert.IsNotNull (originalValue);
      Assert.AreEqual (originalValue, property.GetOriginalValue<string> ());

      property.SetValue ("Foo");
      Assert.IsTrue (property.HasChanged);
      Assert.AreEqual ("Foo", property.GetValue<string>());
      Assert.AreNotEqual (property.GetValue<string>(), property.GetOriginalValue<string> ());
      Assert.AreEqual (originalValue, property.GetOriginalValue<string> ());
    }

    [Test]
    public void HasChangedAndOriginalValueRelated()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      PropertyAccessor property = CreateAccessor (computer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee");
      Assert.IsFalse (property.HasChanged);
      Employee originalValue = property.GetOriginalValue<Employee>();

      property.GetValue<Employee> ().Name = "FooBarBazFred";
      Assert.IsFalse (property.HasChanged);

      Employee newValue = Employee.NewObject ();
      property.SetValue (newValue);
      Assert.IsTrue (property.HasChanged);
      Assert.AreEqual (newValue, property.GetValue<Employee> ());
      Assert.AreNotEqual (property.GetValue<Employee> (), property.GetOriginalValue<Employee> ());
      Assert.AreEqual (originalValue, property.GetOriginalValue<Employee> ());

    }

    [Test]
    public void HasChangedAndOriginalValueRelatedCollection()
    {
      IndustrialSector sector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      PropertyAccessor property = CreateAccessor (sector, "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies");

      Assert.IsFalse (property.HasChanged);
      ObjectList<Company> originalValue = property.GetValue<ObjectList<Company>> ();
      int originalCount = originalValue.Count;
      Assert.IsNotNull (originalValue);
      Assert.AreEqual (originalValue, property.GetOriginalValue<ObjectList<Company>> ());

      property.GetValue<ObjectList<Company>> ().Add (Company.NewObject ());
      Assert.AreNotEqual (originalCount, property.GetValue<ObjectList<Company>> ().Count);
      Assert.IsTrue (property.HasChanged);

      Assert.AreSame (originalValue, property.GetValue<ObjectList<Company>> ()); // !!
      Assert.AreNotSame (property.GetValue<ObjectList<Company>> (), property.GetOriginalValue<ObjectList<Company>> ());
      Assert.AreEqual (originalCount, property.GetOriginalValue<ObjectList<Company>>().Count);
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException), ExpectedMessage =  "Actual type .* of property .* does not match expected type 'System.Int32'",
        MatchType = MessageMatch.Regex)]
    public void GetOriginalValueThrowsWithWrongType()
    {
      CreateAccessor (IndustrialSector.NewObject(), "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies")
          .GetOriginalValue<int>();
    }

    [Test]
    public void HasBeenTouchedSimple ()
    {
      IndustrialSector sector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      PropertyAccessor property = CreateAccessor (sector, "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name");

      Assert.IsFalse (property.HasBeenTouched);
      property.SetValueWithoutTypeCheck (property.GetValueWithoutTypeCheck ());
      Assert.IsTrue (property.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedRelated ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      PropertyAccessor property = CreateAccessor (computer, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee");

      Assert.IsFalse (property.HasBeenTouched);
      property.SetValueWithoutTypeCheck (property.GetValueWithoutTypeCheck ());
      Assert.IsTrue (property.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedRelatedCollection ()
    {
      IndustrialSector sector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      PropertyAccessor property = CreateAccessor (sector, "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies");

      Assert.IsFalse (property.HasBeenTouched);
      ((DomainObjectCollection) property.GetValueWithoutTypeCheck ())[0] = ((DomainObjectCollection) property.GetValueWithoutTypeCheck ())[0];
      Assert.IsTrue (property.HasBeenTouched);
    }

    [Test]
    public void IsNullPropertyValue ()
    {
      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.NewObject ();
      Assert.IsTrue (cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].IsNull);
      Assert.IsFalse (cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty"].IsNull);

      cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].SetValue<bool?> (true);
      Assert.IsFalse (cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].IsNull);

      cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].SetValue<bool?> (null);
      Assert.IsTrue (cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].IsNull);

      cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].SetValue<bool?> (null);
      Assert.IsTrue (cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].IsNull);

      Assert.IsTrue (cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"].IsNull);
      Assert.IsFalse (cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"].IsNull);

      cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"].SetValue ("");
      Assert.IsFalse (cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"].IsNull);

      cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"].SetValue<string> (null);
      Assert.IsTrue (cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"].IsNull);
    }

    [Test]
    public void IsNullRelatedObjectCollection ()
    {
      Order newOrder = Order.NewObject ();
      Assert.IsFalse (newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].IsNull);
    }

    [Test]
    public void IsNullRelatedObjectNonVirtualEndPoint ()
    {
      Order newOrder = Order.NewObject ();
      Assert.IsTrue (newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].IsNull);

      newOrder.Customer = Customer.NewObject ();
      Assert.IsFalse (newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].IsNull);

      newOrder.Customer = null;
      Assert.IsTrue (newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].IsNull);

      ClientTransactionEventReceiver eventReceiver = new ClientTransactionEventReceiver (ClientTransactionScope.CurrentTransaction);
      Order existingOrder = Order.GetObject (DomainObjectIDs.Order1);

      eventReceiver.Clear ();
      Assert.AreEqual (0, eventReceiver.LoadedDomainObjects.Count);

      Assert.IsFalse (existingOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].IsNull);
      Assert.AreEqual (0, eventReceiver.LoadedDomainObjects.Count, "The IsNull check did not cause the object to be loaded.");

      Assert.IsFalse (existingOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].GetValue<Customer> () == null);
      Assert.AreEqual (1, eventReceiver.LoadedDomainObjects.Count, "An ordinary check does cause the object to be loaded.");
    }

    [Test]
    public void IsNullRelatedObjectVirtualEndPoint ()
    {
      Order newOrder = Order.NewObject ();
      Assert.IsTrue (newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].IsNull);

      newOrder.OrderTicket = OrderTicket.NewObject ();
      Assert.IsFalse (newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].IsNull);

      newOrder.OrderTicket = null;
      Assert.IsTrue (newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].IsNull);

      ClientTransactionEventReceiver eventReceiver = new ClientTransactionEventReceiver (ClientTransactionScope.CurrentTransaction);
      Order existingOrder = Order.GetObject (DomainObjectIDs.Order1);

      eventReceiver.Clear ();
      Assert.AreEqual (0, eventReceiver.LoadedDomainObjects.Count);

      Assert.IsFalse (existingOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].IsNull);
      Assert.AreEqual (1, eventReceiver.LoadedDomainObjects.Count, "For virtual end points, the IsNull unfortunately does cause a load.");

      Assert.IsFalse (existingOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetValue<OrderTicket> () == null);
      Assert.AreEqual (1, eventReceiver.LoadedDomainObjects.Count, "An ordinary check does cause the object to be loaded.");
    }

    [Test]
    public void GetValueWithoutTypeCheck ()
    {
      Order newOrder = Order.NewObject ();

      object ticket = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetValueWithoutTypeCheck();
      Assert.AreSame (ticket, newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetValue<OrderTicket>());

      object items = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetValueWithoutTypeCheck ();
      Assert.AreSame (items,
          newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetValue<ObjectList<OrderItem>> ());

      object number = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetValueWithoutTypeCheck ();
      Assert.AreEqual (number,
          newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetValue<int> ());
    }

    [Test]
    public void GetOriginalValueWithoutTypeCheck ()
    {
      Order newOrder = Order.NewObject ();

      newOrder.OrderTicket = OrderTicket.NewObject ();

      object ticket = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetOriginalValueWithoutTypeCheck ();
      Assert.AreSame (ticket, newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetOriginalValue<OrderTicket> ());

      newOrder.OrderItems.Add (OrderItem.NewObject ());

      object items = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetOriginalValueWithoutTypeCheck ();
      Assert.AreSame (items,
          newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetOriginalValue<ObjectList<OrderItem>> ());

      ++newOrder.OrderNumber;

      object number = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetOriginalValueWithoutTypeCheck ();
      Assert.AreEqual (number,
          newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetOriginalValue<int> ());
    }

    [Test]
    public void SetValueWithoutTypeCheck ()
    {
      Order newOrder = Order.NewObject ();
      newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].SetValueWithoutTypeCheck (7);
      Assert.AreEqual (7, newOrder.OrderNumber);

      OrderTicket orderTicket = OrderTicket.NewObject ();
      newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].SetValueWithoutTypeCheck (orderTicket);
      Assert.AreSame (orderTicket, newOrder.OrderTicket);
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException), ExpectedMessage = "Actual type 'System.String' of property "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber' does not match expected type 'System.Int32'.")]
    public void SetValueWithoutTypeCheckThrowsOnWrongType ()
    {
      Order newOrder = Order.NewObject ();
      newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].SetValueWithoutTypeCheck ("7");
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Related object collections cannot be set.")]
    public void SetValueWithoutTypeCheckThrowsOnRelatedObjectCollection ()
    {
      Order newOrder = Order.NewObject ();
      newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].SetValueWithoutTypeCheck (new ObjectList<OrderItem>());
    }

    [Test]
    public void GetValueTx ()
    {
      Order newOrder = Order.NewObject ();
      
      newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].SetValue(9);
      newOrder.OrderItems.Add (RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem1, false));
      newOrder.OrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].SetValue (10);
        newOrder.OrderItems.Clear();
        newOrder.OrderItems.Add (RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem2, false));
        newOrder.OrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

        Assert.AreEqual (
            10,
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetValueTx<int> (
                ClientTransactionScope.CurrentTransaction));
        Assert.AreEqual (
            9,
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetValueTx<int> (
                ClientTransactionScope.CurrentTransaction.ParentTransaction));

        Assert.AreEqual (
            RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem2, false),
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetValueTx<ObjectList<OrderItem>> (
                ClientTransactionScope.CurrentTransaction)[0]);
        Assert.AreEqual (
            RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem1, false),
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetValueTx<ObjectList<OrderItem>> (
                ClientTransactionScope.CurrentTransaction.ParentTransaction)[0]);

        Assert.AreEqual (
            RepositoryAccessor.GetObject (DomainObjectIDs.OrderTicket2, false),
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetValueTx<OrderTicket> (
                ClientTransactionScope.CurrentTransaction));
        Assert.AreEqual (
            RepositoryAccessor.GetObject (DomainObjectIDs.OrderTicket1, false),
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetValueTx<OrderTicket> (
                ClientTransactionScope.CurrentTransaction.ParentTransaction));
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void GetValueTxWithInvalidTransactionNew ()
    {
      Order newOrder = Order.NewObject ();
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        accessor.GetValueTx<int> (ClientTransactionScope.CurrentTransaction);
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void GetValueTxWithInvalidTransactionLoaded ()
    {
      Order newOrder = Order.GetObject (DomainObjectIDs.Order1);
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        accessor.GetValueTx<int> (ClientTransactionScope.CurrentTransaction);
      }
    }

    [Test]
    public void GetValueWithoutTypeCheckTx ()
    {
      Order newOrder = Order.NewObject ();

      newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].SetValue (9);
      newOrder.OrderItems.Add (RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem1, false));
      newOrder.OrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].SetValue (10);
        newOrder.OrderItems.Clear ();
        newOrder.OrderItems.Add (RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem2, false));
        newOrder.OrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

        Assert.AreEqual (
            10,
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction));
        Assert.AreEqual (
            9,
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction.ParentTransaction));

        Assert.AreEqual (
            RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem2, false),
            ((DomainObjectCollection)newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction))[0]);
        Assert.AreEqual (
            RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem1, false),
            ((DomainObjectCollection)newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction.ParentTransaction))[0]);

        Assert.AreEqual (
            RepositoryAccessor.GetObject (DomainObjectIDs.OrderTicket2, false),
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction));
        Assert.AreEqual (
            RepositoryAccessor.GetObject (DomainObjectIDs.OrderTicket1, false),
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction.ParentTransaction));
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void GetValueWithoutTypeCheckTxWithInvalidTransactionNew ()
    {
      Order newOrder = Order.NewObject ();
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        accessor.GetValueWithoutTypeCheckTx (ClientTransactionScope.CurrentTransaction);
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void GetValueWithoutTypeCheckTxWithInvalidTransactionLoaded ()
    {
      Order newOrder = Order.GetObject (DomainObjectIDs.Order1);
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        accessor.GetValueWithoutTypeCheckTx (ClientTransactionScope.CurrentTransaction);
      }
    }

    [Test]
    public void GetOriginalValueTx ()
    {
      Order newOrder = Order.NewObject ();
      
      newOrder.OrderNumber = 9;
      OrderItem newOrderItem = OrderItem.NewObject ();
      newOrder.OrderItems.Add (newOrderItem);
      OrderTicket newOrderTicket = OrderTicket.NewObject ();
      newOrder.OrderTicket = newOrderTicket;

      newOrder.Official = Official.NewObject ();

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.Commit();
        
        Assert.AreEqual (
            9,
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetOriginalValueTx<int> (
                ClientTransactionScope.CurrentTransaction));
        Assert.AreEqual (
            0,
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetOriginalValueTx<int> (
                ClientTransactionScope.CurrentTransaction.ParentTransaction));

        Assert.AreEqual (
            newOrderItem,
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetOriginalValueTx<ObjectList<OrderItem>> (
                ClientTransactionScope.CurrentTransaction)[0]);
        Assert.AreEqual (
            0,
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetOriginalValueTx<ObjectList<OrderItem>> (
                ClientTransactionScope.CurrentTransaction.ParentTransaction).Count);

        Assert.AreEqual (
            newOrderTicket,
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetOriginalValueTx<OrderTicket> (
                ClientTransactionScope.CurrentTransaction));
        Assert.AreEqual (
            null,
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetOriginalValueTx<OrderTicket> (
                ClientTransactionScope.CurrentTransaction.ParentTransaction));
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void GetOriginalValueTxWithInvalidTransactionNew ()
    {
      Order newOrder = Order.NewObject ();
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        accessor.GetOriginalValueTx<int> (ClientTransactionScope.CurrentTransaction);
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void GetOriginalValueTxWithInvalidTransactionLoaded ()
    {
      Order newOrder = Order.GetObject (DomainObjectIDs.Order1);
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        accessor.GetOriginalValueTx<int> (ClientTransactionScope.CurrentTransaction);
      }
    }

    [Test]
    public void GetOriginalValueWithoutTypeCheckTx ()
    {
      Order newOrder = Order.NewObject ();

      newOrder.OrderNumber = 9;
      OrderItem newOrderItem = OrderItem.NewObject ();
      newOrder.OrderItems.Add (newOrderItem);
      OrderTicket newOrderTicket = OrderTicket.NewObject ();
      newOrder.OrderTicket = newOrderTicket;

      newOrder.Official = Official.NewObject ();

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.Commit ();

        Assert.AreEqual (
            9,
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetOriginalValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction));
        Assert.AreEqual (
            0,
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetOriginalValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction.ParentTransaction));

        Assert.AreEqual (
            newOrderItem,
            ((ObjectList<OrderItem>) newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetOriginalValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction))[0]);
        Assert.AreEqual (
            0,
            ((ObjectList<OrderItem>) newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetOriginalValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction.ParentTransaction)).Count);

        Assert.AreEqual (
            newOrderTicket,
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetOriginalValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction));
        Assert.AreEqual (
            null,
            newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetOriginalValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction.ParentTransaction));
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void GetOriginalValueWithoutTypeCheckTxWithInvalidTransactionNew ()
    {
      Order newOrder = Order.NewObject ();
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        accessor.GetOriginalValueWithoutTypeCheckTx (ClientTransactionScope.CurrentTransaction);
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void GetOriginalValueWithoutTypeCheckTxWithInvalidTransactionLoaded ()
    {
      Order newOrder = Order.GetObject (DomainObjectIDs.Order1);
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        accessor.GetOriginalValueWithoutTypeCheckTx (ClientTransactionScope.CurrentTransaction);
      }
    }

    [Test]
    public void SetValueTx ()
    {
      Order order = Order.GetObject(DomainObjectIDs.Order1);
      OrderTicket orderTicket2 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);
        OrderTicket orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);

        order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].SetValueTx (
            ClientTransactionScope.CurrentTransaction, 1);
        order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].SetValueTx (ClientTransactionMock, 2);

        order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].SetValueTx (
            ClientTransactionScope.CurrentTransaction, orderTicket1);
        order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].SetValueTx (
            ClientTransactionMock, orderTicket2);

        Assert.AreEqual (1, order.OrderNumber);
        Assert.AreSame (orderTicket1, order.OrderTicket);
      }
      Assert.AreEqual (2, order.OrderNumber);
      Assert.AreSame (orderTicket2, order.OrderTicket);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void SetValueTxWithInvalidTransactionNew ()
    {
      Order newOrder = Order.NewObject ();
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        accessor.SetValueTx (ClientTransactionScope.CurrentTransaction, 1);
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void SetValueTxWithInvalidTransactionLoaded ()
    {
      Order newOrder = Order.GetObject (DomainObjectIDs.Order1);
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        accessor.SetValueTx (ClientTransactionScope.CurrentTransaction, 2);
      }
    }

    [Test]
    public void SetValueWithoutTypeCheckTx ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderTicket orderTicket2 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);
        OrderTicket orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);

        order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].SetValueWithoutTypeCheckTx (
            ClientTransactionScope.CurrentTransaction, 1);
        order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].SetValueWithoutTypeCheckTx (ClientTransactionMock, 2);

        order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].SetValueWithoutTypeCheckTx (
            ClientTransactionScope.CurrentTransaction, orderTicket1);
        order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].SetValueWithoutTypeCheckTx (
            ClientTransactionMock, orderTicket2);

        Assert.AreEqual (1, order.OrderNumber);
        Assert.AreSame (orderTicket1, order.OrderTicket);
      }
      Assert.AreEqual (2, order.OrderNumber);
      Assert.AreSame (orderTicket2, order.OrderTicket);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void SetValueWithoutTypeCheckTxWithInvalidTransactionNew ()
    {
      Order newOrder = Order.NewObject ();
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        accessor.SetValueWithoutTypeCheckTx (ClientTransactionScope.CurrentTransaction, 1);
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void SetValueWithoutTypeCheckTxWithInvalidTransactionLoaded ()
    {
      Order newOrder = Order.GetObject (DomainObjectIDs.Order1);
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        accessor.SetValueWithoutTypeCheckTx (ClientTransactionScope.CurrentTransaction, 2);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException))]
    public void SetValueWithoutTypeCheckTxForWrongType ()
    {
      Order newOrder = Order.NewObject ();
      newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].SetValueWithoutTypeCheckTx (
            ClientTransactionScope.CurrentTransaction, "1");
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This operation can only be used on related object properties.")]
    public void GetRelatedObjectIDSimple ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetRelatedObjectID ();
    }

    [Test]
    public void GetRelatedObjectIDRelatedRealEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.AreEqual (order.Customer.ID, order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].GetRelatedObjectID ());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "ObjectIDs only exist on the real side of a relation, not on the virtual side.")]
    public void GetRelatedObjectIDRelatedVirtualEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetRelatedObjectID ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This operation can only be used on related object properties.")]
    public void GetRelatedObjectIDRelatedCollection ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetRelatedObjectID ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This operation can only be used on related object properties.")]
    public void GetRelatedObjectIDTxSimple ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetRelatedObjectIDTx (ClientTransactionMock);
      }
    }

    [Test]
    public void GetRelatedObjectIDTxRelatedRealEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ObjectID customerID = order.Customer.ID;
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (customerID, order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].GetRelatedObjectIDTx (ClientTransactionMock));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "ObjectIDs only exist on the real side of a relation, not on the virtual side.")]
    public void GetRelatedObjectIDTxRelatedVirtualEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetRelatedObjectIDTx (ClientTransactionMock);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "ObjectIDs only exist on the real side of a relation, not on the virtual side.")]
    public void GetRelatedObjectTxIDRelatedVirtualEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetRelatedObjectIDTx (ClientTransactionMock);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This operation can only be used on related object properties.")]
    public void GetOriginalRelatedObjectIDSimple ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetOriginalRelatedObjectID ();
    }

    [Test]
    public void GetOriginalRelatedObjectIDRelatedRealEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ObjectID originalID = order.Customer.ID;
      order.Customer = Customer.NewObject ();
      Assert.AreNotEqual (order.Customer.ID, order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].GetOriginalRelatedObjectID ());
      Assert.AreEqual (originalID, order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].GetOriginalRelatedObjectID ());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "ObjectIDs only exist on the real side of a relation, not on the virtual side.")]
    public void GetOriginalRelatedObjectIDRelatedVirtualEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetOriginalRelatedObjectID ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This operation can only be used on related object properties.")]
    public void GetOriginalRelatedObjectIDRelatedCollection ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetOriginalRelatedObjectID ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This operation can only be used on related object properties.")]
    public void GetOriginalRelatedObjectIDTxSimple ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetOriginalRelatedObjectIDTx (ClientTransactionMock);
      }
    }

    [Test]
    public void GetOriginalRelatedObjectIDTxRelatedRealEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ObjectID originalCustomerID = order.Customer.ID;
      order.Customer = Customer.NewObject ();
      ObjectID customerID = order.Customer.ID;
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreNotEqual (customerID, order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].GetOriginalRelatedObjectIDTx (ClientTransactionMock));
        Assert.AreEqual (originalCustomerID, order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].GetOriginalRelatedObjectIDTx (ClientTransactionMock));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "ObjectIDs only exist on the real side of a relation, not on the virtual side.")]
    public void GetOriginalRelatedObjectIDTxRelatedVirtualEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetOriginalRelatedObjectIDTx (ClientTransactionMock);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "ObjectIDs only exist on the real side of a relation, not on the virtual side.")]
    public void GetOriginalRelatedObjectTxIDRelatedVirtualEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetOriginalRelatedObjectIDTx (ClientTransactionMock);
      }
    }

    [Test]
    public void GetSetValueTx_WithNoScope ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionScope.EnterNullScope ())
      {
        int orderNumber = order.Properties[typeof (Order), "OrderNumber"].GetValueTx<int> (ClientTransactionMock);
        order.Properties[typeof (Order), "OrderNumber"].SetValueTx (ClientTransactionMock, orderNumber + 1);
        Assert.AreEqual (orderNumber + 1, order.Properties[typeof (Order), "OrderNumber"].GetValueTx<int> (ClientTransactionMock));
      }
    }

    [Test]
    public void DiscardCheck ()
    {
      Order order = Order.NewObject ();
      order.Delete ();

      PropertyAccessor property = order.Properties[typeof (Order), "OrderNumber"];

      ExpectDiscarded (delegate { Dev.Null = property.HasChanged; });
      ExpectDiscarded (delegate { Dev.Null = property.HasBeenTouched; });
      ExpectDiscarded (delegate { Dev.Null = property.IsNull; });
      ExpectDiscarded (delegate { Dev.Null = property.GetOriginalRelatedObjectID(); });
      ExpectDiscarded (delegate { Dev.Null = property.GetOriginalValue<int> (); });
      ExpectDiscarded (delegate { Dev.Null = property.GetOriginalValueWithoutTypeCheck(); });
      ExpectDiscarded (delegate { Dev.Null = property.GetRelatedObjectID (); });
      ExpectDiscarded (delegate { Dev.Null = property.GetValue<int> (); });
      ExpectDiscarded (delegate { Dev.Null = property.GetValueWithoutTypeCheck (); });
      ExpectDiscarded (delegate { property.SetValue (0); });
      ExpectDiscarded (delegate { property.SetValueWithoutTypeCheck (0); });
      
      // no exceptions
      Dev.Null = property.Kind;
      Dev.Null = property.PropertyDefinition;
      Dev.Null = property.PropertyIdentifier;
      Dev.Null = property.PropertyType;
      Dev.Null = property.RelationEndPointDefinition;
      Dev.Null = property.ClassDefinition;
      Dev.Null = property.DomainObject;
    }

    [Test]
    public void DiscardCheck_Tx ()
    {
      ClassWithAllDataTypes instance = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      ClientTransaction otherTransaction = ClientTransaction.NewRootTransaction ();
      using (otherTransaction.EnterNonDiscardingScope ())
      {
        otherTransaction.EnlistDomainObject (instance);
        instance.Delete();
        SetDatabaseModifyable ();
        otherTransaction.Commit ();
        Assert.IsTrue (instance.IsDiscarded);
      }

      Assert.IsFalse (instance.IsDiscarded);

      PropertyAccessor property = instance.Properties[typeof (ClassWithAllDataTypes), "Int32Property"];

      ExpectDiscarded (delegate { Dev.Null = property.GetOriginalRelatedObjectIDTx (otherTransaction); });
      ExpectDiscarded (delegate { Dev.Null = property.GetOriginalValueTx<int> (otherTransaction); });
      ExpectDiscarded (delegate { Dev.Null = property.GetOriginalValueWithoutTypeCheckTx (otherTransaction); });
      ExpectDiscarded (delegate { Dev.Null = property.GetRelatedObjectIDTx (otherTransaction); });
      ExpectDiscarded (delegate { Dev.Null = property.GetValueTx<int> (otherTransaction); });
      ExpectDiscarded (delegate { Dev.Null = property.GetValueWithoutTypeCheckTx (otherTransaction); });
      ExpectDiscarded (delegate { property.SetValueTx (otherTransaction, 0); });
      ExpectDiscarded (delegate { property.SetValueWithoutTypeCheckTx (otherTransaction, 0); });

      try { Dev.Null = property.GetOriginalRelatedObjectIDTx (ClientTransaction.Current); } catch (InvalidOperationException) { }
      Dev.Null = property.GetOriginalValueTx<int> (ClientTransaction.Current);
      Dev.Null = property.GetOriginalValueWithoutTypeCheckTx (ClientTransaction.Current);
      try { Dev.Null = property.GetRelatedObjectIDTx (ClientTransaction.Current); } catch (InvalidOperationException) { }
      Dev.Null = property.GetValueTx<int> (ClientTransaction.Current);
      Dev.Null = property.GetValueWithoutTypeCheckTx (ClientTransaction.Current);
      property.SetValueTx (ClientTransaction.Current, 0);
      property.SetValueWithoutTypeCheckTx (ClientTransaction.Current, 0);
    }


    private void ExpectDiscarded (Proc action)
    {
      try
      {
        action ();
        Assert.Fail ("Expected ObjectDiscardedException.");
      }
      catch (ObjectDiscardedException)
      {
        // ok
      }
    }
  }
}
