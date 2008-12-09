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


    // Check Equals operator for each property of AclExpansionAccessConditions whether changing the property from its default value 
    // leads to inequality. 
    [Test]
    public void Equals_UsingPropertyObject ()
    {
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
      Assert.That (result, Is.EqualTo ("[]"));
    }

    [Test]
    public void ToTextTest ()
    {
      var accessConditions = new AclExpansionAccessConditions ();
      accessConditions.AbstractRole = TestHelper.CreateAbstractRoleDefinition ("xyz", 123);
      accessConditions.IsOwningUserRequired = true;
      accessConditions.OwningGroup = Group2;
      accessConditions.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParentAndChildren;
      accessConditions.OwningTenant = Tenant;
      accessConditions.TenantHierarchyCondition = TenantHierarchyCondition.ThisAndParent;
      var result = To.String.e (accessConditions).CheckAndConvertToString ();
      Assert.That (result, Is.EqualTo (@"[userMustOwn=True,owningGroup=[""Anotha Group""],groupHierarchyCondition=ThisAndParentAndChildren,tenantMustOwn=True,abstractRoleMustMatch=True,abstractRole=[""xyz""]]"));
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
      // Note: We do not assert here that GetHashCode() returns a different result, since this cannot be guaranteed.
    }


  }
}
