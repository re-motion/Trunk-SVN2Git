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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Configuration;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.UnitTests.Configuration;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests
{
  [TestFixture]
  public class EnumerationValueFilterTest : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp();

      ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();
    }

    [Test]
    public void GetEnabledEnumValuesFor_UserCondition_Stateful ()
    {
      var ace = CreateAceForStateful();
      var property = GetPropertyDefinition (ace, "UserCondition");
      Assert.That (SecurityManagerConfiguration.Current.AccessControl.DisableSpecificUser, Is.False);

      Assert.That (
          property.GetEnabledValues (ace).Select (value => value.Value).ToArray(),
          Is.EquivalentTo (Enum.GetValues (typeof (UserCondition))));
    }

    [Test]
    public void GetEnabledEnumValuesFor_UserCondition_Stateless ()
    {
      var ace = CreateAceForStateless();
      var property = GetPropertyDefinition (ace, "UserCondition");

      Assert.That (property.GetEnabledValues (ace).Select (value => value.Value).ToArray(), List.Not.Contains (UserCondition.Owner));
    }

    [Test]
    public void GetEnabledEnumValuesFor_GroupCondition_Stateful ()
    {
      var ace = CreateAceForStateful();
      var property = GetPropertyDefinition (ace, "GroupCondition");

      Assert.That (
          property.GetEnabledValues (ace).Select (value => value.Value).ToArray(), 
          Is.EquivalentTo (Enum.GetValues (typeof (GroupCondition))));
    }

    [Test]
    public void GetEnabledEnumValuesFor_GroupCondition_Stateless ()
    {
      var ace = CreateAceForStateless();
      var property = GetPropertyDefinition (ace, "GroupCondition");

      var actual = property.GetEnabledValues (ace).Select (value => value.Value).ToArray();
      Assert.That (actual, List.Not.Contains (GroupCondition.OwningGroup));
      Assert.That (actual, List.Not.Contains (GroupCondition.BranchOfOwningGroup));
    }

    [Test]
    public void GetEnabledEnumValuesFor_TenantCondition_Statefull ()
    {
      var ace = CreateAceForStateful();
      var property = GetPropertyDefinition (ace, "TenantCondition");

      Assert.That (
          property.GetEnabledValues (ace).Select (value => value.Value).ToArray(), 
          Is.EquivalentTo (Enum.GetValues (typeof (TenantCondition))));
    }

    [Test]
    public void GetEnabledEnumValuesFor_TenantCondition_Stateless ()
    {
      var ace = CreateAceForStateless();
      var property = GetPropertyDefinition (ace, "TenantCondition");

      Assert.That (property.GetEnabledValues (ace).Select (value => value.Value).ToArray(), List.Not.Contains (TenantCondition.OwningTenant));
    }

    [Test]
    public void GetEnabledEnumValuesFor_TenantHierarchyCondition ()
    {
      var ace = CreateAceForStateful();
      var property = GetPropertyDefinition (ace, "TenantHierarchyCondition");

      Assert.That (property.GetEnabledValues (ace).Select (value => value.Value).ToArray(), List.Not.Contains (TenantHierarchyCondition.Parent));
    }

    [Test]
    public void GetEnabledEnumValuesFor_GroupHierarchyCondition ()
    {
      var ace = CreateAceForStateful();
      var property = GetPropertyDefinition (ace, "GroupHierarchyCondition");

      var actual = property.GetEnabledValues (ace).Select (value => value.Value).ToArray();
      Assert.That (actual, List.Not.Contains (GroupHierarchyCondition.Parent));
      Assert.That (actual, List.Not.Contains (GroupHierarchyCondition.Children));
    }

    private AccessControlEntry CreateAceForStateless ()
    {
      var ace = AccessControlEntry.NewObject();
      ace.AccessControlList = StatelessAccessControlList.NewObject();

      return ace;
    }

    private AccessControlEntry CreateAceForStateful ()
    {
      var ace = AccessControlEntry.NewObject();
      ace.AccessControlList = StatefulAccessControlList.NewObject();

      return ace;
    }

    private IBusinessObjectEnumerationProperty GetPropertyDefinition (AccessControlEntry ace, string propertyName)
    {
      var property = (IBusinessObjectEnumerationProperty) ((IBusinessObject) ace).BusinessObjectClass.GetPropertyDefinition (propertyName);
      Assert.That (property, Is.Not.Null, propertyName);
      return property;
    }
  }
}
