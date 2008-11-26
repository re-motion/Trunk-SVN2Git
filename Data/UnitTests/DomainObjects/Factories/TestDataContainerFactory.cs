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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Resources;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Factories
{
  public class TestDataContainerFactory
  {
    // types

    // members and constants

    // member fields

    private ClientTransactionMock _clientTransactionMock;
    private DomainObjectIDs _domainObjectIDs;

    // construction and disposing

    public TestDataContainerFactory (ClientTransactionMock clientTransactionMock)
    {
      ArgumentUtility.CheckNotNull ("clientTransactionMock", clientTransactionMock);
      _clientTransactionMock = clientTransactionMock;

      _domainObjectIDs = StandardConfiguration.Instance.GetDomainObjectIDs();
    }

    // methods and properties

    public DataContainer CreateCustomer1DataContainer ()
    {
      ObjectID id = _domainObjectIDs.Customer1;
      ClassDefinition classDefinition = id.ClassDefinition;
      Dictionary<PropertyDefinition, object> persistentPropertyValues = new Dictionary<PropertyDefinition, object> ();

      // use GetPropertyDefinition because we are setting properties from the base class here
      persistentPropertyValues.Add (classDefinition.GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.Name"), "Kunde 1");
      persistentPropertyValues.Add (classDefinition.GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.CustomerSince"), new DateTime (2000, 1, 1));
      persistentPropertyValues.Add (classDefinition.GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Type"), Customer.CustomerType.Standard);
      persistentPropertyValues.Add (classDefinition.GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.IndustrialSector"), _domainObjectIDs.IndustrialSector1);

      DataContainer dataContainer = CreateExistingDataContainer (id, persistentPropertyValues);
      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }

    public DataContainer CreateClassWithAllDataTypesDataContainer ()
    {
      ObjectID id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      ClassDefinition classDefinition = id.ClassDefinition;
      Dictionary<PropertyDefinition, object> persistentPropertyValues = new Dictionary<PropertyDefinition, object> ();

      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty"], false);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ByteProperty"], (byte) 85);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateProperty"], new DateTime (2005, 1, 1));
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateTimeProperty"], new DateTime (2005, 1, 1, 17, 0, 0));
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DecimalProperty"], (decimal) 123456.789);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DoubleProperty"], 987654.321);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"], ClassWithAllDataTypes.EnumType.Value1);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.FlagsProperty"], ClassWithAllDataTypes.FlagsType.Flag2);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.GuidProperty"], new Guid ("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}"));
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int16Property"], (short) 32767);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"], 2147483647);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int64Property"], (long) 9223372036854775807);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.SingleProperty"], (float) 6789.321);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty"], "abcdef���");
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"], "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890");
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"], ResourceManager.GetImage1 ());

      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"], true);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"], (byte) 78);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"], new DateTime (2005, 2, 1));
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"], new DateTime (2005, 2, 1, 5, 0, 0));
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"], 765.098m);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"], 654321.789d);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"], ClassWithAllDataTypes.EnumType.Value2);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaFlagsProperty"], ClassWithAllDataTypes.FlagsType.Flag1|ClassWithAllDataTypes.FlagsType.Flag2);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"], new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"));
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"], (short) 12000);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"], -2147483647);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"], 3147483647L);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"], 12.456F);

      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanWithNullValueProperty"], null);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteWithNullValueProperty"], null);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateWithNullValueProperty"], null);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeWithNullValueProperty"], null);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalWithNullValueProperty"], null);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleWithNullValueProperty"], null);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumWithNullValueProperty"], null);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaFlagsWithNullValueProperty"], null);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidWithNullValueProperty"], null);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16WithNullValueProperty"], null);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32WithNullValueProperty"], null);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64WithNullValueProperty"], null);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleWithNullValueProperty"], null);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"], null);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"], null);

      DataContainer dataContainer = CreateExistingDataContainer (id, persistentPropertyValues);
      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }

    public DataContainer CreatePartner1DataContainer ()
    {
      ObjectID id = _domainObjectIDs.Partner1;

      ClassDefinition classDefinition = id.ClassDefinition;
      Dictionary<PropertyDefinition, object> persistentPropertyValues = new Dictionary<PropertyDefinition, object> ();

      // use GetPropertyDefinition because we are setting properties from the base class here
      persistentPropertyValues.Add (classDefinition.GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.Name"), "Partner 1");
      persistentPropertyValues.Add (classDefinition.GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson"), _domainObjectIDs.Person1);
      persistentPropertyValues.Add (classDefinition.GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.IndustrialSector"), _domainObjectIDs.IndustrialSector1);

      DataContainer dataContainer = CreateExistingDataContainer (id, persistentPropertyValues);
      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }

    public DataContainer CreateDistributor2DataContainer ()
    {
      ObjectID id = _domainObjectIDs.Distributor2;

      ClassDefinition classDefinition = id.ClassDefinition;
      Dictionary<PropertyDefinition, object> persistentPropertyValues = new Dictionary<PropertyDefinition, object> ();

      // use GetPropertyDefinition because we are setting properties from the base class here
      persistentPropertyValues.Add (classDefinition.GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.Name"), "H�ndler 2");
      persistentPropertyValues.Add (classDefinition.GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson"), _domainObjectIDs.Person6);
      persistentPropertyValues.Add (classDefinition.GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.IndustrialSector"), _domainObjectIDs.IndustrialSector1);
      persistentPropertyValues.Add (classDefinition.GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Distributor.NumberOfShops"), 10);

      DataContainer dataContainer = CreateExistingDataContainer (id, persistentPropertyValues);
      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }

    public DataContainer CreateOrder1DataContainer ()
    {
      ObjectID id = _domainObjectIDs.Order1;

      ClassDefinition classDefinition = id.ClassDefinition;
      Dictionary<PropertyDefinition, object> persistentPropertyValues = new Dictionary<PropertyDefinition, object> ();


      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"], 1);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.DeliveryDate"], new DateTime (2005, 1, 1));
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official"], _domainObjectIDs.Official1);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"], _domainObjectIDs.Customer1);

      DataContainer dataContainer = CreateExistingDataContainer (id, persistentPropertyValues);
      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }

    public DataContainer CreateOrder2DataContainer ()
    {
      ObjectID id = _domainObjectIDs.Order2;

      ClassDefinition classDefinition = id.ClassDefinition;
      Dictionary<PropertyDefinition, object> persistentPropertyValues = new Dictionary<PropertyDefinition, object> ();
      
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"], 3);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.DeliveryDate"], new DateTime (2005, 3, 1));
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official"], _domainObjectIDs.Official1);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"], _domainObjectIDs.Customer3);

      DataContainer dataContainer = CreateExistingDataContainer (id, persistentPropertyValues);
      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }

    public DataContainer CreateOrderWithoutOrderItemDataContainer ()
    {
      ObjectID id = _domainObjectIDs.OrderWithoutOrderItem;

      ClassDefinition classDefinition = id.ClassDefinition;
      Dictionary<PropertyDefinition, object> persistentPropertyValues = new Dictionary<PropertyDefinition, object> ();
      
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"], 2);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.DeliveryDate"], new DateTime (2005, 2, 1));
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official"], _domainObjectIDs.Official1);
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"], _domainObjectIDs.Customer1);

      DataContainer dataContainer = CreateExistingDataContainer (id, persistentPropertyValues);
      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }

    public DataContainer CreateOrderTicket1DataContainer ()
    {
      ObjectID id = _domainObjectIDs.OrderTicket1;

      ClassDefinition classDefinition = id.ClassDefinition;
      Dictionary<PropertyDefinition, object> persistentPropertyValues = new Dictionary<PropertyDefinition, object> ();

      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.FileName"], @"C:\order1.png");
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order"], _domainObjectIDs.Order1);

      DataContainer dataContainer = CreateExistingDataContainer (id, persistentPropertyValues);
      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }

    public DataContainer CreateOrderTicket2DataContainer ()
    {
      ObjectID id = _domainObjectIDs.OrderTicket2;

      ClassDefinition classDefinition = id.ClassDefinition;
      Dictionary<PropertyDefinition, object> persistentPropertyValues = new Dictionary<PropertyDefinition, object> ();

      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.FileName"], @"C:\order2.png");
      persistentPropertyValues.Add (classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order"], _domainObjectIDs.OrderWithoutOrderItem);

      DataContainer dataContainer = CreateExistingDataContainer (id, persistentPropertyValues);
      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }

    public DataContainer CreateClassWithGuidKeyDataContainer ()
    {
      ObjectID id = new ObjectID ("ClassWithGuidKey", new Guid ("{7D1F5F2E-D111-433b-A675-300B55DC4756}"));

      Dictionary<PropertyDefinition, object> persistentPropertyValues = new Dictionary<PropertyDefinition, object> ();

      DataContainer dataContainer = CreateExistingDataContainer (id, persistentPropertyValues);
      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }

    private DataContainer CreateExistingDataContainer (ObjectID id, Dictionary<PropertyDefinition, object> persistentPropertyValues)
    {
      return DataContainer.CreateForExisting (id, null, delegate (PropertyDefinition propertyDefinition)
      {
        return persistentPropertyValues.ContainsKey (propertyDefinition) ? persistentPropertyValues[propertyDefinition] : propertyDefinition.DefaultValue;
      });
    }
  }
}