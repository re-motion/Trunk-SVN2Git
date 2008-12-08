// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
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
  // TODO AE: Remove commented code. (Do not commit.)
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
      Assert.That (accessConditions.HasOwningTenantCondition, Is.EqualTo (false));
      Assert.That (accessConditions.IsOwningUserRequired, Is.EqualTo (false));

      Assert.That (accessConditions.OwningGroup, Is.EqualTo (null));
      Assert.That (accessConditions.GroupHierarchyCondition, Is.EqualTo (GroupHierarchyCondition.Undefined));
    }

    // TODO AE: Typical pattern:
    // TODO AE: Equals_True => create two with equal members, assert they are equal
    // TODO AE: Equals_False_Prop1, Equals_False_Prop2, Equals_False_PropN => create two with equal members but one, assert they are not equal
    // TODO AE: GetHashCode_Equal => create two with equal members, assert hash codes are equal
    // TODO AE: GetHashCode_NotEqual should not be tested, too volatile

    [Test]
    public void Equals ()
    {
      //BooleanMemberTest ((aeac, b) => aeac.HasOwningGroupCondition = b);
      //BooleanMemberTest ((aeac, b) => aeac.HasOwningTenantCondition = b);
      BooleanMemberTest ((aeac, b) => aeac.IsOwningUserRequired = b); // TODO AE: Inline this member, remove method.
    }

    //[Test]
    //public void Equals_UsingLambda ()
    //{
    //  //CheckIfPassedPropertyChangeChangesEquality<AclExpansionAccessConditions> (aeac => aeac.HasOwningGroupCondition = true);
    //  CheckIfPassedPropertyChangeChangesEquality<AclExpansionAccessConditions> (aeac => aeac.HasOwningTenantCondition = true);
    //  CheckIfPassedPropertyChangeChangesEquality<AclExpansionAccessConditions> (aeac => aeac.IsOwningUserRequired = true);
    //  CheckIfPassedPropertyChangeChangesEquality<AclExpansionAccessConditions> (aeac => aeac.AbstractRole = TestHelper.CreateAbstractRoleDefinition ("titatutest", 11235));
    //  CheckIfPassedPropertyChangeChangesEquality<AclExpansionAccessConditions> (aeac => aeac.OwningGroup = Group2);
    //}


    // Check Equals operator for each property of AclExpansionAccessConditions if changing the property from its default value 
    // leaves to two instances being unequal. // TODO AE: leads to two or leaves two? :)
    [Test]
    public void Equals_UsingPropertyObject ()
    {
      //CheckIfPassedPropertyChangeChangesEquality (Properties<AclExpansionAccessConditions>.Get (aeac => aeac.HasOwningGroupCondition), true);
      //CheckIfPassedPropertyChangeChangesEquality (Properties<AclExpansionAccessConditions>.Get (aeac => aeac.HasOwningTenantCondition), true);
      CheckIfPassedPropertyChangeChangesEquality (Properties<AclExpansionAccessConditions>.Get (aeac => aeac.IsOwningUserRequired), true);
      CheckIfPassedPropertyChangeChangesEquality (Properties<AclExpansionAccessConditions>.Get (aeac => aeac.AbstractRole), TestHelper.CreateAbstractRoleDefinition ("titatutest", 11235));
      CheckIfPassedPropertyChangeChangesEquality (Properties<AclExpansionAccessConditions>.Get (aeac => aeac.OwningGroup), Group3);
      CheckIfPassedPropertyChangeChangesEquality (Properties<AclExpansionAccessConditions>.Get (aeac => aeac.GroupHierarchyCondition), GroupHierarchyCondition.ThisAndParentAndChildren);
      CheckIfPassedPropertyChangeChangesEquality (Properties<AclExpansionAccessConditions>.Get (aeac => aeac.OwningTenant), Tenant);
      CheckIfPassedPropertyChangeChangesEquality (Properties<AclExpansionAccessConditions>.Get (aeac => aeac.TenantHierarchyCondition), TenantHierarchyCondition.ThisAndParent);
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
      //accessConditions.HasOwningTenantCondition = true;
      accessConditions.IsOwningUserRequired = true;
      accessConditions.OwningGroup = Group2;
      accessConditions.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParentAndChildren;
      accessConditions.OwningTenant = Tenant;
      accessConditions.TenantHierarchyCondition = TenantHierarchyCondition.ThisAndParent;
      var result = To.String.e (accessConditions).CheckAndConvertToString ();
      // To.Console.s (result);
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
    // TODO AE: Remove unused method.
    private void CheckIfPassedPropertyChangeChangesEquality<TProperty> (Action<AclExpansionAccessConditions> changeProperty)
    {
      var accessConditions0 = new AclExpansionAccessConditions ();
      var accessConditions1 = new AclExpansionAccessConditions ();
      Assert.That (accessConditions0.Equals (accessConditions1), Is.True);
      changeProperty(accessConditions1);
      Assert.That (accessConditions0.Equals (accessConditions1), Is.False);
    }


    // Check if changing the passed Property of AclExpansionAccessConditions in only one instance flips equality. // TODO AE: Seems obvious, given the method name.
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
