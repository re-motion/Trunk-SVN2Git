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

namespace Remotion.Data.UnitTests.DomainObjects.Factories
{
  public sealed class DomainObjectIDs
  {
    #region Employee

    // Supervisor: -
    // Subordinates: Employee4, Employee5
    // Computer: -
    private readonly ObjectID _employee1 = new ObjectID ("Employee", new Guid ("{51ECE39B-F040-45b0-8B72-AD8B45353990}"));

    // Supervisor: -
    // Subordinates: Employee3
    // Computer: -
    private readonly ObjectID _employee2 = new ObjectID ("Employee", new Guid ("{C3B2BBC3-E083-4974-BAC7-9CEE1FB85A5E}"));

    // Supervisor: Employee2
    // Subordinates: -
    // Computer: Computer1
    private readonly ObjectID _employee3 = new ObjectID ("Employee", new Guid ("{3C4F3FC8-0DB2-4c1f-AA00-ADE72E9EDB32}"));

    // Supervisor: Employee1
    // Subordinates: -
    // Computer: Computer2
    private readonly ObjectID _employee4 = new ObjectID ("Employee", new Guid ("{890BF138-7559-40d6-9C7F-436BC1AD4F59}"));

    // Supervisor: Employee1
    // Subordinates: -
    // Computer: Computer3
    private readonly ObjectID _employee5 = new ObjectID ("Employee", new Guid ("{43329F84-D8BB-4988-BFD2-96D4F48EE5DE}"));

    // Supervisor: -
    // Subordinates: Employee7
    // Computer: -
    private readonly ObjectID _employee6 = new ObjectID ("Employee", new Guid ("{3A24D098-EAAD-4dd7-ADA2-932D9B6935F1}"));
    
    // Supervisor: Employee6
    // Subordinates: -
    // Computer: -
    private readonly ObjectID _employee7 = new ObjectID ("Employee", new Guid ("{DBD9EA74-8C97-4411-AC02-9205D1D6D031}"));


    public ObjectID Employee1
    {
      get { return _employee1; }
    }

    public ObjectID Employee2
    {
      get { return _employee2; }
    }

    public ObjectID Employee3
    {
      get { return _employee3; }
    }

    public ObjectID Employee4
    {
      get { return _employee4; }
    }

    public ObjectID Employee5
    {
      get { return _employee5; }
    }

    public ObjectID Employee6
    {
      get { return _employee6; }
    }

    public ObjectID Employee7
    {
      get { return _employee7; }
    }

    #endregion

    #region Computer

    // Employee: Employee3
    private readonly ObjectID _computer1 = new ObjectID ("Computer", new Guid ("{C7C26BF5-871D-48c7-822A-E9B05AAC4E5A}"));

    // Employee: Employee4
    private readonly ObjectID _computer2 = new ObjectID ("Computer", new Guid ("{176A0FF6-296D-4934-BD1A-23CF52C22411}"));

    // Employee: Employee5
    private readonly ObjectID _computer3 = new ObjectID ("Computer", new Guid ("{704CE38C-4A08-4ef2-A6FE-9ED849BA31E5}"));

    // Employee: -
    private readonly ObjectID _computer4 = new ObjectID ("Computer", new Guid ("{D6F50E77-2041-46b8-A840-AAA4D2E1BF5A}"));

    // Employee: -
    private readonly ObjectID _computer5 = new ObjectID ("Computer", new Guid ("{AEAC0C5D-44E0-45cc-B716-103B0A4981A4}"));

    public ObjectID Computer1
    {
      get { return _computer1; }
    }

    public ObjectID Computer2
    {
      get { return _computer2; }
    }

    public ObjectID Computer3
    {
      get { return _computer3; }
    }

    public ObjectID Computer4
    {
      get { return _computer4; }
    }

    public ObjectID Computer5
    {
      get { return _computer5; }
    }

    #endregion

    #region Person

