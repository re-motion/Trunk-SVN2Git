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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.BindableObjectMixinTests
{
  [TestFixture]
  public class SetProperty : TestBase
  {
    private SimpleBusinessObjectClass _bindableObject;
    private BindableObjectMixin _bindableObjectMixin;
    private IBusinessObject _businessObject;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObject = ObjectFactory.Create<SimpleBusinessObjectClass>().With();
      _bindableObjectMixin = Mixin.Get<BindableObjectMixin> (_bindableObject);
      _businessObject = _bindableObjectMixin;
    }

    [Test]
    public void WithBusinessObjectProperty ()
    {
      _businessObject.SetProperty (_businessObject.BusinessObjectClass.GetPropertyDefinition ("String"), "A String");

      Assert.That (_bindableObject.String, Is.EqualTo ("A String"));
    }

    [Test]
    public void WithPropertyIdentifier ()
    {
      _businessObject.SetProperty ("String", "A String");

      Assert.That (_bindableObject.String, Is.EqualTo ("A String"));
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException), ExpectedMessage = "The property 'StringWithoutSetter' was not found on business object class "
        + "'Remotion.ObjectBinding.UnitTests.Core.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests'.")]
    [Ignore ("TODO: discuss desired behavior")]
    public void WithoutSetter ()
    {
      IBusinessObject businessObject = Mixin.Get<BindableObjectMixin> (ObjectFactory.Create<SimpleBusinessObjectClass>().With());
      businessObject.SetProperty ("StringWithoutSetter", null);
    }
  }
}
