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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests
{
  [TestFixture]
  public class EnumerationValueFilterTest : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();

    }

    [Test]
    public void UserConditionStateless ()
    {
      var ace = CreateAceForStateless();
      var property = GetPropertyDefinition (ace, "UserCondition");

      Assert.That (
          property.GetEnabledValues (ace).Select (value => value.Value).ToArray(),
          List.Not.Contains (new[] { UserCondition.Owner }));
    }

    [Test]
    public void UserConditionStateful ()
    {
      var ace = CreateAceForStateful ();
      var property = GetPropertyDefinition (ace, "UserCondition");

      Assert.That (
          property.GetEnabledValues (ace).Select (value => value.Value).ToArray (),
          Is.EquivalentTo (Enum.GetValues (typeof (UserCondition))));
    }

    [Test]
    public void GroupConditionStateful ()
    {
      var ace = CreateAceForStateful ();
      var property = GetPropertyDefinition (ace, "GroupCondition");

      Assert.That (
          property.GetEnabledValues (ace).Select (value => value.Value).ToArray (),
          List.Not.Contains (new[] { GroupCondition.OwningGroup, GroupCondition.BranchOfOwningGroup}));
    }

    [Test]
    public void GroupConditionStateless ()
    {
      var ace = CreateAceForStateful ();
      var property = GetPropertyDefinition (ace, "GroupCondition");

      Assert.That (
          property.GetEnabledValues (ace).Select (value => value.Value).ToArray (),
          Is.EquivalentTo (Enum.GetValues (typeof (GroupCondition))));
    }

    [Test]
    public void TenantConditionStateful ()
    {
      var ace = CreateAceForStateful ();
      var property = GetPropertyDefinition (ace, "TenantCondition");

      Assert.That (
          property.GetEnabledValues (ace).Select (value => value.Value).ToArray (),
          List.Not.Contains (new[] { TenantCondition.OwningTenant }));
    }

    [Test]
    public void TenantConditionStateless ()
    {
      var ace = CreateAceForStateful ();
      var property = GetPropertyDefinition (ace, "TenantCondition");

      Assert.That (
          property.GetEnabledValues (ace).Select (value => value.Value).ToArray (),
          Is.EquivalentTo (Enum.GetValues (typeof (TenantCondition))));
    }

    protected IEnumerationValueInfo CreateEnumValueInfo (Enum enumValue)
    {
      return new EnumerationValueInfo (enumValue, "ID", "Name", true);
    }

    protected IEnumerationValueInfo CreateEnumValueInfo_Disabled (Enum enumValue)
    {
      return new EnumerationValueInfo (enumValue, "ID", "Name", false);
    }

    protected AccessControlEntry CreateAceForStateless ()
    {
      var ace = AccessControlEntry.NewObject ();
      ace.AccessControlList = StatelessAccessControlList.NewObject ();

      return ace;
    }

    protected AccessControlEntry CreateAceForStateful ()
    {
      var ace = AccessControlEntry.NewObject ();
      ace.AccessControlList = StatefulAccessControlList.NewObject ();

      return ace;
    }

    protected IBusinessObjectEnumerationProperty GetPropertyDefinition (AccessControlEntry ace, string propertyName)
    {
      var property = (IBusinessObjectEnumerationProperty) ((IBusinessObject) ace).BusinessObjectClass.GetPropertyDefinition (propertyName);
      Assert.That (property, Is.Not.Null, propertyName);
      return property;
    }
  }
}