    private readonly ObjectID _person1 = new ObjectID ("Person", new Guid ("{2001BF42-2AA4-4c81-AD8E-73E9145411E9}"));

    private readonly ObjectID _person2 = new ObjectID ("Person", new Guid ("{DC50A962-EC95-4cf6-A4E7-A6608EAA23C8}"));

    private readonly ObjectID _person3 = new ObjectID ("Person", new Guid ("{10F36130-E97B-4078-A535-B79E07F16AB2}"));

    private readonly ObjectID _person4 = new ObjectID ("Person", new Guid ("{45C6730A-DE0B-40d2-9D35-C1E56B8A89D6}"));

    private readonly ObjectID _person5 = new ObjectID ("Person", new Guid ("{70C91528-4DB4-4e6a-B3F8-70C53A728DCC}"));

    private readonly ObjectID _person6 = new ObjectID ("Person", new Guid ("{19C04A28-094F-4d1f-9705-E2FC7107A68F}"));

    private readonly ObjectID _person7 = new ObjectID ("Person", new Guid ("{E4F6F59F-80F7-4e41-A004-1A5BA0F68F78}"));

    private readonly ObjectID _contactPersonInTwoOrganizations = new ObjectID ("Person", new Guid ("{911957D1-483C-4a8b-AA53-FF07464C58F9}"));

    public ObjectID Person1
    {
      get { return _person1; }
    }

    public ObjectID Person2
    {
      get { return _person2; }
    }

    public ObjectID Person3
    {
      get { return _person3; }
    }

    public ObjectID Person4
    {
      get { return _person4; }
    }

    public ObjectID Person5
    {
      get { return _person5; }
    }

    public ObjectID Person6
    {
      get { return _person6; }
    }

    public ObjectID Person7
    {
      get { return _person7; }
    }

    public ObjectID ContactPersonInTwoOrganizations
    {
      get { return _contactPersonInTwoOrganizations; }
    }

    #endregion

    #region IndustrialSector

    // Companies: Customer1, Partner1, PartnerWithoutCeo, Supplier1, Distributor2
    private readonly ObjectID _industrialSector1 = new ObjectID ("IndustrialSector", new Guid ("{3BB7BEE9-2AF9-4a85-998E-618BEBBE5A6B}"));

    // Companies: Company1, Company2, Customer2, Customer3, Partner2, Supplier2, Distributor1
    private readonly ObjectID _industrialSector2 = new ObjectID ("IndustrialSector", new Guid ("{8565A077-EA01-4b5d-BEAA-293DC484BDDC}"));

    // Companies: DistributorWithoutContactPerson
    private readonly ObjectID _industrialSector3 = new ObjectID ("IndustrialSector", new Guid ("{53B322BF-25D8-4fe1-96C8-508E055143E7}"));

    public ObjectID IndustrialSector1
    {
      get { return _industrialSector1; }
    }

    public ObjectID IndustrialSector2
    {
      get { return _industrialSector2; }
    }

    public ObjectID IndustrialSector3
    {
      get { return _industrialSector3; }
    }

    #endregion

    #region Company

    // IndustrialSector: IndustrialSector2
    // Ceo: Ceo1
    private readonly ObjectID _company1 = new ObjectID ("Company", new Guid ("{C4954DA8-8870-45c1-B7A3-C7E5E6AD641A}"));

    // IndustrialSector: IndustrialSector2
    // Ceo: Ceo2
    private readonly ObjectID _company2 = new ObjectID ("Company", new Guid ("{A21A9EC2-17D6-44de-9F1A-2AB6FC3742DF}"));

    public ObjectID Company1
    {
      get { return _company1; }
    }

    public ObjectID Company2
    {
      get { return _company2; }
    }

    #endregion

    #region Customer

    // IndustrialSector: IndustrialSector1
    // Ceo: Ceo3
    // Orders: Order1, OrderWithoutOrderItem
    private readonly ObjectID _customer1 = new ObjectID ("Customer", new Guid ("{55B52E75-514B-4e82-A91B-8F0BB59B80AD}"));

