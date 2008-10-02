/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Development.UnitTesting.ObjectMother;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.AclTools
{
  public class AclToolsTestBase : DomainTest
  {
    public AccessControlList Acl { get; private set; }
    public AccessTypeDefinition DeleteAccessType { get; private set; }
    public AccessTypeDefinition WriteAccessType { get; private set; }
    public AccessTypeDefinition ReadAccessType { get; private set; }

    public AccessTypeDefinition[] AccessTypeDefinitions { get; private set; }
    public AccessControlTestHelper TestHelper { get; private set; }
    public Tenant Tenant { get; private set; }
    public Group Group { get; private set; }
    public Position Position { get; private set; }
    public Role Role { get; private set; }
    public User User { get; private set; }
    public AccessControlEntry Ace { get; private set; }

    public AccessTypeDefinition[] AccessTypeDefinitions2 { get; private set; }
    public AccessControlEntry Ace2 { get; private set; }
    public Role Role2 { get; private set; }
    public User User2 { get; private set; }
    public Position Position2 { get; private set; }
    public Group Group2 { get; private set; }

    public AccessTypeDefinition[] AccessTypeDefinitions3 { get; private set; }
    public AccessControlEntry Ace3 { get; private set; }
    public Role Role3 { get; private set; }
    public User User3 { get; private set; }
    public Position Position3 { get; private set; }
    public Group Group3 { get; private set; }


    public override void SetUp ()
    {
      base.SetUp ();
      TestHelper = new AccessControlTestHelper ();
      TestHelper.Transaction.EnterNonDiscardingScope ();


      Tenant = TestHelper.CreateTenant ("Da Tenant");
      Group = TestHelper.CreateGroup ("Da Group", null, Tenant);
      Position = TestHelper.CreatePosition ("Supreme Being");
      User = TestHelper.CreateUser ("DaUs", "Da", "Usa", "Dr.", Group, Tenant);
      Role = TestHelper.CreateRole (User, Group, Position);
      Ace = TestHelper.CreateAceWithOwningTenant();

      ReadAccessType = TestHelper.CreateReadAccessType ();  // read access
      WriteAccessType = TestHelper.CreateWriteAccessType();  // write access
      DeleteAccessType = TestHelper.CreateDeleteAccessType();  // delete permission

      AccessTypeDefinitions  = List.New (
        TestHelper.CreateReadAccessTypeAndSetWithValueAtAce (Ace, true),
        TestHelper.CreateWriteAccessTypeAndSetWithValueAtAce (Ace, null), 
        TestHelper.CreateDeleteAccessTypeAndSetWithValueAtAce(Ace,true)
      ).ToArray();

      Group2 = TestHelper.CreateGroup ("Anotha Group", null, Tenant);
      Position2 = TestHelper.CreatePosition ("Working Drone");
      User2 = TestHelper.CreateUser ("mr.smith", "", "Smith", "Mr.", Group2, Tenant);
      Role2 = TestHelper.CreateRole (User2, Group2, Position2);
      Ace2 = TestHelper.CreateAceWithSpecficTenant (Tenant);
      AccessTypeDefinitions2 = List.New (
        TestHelper.CreateReadAccessTypeAndSetWithValueAtAce (Ace2, null),
        TestHelper.CreateWriteAccessTypeAndSetWithValueAtAce (Ace2, true),
        TestHelper.CreateDeleteAccessTypeAndSetWithValueAtAce (Ace2, true)
      ).ToArray ();

      Group3 = TestHelper.CreateGroup ("Da 3rd Group", null, Tenant);
      Position3 = TestHelper.CreatePosition ("Warrior");
      User3 = TestHelper.CreateUser ("ryan_james", "Ryan", "James", "", Group3, Tenant);
      Role3 = TestHelper.CreateRole (User3, Group3, Position3);
      Ace3 = TestHelper.CreateAceWithOwningGroup();
      AccessTypeDefinitions3 = List.New (
        TestHelper.CreateReadAccessTypeAndSetWithValueAtAce (Ace3, null),
        TestHelper.CreateWriteAccessTypeAndSetWithValueAtAce (Ace3, true),
        TestHelper.CreateDeleteAccessTypeAndSetWithValueAtAce (Ace3, null)
      ).ToArray ();


      Acl = TestHelper.CreateAcl (Ace, Ace2, Ace3);


      //ReadAccessType = TestHelper.CreateReadAccessTypeAndSetWithValueAtAce (Ace, true);  // allow read access
      //WriteAccessType = TestHelper.CreateWriteAccessTypeAndSetWithValueAtAce (Ace, true);  // allow write access
      //DeleteAccessType = TestHelper.CreateDeleteAccessTypeAndSetWithValueAtAce (Ace, false);  // allow delete operations

      //TestHelper.AttachAccessType (ace2, readAccessType, null);
      //TestHelper.AttachAccessType (ace2, writeAccessType, true);
      //TestHelper.AttachAccessType (ace2, deleteAccessType, null);
    }


    private User CreateUser (Tenant userTenant, Group userGroup)
    {
      return TestHelper.CreateUser ("JoDo", "John", "Doe", "Prof.", userGroup, userTenant);
    }
  }
}