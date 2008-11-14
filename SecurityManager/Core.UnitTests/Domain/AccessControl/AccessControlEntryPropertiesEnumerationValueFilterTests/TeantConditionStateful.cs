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
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryPropertiesEnumerationValueFilterTests
{
  [TestFixture]
  public class TenantConditionStateful : AccessControlEntryPropertiesEnumerationValueFilterTestBase
  {
    private IBusinessObjectEnumerationProperty _property;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp();
      _ace = CreateAceForStateful();
      _property = GetPropertyDefinition (_ace, "TenantCondition");
    }

    [Test]
    public void None ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo (TenantCondition.None), _ace, _property), Is.True);
    }

    [Test]
    public void None_Disabled ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo_Disabled (TenantCondition.None), _ace, _property), Is.False);
    }

    [Test]
    public void OwningTenant ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo (TenantCondition.OwningTenant), _ace, _property), Is.True);
    }

    [Test]
    public void OwningTenant_Disabled ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo_Disabled (TenantCondition.OwningTenant), _ace, _property), Is.False);
    }

    [Test]
    public void SpecificTenant ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo (TenantCondition.SpecificTenant), _ace, _property), Is.True);
    }

    [Test]
    public void SpecificTenant_Disabled ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo_Disabled (TenantCondition.SpecificTenant), _ace, _property), Is.False);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The value '1000' is not a valid value for 'TenantCondition'.")]
    public void InvalidValue ()
    {
      Filter.IsEnabled (CreateEnumValueInfo ((TenantCondition) 1000), _ace, _property);
    }
  }
}