    // IndustrialSector: IndustrialSector2
    // Ceo: Ceo4
    // Orders: -
    private readonly ObjectID _customer2 = new ObjectID ("Customer", new Guid ("{F577F879-2DB4-4a3c-A18A-AFB4E57CE098}"));

    // IndustrialSector: IndustrialSector2
    // Ceo: Ceo5
    // Orders: Order2
    private readonly ObjectID _customer3 = new ObjectID ("Customer", new Guid ("{DD3E3D55-C16F-497f-A3E1-384D08DE0D66}"));
    
    // IndustrialSector: -
    // Ceo: Ceo12
    // Orders: Order3, Order4
    private readonly ObjectID _customer4 = new ObjectID ("Customer", new Guid ("{B3F0A333-EC2A-4ddd-9035-9ADA34052450}"));

    private readonly ObjectID _customer5 = new ObjectID ("Customer", new Guid ("{DA658F26-8107-44CE-9DD0-1804503ECCAF}"));

    public ObjectID Customer1
    {
      get { return _customer1; }
    }

    public ObjectID Customer2
    {
      get { return _customer2; }
    }

    public ObjectID Customer3
    {
      get { return _customer3; }
    }

    public ObjectID Customer4
    {
      get { return _customer4; }
    }

    public ObjectID Customer5
    {
      get { return _customer5; }
    }

    #endregion

    #region Partner

    // IndustrialSector: IndustrialSector1
    // ContactPerson: Person1
    // Ceo: Ceo6
    private readonly ObjectID _partner1 = new ObjectID ("Partner", new Guid ("{5587A9C0-BE53-477d-8C0A-4803C7FAE1A9}"));

    // IndustrialSector: IndustrialSector2
    // ContactPerson: Person2
    // Ceo: Ceo7
    private readonly ObjectID _partner2 = new ObjectID ("Partner", new Guid ("{B403E58E-9FA5-47ed-883C-73420D64DEB3}"));

    // IndustrialSector: IndustrialSector1
    // ContactPerson: Person7
    // Ceo: -
    private readonly ObjectID _partnerWithoutCeo = new ObjectID ("Partner", new Guid ("{A65B123A-6E17-498e-A28E-946217C0AE30}"));

    public ObjectID Partner1
    {
      get { return _partner1; }
    }

    public ObjectID Partner2
    {
      get { return _partner2; }
    }

    public ObjectID PartnerWithoutCeo
    {
      get { return _partnerWithoutCeo; }
    }

    #endregion

    #region Supplier

    // IndustrialSector: IndustrialSector1
    // ContactPerson: Person3
    // Ceo: Ceo8
    private readonly ObjectID _supplier1 = new ObjectID ("Supplier", new Guid ("{FD392135-1FDD-42a3-8E2F-232BAB9893A2}"));

    // IndustrialSector: IndustrialSector2
    // ContactPerson: Person4
    // Ceo: Ceo9
    private readonly ObjectID _supplier2 = new ObjectID ("Supplier", new Guid ("{92A8BB6A-412A-4fe3-9B09-3E1B6136E425}"));

    public ObjectID Supplier1
    {
      get { return _supplier1; }
    }

    public ObjectID Supplier2
    {
      get { return _supplier2; }
    }

    #endregion

    #region Distributor

    // IndustrialSector: IndustrialSector2
    // ContactPerson: Person5
    // Ceo: Ceo10
    private readonly ObjectID _distributor1 = new ObjectID ("Distributor", new Guid ("{E4087155-D60A-4d31-95B3-9A401A3E4E78}"));

    // IndustrialSector: IndustrialSector1
    // ContactPerson: Person6
    // Ceo: Ceo11
    private readonly ObjectID _distributor2 = new ObjectID ("Distributor", new Guid ("{247206C3-7B48-4e17-91DD-3363B568D7E4}"));

