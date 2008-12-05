// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;
using List = Remotion.Development.UnitTesting.ObjectMother.List;
using NUnitText = NUnit.Framework.SyntaxHelpers.Text;


namespace Remotion.SecurityManager.UnitTests.Domain.ToTextSpecificTypeHandlers
{
  [TestFixture]
  public class ToTextSpecificTypeHandlerTests  //: AclToolsTestBase
  {
    public AccessControlTestHelper TestHelper { get; private set; }

    public AccessControlList Acl { get; private set; }
    public AccessControlList Acl2 { get; private set; }

    public AccessTypeDefinition DeleteAccessType { get; private set; }
    public AccessTypeDefinition WriteAccessType { get; private set; }
    public AccessTypeDefinition ReadAccessType { get; private set; }

    public AccessTypeDefinition[] AccessTypeDefinitionArray { get; private set; }
    public Tenant Tenant { get; private set; }
    public Group Group { get; private set; }
    public Position Position { get; private set; }
    public Role Role { get; private set; }
    public User User { get; private set; }
    public AccessControlEntry Ace { get; private set; }

    //public AccessTypeDefinition[] AccessTypeDefinitions2 { get; private set; }
    public AccessControlEntry Ace2 { get; private set; }
    public Role Role2 { get; private set; }
    public User User2 { get; private set; }
    public Position Position2 { get; private set; }
    public Group Group2 { get; private set; }

    //public AccessTypeDefinition[] AccessTypeDefinitions3 { get; private set; }
    public AccessControlEntry Ace3 { get; private set; }
    public Role Role3 { get; private set; }
    public User User3 { get; private set; }
    public Position Position3 { get; private set; }
    public Group Group3 { get; private set; }

    [SetUp]
    public void SetUp ()
    {
      TestHelper = new AccessControlTestHelper ();
      TestHelper.Transaction.EnterNonDiscardingScope ();

      ReadAccessType = TestHelper.CreateReadAccessType ();  // read access
      WriteAccessType = TestHelper.CreateWriteAccessType ();  // write access
      DeleteAccessType = TestHelper.CreateDeleteAccessType ();  // delete permission

      AccessTypeDefinitionArray = new[] { ReadAccessType, WriteAccessType, DeleteAccessType };


      Tenant = TestHelper.CreateTenant ("Da Tenant");
      Group = TestHelper.CreateGroup ("Da Group", null, Tenant);
      Position = TestHelper.CreatePosition ("Supreme Being");
      User = TestHelper.CreateUser ("DaUs", "Da", "Usa", "Dr.", Group, Tenant);
      Role = TestHelper.CreateRole (User, Group, Position);
      Ace = TestHelper.CreateAceWithOwningTenant ();

      TestHelper.AttachAccessType (Ace, ReadAccessType, null);
      TestHelper.AttachAccessType (Ace, WriteAccessType, true);
      TestHelper.AttachAccessType (Ace, DeleteAccessType, null);


      Group2 = TestHelper.CreateGroup ("Anotha Group", null, Tenant);
      Position2 = TestHelper.CreatePosition ("Working Drone");
      User2 = TestHelper.CreateUser ("mr.smith", "", "Smith", "Mr.", Group2, Tenant);
      Role2 = TestHelper.CreateRole (User2, Group2, Position2);
      Ace2 = TestHelper.CreateAceWithSpecificTenant (Tenant);

      TestHelper.AttachAccessType (Ace2, ReadAccessType, true);
      TestHelper.AttachAccessType (Ace2, WriteAccessType, null);
      TestHelper.AttachAccessType (Ace2, DeleteAccessType, true);


      Group3 = TestHelper.CreateGroup ("Da 3rd Group", null, Tenant);
      Position3 = TestHelper.CreatePosition ("Combatant");
      User3 = TestHelper.CreateUser ("ryan_james", "Ryan", "James", "", Group3, Tenant);
      Role3 = TestHelper.CreateRole (User3, Group3, Position3);
      // DO NOT use TestHelper.CreateAceWithOwningGroup() here; functionality for group matching is
      // incomplete and therefore the ACE entry will always match.
      Ace3 = TestHelper.CreateAceWithPositionAndGroupCondition (Position3, GroupCondition.None);

      TestHelper.AttachAccessType (Ace3, ReadAccessType, true);
      TestHelper.AttachAccessType (Ace3, WriteAccessType, true);
      TestHelper.AttachAccessType (Ace3, DeleteAccessType, null);
    }

    [TearDown]
    public virtual void TearDown ()
    {
      ClientTransactionScope.ResetActiveScope ();
    }



    [Test]
    public void AbstractRoleDefinitionTest ()
    {
      var x = TestHelper.CreateAbstractRoleDefinition ("xyz", 0);
      //To.ConsoleLine.e (x);
      Assert.That (To.String.e (x).CheckAndConvertToString(), NUnitText.Contains("xyz"));
    }

    [Test]
    public void AccessControlEntryTest ()
    {
      var x = TestHelper.CreateAceWithOwningTenant ();
      //To.ConsoleLine.e (x);
      Assert.That (To.String.e (x).CheckAndConvertToString (), NUnitText.Contains ("SelUser=None,SelGroup=None,SelTenant=OwningTenant"));
    }

