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
  public class UserConditionStateless : AccessControlEntryPropertiesEnumerationValueFilterTestBase
  {
    private IBusinessObjectEnumerationProperty _property;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp();
      _ace = CreateAceForStateless();
      _property = GetPropertyDefinition (_ace, "UserCondition");
    }

    [Test]
    public void None ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo (UserCondition.None), _ace, _property), Is.True);
    }

    [Test]
    public void None_Disabled ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo_Disabled (UserCondition.None), _ace, _property), Is.False);
    }

    [Test]
    public void Owner ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo (UserCondition.Owner), _ace, _property), Is.False);
    }

    [Test]
    public void Owner_Disabled ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo_Disabled (UserCondition.Owner), _ace, _property), Is.False);
    }

    [Test]
    public void SpecificUser ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo (UserCondition.SpecificUser), _ace, _property), Is.True);
    }

    [Test]
    public void SpecificUser_Disabled ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo_Disabled (UserCondition.SpecificUser), _ace, _property), Is.False);
    }

    [Test]
    public void SpecificPosition ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo (UserCondition.SpecificPosition), _ace, _property), Is.True);
    }

    [Test]
    public void SpecificPosition_Disabled ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo_Disabled (UserCondition.SpecificPosition), _ace, _property), Is.False);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The value '1000' is not a valid value for 'UserCondition'.")]
    public void InvalidValue ()
    {
      Filter.IsEnabled (CreateEnumValueInfo ((UserCondition) 1000), _ace, _property);
    }
  }
}