    // IndustrialSector: IndustrialSector3
    // ContactPerson: -
    // Ceo: -
    private readonly ObjectID _distributorWithoutContactPersonAndCeo = new ObjectID ("Distributor", new Guid ("{1514D668-A0A5-40e9-AC22-F24900E0EB39}"));

    public ObjectID Distributor1
    {
      get { return _distributor1; }
    }

    public ObjectID Distributor2
    {
      get { return _distributor2; }
    }

    public ObjectID DistributorWithoutContactPersonAndCeo
    {
      get { return _distributorWithoutContactPersonAndCeo; }
    }

    #endregion

    #region Order

    // OrderTicket: OrderTicket1
    // OrderItems: OrderItem1, OrderItem2
    // Customer: Customer1
    // Official: Official1
    // OrderNumber: 1
    private readonly ObjectID _order1 = new ObjectID ("Order", new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}"));

    // OrderTicket: OrderTicket2
    // OrderItems: -
    // Customer: Customer1
    // Official: Official1
    // OrderNumber: 2
    private readonly ObjectID _orderWithoutOrderItem = new ObjectID ("Order", new Guid ("{F4016F41-F4E4-429e-B8D1-659C8C480A67}"));

    // OrderTicket: OrderTicket3
    // OrderItems: OrderItem3
    // Customer: Customer3
    // Official: Official1
    // OrderNumber: 3
    private readonly ObjectID _order2 = new ObjectID ("Order", new Guid ("{83445473-844A-4d3f-A8C3-C27F8D98E8BA}"));

    // OrderTicket: OrderTicket4
    // OrderItems: OrderItem4
    // Customer: Customer4
    // Official: Official1
    // OrderNumber: 4
    private readonly ObjectID _order3 = new ObjectID ("Order", new Guid ("{3C0FB6ED-DE1C-4e70-8D80-218E0BF58DF3}"));

    // OrderTicket: OrderTicket5
    // OrderItems: OrderItem5
    // Customer: Customer4
    // Official: Official1
    // OrderNumber: 5
    private readonly ObjectID _order4 = new ObjectID ("Order", new Guid ("{90E26C86-611F-4735-8D1B-E1D0918515C2}"));

    // OrderTicket: -
    // OrderItems: -
    // Customer: invalid
    // Official: does not exist
    // OrderNumber: 6
    private readonly ObjectID _invalidOrder = new ObjectID ("Order", new Guid ("{DA658F26-8107-44ce-9DD0-1804503ECCAF}"));

    public ObjectID Order1
    {
      get { return _order1; }
    }

    public ObjectID OrderWithoutOrderItem
    {
      get { return _orderWithoutOrderItem; }
    }

    public ObjectID Order2
    {
      get { return _order2; }
    }

    public ObjectID Order3
    {
      get { return _order3; }
    }

    public ObjectID Order4
    {
      get { return _order4; }
    }

    public ObjectID InvalidOrder
    {
      get { return _invalidOrder; }
    }

    #endregion

    #region OrderItem

    // Order: Order1
    // Product: Mainboard
    private readonly ObjectID _orderItem1 = new ObjectID ("OrderItem", new Guid ("{2F4D42C7-7FFA-490d-BFCD-A9101BBF4E1A}"));

    // Order: Order1
    // Product: CPU Fan
    private readonly ObjectID _orderItem2 = new ObjectID ("OrderItem", new Guid ("{AD620A11-4BC4-4791-BCF4-A0770A08C5B0}"));

    // Order: Order2
    // Product: Harddisk
    private readonly ObjectID _orderItem3 = new ObjectID ("OrderItem", new Guid ("{0D7196A5-8161-4048-820D-B1BBDABE3293}"));

    // Order: Order3
    // Product: Hitchhiker's guide
    private readonly ObjectID _orderItem4 = new ObjectID ("OrderItem", new Guid ("{DC20E0EB-4B55-4f23-89CF-6D6478F96D3B}"));