    [Test]
    public void AccessControlEntryTest2 ()
    {
      var x = TestHelper.CreateAceWithAbstractRole ();
      //To.ConsoleLine.e (x);
      Assert.That (To.String.e (x).CheckAndConvertToString (), NUnitText.Contains ("abstr.role=[\"Test\"]"));
    }

    [Test]
    public void AccessControlListTest ()
    {
      var ace = TestHelper.CreateAceWithOwningGroup ();
      var acl = TestHelper.CreateStatefulAcl (ace);
      //To.ConsoleLine.e (acl);
      // Note: test string is similar to AccessControlEntry test above, since rest of test would retest standard ToText sequence output functionality
      Assert.That (To.String.e (acl).CheckAndConvertToString (), NUnitText.Contains ("SelUser=None,SelGroup=OwningGroup,SelTenant=None"));
    }

    [Test]
    public void AccessTypeDefinitionTest ()
    {
      var x = TestHelper.CreateAccessType ("topsecret", 123);
      var basic = To.String.SetOutputComplexityToBasic().e (x).CheckAndConvertToString();
      var complex = To.String.SetOutputComplexityToComplex().e (x).CheckAndConvertToString();
      //To.ConsoleLine.e (() => basic);
      //To.ConsoleLine.e (() => complex);
      Assert.That (basic, NUnitText.Contains ("topsecret"));
      Assert.That (basic, NUnitText.Not.Contains ("123"));
      Assert.That (complex, NUnitText.Contains ("topsecret"));
      Assert.That (complex, NUnitText.Contains ("123"));
    }


    [Test]
    public void GroupTest ()
    {
      var x = TestHelper.CreateGroup ("DieGruppe", null, TestHelper.CreateTenant("DasAmt"));
      //To.ConsoleLine.e (x);
      Assert.That (To.String.e (x).CheckAndConvertToString (), NUnitText.Contains ("DieGruppe"));
    }

    [Test]
    public void PermissionTest ()
    {
      var x = Permission.NewObject();
      x.AccessType = TestHelper.CreateAccessType ("topsecret", 123);
      x.Allowed = true;
      var result = To.String.e (x).CheckAndConvertToString();
      //To.ConsoleLine.e (() => result);
      Assert.That (result, NUnitText.Contains ("topsecret"));
      Assert.That (result, NUnitText.Contains ("True"));
    }

   [Test]
    public void PositionTest ()
    {
      var x = TestHelper.CreatePosition ("Praktikant");
      //To.ConsoleLine.e (x);
      Assert.That (To.String.e (x).CheckAndConvertToString (), NUnitText.Contains ("Praktikant"));
    }

    [Test]
   public void RoleTest ()
    {
      var x = Role;
      //To.ConsoleLine.e (x);
      var result = To.String.e (x).CheckAndConvertToString ();
      Assert.That (result, NUnitText.Contains ("DaUs"));
      Assert.That (result, NUnitText.Contains ("Da Group"));
      Assert.That (result, NUnitText.Contains ("Supreme Being"));
    }

    [Test]
    public void SecurityTokenTest ()
    {
      var x = new SecurityToken (User, Tenant, Group, User2, List.New (TestHelper.CreateAbstractRoleDefinition("arole",456)));
      var result = To.String.e (x).CheckAndConvertToString ();
      //To.ConsoleLine.e (() => result);
      //Assert.That (result, NUnitText.Contains ("\"DaUs\"],tenant=[\"Da Tenant\"]," + Environment.NewLine + "roles={[\"DaUs\",\"Da Group\",\"Supreme Being\"]}],[\"Da Tenant\"],{[\"Da Group\"],[\"Anotha Group\"]},{[\"DaUs\",\"Da Group\",\"Supreme Being\"]},{[\"arole\"]}]"));

      Assert.That (result, NUnitText.Contains ("DaUs"));
      Assert.That (result, NUnitText.Contains ("Da Tenant"));
      Assert.That (result, NUnitText.Contains ("Da Group"));
      Assert.That (result, NUnitText.Contains ("mr.smith"));
      Assert.That (result, NUnitText.Contains ("arole"));
    }

    [Test]
    public void TenantTest ()
    {
      var x = TestHelper.CreateTenant ("Tenantative");
      //To.ConsoleLine.e (x);
      Assert.That (To.String.e (x).CheckAndConvertToString (), NUnitText.Contains ("Tenantative"));
    }

    [Test]
    public void UserTest ()
    {
      var x = User;
      var basic = To.String.SetOutputComplexityToBasic ().e (x).CheckAndConvertToString ();
      var complex = To.String.SetOutputComplexityToComplex ().e (x).CheckAndConvertToString ();
      //To.ConsoleLine.e (() => basic);
      //To.ConsoleLine.e (() => complex);
      Assert.That (basic, Is.EqualTo("[\"DaUs\"]"));
      Assert.That (complex, NUnitText.Contains ("Da Group"));
      Assert.That (complex, NUnitText.Contains ("Da Tenant"));
      Assert.That (complex, NUnitText.Contains ("Supreme Being"));
    }


  }
}
