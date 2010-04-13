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
    public readonly ObjectID Employee1 = new ObjectID ("Employee", new Guid ("{51ECE39B-F040-45b0-8B72-AD8B45353990}"));

    // Supervisor: -
    // Subordinates: Employee3
    // Computer: -
    public readonly ObjectID Employee2 = new ObjectID ("Employee", new Guid ("{C3B2BBC3-E083-4974-BAC7-9CEE1FB85A5E}"));

    // Supervisor: Employee2
    // Subordinates: -
    // Computer: Computer1
    public readonly ObjectID Employee3 = new ObjectID ("Employee", new Guid ("{3C4F3FC8-0DB2-4c1f-AA00-ADE72E9EDB32}"));

    // Supervisor: Employee1
    // Subordinates: -
    // Computer: Computer2
    public readonly ObjectID Employee4 = new ObjectID ("Employee", new Guid ("{890BF138-7559-40d6-9C7F-436BC1AD4F59}"));

    // Supervisor: Employee1
    // Subordinates: -
    // Computer: Computer3
    public readonly ObjectID Employee5 = new ObjectID ("Employee", new Guid ("{43329F84-D8BB-4988-BFD2-96D4F48EE5DE}"));

    // Supervisor: -
    // Subordinates: Employee7
    // Computer: -
    public readonly ObjectID Employee6 = new ObjectID ("Employee", new Guid ("{3A24D098-EAAD-4dd7-ADA2-932D9B6935F1}"));


    // Supervisor: Employee6
    // Subordinates: -
    // Computer: -
    public readonly ObjectID Employee7 = new ObjectID ("Employee", new Guid ("{DBD9EA74-8C97-4411-AC02-9205D1D6D031}"));

    #endregion

    #region Computer

    // Employee: Employee3
    public readonly ObjectID Computer1 = new ObjectID ("Computer", new Guid ("{C7C26BF5-871D-48c7-822A-E9B05AAC4E5A}"));

    // Employee: Employee4
    public readonly ObjectID Computer2 = new ObjectID ("Computer", new Guid ("{176A0FF6-296D-4934-BD1A-23CF52C22411}"));

    // Employee: Employee5
    public readonly ObjectID Computer3 = new ObjectID ("Computer", new Guid ("{704CE38C-4A08-4ef2-A6FE-9ED849BA31E5}"));

    // Employee: -
    public readonly ObjectID Computer4 = new ObjectID ("Computer", new Guid ("{D6F50E77-2041-46b8-A840-AAA4D2E1BF5A}"));

    // Employee: -
    public readonly ObjectID Computer5 = new ObjectID ("Computer", new Guid ("{AEAC0C5D-44E0-45cc-B716-103B0A4981A4}"));

    #endregion

    #region Person

    public readonly ObjectID Person1 = new ObjectID ("Person", new Guid ("{2001BF42-2AA4-4c81-AD8E-73E9145411E9}"));

    public readonly ObjectID Person2 = new ObjectID ("Person", new Guid ("{DC50A962-EC95-4cf6-A4E7-A6608EAA23C8}"));

    public readonly ObjectID Person3 = new ObjectID ("Person", new Guid ("{10F36130-E97B-4078-A535-B79E07F16AB2}"));

    public readonly ObjectID Person4 = new ObjectID ("Person", new Guid ("{45C6730A-DE0B-40d2-9D35-C1E56B8A89D6}"));

    public readonly ObjectID Person5 = new ObjectID ("Person", new Guid ("{70C91528-4DB4-4e6a-B3F8-70C53A728DCC}"));

    public readonly ObjectID Person6 = new ObjectID ("Person", new Guid ("{19C04A28-094F-4d1f-9705-E2FC7107A68F}"));

    public readonly ObjectID Person7 = new ObjectID ("Person", new Guid ("{E4F6F59F-80F7-4e41-A004-1A5BA0F68F78}"));

    public readonly ObjectID ContactPersonInTwoOrganizations = new ObjectID ("Person", new Guid ("{911957D1-483C-4a8b-AA53-FF07464C58F9}"));

    #endregion

    #region IndustrialSector

    // Companies: Customer1, Partner1, PartnerWithoutCeo, Supplier1, Distributor2
    public readonly ObjectID IndustrialSector1 = new ObjectID ("IndustrialSector", new Guid ("{3BB7BEE9-2AF9-4a85-998E-618BEBBE5A6B}"));

    // Companies: Company1, Company2, Customer2, Customer3, Partner2, Supplier2, Distributor1
    public readonly ObjectID IndustrialSector2 = new ObjectID ("IndustrialSector", new Guid ("{8565A077-EA01-4b5d-BEAA-293DC484BDDC}"));

    // Companies: DistributorWithoutContactPerson
    public readonly ObjectID IndustrialSector3 = new ObjectID ("IndustrialSector", new Guid ("{53B322BF-25D8-4fe1-96C8-508E055143E7}"));

    #endregion

    #region Company

    // IndustrialSector: IndustrialSector2
    // Ceo: Ceo1
    public readonly ObjectID Company1 = new ObjectID ("Company", new Guid ("{C4954DA8-8870-45c1-B7A3-C7E5E6AD641A}"));

    // IndustrialSector: IndustrialSector2
    // Ceo: Ceo2
    public readonly ObjectID Company2 = new ObjectID ("Company", new Guid ("{A21A9EC2-17D6-44de-9F1A-2AB6FC3742DF}"));

    #endregion

    #region Customer

    // IndustrialSector: IndustrialSector1
    // Ceo: Ceo3
    // Orders: Order1, OrderWithoutOrderItem
    public readonly ObjectID Customer1 = new ObjectID ("Customer", new Guid ("{55B52E75-514B-4e82-A91B-8F0BB59B80AD}"));

    // IndustrialSector: IndustrialSector2
    // Ceo: Ceo4
    // Orders: -
    public readonly ObjectID Customer2 = new ObjectID ("Customer", new Guid ("{F577F879-2DB4-4a3c-A18A-AFB4E57CE098}"));

    // IndustrialSector: IndustrialSector2
    // Ceo: Ceo5
    // Orders: Order2
    public readonly ObjectID Customer3 = new ObjectID ("Customer", new Guid ("{DD3E3D55-C16F-497f-A3E1-384D08DE0D66}"));


    // IndustrialSector: -
    // Ceo: Ceo12
    // Orders: Order3, Order4
    public readonly ObjectID Customer4 = new ObjectID ("Customer", new Guid ("{B3F0A333-EC2A-4ddd-9035-9ADA34052450}"));

    #endregion

    #region Partner

    // IndustrialSector: IndustrialSector1
    // ContactPerson: Person1
    // Ceo: Ceo6
    public readonly ObjectID Partner1 = new ObjectID ("Partner", new Guid ("{5587A9C0-BE53-477d-8C0A-4803C7FAE1A9}"));

    // IndustrialSector: IndustrialSector2
    // ContactPerson: Person2
    // Ceo: Ceo7
    public readonly ObjectID Partner2 = new ObjectID ("Partner", new Guid ("{B403E58E-9FA5-47ed-883C-73420D64DEB3}"));

    // IndustrialSector: IndustrialSector1
    // ContactPerson: Person7
    // Ceo: -
    public readonly ObjectID PartnerWithoutCeo = new ObjectID ("Partner", new Guid ("{A65B123A-6E17-498e-A28E-946217C0AE30}"));

    #endregion

    #region Supplier

    // IndustrialSector: IndustrialSector1
    // ContactPerson: Person3
    // Ceo: Ceo8
    public readonly ObjectID Supplier1 = new ObjectID ("Supplier", new Guid ("{FD392135-1FDD-42a3-8E2F-232BAB9893A2}"));

    // IndustrialSector: IndustrialSector2
    // ContactPerson: Person4
    // Ceo: Ceo9
    public readonly ObjectID Supplier2 = new ObjectID ("Supplier", new Guid ("{92A8BB6A-412A-4fe3-9B09-3E1B6136E425}"));

    #endregion

    #region Distributor

    // IndustrialSector: IndustrialSector2
    // ContactPerson: Person5
    // Ceo: Ceo10
    public readonly ObjectID Distributor1 = new ObjectID ("Distributor", new Guid ("{E4087155-D60A-4d31-95B3-9A401A3E4E78}"));

    // IndustrialSector: IndustrialSector1
    // ContactPerson: Person6
    // Ceo: Ceo11
    public readonly ObjectID Distributor2 = new ObjectID ("Distributor", new Guid ("{247206C3-7B48-4e17-91DD-3363B568D7E4}"));

    // IndustrialSector: IndustrialSector3
    // ContactPerson: -
    // Ceo: -
    public readonly ObjectID DistributorWithoutContactPersonAndCeo = new ObjectID ("Distributor", new Guid ("{1514D668-A0A5-40e9-AC22-F24900E0EB39}"));

    #endregion

    #region Order

    // OrderTicket: OrderTicket1
    // OrderItems: OrderItem1, OrderItem2
    // Customer: Customer1
    // Official: Official1
    // OrderNumber: 1
    public readonly ObjectID Order1 = new ObjectID ("Order", new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}"));

    // OrderTicket: OrderTicket3
    // OrderItems: OrderItem3
    // Customer: Customer3
    // Official: Official1
    // OrderNumber: 3
    public readonly ObjectID Order2 = new ObjectID ("Order", new Guid ("{83445473-844A-4d3f-A8C3-C27F8D98E8BA}"));

    // OrderTicket: OrderTicket2
    // OrderItems: -
    // Customer: Customer1
    // Official: Official1
    public readonly ObjectID OrderWithoutOrderItem = new ObjectID ("Order", new Guid ("{F4016F41-F4E4-429e-B8D1-659C8C480A67}"));

    // OrderTicket: OrderTicket4
    // OrderItems: OrderItem4
    // Customer: Customer4
    // Official: Official1
    public readonly ObjectID Order3 = new ObjectID ("Order", new Guid ("{3C0FB6ED-DE1C-4e70-8D80-218E0BF58DF3}"));

    // OrderTicket: OrderTicket5
    // OrderItems: OrderItem5
    // Customer: Customer4
    // Official: Official1
    public readonly ObjectID Order4 = new ObjectID ("Order", new Guid ("{90E26C86-611F-4735-8D1B-E1D0918515C2}"));

    // OrderTicket: -
    // OrderItems: -
    // Customer: invalid
    // Official: does not exist
    public readonly ObjectID InvalidOrder = new ObjectID ("Order", new Guid ("{DA658F26-8107-44ce-9DD0-1804503ECCAF}"));

    #endregion

    #region OrderItem

    // Order: Order1
    // Product: Mainboard
    public readonly ObjectID OrderItem1 = new ObjectID ("OrderItem", new Guid ("{2F4D42C7-7FFA-490d-BFCD-A9101BBF4E1A}"));

    // Order: Order1
    // Product: CPU Fan
    public readonly ObjectID OrderItem2 = new ObjectID ("OrderItem", new Guid ("{AD620A11-4BC4-4791-BCF4-A0770A08C5B0}"));

    // Order: Order2
    // Product: Harddisk
    public readonly ObjectID OrderItem3 = new ObjectID ("OrderItem", new Guid ("{0D7196A5-8161-4048-820D-B1BBDABE3293}"));

    // Order: Order3
    // Product: Hitchhiker's guide
    public readonly ObjectID OrderItem4 = new ObjectID ("OrderItem", new Guid ("{DC20E0EB-4B55-4f23-89CF-6D6478F96D3B}"));

    // Order: Order4
    // Product: Blumentopf
    public readonly ObjectID OrderItem5 = new ObjectID ("OrderItem", new Guid ("{EA505094-770A-4505-82C1-5A4F94F56FE2}"));

    #endregion

    #region OrderWithNewPropertyAccess

    // OrderItems: OrderItemWithNewPropertyAccess1, OrderItemWithNewPropertyAccess2
    // Customer: Customer1
    public readonly ObjectID OrderWithNewPropertyAccess1 = new ObjectID ("OrderWithNewPropertyAccess", new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}"));

    #endregion

    #region OrderItemWithNewPropertyAccess

    // Order: OrderWithNewPropertyAccess1
    public readonly ObjectID OrderItemWithNewPropertyAccess1 = new ObjectID ("OrderItemWithNewPropertyAccess", new Guid ("{2F4D42C7-7FFA-490d-BFCD-A9101BBF4E1A}"));

    // Order: OrderWithNewPropertyAccess1
    public readonly ObjectID OrderItemWithNewPropertyAccess2 = new ObjectID ("OrderItemWithNewPropertyAccess", new Guid ("{AD620A11-4BC4-4791-BCF4-A0770A08C5B0}"));

    #endregion

    #region OrderTicket

    // Order: Order1
    public readonly ObjectID OrderTicket1 = new ObjectID ("OrderTicket", new Guid ("{058EF259-F9CD-4cb1-85E5-5C05119AB596}"));

    // Order: OrderWithoutOrderItem
    public readonly ObjectID OrderTicket2 = new ObjectID ("OrderTicket", new Guid ("{0005BDF4-4CCC-4a41-B9B5-BAAB3EB95237}"));

    // Order: Order2
    public readonly ObjectID OrderTicket3 = new ObjectID ("OrderTicket", new Guid ("{BCF6C5F6-323F-4471-9CA5-7DF0A48C7A59}"));

    // Order: Order3
    public readonly ObjectID OrderTicket4 = new ObjectID ("OrderTicket", new Guid ("{6768DB2B-9C66-4e2f-BBA2-89C56718FF2B}"));

    // Order: Order4
    public readonly ObjectID OrderTicket5 = new ObjectID ("OrderTicket", new Guid ("{DC20E0EB-4B55-4f23-89CF-6D6478F96D3B}"));

    #endregion

    #region Ceo

    // Company: Company1
    public readonly ObjectID Ceo1 = new ObjectID ("Ceo", new Guid ("{A1691AF1-F96D-42e1-B021-B5099840D572}"));

    // Company: Company2
    public readonly ObjectID Ceo2 = new ObjectID ("Ceo", new Guid ("{A6A848CE-505F-4cd3-A337-1F5EEA1D2260}"));

    // Company: Customer1
    public readonly ObjectID Ceo3 = new ObjectID ("Ceo", new Guid ("{481C7840-9D8A-4872-BBCD-B41A9BD85528}"));

    // Company: Customer2
    public readonly ObjectID Ceo4 = new ObjectID ("Ceo", new Guid ("{BE7F24E2-600C-4cd8-A7C3-8669AFD54154}"));

    // Company: Customer3
    public readonly ObjectID Ceo5 = new ObjectID ("Ceo", new Guid ("{7236BA88-48C6-415f-A0BA-A328A1A22DFE}"));

    // Company: Partner1
    public readonly ObjectID Ceo6 = new ObjectID ("Ceo", new Guid ("{C7837D11-C1D6-458f-A3F7-7D5C96C1F726}"));

    // Company: Partner2
    public readonly ObjectID Ceo7 = new ObjectID ("Ceo", new Guid ("{9F0AC953-E78E-4939-8AFE-0EFF9B3B3ED9}"));

    // Company: Supplier1
    public readonly ObjectID Ceo8 = new ObjectID ("Ceo", new Guid ("{394C69B2-BD40-48d1-A2AE-A73FB63C0B66}"));

    // Company: Supplier2
    public readonly ObjectID Ceo9 = new ObjectID ("Ceo", new Guid ("{421D04B4-BC77-4682-B0FE-58B96802C524}"));

    // Company: Distributor1
    public readonly ObjectID Ceo10 = new ObjectID ("Ceo", new Guid ("{6B801331-2163-4837-B20C-973BD9B8768E}"));

    // Company: Distributor2
    public readonly ObjectID Ceo11 = new ObjectID ("Ceo", new Guid ("{2E8AE776-DC3A-45a5-9B0C-35900CC78FDC}"));

    // Company: Customer4
    public readonly ObjectID Ceo12 = new ObjectID ("Ceo", new Guid ("{FD1B587C-3E26-43f8-9866-8B770194D70F}"));

    #endregion

    #region Official

    // Orders: Order1, Order2, OrderWithoutOrderItem, Order3, Order4
    public readonly ObjectID Official1 = new ObjectID ("Official", 1);

    // Orders: -
    public readonly ObjectID Official2 = new ObjectID ("Official", 2);

    #endregion

    #region Client

    // ChildClients: Client2, Client3
    // ParentClient: -
    // Location: Location1, Location2
    public readonly ObjectID Client1 = new ObjectID ("Client", new Guid ("{1627ADE8-125F-4819-8E33-CE567C42B00C}"));

    // ChildClients: -
    // ParentClient: Client1
    // Location: Location3
    public readonly ObjectID Client2 = new ObjectID ("Client", new Guid ("{090D54F2-738C-48ac-9C78-F40365A72305}"));

    // ChildClients: -
    // ParentClient: Client1
    // Location: -
    public readonly ObjectID Client3 = new ObjectID ("Client", new Guid ("{01349595-88A3-4583-A7BA-CB08795C97F6}"));

    // ChildClients: -
    // ParentClient: -
    // Location: -
    public readonly ObjectID Client4 = new ObjectID ("Client", new Guid ("{015E25B1-ACFA-4364-87F5-D28A45384D11}"));

    #endregion

    #region Location

    // Client: Client1
    public readonly ObjectID Location1 = new ObjectID ("Location", new Guid ("{D527B630-B0AC-4572-A614-EAC9F486148D}"));

    // Client: Client1
    public readonly ObjectID Location2 = new ObjectID ("Location", new Guid ("{20380C9D-B70F-4d9a-880E-EAE5D6E3919C}"));

    // Client: Client2
    public readonly ObjectID Location3 = new ObjectID ("Location", new Guid ("{903E7EE5-CBB8-44c0-BEB6-ACAFFA5ADA7F}"));

    #endregion

    #region ClassWithAllDataTypes

    public readonly ObjectID ClassWithAllDataTypes1 = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));
    public readonly ObjectID ClassWithAllDataTypes2 = new ObjectID ("ClassWithAllDataTypes", new Guid ("{583EC716-8443-4b55-92BF-09F7C8768529}"));

    #endregion

    #region StorageGroupClass

    public readonly ObjectID StorageGroupClass1 = new ObjectID ("StorageGroupClass", new Guid ("{09755471-E551-496d-941B-84D90D0C9ECA}"));

    public readonly ObjectID StorageGroupClass2 = new ObjectID ("StorageGroupClass", new Guid ("{F394AE2E-CB4E-4e38-8E08-9C847EE1F376}"));
    
    #endregion

    public DomainObjectIDs ()
    {
    }
  }
}