    // Order: Order4
    // Product: Blumentopf
    private readonly ObjectID _orderItem5 = new ObjectID ("OrderItem", new Guid ("{EA505094-770A-4505-82C1-5A4F94F56FE2}"));

    public ObjectID OrderItem1
    {
      get { return _orderItem1; }
    }

    public ObjectID OrderItem2
    {
      get { return _orderItem2; }
    }

    public ObjectID OrderItem3
    {
      get { return _orderItem3; }
    }

    public ObjectID OrderItem4
    {
      get { return _orderItem4; }
    }

    public ObjectID OrderItem5
    {
      get { return _orderItem5; }
    }

    #endregion

    #region OrderWithNewPropertyAccess

    // OrderItems: OrderItemWithNewPropertyAccess1, OrderItemWithNewPropertyAccess2
    // Customer: Customer1
    private readonly ObjectID _orderWithNewPropertyAccess1 = new ObjectID ("OrderWithNewPropertyAccess", new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}"));

    public ObjectID OrderWithNewPropertyAccess1
    {
      get { return _orderWithNewPropertyAccess1; }
    }

    #endregion

    #region OrderItemWithNewPropertyAccess

    // Order: OrderWithNewPropertyAccess1
    private readonly ObjectID _orderItemWithNewPropertyAccess1 = new ObjectID ("OrderItemWithNewPropertyAccess", new Guid ("{2F4D42C7-7FFA-490d-BFCD-A9101BBF4E1A}"));

    // Order: OrderWithNewPropertyAccess1
    private readonly ObjectID _orderItemWithNewPropertyAccess2 = new ObjectID ("OrderItemWithNewPropertyAccess", new Guid ("{AD620A11-4BC4-4791-BCF4-A0770A08C5B0}"));

    public ObjectID OrderItemWithNewPropertyAccess1
    {
      get { return _orderItemWithNewPropertyAccess1; }
    }

    public ObjectID OrderItemWithNewPropertyAccess2
    {
      get { return _orderItemWithNewPropertyAccess2; }
    }

    #endregion

    #region OrderTicket

    // Order: Order1
    private readonly ObjectID _orderTicket1 = new ObjectID ("OrderTicket", new Guid ("{058EF259-F9CD-4cb1-85E5-5C05119AB596}"));

    // Order: OrderWithoutOrderItem
    private readonly ObjectID _orderTicket2 = new ObjectID ("OrderTicket", new Guid ("{0005BDF4-4CCC-4a41-B9B5-BAAB3EB95237}"));

    // Order: Order2
    private readonly ObjectID _orderTicket3 = new ObjectID ("OrderTicket", new Guid ("{BCF6C5F6-323F-4471-9CA5-7DF0A48C7A59}"));

    // Order: Order3
    private readonly ObjectID _orderTicket4 = new ObjectID ("OrderTicket", new Guid ("{6768DB2B-9C66-4e2f-BBA2-89C56718FF2B}"));

    // Order: Order4
    private readonly ObjectID _orderTicket5 = new ObjectID ("OrderTicket", new Guid ("{DC20E0EB-4B55-4f23-89CF-6D6478F96D3B}"));

    public ObjectID OrderTicket1
    {
      get { return _orderTicket1; }
    }

    public ObjectID OrderTicket2
    {
      get { return _orderTicket2; }
    }

    public ObjectID OrderTicket3
    {
      get { return _orderTicket3; }
    }

    public ObjectID OrderTicket4
    {
      get { return _orderTicket4; }
    }

    public ObjectID OrderTicket5
    {
      get { return _orderTicket5; }
    }

    #endregion

    #region Ceo

    // Company: Company1
    private readonly ObjectID _ceo1 = new ObjectID ("Ceo", new Guid ("{A1691AF1-F96D-42e1-B021-B5099840D572}"));

    // Company: Company2
    private readonly ObjectID _ceo2 = new ObjectID ("Ceo", new Guid ("{A6A848CE-505F-4cd3-A337-1F5EEA1D2260}"));

