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
    private AccessControlTestHelper _testHelper;

    private Tenant _tenant;
    private Group _group;
    private Position _position;
    private Role _role;
    private User _user;
    private AccessTypeDefinition[] _accessTypeDefinitions;
    private AccessControlEntry _ace;
    
    private Group _group2;
    private Position _position2;
    private User _user2;
    private Role _role2;
    private AccessControlEntry _ace2;
    private AccessTypeDefinition[] _accessTypeDefinitions2;

    private Group _group3;
    private Position _position3;
    private User _user3;
    private Role _role3;
    private AccessControlEntry _ace3;
    private AccessTypeDefinition[] _accessTypeDefinitions3;


 

    public AccessTypeDefinition[] AccessTypeDefinitions
    {
      get { return _accessTypeDefinitions; }
    }

    public AccessControlTestHelper TestHelper
    {
      get { return _testHelper; }
    }

    public Tenant Tenant
    {
      get { return _tenant; }
    }

    public Group Group
    {
      get { return _group; }
    }

    public Position Position
    {
      get { return _position; }
    }

    public Role Role
    {
      get { return _role; }
    }

    public User User
    {
      get { return _user; }
    }

    public AccessControlEntry Ace
    {
      get { return _ace; }
    }


    public AccessTypeDefinition[] AccessTypeDefinitions2
    {
      get { return _accessTypeDefinitions2; }
    }

    public AccessControlEntry Ace2
    {
      get { return _ace2; }
    }

    public Role Role2
    {
      get { return _role2; }
    }

    public User User2
    {
      get { return _user2; }
    }

    public Position Position2
    {
      get { return _position2; }
    }

    public Group Group2
    {
      get { return _group2; }
    }



    public AccessTypeDefinition[] AccessTypeDefinitions3
    {
      get { return _accessTypeDefinitions3; }
    }

    public AccessControlEntry Ace3
    {
      get { return _ace3; }
    }

    public Role Role3
    {
      get { return _role3; }
    }

    public User User3
    {
      get { return _user3; }
    }

    public Position Position3
    {
      get { return _position3; }
    }

    public Group Group3
    {
      get { return _group3; }
    }



    public override void SetUp ()
    {
      base.SetUp ();
      _testHelper = new AccessControlTestHelper ();
      _testHelper.Transaction.EnterNonDiscardingScope ();

      _tenant = _testHelper.CreateTenant ("Da Tenant");
      _group = _testHelper.CreateGroup ("Da Group", null, Tenant);
      _position = _testHelper.CreatePosition ("Supreme Being");
      _user = _testHelper.CreateUser ("DaUs", "Da", "Usa", "Dr.", Group, Tenant);
      _role = _testHelper.CreateRole (User, Group, Position);
      _ace = _testHelper.CreateAceWithOwningTenant();
      _accessTypeDefinitions  = List.New (
        _testHelper.CreateReadAccessType (Ace, true), 
        _testHelper.CreateWriteAccessType (Ace, false), 
        _testHelper.CreateDeleteAccessType(Ace,true)
      ).ToArray();

      _group2 = _testHelper.CreateGroup ("Anotha Group", null, Tenant);
      _position2 = _testHelper.CreatePosition ("Working Drone");
      _user2 = _testHelper.CreateUser ("mr.smith", "", "Smith", "Mr.", Group2, Tenant);
      _role2 = _testHelper.CreateRole (User2, Group2, Position2);
      _ace2 = _testHelper.CreateAceWithSpecficTenant (Tenant);
      _accessTypeDefinitions2 = List.New (
        _testHelper.CreateReadAccessType (Ace2, false),
        _testHelper.CreateWriteAccessType (Ace2, true),
        _testHelper.CreateDeleteAccessType (Ace2, true)
      ).ToArray ();

      _group3 = _testHelper.CreateGroup ("Da 3rd Group", null, Tenant);
      _position3 = _testHelper.CreatePosition ("Warrior");
      _user3 = _testHelper.CreateUser ("ryan_james", "Ryan", "James", "", Group3, Tenant);
      _role3 = _testHelper.CreateRole (User3, Group3, Position3);
      _ace3 = _testHelper.CreateAceWithSpecficTenant (Tenant);
      _accessTypeDefinitions3 = List.New (
        _testHelper.CreateReadAccessType (Ace3, false),
        _testHelper.CreateWriteAccessType (Ace3, false),
        _testHelper.CreateDeleteAccessType (Ace3, false)
      ).ToArray ();
    
    }

    private User CreateUser (Tenant userTenant, Group userGroup)
    {
      return _testHelper.CreateUser ("JoDo", "John", "Doe", "Prof.", userGroup, userTenant);
    }
  }
}