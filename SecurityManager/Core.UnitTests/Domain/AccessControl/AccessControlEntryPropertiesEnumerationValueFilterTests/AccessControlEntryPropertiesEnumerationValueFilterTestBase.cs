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
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryPropertiesEnumerationValueFilterTests
{
  public class AccessControlEntryPropertiesEnumerationValueFilterTestBase : DomainTest
  {
    private IEnumerationValueFilter _filter;
    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();

      _filter = new AccessControlEntryPropertiesEnumerationValueFilter();
    }

    protected IEnumerationValueFilter Filter
    {
      get { return _filter; }
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