    // Company: Customer1
    private readonly ObjectID _ceo3 = new ObjectID ("Ceo", new Guid ("{481C7840-9D8A-4872-BBCD-B41A9BD85528}"));

    // Company: Customer2
    private readonly ObjectID _ceo4 = new ObjectID ("Ceo", new Guid ("{BE7F24E2-600C-4cd8-A7C3-8669AFD54154}"));

    // Company: Customer3
    private readonly ObjectID _ceo5 = new ObjectID ("Ceo", new Guid ("{7236BA88-48C6-415f-A0BA-A328A1A22DFE}"));

    // Company: Partner1
    private readonly ObjectID _ceo6 = new ObjectID ("Ceo", new Guid ("{C7837D11-C1D6-458f-A3F7-7D5C96C1F726}"));

    // Company: Partner2
    private readonly ObjectID _ceo7 = new ObjectID ("Ceo", new Guid ("{9F0AC953-E78E-4939-8AFE-0EFF9B3B3ED9}"));

    // Company: Supplier1
    private readonly ObjectID _ceo8 = new ObjectID ("Ceo", new Guid ("{394C69B2-BD40-48d1-A2AE-A73FB63C0B66}"));

    // Company: Supplier2
    private readonly ObjectID _ceo9 = new ObjectID ("Ceo", new Guid ("{421D04B4-BC77-4682-B0FE-58B96802C524}"));

    // Company: Distributor1
    private readonly ObjectID _ceo10 = new ObjectID ("Ceo", new Guid ("{6B801331-2163-4837-B20C-973BD9B8768E}"));

    // Company: Distributor2
    private readonly ObjectID _ceo11 = new ObjectID ("Ceo", new Guid ("{2E8AE776-DC3A-45a5-9B0C-35900CC78FDC}"));

    // Company: Customer4
    private readonly ObjectID _ceo12 = new ObjectID ("Ceo", new Guid ("{FD1B587C-3E26-43f8-9866-8B770194D70F}"));

    public ObjectID Ceo1
    {
      get { return _ceo1; }
    }

    public ObjectID Ceo2
    {
      get { return _ceo2; }
    }

    public ObjectID Ceo3
    {
      get { return _ceo3; }
    }

    public ObjectID Ceo4
    {
      get { return _ceo4; }
    }

    public ObjectID Ceo5
    {
      get { return _ceo5; }
    }

    public ObjectID Ceo6
    {
      get { return _ceo6; }
    }

    public ObjectID Ceo7
    {
      get { return _ceo7; }
    }

    public ObjectID Ceo8
    {
      get { return _ceo8; }
    }

    public ObjectID Ceo9
    {
      get { return _ceo9; }
    }

    public ObjectID Ceo10
    {
      get { return _ceo10; }
    }

    public ObjectID Ceo11
    {
      get { return _ceo11; }
    }

    public ObjectID Ceo12
    {
      get { return _ceo12; }
    }

    #endregion

    #region Official

    // Orders: Order1, Order2, OrderWithoutOrderItem, Order3, Order4
    private readonly ObjectID _official1 = new ObjectID ("Official", 1);

    // Orders: -
    private readonly ObjectID _official2 = new ObjectID ("Official", 2);

    public ObjectID Official1
    {
      get { return _official1; }
    }

    public ObjectID Official2
    {
      get { return _official2; }
    }

    #endregion

    #region Client

    // ChildClients: Client2, Client3
    // ParentClient: -
    // Location: Location1, Location2
    private readonly ObjectID _client1 = new ObjectID ("Client", new Guid ("{1627ADE8-125F-4819-8E33-CE567C42B00C}"));

    // ChildClients: -
    // ParentClient: Client1
    // Location: Location3
    private readonly ObjectID _client2 = new ObjectID ("Client", new Guid ("{090D54F2-738C-48ac-9C78-F40365A72305}"));

