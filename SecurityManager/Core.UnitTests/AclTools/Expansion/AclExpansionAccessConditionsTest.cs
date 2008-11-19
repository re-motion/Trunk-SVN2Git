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
using Remotion.Reflection;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.Domain.AccessControl;
using NUnitText = NUnit.Framework.SyntaxHelpers.Text;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionAccessConditionsTest : AclToolsTestBase
  {
    [Test]
    public void DefaultCtor ()
    {
      var accessConditions = new AclExpansionAccessConditions ();
      Assert.That (accessConditions.AbstractRole, Is.Null);
      Assert.That (accessConditions.IsAbstractRoleRequired, Is.EqualTo(false));
      Assert.That (accessConditions.HasOwningGroupCondition, Is.EqualTo (false));
      Assert.That (accessConditions.IsOwningTenantRequired, Is.EqualTo (false));
      Assert.That (accessConditions.IsOwningUserRequired, Is.EqualTo (false));

      Assert.That (accessConditions.OwningGroup, Is.EqualTo (null));
      Assert.That (accessConditions.GroupHierarchyCondition, Is.EqualTo (GroupHierarchyCondition.Undefined));
    }

    [Test]
    public void Equals ()
    {
      //BooleanMemberTest ((aeac, b) => aeac.HasOwningGroupCondition = b);
      BooleanMemberTest ((aeac, b) => aeac.IsOwningTenantRequired = b);
      BooleanMemberTest ((aeac, b) => aeac.IsOwningUserRequired = b);
    }

    [Test]
    public void Equals_UsingLambda ()
    {
      //CheckIfPassedPropertyChangeChangesEquality<AclExpansionAccessConditions> (aeac => aeac.HasOwningGroupCondition = true);
      CheckIfPassedPropertyChangeChangesEquality<AclExpansionAccessConditions> (aeac => aeac.IsOwningTenantRequired = true);
      CheckIfPassedPropertyChangeChangesEquality<AclExpansionAccessConditions> (aeac => aeac.IsOwningUserRequired = true);
      CheckIfPassedPropertyChangeChangesEquality<AclExpansionAccessConditions> (aeac => aeac.AbstractRole = TestHelper.CreateAbstractRoleDefinition ("titatutest", 11235));
      CheckIfPassedPropertyChangeChangesEquality<AclExpansionAccessConditions> (aeac => aeac.OwningGroup = Group2);
    }


    // Check Equals operator for each property of AclExpansionAccessConditions if changing the property from its default value 
    // leaves to two instances being unequal.
    [Test]
    public void Equals_UsingPropertyObject ()
    {
      //CheckIfPassedPropertyChangeChangesEquality (Properties<AclExpansionAccessConditions>.Get (aeac => aeac.HasOwningGroupCondition), true);
      CheckIfPassedPropertyChangeChangesEquality (Properties<AclExpansionAccessConditions>.Get (aeac => aeac.IsOwningTenantRequired), true);
      CheckIfPassedPropertyChangeChangesEquality (Properties<AclExpansionAccessConditions>.Get (aeac => aeac.IsOwningUserRequired), true);
      CheckIfPassedPropertyChangeChangesEquality (Properties<AclExpansionAccessConditions>.Get (aeac => aeac.AbstractRole), TestHelper.CreateAbstractRoleDefinition ("titatutest", 11235));
      CheckIfPassedPropertyChangeChangesEquality (Properties<AclExpansionAccessConditions>.Get (aeac => aeac.OwningGroup), Group3);
      CheckIfPassedPropertyChangeChangesEquality (Properties<AclExpansionAccessConditions>.Get (aeac => aeac.GroupHierarchyCondition), GroupHierarchyCondition.ThisAndParentAndChildren);
    }


    [Test]
    public void ToTextDefaultConstructed ()
    {
      var accessConditions = new AclExpansionAccessConditions ();
      var result = To.String.e (accessConditions).CheckAndConvertToString ();
      //To.Console.s (result);
      Assert.That (result, Is.EqualTo ("[]"));
    }

    [Test]
    public void ToTextTest ()
    {
      var accessConditions = new AclExpansionAccessConditions ();
      accessConditions.AbstractRole = TestHelper.CreateAbstractRoleDefinition ("xyz", 123);
      accessConditions.OwningGroup = Group;
      accessConditions.IsOwningTenantRequired = true;
      accessConditions.IsOwningUserRequired = true;
      accessConditions.OwningGroup = Group2;
      accessConditions.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParentAndChildren;
      var result = To.String.e (accessConditions).CheckAndConvertToString ();
      To.Console.s (result);
      Assert.That (result, Is.EqualTo (@"[userMustOwn=True,owningGroup=[""Anotha Group""],groupHierarchyCondition=ThisAndParentAndChildren,tenantMustOwn=True,abstractRoleMustMatch=True,abstractRole=[""xyz""]]"));
    }



    private void BooleanMemberTest (Action<AclExpansionAccessConditions, bool> setBoolProperty)
    {
      var accessConditions0 = new AclExpansionAccessConditions ();
      var accessConditions1 = new AclExpansionAccessConditions ();
      Assert.That (accessConditions0.Equals (accessConditions1), Is.True);
      setBoolProperty (accessConditions1, true);
      Assert.That (accessConditions0.Equals (accessConditions1), Is.False);
    }


    // Check if applying the passed Action to only one instance of AclExpansionAccessConditions flips equality.
    private void CheckIfPassedPropertyChangeChangesEquality<TProperty> (Action<AclExpansionAccessConditions> changeProperty)
    {
      var accessConditions0 = new AclExpansionAccessConditions ();
      var accessConditions1 = new AclExpansionAccessConditions ();
      Assert.That (accessConditions0.Equals (accessConditions1), Is.True);
      changeProperty(accessConditions1);
      Assert.That (accessConditions0.Equals (accessConditions1), Is.False);
    }


    // Check if changing the passed Property of AclExpansionAccessConditions in only one instance flips equality.
    private void CheckIfPassedPropertyChangeChangesEquality<TProperty> (Property<AclExpansionAccessConditions, TProperty> boolProperty, TProperty notEqualValue)
    {
      var accessConditions0 = new AclExpansionAccessConditions ();
      var accessConditions1 = new AclExpansionAccessConditions ();
      Assert.That (accessConditions0.Equals (accessConditions1), Is.True);
      Assert.That (accessConditions0.GetHashCode (), Is.EqualTo (accessConditions1.GetHashCode ()));
      boolProperty.Set (accessConditions1, notEqualValue);
      Assert.That (accessConditions0.Equals (accessConditions1), Is.False);
      Assert.That (accessConditions0.GetHashCode (), Is.Not.EqualTo (accessConditions1.GetHashCode ()));
    }


  }
}
