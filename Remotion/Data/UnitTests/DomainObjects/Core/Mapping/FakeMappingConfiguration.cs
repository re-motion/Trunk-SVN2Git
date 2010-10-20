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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  public class FakeMappingConfiguration
  {
    // types

    // static members and constants

    private static readonly DoubleCheckedLockingContainer<FakeMappingConfiguration> s_current
        = new DoubleCheckedLockingContainer<FakeMappingConfiguration> (() => new FakeMappingConfiguration());

    public static FakeMappingConfiguration Current
    {
      get { return s_current.Value; }
    }

    public static void Reset ()
    {
      s_current.Value = null;
    }

    // member fields

    private ClassDefinitionCollection _classDefinitions;
    private RelationDefinitionCollection _relationDefinitions;

    // construction and disposing

    private FakeMappingConfiguration ()
    {
      _classDefinitions = CreateClassDefinitions();
      _relationDefinitions = CreateRelationDefinitions();

      foreach (ClassDefinition classDefinition in _classDefinitions)
        classDefinition.SetReadOnly();
    }

    // methods and properties

    public ClassDefinitionCollection ClassDefinitions
    {
      get { return _classDefinitions; }
    }

    public RelationDefinitionCollection RelationDefinitions
    {
      get { return _relationDefinitions; }
    }

    #region Methods for creating class definitions

    private ClassDefinitionCollection CreateClassDefinitions ()
    {
      ClassDefinitionCollection classDefinitions = new ClassDefinitionCollection();

      ReflectionBasedClassDefinition testDomainBase = CreateTestDomainBaseDefinition();
      //classDefinitions.Add (testDomainBase);

      ReflectionBasedClassDefinition storageProviderStubDomainBase = CreateStorageProviderStubDomainBaseDefinition();
      //classDefinitions.Add (storageProviderStubDomainBase);

      ReflectionBasedClassDefinition company = CreateCompanyDefinition (null);
      classDefinitions.Add (company);

      ReflectionBasedClassDefinition customer = CreateCustomerDefinition (company);
      classDefinitions.Add (customer);

      ReflectionBasedClassDefinition partner = CreatePartnerDefinition (company);
      classDefinitions.Add (partner);

      ReflectionBasedClassDefinition supplier = CreateSupplierDefinition (partner);
      classDefinitions.Add (supplier);

      ReflectionBasedClassDefinition distributor = CreateDistributorDefinition (partner);
      classDefinitions.Add (distributor);

      classDefinitions.Add (CreateOrderDefinition (null));
      classDefinitions.Add (CreateOrderTicketDefinition (null));
      classDefinitions.Add (CreateOrderItemDefinition (null));

      ReflectionBasedClassDefinition officialDefinition = CreateOfficialDefinition (null);
      classDefinitions.Add (officialDefinition);
      classDefinitions.Add (CreateSpecialOfficialDefinition (officialDefinition));

      classDefinitions.Add (CreateCeoDefinition (null));
      classDefinitions.Add (CreatePersonDefinition (null));

      classDefinitions.Add (CreateClientDefinition (null));
      classDefinitions.Add (CreateLocationDefinition (null));

      ReflectionBasedClassDefinition fileSystemItemDefinition = CreateFileSystemItemDefinition (null);
      classDefinitions.Add (fileSystemItemDefinition);
      classDefinitions.Add (CreateFolderDefinition (fileSystemItemDefinition));
      classDefinitions.Add (CreateFileDefinition (fileSystemItemDefinition));

      classDefinitions.Add (CreateClassWithoutRelatedClassIDColumnAndDerivationDefinition (null));
      classDefinitions.Add (CreateClassWithOptionalOneToOneRelationAndOppositeDerivedClassDefinition (null));
      classDefinitions.Add (CreateClassWithoutRelatedClassIDColumnDefinition (null));
      classDefinitions.Add (CreateClassWithAllDataTypesDefinition (null));
      classDefinitions.Add (CreateClassWithGuidKeyDefinition (null));
      classDefinitions.Add (CreateClassWithInvalidKeyTypeDefinition (null));
      classDefinitions.Add (CreateClassWithoutIDColumnDefinition (null));
      classDefinitions.Add (CreateClassWithoutClassIDColumnDefinition (null));
      classDefinitions.Add (CreateClassWithoutTimestampColumnDefinition (null));
      classDefinitions.Add (CreateClassWithValidRelationsDefinition (null));
      classDefinitions.Add (CreateClassWithInvalidRelationDefinition (null));
      classDefinitions.Add (CreateIndustrialSectorDefinition (null));
      classDefinitions.Add (CreateEmployeeDefinition (null));
      classDefinitions.Add (CreateComputerDefinition (null));
      classDefinitions.Add (CreateClassWithRelatedClassIDColumnAndNoInheritanceDefinition (null));

      var targetClassForPersistentMixinDefinition = CreateTargetClassForPersistentMixinDefinition (null);
      classDefinitions.Add (targetClassForPersistentMixinDefinition);
      var derivedTargetClassForPersistentMixinDefinition = CreateDerivedTargetClassForPersistentMixinDefinition (targetClassForPersistentMixinDefinition);
      classDefinitions.Add (derivedTargetClassForPersistentMixinDefinition);
      var derivedDerivedTargetClassForPersistentMixinDefinition = CreateDerivedDerivedTargetClassForPersistentMixinDefinition (derivedTargetClassForPersistentMixinDefinition);
      classDefinitions.Add (derivedDerivedTargetClassForPersistentMixinDefinition);
      var relationTargetForPersistentMixinDefinition = CreateRelationTargetForPersistentMixinDefinition (null);
      classDefinitions.Add (relationTargetForPersistentMixinDefinition);

      return classDefinitions;
    }

    private ReflectionBasedClassDefinition CreateTestDomainBaseDefinition ()
    {
      ReflectionBasedClassDefinition testDomainBase = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "TestDomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (TestDomainBase), true);

      return testDomainBase;
    }

    private ReflectionBasedClassDefinition CreateStorageProviderStubDomainBaseDefinition ()
    {
      ReflectionBasedClassDefinition storageProviderStubDomainBase =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
              "StorageProviderStubDomainBase", null, DatabaseTest.c_unitTestStorageProviderStubID, typeof (StorageProviderStubDomainBase), true);

      return storageProviderStubDomainBase;
    }

    private ReflectionBasedClassDefinition CreateCompanyDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition company = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Company", "Company", DatabaseTest.c_testDomainProviderID, typeof (Company), false, baseClass);

      company.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              company, typeof (Company), "Name", "Name", typeof (string), false, 100));
      company.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              company, typeof (Company), "IndustrialSector", "IndustrialSectorID", typeof (ObjectID), true));

      return company;
    }

    private ReflectionBasedClassDefinition CreateCustomerDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition customer = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Customer", null, DatabaseTest.c_testDomainProviderID, typeof (Customer), false, baseClass);

      customer.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              customer, typeof (Customer), "CustomerSince", "CustomerSince", typeof (DateTime?)));

      customer.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              customer, typeof (Customer), "Type", "CustomerType", typeof (Customer.CustomerType)));

      return customer;
    }

    private ReflectionBasedClassDefinition CreatePartnerDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition partner = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Partner", null, DatabaseTest.c_testDomainProviderID, typeof (Partner), false, baseClass);

      partner.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              partner, typeof (Partner), "ContactPerson", "ContactPersonID", typeof (ObjectID), true));

      return partner;
    }

    private ReflectionBasedClassDefinition CreateSupplierDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition supplier = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Supplier", null, DatabaseTest.c_testDomainProviderID, typeof (Supplier), false, baseClass);

      supplier.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              supplier, typeof (Supplier), "SupplierQuality", "SupplierQuality", typeof (int)));

      return supplier;
    }

    private ReflectionBasedClassDefinition CreateDistributorDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition distributor = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Distributor", null, DatabaseTest.c_testDomainProviderID, typeof (Distributor), false, baseClass);

      distributor.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              distributor, typeof (Distributor), "NumberOfShops", "NumberOfShops", typeof (int)));

      return distributor;
    }

    private ReflectionBasedClassDefinition CreateOrderDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition order = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "Order", DatabaseTest.c_testDomainProviderID, typeof (Order), false, baseClass);

      order.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              order, typeof (Order), "OrderNumber", "OrderNo", typeof (int)));
      order.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              order, typeof (Order), "DeliveryDate", "DeliveryDate", typeof (DateTime)));
      order.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              order, typeof (Order), "Customer", "CustomerID", typeof (ObjectID), true));
      order.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              order, typeof (Order), "Official", "OfficialID", typeof (ObjectID), true));

      return order;
    }

    private ReflectionBasedClassDefinition CreateOfficialDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition official = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Official", "Official", DatabaseTest.c_unitTestStorageProviderStubID, typeof (Official), false, baseClass);

      official.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              official, typeof (Official), "Name", "Name", typeof (string), false, 100));

      return official;
    }

    private ReflectionBasedClassDefinition CreateSpecialOfficialDefinition (ReflectionBasedClassDefinition officialDefinition)
    {
      return ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "SpecialOfficial", null, DatabaseTest.c_unitTestStorageProviderStubID, typeof (SpecialOfficial), false, officialDefinition);
    }

    private ReflectionBasedClassDefinition CreateOrderTicketDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition orderTicket = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "OrderTicket", "OrderTicket", DatabaseTest.c_testDomainProviderID, typeof (OrderTicket), false, baseClass);

      orderTicket.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              orderTicket, typeof (OrderTicket), "FileName", "FileName", typeof (string), false, 255));
      orderTicket.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              orderTicket, typeof (OrderTicket), "Order", "OrderID", typeof (ObjectID), true));
      orderTicket.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              orderTicket,
              typeof (OrderTicket), "Int32TransactionProperty",
              null,
              typeof (int),
              null,
              null,
              StorageClass.Transaction));

      return orderTicket;
    }

    private ReflectionBasedClassDefinition CreateOrderItemDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition orderItem = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "OrderItem", "OrderItem", DatabaseTest.c_testDomainProviderID, typeof (OrderItem), false, baseClass);

      orderItem.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              orderItem, typeof (OrderItem), "Order", "OrderID", typeof (ObjectID), true));
      orderItem.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              orderItem, typeof (OrderItem), "Position", "Position", typeof (int)));
      orderItem.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              orderItem, typeof (OrderItem), "Product", "Product", typeof (string), false, 100));

      return orderItem;
    }

    private ReflectionBasedClassDefinition CreateCeoDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition ceo = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Ceo", "Ceo", DatabaseTest.c_testDomainProviderID, typeof (Ceo), false, baseClass);

      ceo.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              ceo, typeof (Ceo), "Name", "Name", typeof (string), false, 100));
      ceo.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              ceo, typeof (Ceo), "Company", "CompanyID", typeof (ObjectID), true));

      return ceo;
    }

    private ReflectionBasedClassDefinition CreatePersonDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition person = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Person", "Person", DatabaseTest.c_testDomainProviderID, typeof (Person), false, baseClass);

      person.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              person, typeof (Person), "Name", "Name", typeof (string), false, 100));

      return person;
    }

    private ReflectionBasedClassDefinition CreateClientDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition client = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Client", "Client", DatabaseTest.c_testDomainProviderID, typeof (Client), false, baseClass);

      client.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              client, typeof (Client), "ParentClient", "ParentClientID", typeof (ObjectID), true));

      return client;
    }

    private ReflectionBasedClassDefinition CreateLocationDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition location = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Location", "Location", DatabaseTest.c_testDomainProviderID, typeof (Location), false, baseClass);

      location.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              location, typeof (Location), "Client", "ClientID", typeof (ObjectID), true));

      return location;
    }

    private ReflectionBasedClassDefinition CreateFileSystemItemDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition fileSystemItem = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "FileSystemItem", "FileSystemItem", DatabaseTest.c_testDomainProviderID, typeof (FileSystemItem), false, baseClass);

      fileSystemItem.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              fileSystemItem,
              typeof (FileSystemItem), "ParentFolder",
              "ParentFolderID",
              typeof (ObjectID),
              true));

      return fileSystemItem;
    }

    private ReflectionBasedClassDefinition CreateFolderDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition folder = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Folder", null, DatabaseTest.c_testDomainProviderID, typeof (Folder), false, baseClass);

      return folder;
    }

    private ReflectionBasedClassDefinition CreateFileDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition file = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "File", null, DatabaseTest.c_testDomainProviderID, typeof (File), false, baseClass);

      return file;
    }

    //TODO: remove Date and NaDate properties
    private ReflectionBasedClassDefinition CreateClassWithAllDataTypesDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classWithAllDataTypes = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassWithAllDataTypes", "TableWithAllDataTypes", DatabaseTest.c_testDomainProviderID, typeof (ClassWithAllDataTypes), false, baseClass);

      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "BooleanProperty",
              "Boolean",
              typeof (bool)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes, typeof (ClassWithAllDataTypes), "ByteProperty", "Byte", typeof (byte)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes, typeof (ClassWithAllDataTypes), "DateProperty", "Date", typeof (DateTime)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "DateTimeProperty",
              "DateTime",
              typeof (DateTime)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "DecimalProperty",
              "Decimal",
              typeof (Decimal)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "DoubleProperty",
              "Double",
              typeof (double)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "EnumProperty",
              "Enum",
              typeof (ClassWithAllDataTypes.EnumType)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "ExtensibleEnumProperty",
              "ExtensibleEnum",
              typeof (Color),
              false));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "FlagsProperty",
              "Flags",
              typeof (ClassWithAllDataTypes.FlagsType)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes, typeof (ClassWithAllDataTypes), "GuidProperty", "Guid", typeof (Guid)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int16Property", "Int16", typeof (short)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int32Property", "Int32", typeof (int)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int64Property", "Int64", typeof (long)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes, typeof (ClassWithAllDataTypes), "SingleProperty", "Single", typeof (float)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "StringProperty",
              "String",
              typeof (string),
              false,
              100));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "StringPropertyWithoutMaxLength",
              "StringWithoutMaxLength",
              typeof (string),
              false));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "BinaryProperty",
              "Binary",
              typeof (byte[]),
              false));

      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaBooleanProperty",
              "NaBoolean",
              typeof (bool?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaByteProperty", "NaByte", typeof (byte?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaDateProperty",
              "NaDate",
              typeof (DateTime?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaDateTimeProperty",
              "NaDateTime",
              typeof (DateTime?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaDecimalProperty",
              "NaDecimal",
              typeof (Decimal?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaDoubleProperty",
              "NaDouble",
              typeof (double?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaEnumProperty",
              "NaEnum",
              typeof (ClassWithAllDataTypes.EnumType?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaFlagsProperty",
              "NaFlags",
              typeof (ClassWithAllDataTypes.FlagsType?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaGuidProperty", "NaGuid", typeof (Guid?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaInt16Property",
              "NaInt16",
              typeof (short?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaInt32Property",
              "NaInt32",
              typeof (int?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaInt64Property",
              "NaInt64",
              typeof (long?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaSingleProperty",
              "NaSingle",
              typeof (float?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "StringWithNullValueProperty",
              "StringWithNullValue",
              typeof (string),
              true,
              100));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "ExtensibleEnumWithNullValueProperty",
              "ExtensibleEnumWithNullValue",
              typeof (Color),
              true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaBooleanWithNullValueProperty",
              "NaBooleanWithNullValue",
              typeof (bool?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaByteWithNullValueProperty",
              "NaByteWithNullValue",
              typeof (byte?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaDateWithNullValueProperty",
              "NaDateWithNullValue",
              typeof (DateTime?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaDateTimeWithNullValueProperty",
              "NaDateTimeWithNullValue",
              typeof (DateTime?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaDecimalWithNullValueProperty",
              "NaDecimalWithNullValue",
              typeof (Decimal?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaDoubleWithNullValueProperty",
              "NaDoubleWithNullValue",
              typeof (double?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaEnumWithNullValueProperty",
              "NaEnumWithNullValue",
              typeof (ClassWithAllDataTypes.EnumType?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaFlagsWithNullValueProperty",
              "NaFlagsWithNullValue",
              typeof (ClassWithAllDataTypes.FlagsType?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaGuidWithNullValueProperty",
              "NaGuidWithNullValue",
              typeof (Guid?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaInt16WithNullValueProperty",
              "NaInt16WithNullValue",
              typeof (short?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaInt32WithNullValueProperty",
              "NaInt32WithNullValue",
              typeof (int?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaInt64WithNullValueProperty",
              "NaInt64WithNullValue",
              typeof (long?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaSingleWithNullValueProperty",
              "NaSingleWithNullValue",
              typeof (float?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NullableBinaryProperty",
              "NullableBinary",
              typeof (byte[]),
              true,
              1000000));

      return classWithAllDataTypes;
    }

    private ReflectionBasedClassDefinition CreateClassWithGuidKeyDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassWithGuidKey",
          "TableWithGuidKey",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithGuidKey),
          false,
          baseClass);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithInvalidKeyTypeDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassWithKeyOfInvalidType",
          "TableWithKeyOfInvalidType",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithKeyOfInvalidType),
          false,
          baseClass);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithoutIDColumnDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassWithoutIDColumn",
          "TableWithoutIDColumn",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithoutIDColumn),
          false,
          baseClass);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithoutClassIDColumnDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassWithoutClassIDColumn",
          "TableWithoutClassIDColumn",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithoutClassIDColumn),
          false,
          baseClass);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithoutTimestampColumnDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
              "ClassWithoutTimestampColumn",
              "TableWithoutTimestampColumn",
              DatabaseTest.c_testDomainProviderID,
              typeof (ClassWithoutTimestampColumn),
              false,
              baseClass);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithValidRelationsDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassWithValidRelations",
          "TableWithValidRelations",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithValidRelations),
          false,
          baseClass);

      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classDefinition,
              typeof (ClassWithValidRelations), "ClassWithGuidKeyOptional",
              "TableWithGuidKeyOptionalID",
              typeof (ObjectID),
              true));

      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classDefinition,
              typeof (ClassWithValidRelations), "ClassWithGuidKeyNonOptional",
              "TableWithGuidKeyNonOptionalID",
              typeof (ObjectID),
              true));

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithInvalidRelationDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassWithInvalidRelation",
          "TableWithInvalidRelation",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithInvalidRelation),
          false,
          baseClass);

      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classDefinition,
              typeof (ClassWithInvalidRelation), "ClassWithGuidKey",
              "TableWithGuidKeyID",
              typeof (ObjectID),
              true));

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithoutRelatedClassIDColumnDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassWithoutRelatedClassIDColumn",
          "TableWithoutRelatedClassIDColumn",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithoutRelatedClassIDColumn),
          false,
          baseClass);

      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classDefinition,
              typeof (ClassWithoutRelatedClassIDColumn), "Distributor",
              "DistributorID",
              typeof (ObjectID),
              true));

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithoutRelatedClassIDColumnAndDerivationDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
              "ClassWithOptionalOneToOneRelationAndOppositeDerivedClass",
              "TableWithOptionalOneToOneRelationAndOppositeDerivedClass",
              DatabaseTest.c_testDomainProviderID,
              typeof (ClassWithOptionalOneToOneRelationAndOppositeDerivedClass),
              false,
              baseClass);

      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classDefinition,
              typeof (ClassWithOptionalOneToOneRelationAndOppositeDerivedClass), "Company",
              "CompanyID",
              typeof (ObjectID),
              true));

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithOptionalOneToOneRelationAndOppositeDerivedClassDefinition (
        ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
              "ClassWithoutRelatedClassIDColumnAndDerivation",
              "TableWithoutRelatedClassIDColumnAndDerivation",
              DatabaseTest.c_testDomainProviderID,
              typeof (ClassWithoutRelatedClassIDColumnAndDerivation),
              false,
              baseClass);

      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classDefinition,
              typeof (ClassWithoutRelatedClassIDColumnAndDerivation), "Company",
              "CompanyID",
              typeof (ObjectID),
              true));

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateIndustrialSectorDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition industrialSector = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "IndustrialSector", "IndustrialSector", DatabaseTest.c_testDomainProviderID, typeof (IndustrialSector), false, baseClass);

      industrialSector.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              industrialSector, typeof (IndustrialSector), "Name", "Name", typeof (string), false, 100));

      return industrialSector;
    }

    private ReflectionBasedClassDefinition CreateEmployeeDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition employee = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Employee", "Employee", DatabaseTest.c_testDomainProviderID, typeof (Employee), false, baseClass);

      employee.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              employee, typeof (Employee), "Name", "Name", typeof (string), false, 100));
      employee.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              employee, typeof (Employee), "Supervisor", "SupervisorID", typeof (ObjectID), true));

      return employee;
    }

    private ReflectionBasedClassDefinition CreateTargetClassForPersistentMixinDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition targetClassForPersistentMixin = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "TargetClassForPersistentMixin",
          "MixedDomains_Target",
          DatabaseTest.c_testDomainProviderID,
          typeof (TargetClassForPersistentMixin),
          false,
          baseClass,
          typeof (MixinAddingPersistentProperties));

      targetClassForPersistentMixin.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              targetClassForPersistentMixin,
              typeof (MixinAddingPersistentProperties), "PersistentProperty",
              "PersistentProperty",
              typeof (int)));
      targetClassForPersistentMixin.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              targetClassForPersistentMixin,
              typeof (MixinAddingPersistentProperties), "ExtraPersistentProperty",
              "ExtraPersistentProperty",
              typeof (int)));
      targetClassForPersistentMixin.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              targetClassForPersistentMixin,
              typeof (MixinAddingPersistentProperties), "UnidirectionalRelationProperty",
              "UnidirectionalRelationPropertyID",
              typeof (ObjectID)));
      targetClassForPersistentMixin.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              targetClassForPersistentMixin,
              typeof (MixinAddingPersistentProperties), "RelationProperty",
              "RelationPropertyID",
              typeof (ObjectID)));
      targetClassForPersistentMixin.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              targetClassForPersistentMixin,
              typeof (MixinAddingPersistentProperties), "CollectionPropertyNSide",
              "CollectionPropertyNSideID",
              typeof (ObjectID)));
      targetClassForPersistentMixin.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              targetClassForPersistentMixin,
              typeof (BaseForMixinAddingPersistentProperties), "PrivateBaseRelationProperty",
              "PrivateBaseRelationPropertyID",
              typeof (ObjectID)));

      return targetClassForPersistentMixin;
    }

    private ReflectionBasedClassDefinition CreateDerivedTargetClassForPersistentMixinDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition derivedTargetClassForPersistentMixin = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "DerivedTargetClassForPersistentMixin",
          null,
          DatabaseTest.c_testDomainProviderID,
          typeof (DerivedTargetClassForPersistentMixin),
          false,
          baseClass);

      return derivedTargetClassForPersistentMixin;
    }

    private ReflectionBasedClassDefinition CreateDerivedDerivedTargetClassForPersistentMixinDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition derivedDerivedTargetClassForPersistentMixin = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "DerivedDerivedTargetClassForPersistentMixin",
          null,
          DatabaseTest.c_testDomainProviderID,
          typeof (DerivedDerivedTargetClassForPersistentMixin),
          false,
          baseClass);

      return derivedDerivedTargetClassForPersistentMixin;
    }

    private ReflectionBasedClassDefinition CreateComputerDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition computer = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Computer", "Computer", DatabaseTest.c_testDomainProviderID, typeof (Computer), false, baseClass);

      computer.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              computer, typeof (Computer), "SerialNumber", "SerialNumber", typeof (string), false, 20));
      computer.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              computer, typeof (Computer), "Employee", "EmployeeID", typeof (ObjectID), true));
      computer.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              computer,
              typeof (Computer), "Int32TransactionProperty",
              null,
              typeof (int),
              null,
              null,
              StorageClass.Transaction));
      computer.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              computer,
              typeof (Computer), "DateTimeTransactionProperty",
              null,
              typeof (DateTime),
              null,
              null,
              StorageClass.Transaction));
      computer.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              computer,
              typeof (Computer), "EmployeeTransactionProperty",
              null,
              typeof (ObjectID),
              true,
              null,
              StorageClass.Transaction));

      return computer;
    }

    private ReflectionBasedClassDefinition CreateClassWithRelatedClassIDColumnAndNoInheritanceDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
              "ClassWithRelatedClassIDColumnAndNoInheritance",
              "TableWithRelatedClassIDColumnAndNoInheritance",
              DatabaseTest.c_testDomainProviderID,
              typeof (ClassWithRelatedClassIDColumnAndNoInheritance),
              false,
              baseClass);

      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classDefinition,
              typeof (ClassWithRelatedClassIDColumnAndNoInheritance), "ClassWithGuidKey",
              "TableWithGuidKeyID",
              typeof (ObjectID),
              true));

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateRelationTargetForPersistentMixinDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition relationTargetForPersistentMixinDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "RelationTargetForPersistentMixin",
          "MixedDomains_RelationTarget",
          DatabaseTest.c_testDomainProviderID,
          typeof (RelationTargetForPersistentMixin),
          false,
          baseClass);

      relationTargetForPersistentMixinDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              relationTargetForPersistentMixinDefinition,
              typeof (RelationTargetForPersistentMixin), "RelationProperty2",
              "RelationProperty2ID",
              typeof (ObjectID),
              true));
      relationTargetForPersistentMixinDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              relationTargetForPersistentMixinDefinition,
              typeof (RelationTargetForPersistentMixin), "RelationProperty3",
              "RelationProperty3ID",
              typeof (ObjectID),
              true));

      return relationTargetForPersistentMixinDefinition;
    }

    #endregion

    #region Methods for creating relation definitions

    private RelationDefinitionCollection CreateRelationDefinitions ()
    {
      RelationDefinitionCollection relationDefinitions = new RelationDefinitionCollection();

      relationDefinitions.Add (CreateCustomerToOrderRelationDefinition());
      relationDefinitions.Add (CreateOrderToOrderItemRelationDefinition());
      relationDefinitions.Add (CreateOrderToOrderTicketRelationDefinition());
      relationDefinitions.Add (CreateOrderToOfficialRelationDefinition());
      relationDefinitions.Add (CreateCompanyToCeoRelationDefinition());
      relationDefinitions.Add (CreatePartnerToPersonRelationDefinition());
      relationDefinitions.Add (CreateClientToLocationRelationDefinition());
      relationDefinitions.Add (CreateParentClientToChildClientRelationDefinition());
      relationDefinitions.Add (CreateFolderToFileSystemItemRelationDefinition());

      relationDefinitions.Add (CreateCompanyToClassWithoutRelatedClassIDColumnAndDerivationRelationDefinition());
      relationDefinitions.Add (CreateCompanyToClassWithOptionalOneToOneRelationAndOppositeDerivedClassRelationDefinition());
      relationDefinitions.Add (CreateDistributorToClassWithoutRelatedClassIDColumnRelationDefinition());
      relationDefinitions.Add (CreateClassWithGuidKeyToClassWithValidRelationsOptional());
      relationDefinitions.Add (CreateClassWithGuidKeyToClassWithValidRelationsNonOptional());
      relationDefinitions.Add (CreateClassWithGuidKeyToClassWithInvalidRelation());
      relationDefinitions.Add (CreateClassWithGuidKeyToClassWithRelatedClassIDColumnAndNoInheritanceRelation());
      relationDefinitions.Add (CreateIndustrialSectorToCompanyRelationDefinition());
      relationDefinitions.Add (CreateSupervisorToSubordinateRelationDefinition());
      relationDefinitions.Add (CreateEmployeeToComputerRelationDefinition ());

      relationDefinitions.Add (CreateTargetClassForPersistentMixinMixedUnidirectionalRelationDefinition ());
      relationDefinitions.Add (CreateTargetClassForPersistentMixinMixedRelationPropertyRelationDefinition ());
      relationDefinitions.Add (CreateTargetClassForPersistentMixinMixedVirtualRelationPropertyRelationDefinition());
      relationDefinitions.Add (CreateTargetClassForPersistentMixinMixedCollectionProperty1SideCreateTargetClassForPersistentMixinMixedCollectionPropertyRelationDefinition());
      relationDefinitions.Add (CreateTargetClassForPersistentMixinMixedCollectionPropertyNSideRelationDefinition());

      return relationDefinitions;
    }

    private RelationDefinition CreateCustomerToOrderRelationDefinition ()
    {
      ClassDefinition customer = _classDefinitions[typeof (Customer)];

      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              customer,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer.Orders",
              false,
              CardinalityType.Many,
              typeof (OrderCollection),
              "OrderNo asc");

      ClassDefinition orderClass = _classDefinitions[typeof (Order)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          orderClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer", true);

      RelationDefinition relation = new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer", endPoint1, endPoint2);

      customer.MyRelationDefinitions.Add (relation);
      orderClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateOrderToOrderTicketRelationDefinition ()
    {
      ClassDefinition orderClass = _classDefinitions[typeof (Order)];
      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              orderClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

      ClassDefinition orderTicketClass = _classDefinitions[typeof (OrderTicket)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          orderTicketClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket.Order", true);

      RelationDefinition relation = new RelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket.Order", endPoint1, endPoint2);

      orderClass.MyRelationDefinitions.Add (relation);
      orderTicketClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateOrderToOrderItemRelationDefinition ()
    {
      ClassDefinition orderClass = _classDefinitions[typeof (Order)];
      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              orderClass,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems",
              true,
              CardinalityType.Many,
              typeof (ObjectList<OrderItem>));

      ClassDefinition orderItemClass = _classDefinitions[typeof (OrderItem)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          orderItemClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem.Order", true);

      RelationDefinition relation = new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem.Order", endPoint1, endPoint2);

      orderClass.MyRelationDefinitions.Add (relation);
      orderItemClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateOrderToOfficialRelationDefinition ()
    {
      ClassDefinition officialClass = _classDefinitions[typeof (Official)];

      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              officialClass,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Official.Orders",
              false,
              CardinalityType.Many,
              typeof (ObjectList<Order>));

      ClassDefinition orderClass = _classDefinitions[typeof (Order)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          orderClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Official", true);

      RelationDefinition relation = new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Official", endPoint1, endPoint2);

      officialClass.MyRelationDefinitions.Add (relation);
      orderClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateCompanyToCeoRelationDefinition ()
    {
      ClassDefinition companyClass = _classDefinitions[typeof (Company)];

      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              companyClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.Ceo", true, CardinalityType.One, typeof (Ceo));

      ClassDefinition ceoClass = _classDefinitions[typeof (Ceo)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          ceoClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Ceo.Company", true);

      RelationDefinition relation = new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Ceo.Company", endPoint1, endPoint2);

      companyClass.MyRelationDefinitions.Add (relation);
      ceoClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreatePartnerToPersonRelationDefinition ()
    {
      ClassDefinition partnerClass = _classDefinitions[typeof (Partner)];
      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition (
          partnerClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson", true);

      ClassDefinition personClass = _classDefinitions[typeof (Person)];
      VirtualRelationEndPointDefinition endPoint2 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              personClass,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Person.AssociatedPartnerCompany",
              false,
              CardinalityType.One,
              typeof (Partner));

      RelationDefinition relation =
          new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson", endPoint1, endPoint2);

      personClass.MyRelationDefinitions.Add (relation);
      partnerClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateParentClientToChildClientRelationDefinition ()
    {
      ClassDefinition clientClass = _classDefinitions[typeof (Client)];

      AnonymousRelationEndPointDefinition endPoint1 = new AnonymousRelationEndPointDefinition (clientClass);
      RelationEndPointDefinition endPoint2 =
          new RelationEndPointDefinition (clientClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Client.ParentClient", false);
      RelationDefinition relation =
          new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Client.ParentClient", endPoint1, endPoint2);

      clientClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClientToLocationRelationDefinition ()
    {
      ClassDefinition clientClass = _classDefinitions[typeof (Client)];
      ClassDefinition locationClass = _classDefinitions[typeof (Location)];

      AnonymousRelationEndPointDefinition endPoint1 = new AnonymousRelationEndPointDefinition (clientClass);

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          locationClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Location.Client", true);

      RelationDefinition relation = new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Location.Client", endPoint1, endPoint2);

      locationClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateFolderToFileSystemItemRelationDefinition ()
    {
      ClassDefinition fileSystemItemClass = _classDefinitions[typeof (FileSystemItem)];
      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition (
          fileSystemItemClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder", false);

      ClassDefinition folderClass = _classDefinitions[typeof (Folder)];
      VirtualRelationEndPointDefinition endPoint2 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              folderClass,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems",
              false,
              CardinalityType.Many,
              typeof (ObjectList<FileSystemItem>));

      RelationDefinition relation =
          new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder", endPoint1, endPoint2);

      folderClass.MyRelationDefinitions.Add (relation);
      fileSystemItemClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateDistributorToClassWithoutRelatedClassIDColumnRelationDefinition ()
    {
      ClassDefinition distributorClass = _classDefinitions[typeof (Distributor)];

      ClassDefinition classWithoutRelatedClassIDColumnClass = _classDefinitions[typeof (ClassWithoutRelatedClassIDColumn)];

      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              distributorClass,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Distributor.ClassWithoutRelatedClassIDColumn",
              false,
              CardinalityType.One,
              typeof (ClassWithoutRelatedClassIDColumn));

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithoutRelatedClassIDColumnClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithoutRelatedClassIDColumn.Distributor",
          false);

      RelationDefinition relation =
          new RelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithoutRelatedClassIDColumn.Distributor", endPoint1, endPoint2);

      distributorClass.MyRelationDefinitions.Add (relation);
      classWithoutRelatedClassIDColumnClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateCompanyToClassWithoutRelatedClassIDColumnAndDerivationRelationDefinition ()
    {
      ClassDefinition companyClass = _classDefinitions[typeof (Company)];

      ClassDefinition classWithoutRelatedClassIDColumnAndDerivation = _classDefinitions[typeof (ClassWithoutRelatedClassIDColumnAndDerivation)];

      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              companyClass,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.ClassWithoutRelatedClassIDColumnAndDerivation",
              false,
              CardinalityType.One,
              typeof (ClassWithoutRelatedClassIDColumnAndDerivation));

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithoutRelatedClassIDColumnAndDerivation,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithoutRelatedClassIDColumnAndDerivation.Company",
          false);

      RelationDefinition relation =
          new RelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithoutRelatedClassIDColumnAndDerivation.Company", endPoint1, endPoint2);

      companyClass.MyRelationDefinitions.Add (relation);
      classWithoutRelatedClassIDColumnAndDerivation.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateCompanyToClassWithOptionalOneToOneRelationAndOppositeDerivedClassRelationDefinition ()
    {
      ClassDefinition companyClass = _classDefinitions[typeof (Company)];
      ClassDefinition classWithOptionalOneToOneRelationAndOppositeDerivedClass =
          _classDefinitions[typeof (ClassWithOptionalOneToOneRelationAndOppositeDerivedClass)];

      AnonymousRelationEndPointDefinition endPoint1 = new AnonymousRelationEndPointDefinition (companyClass);

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithOptionalOneToOneRelationAndOppositeDerivedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithOptionalOneToOneRelationAndOppositeDerivedClass.Company",
          false);

      RelationDefinition relation =
          new RelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithOptionalOneToOneRelationAndOppositeDerivedClass.Company",
              endPoint1,
              endPoint2);

      classWithOptionalOneToOneRelationAndOppositeDerivedClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithValidRelationsOptional ()
    {
      ClassDefinition classWithGuidKey = _classDefinitions[typeof (ClassWithGuidKey)];
      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              classWithGuidKey,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithValidRelationsOptional",
              false,
              CardinalityType.One,
              typeof (ClassWithValidRelations));

      ClassDefinition classWithValidRelations = _classDefinitions[typeof (ClassWithValidRelations)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithValidRelations, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithValidRelations.ClassWithGuidKeyOptional", false);

      RelationDefinition relation =
          new RelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithValidRelations.ClassWithGuidKeyOptional",
              endPoint1,
              endPoint2);

      classWithGuidKey.MyRelationDefinitions.Add (relation);
      classWithValidRelations.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithValidRelationsNonOptional ()
    {
      ClassDefinition classWithGuidKey = _classDefinitions[typeof (ClassWithGuidKey)];
      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              classWithGuidKey,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithValidRelationsNonOptional",
              true,
              CardinalityType.One,
              typeof (ClassWithValidRelations));

      ClassDefinition classWithValidRelations = _classDefinitions[typeof (ClassWithValidRelations)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithValidRelations, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithValidRelations.ClassWithGuidKeyNonOptional", true);

      RelationDefinition relation =
          new RelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithValidRelations.ClassWithGuidKeyNonOptional",
              endPoint1,
              endPoint2);

      classWithGuidKey.MyRelationDefinitions.Add (relation);
      classWithValidRelations.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithInvalidRelation ()
    {
      ClassDefinition classWithGuidKey = _classDefinitions[typeof (ClassWithGuidKey)];
      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              classWithGuidKey,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithInvalidRelation",
              false,
              CardinalityType.One,
              typeof (ClassWithInvalidRelation));

      ClassDefinition classWithInvalidRelation = _classDefinitions[typeof (ClassWithInvalidRelation)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithInvalidRelation, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithInvalidRelation.ClassWithGuidKey", false);

      RelationDefinition relation =
          new RelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithInvalidRelation.ClassWithGuidKey",
              endPoint1,
              endPoint2);

      classWithGuidKey.MyRelationDefinitions.Add (relation);
      classWithInvalidRelation.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithRelatedClassIDColumnAndNoInheritanceRelation ()
    {
      ClassDefinition classWithGuidKey = _classDefinitions[typeof (ClassWithGuidKey)];
      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              classWithGuidKey,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithRelatedClassIDColumnAndNoInheritance",
              false,
              CardinalityType.One,
              typeof (ClassWithRelatedClassIDColumnAndNoInheritance));

      ClassDefinition classWithRelatedClassIDColumnAndNoInheritance = _classDefinitions[typeof (ClassWithRelatedClassIDColumnAndNoInheritance)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithRelatedClassIDColumnAndNoInheritance,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithRelatedClassIDColumnAndNoInheritance.ClassWithGuidKey",
          false);

      RelationDefinition relation =
          new RelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithRelatedClassIDColumnAndNoInheritance.ClassWithGuidKey",
              endPoint1,
              endPoint2);

      classWithGuidKey.MyRelationDefinitions.Add (relation);
      classWithRelatedClassIDColumnAndNoInheritance.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateIndustrialSectorToCompanyRelationDefinition ()
    {
      ClassDefinition industrialSectorClass = _classDefinitions[typeof (IndustrialSector)];

      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              industrialSectorClass,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.IndustrialSector.Companies",
              true,
              CardinalityType.Many,
              typeof (ObjectList<Company>));

      ClassDefinition companyClass = _classDefinitions[typeof (Company)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          companyClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.IndustrialSector", false);

      RelationDefinition relation =
          new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.IndustrialSector", endPoint1, endPoint2);

      industrialSectorClass.MyRelationDefinitions.Add (relation);
      companyClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateSupervisorToSubordinateRelationDefinition ()
    {
      ClassDefinition employeeClass = _classDefinitions[typeof (Employee)];

      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              employeeClass,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Employee.Subordinates",
              false,
              CardinalityType.Many,
              typeof (ObjectList<Employee>));

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          employeeClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Employee.Supervisor", false);

      RelationDefinition relation =
          new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Employee.Supervisor", endPoint1, endPoint2);

      employeeClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateEmployeeToComputerRelationDefinition ()
    {
      ClassDefinition employeeClass = _classDefinitions[typeof (Employee)];

      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              employeeClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Employee.Computer", false, CardinalityType.One, typeof (Computer));

      ClassDefinition computerClass = _classDefinitions[typeof (Computer)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          computerClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Computer.Employee", false);

      RelationDefinition relation = new RelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Computer.Employee", endPoint1, endPoint2);

      employeeClass.MyRelationDefinitions.Add (relation);
      computerClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateTargetClassForPersistentMixinMixedUnidirectionalRelationDefinition ()
    {
      ClassDefinition mixedClass = _classDefinitions[typeof (TargetClassForPersistentMixin)];

      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition (
          mixedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.UnidirectionalRelationProperty",
          false);

      ClassDefinition relatedClass = _classDefinitions[typeof (RelationTargetForPersistentMixin)];

      AnonymousRelationEndPointDefinition endPoint2 = new AnonymousRelationEndPointDefinition (relatedClass);

      RelationDefinition relation = new RelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.TargetClassForPersistentMixin->"
          + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.UnidirectionalRelationProperty", 
          endPoint1, 
          endPoint2);

      mixedClass.MyRelationDefinitions.Add (relation);
      relatedClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateTargetClassForPersistentMixinMixedRelationPropertyRelationDefinition ()
    {
      ClassDefinition mixedClass = _classDefinitions[typeof (TargetClassForPersistentMixin)];

      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition (
          mixedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.RelationProperty",
          false);

      ClassDefinition relatedClass = _classDefinitions[typeof (RelationTargetForPersistentMixin)];

      var endPoint2 = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          relatedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty1",
          false,
          CardinalityType.One,
          typeof (TargetClassForPersistentMixin));

      RelationDefinition relation = new RelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.TargetClassForPersistentMixin->"
          + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.RelationProperty",
          endPoint1,
          endPoint2);

      mixedClass.MyRelationDefinitions.Add (relation);
      relatedClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateTargetClassForPersistentMixinMixedVirtualRelationPropertyRelationDefinition ()
    {
      ClassDefinition mixedClass = _classDefinitions[typeof (TargetClassForPersistentMixin)];

      var endPoint1 = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          mixedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.VirtualRelationProperty",
          false,
          CardinalityType.One,
          typeof (RelationTargetForPersistentMixin));

      ClassDefinition relatedClass = _classDefinitions[typeof (RelationTargetForPersistentMixin)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          relatedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty2",
          false);

      RelationDefinition relation = new RelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty2",
          endPoint1,
          endPoint2);

      mixedClass.MyRelationDefinitions.Add (relation);
      relatedClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateTargetClassForPersistentMixinMixedCollectionProperty1SideCreateTargetClassForPersistentMixinMixedCollectionPropertyRelationDefinition ()
    {
      ClassDefinition mixedClass = _classDefinitions[typeof (TargetClassForPersistentMixin)];

      var endPoint1 = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          mixedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.CollectionProperty1Side",
          false,
          CardinalityType.Many,
          typeof (ObjectList<RelationTargetForPersistentMixin>));

      ClassDefinition relatedClass = _classDefinitions[typeof (RelationTargetForPersistentMixin)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          relatedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty3",
          false);

      RelationDefinition relation = new RelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty3",
          endPoint1,
          endPoint2);

      mixedClass.MyRelationDefinitions.Add (relation);
      relatedClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateTargetClassForPersistentMixinMixedCollectionPropertyNSideRelationDefinition ()
    {
      ClassDefinition mixedClass = _classDefinitions[typeof (TargetClassForPersistentMixin)];

      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition (
          mixedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.CollectionPropertyNSide",
          false);

      ClassDefinition relatedClass = _classDefinitions[typeof (RelationTargetForPersistentMixin)];

      var endPoint2 = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          relatedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty4",
          false,
          CardinalityType.Many,
          typeof (ObjectList<TargetClassForPersistentMixin>));

      RelationDefinition relation = new RelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.TargetClassForPersistentMixin->"
          + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.CollectionPropertyNSide",
          endPoint1,
          endPoint2);

      mixedClass.MyRelationDefinitions.Add (relation);
      relatedClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    #endregion
  }
}