    // ChildClients: -
    // ParentClient: Client1
    // Location: -
    private readonly ObjectID _client3 = new ObjectID ("Client", new Guid ("{01349595-88A3-4583-A7BA-CB08795C97F6}"));

    // ChildClients: -
    // ParentClient: -
    // Location: -
    private readonly ObjectID _client4 = new ObjectID ("Client", new Guid ("{015E25B1-ACFA-4364-87F5-D28A45384D11}"));

    public ObjectID Client1
    {
      get { return _client1; }
    }

    public ObjectID Client2
    {
      get { return _client2; }
    }

    public ObjectID Client3
    {
      get { return _client3; }
    }

    public ObjectID Client4
    {
      get { return _client4; }
    }

    #endregion

    #region Location

    // Client: Client1
    private readonly ObjectID _location1 = new ObjectID ("Location", new Guid ("{D527B630-B0AC-4572-A614-EAC9F486148D}"));

    // Client: Client1
    private readonly ObjectID _location2 = new ObjectID ("Location", new Guid ("{20380C9D-B70F-4d9a-880E-EAE5D6E3919C}"));

    // Client: Client2
    private readonly ObjectID _location3 = new ObjectID ("Location", new Guid ("{903E7EE5-CBB8-44c0-BEB6-ACAFFA5ADA7F}"));

    public ObjectID Location1
    {
      get { return _location1; }
    }

    public ObjectID Location2
    {
      get { return _location2; }
    }

    public ObjectID Location3
    {
      get { return _location3; }
    }

    #endregion

    #region ClassWithAllDataTypes

