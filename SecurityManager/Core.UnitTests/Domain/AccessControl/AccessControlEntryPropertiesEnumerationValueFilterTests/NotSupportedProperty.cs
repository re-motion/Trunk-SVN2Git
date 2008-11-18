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
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryPropertiesEnumerationValueFilterTests
{
  [TestFixture]
  public class NotSupportedProperty : AccessControlEntryPropertiesEnumerationValueFilterTestBase
  {
    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The property 'GroupHierarchyCondition' is not supported by the "
        + "'Remotion.SecurityManager.Domain.AccessControl.AccessControlEntryPropertiesEnumerationValueFilter'.")]
    public void InvalidValue ()
    {
      var ace = CreateAceForStateless();
      var property = GetPropertyDefinition (ace, "GroupHierarchyCondition");
      Filter.IsEnabled (CreateEnumValueInfo ((GroupHierarchyCondition) 1000), ace, property);
    }
  }
}