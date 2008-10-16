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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.UnitTests.AclTools;

using List = Remotion.Development.UnitTesting.ObjectMother.List;

namespace Remotion.SecurityManager.UnitTests.Domain.ToTextSpecificTypeHandlers
{
  [TestFixture]
  [Ignore ("TODO MGi: Make handlers work without mixin")]
  public class ToTextSpecificTypeHandlerTests  : AclToolsTestBase
  {
    [Test]
    public void AbstractRoleDefinitionTest ()
    {
      var x = TestHelper.CreateAbstractRoleDefinition ("xyz", 0);
      To.ConsoleLine.e (x);
      Assert.That (To.String.e (x).CheckAndConvertToString(), NUnit.Framework.SyntaxHelpers.Text.Contains("xyz"));
    }

    [Test]
    public void AccessControlEntryTest ()
    {
      var x = TestHelper.CreateAceWithOwningTenant ();
      To.ConsoleLine.e (x);
      Assert.That (To.String.e (x).CheckAndConvertToString (), NUnit.Framework.SyntaxHelpers.Text.Contains ("SelUser=All,SelGroup=All,SelTenant=OwningTenant"));
    }

    [Test]
    public void AccessControlListTest ()
    {
      var ace = TestHelper.CreateAceWithOwningGroup ();
      var acl = TestHelper.CreateAcl (ace);
      To.ConsoleLine.e (acl);
      // Note: test string is similar to AccessControlEntry test above, since rest of test would retest standard ToText sequence output functionality
      Assert.That (To.String.e (acl).CheckAndConvertToString (), NUnit.Framework.SyntaxHelpers.Text.Contains ("SelUser=All,SelGroup=OwningGroup,SelTenant=All"));
    }

    [Test]
    public void AccessTypeDefinitionTest ()
    {
      var x = TestHelper.CreateAccessType ("topsecret", 123);
      var basic = To.String.SetOutputComplexityToBasic().e (x).CheckAndConvertToString();
      var complex = To.String.SetOutputComplexityToComplex().e (x).CheckAndConvertToString();
      To.ConsoleLine.e (() => basic);
      To.ConsoleLine.e (() => complex);
      Assert.That (basic, NUnit.Framework.SyntaxHelpers.Text.Contains ("topsecret"));
      Assert.That (basic, NUnit.Framework.SyntaxHelpers.Text.Not.Contains ("123"));
      Assert.That (complex, NUnit.Framework.SyntaxHelpers.Text.Contains ("topsecret"));
      Assert.That (complex, NUnit.Framework.SyntaxHelpers.Text.Contains ("123"));
    }


    [Test]
    public void GroupTest ()
    {
      var x = TestHelper.CreateGroup ("DieGruppe", null, TestHelper.CreateTenant("DasAmt"));
      To.ConsoleLine.e (x);
      Assert.That (To.String.e (x).CheckAndConvertToString (), NUnit.Framework.SyntaxHelpers.Text.Contains ("DieGruppe"));
    }

    [Test]
    public void PermissionTest ()
    {
      var x = Permission.NewObject();
      x.AccessType = TestHelper.CreateAccessType ("topsecret", 123);
      x.Allowed = true;
      var result = To.String.e (x).CheckAndConvertToString();
      To.ConsoleLine.e (() => result);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("topsecret"));
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("True"));
    }

   [Test]
    public void PositionTest ()
    {
      var x = TestHelper.CreatePosition ("Praktikant");
      To.ConsoleLine.e (x);
      Assert.That (To.String.e (x).CheckAndConvertToString (), NUnit.Framework.SyntaxHelpers.Text.Contains ("Praktikant"));
    }

    [Test]
   public void RoleTest ()
    {
      var x = Role;
      To.ConsoleLine.e (x);
      var result = To.String.e (x).CheckAndConvertToString ();
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("DaUs"));
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("Da Group"));
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("Supreme Being"));
    }

    [Test]
    public void SecurityTokenTest ()
    {
      var x = new SecurityToken (User, Tenant, List.New (Group, Group2), List.New (TestHelper.CreateAbstractRoleDefinition("arole",456)));
      var result = To.String.e (x).CheckAndConvertToString ();
      To.ConsoleLine.e (() => result);
      //Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("\"DaUs\"],tenant=[\"Da Tenant\"]," + Environment.NewLine + "roles={[\"DaUs\",\"Da Group\",\"Supreme Being\"]}],[\"Da Tenant\"],{[\"Da Group\"],[\"Anotha Group\"]},{[\"DaUs\",\"Da Group\",\"Supreme Being\"]},{[\"arole\"]}]"));

      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("DaUs"));
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("Da Tenant"));
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("Da Group"));
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("Anotha Group"));
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("Supreme Being"));
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("arole"));
    }

    [Test]
    public void TenantTest ()
    {
      var x = TestHelper.CreateTenant ("Tenantative");
      To.ConsoleLine.e (x);
      Assert.That (To.String.e (x).CheckAndConvertToString (), NUnit.Framework.SyntaxHelpers.Text.Contains ("Tenantative"));
    }

    [Test]
    public void UserTest ()
    {
      var x = User;
      var basic = To.String.SetOutputComplexityToBasic ().e (x).CheckAndConvertToString ();
      var complex = To.String.SetOutputComplexityToComplex ().e (x).CheckAndConvertToString ();
      To.ConsoleLine.e (() => basic);
      To.ConsoleLine.e (() => complex);
      Assert.That (basic, Is.EqualTo("[\"DaUs\"]"));
      Assert.That (complex, NUnit.Framework.SyntaxHelpers.Text.Contains ("Da Group"));
      Assert.That (complex, NUnit.Framework.SyntaxHelpers.Text.Contains ("Da Tenant"));
      Assert.That (complex, NUnit.Framework.SyntaxHelpers.Text.Contains ("Supreme Being"));
    }


  }
}