    private readonly ObjectID _classWithAllDataTypes1 = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));
    private readonly ObjectID _classWithAllDataTypes2 = new ObjectID ("ClassWithAllDataTypes", new Guid ("{583EC716-8443-4b55-92BF-09F7C8768529}"));

    public ObjectID ClassWithAllDataTypes1
    {
      get { return _classWithAllDataTypes1; }
    }

    public ObjectID ClassWithAllDataTypes2
    {
      get { return _classWithAllDataTypes2; }
    }

    #endregion

    #region StorageGroupClass

    private readonly ObjectID _storageGroupClass1 = new ObjectID ("StorageGroupClass", new Guid ("{09755471-E551-496d-941B-84D90D0C9ECA}"));

    private readonly ObjectID _storageGroupClass2 = new ObjectID ("StorageGroupClass", new Guid ("{F394AE2E-CB4E-4e38-8E08-9C847EE1F376}"));

    public ObjectID StorageGroupClass1
    {
      get { return _storageGroupClass1; }
    }

    public ObjectID StorageGroupClass2
    {
      get { return _storageGroupClass2; }
    }

    #endregion

    #region TargetClassForPersistentMixins

    // PersistentProperty: 99
    // ExtraPersistentProperty: 100
    // RelationProperty: -
    // VirtualRelationProperty: -
    // CollectionProperty1Side: -
    // CollectionPropertyNSide: -
    private readonly ObjectID _targetClassForPersistentMixins1 = new ObjectID ("TargetClassForPersistentMixin", new Guid ("{784EBDDD-EE94-456D-A5F4-F6CB1B41B6CA}"));

    // PersistentProperty: 13
    // ExtraPersistentProperty: 1333
    // RelationProperty: RelationTargetForPersistentMixin1
    // VirtualRelationProperty: RelationTargetForPersistentMixin2
    // CollectionProperty1Side: RelationTargetForPersistentMixin3
    // CollectionPropertyNSide: RelationTargetForPersistentMixin4
    // UnidirectionalRelationProperty: RelationTargetForPersistentMixin5
    private readonly ObjectID _targetClassForPersistentMixins2 = new ObjectID ("TargetClassForPersistentMixin", new Guid ("{FF79502F-FF40-45E0-929A-230006EA3E83}"));

    // PersistentProperty: 199
    // ExtraPersistentProperty: 100
    // RelationProperty: -
    // VirtualRelationProperty: -
    // CollectionProperty1Side: -
    // CollectionPropertyNSide: -
    private readonly ObjectID _derivedTargetClassForPersistentMixin1 = new ObjectID ("DerivedTargetClassForPersistentMixin", new Guid ("{4ED563B8-B337-4C8E-9A77-5FA907919377}"));

    // PersistentProperty: 299
    // ExtraPersistentProperty: 100
    // RelationProperty: -
    // VirtualRelationProperty: -
    // CollectionProperty1Side: -
    // CollectionPropertyNSide: -
    private readonly ObjectID _derivedDerivedTargetClassForPersistentMixin1 = new ObjectID ("DerivedDerivedTargetClassForPersistentMixin", new Guid ("{B551C440-8C80-4930-A2A1-7FBB4F6B69D8}"));

    // PersistentProperty: 199
    // ExtraPersistentProperty: 100
    // RelationProperty: -
    // VirtualRelationProperty: -
    // CollectionProperty1Side: -
    // CollectionPropertyNSide: -
    private readonly ObjectID _derivedTargetClassWithDerivedMixinWithInterface1 = new ObjectID ("DerivedTargetClassWithDerivedMixinWithInterface", new Guid ("{5FFD52D9-2A38-4DEC-9AA1-FA76C30B91A4}"));

    public ObjectID TargetClassForPersistentMixins1
    {
      get { return _targetClassForPersistentMixins1; }
    }

    public ObjectID TargetClassForPersistentMixins2
    {
      get { return _targetClassForPersistentMixins2; }
    }

    public ObjectID DerivedTargetClassForPersistentMixin1
    {
      get { return _derivedTargetClassForPersistentMixin1; }
    }

    public ObjectID DerivedDerivedTargetClassForPersistentMixin1
    {
      get { return _derivedDerivedTargetClassForPersistentMixin1; }
    }

    public ObjectID DerivedTargetClassWithDerivedMixinWithInterface1
    {
      get { return _derivedTargetClassWithDerivedMixinWithInterface1; }
    }

    #endregion

    #region RelationTargetForPersistentMixin

    private readonly ObjectID _relationTargetForPersistentMixin1 = new ObjectID ("RelationTargetForPersistentMixin", new Guid ("{DC42158C-7DA6-4D5C-B522-C0879E404DEC}"));
    private readonly ObjectID _relationTargetForPersistentMixin2 = new ObjectID ("RelationTargetForPersistentMixin", new Guid ("{332F9971-E6FD-411C-862A-23416E0019BC}"));
    private readonly ObjectID _relationTargetForPersistentMixin3 = new ObjectID ("RelationTargetForPersistentMixin", new Guid ("{58458546-9C4A-4E36-9D62-C6CF171748A6}"));
    private readonly ObjectID _relationTargetForPersistentMixin4 = new ObjectID ("RelationTargetForPersistentMixin", new Guid ("{A5AC369E-9742-412C-8275-4B31B48CEFF3}"));
    private readonly ObjectID _relationTargetForPersistentMixin5 = new ObjectID ("RelationTargetForPersistentMixin", new Guid ("{C007F590-7953-4429-A34E-778309F2FC1D}"));

    public ObjectID RelationTargetForPersistentMixin1
    {
      get { return _relationTargetForPersistentMixin1; }
    }

    public ObjectID RelationTargetForPersistentMixin2
    {
      get { return _relationTargetForPersistentMixin2; }
    }

    public ObjectID RelationTargetForPersistentMixin3
    {
      get { return _relationTargetForPersistentMixin3; }
    }

    public ObjectID RelationTargetForPersistentMixin4
    {
      get { return _relationTargetForPersistentMixin4; }
    }

    public ObjectID RelationTargetForPersistentMixin5
    {
      get { return _relationTargetForPersistentMixin5; }
    }

    #endregion
    
  }
}
