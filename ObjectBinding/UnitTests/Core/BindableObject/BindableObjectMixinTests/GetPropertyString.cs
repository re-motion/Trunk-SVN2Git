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
using Rhino.Mocks;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.BindableObjectMixinTests
{
  [TestFixture]
  public class GetPropertyString : TestBase
  {
    private IBusinessObject _businessObject;
    private MockRepository _mockRepository;
    private IBusinessObjectStringFormatterService _mockStringFormatterService;
    private IBusinessObjectProperty _property;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository();
      _mockStringFormatterService = _mockRepository.CreateMock<IBusinessObjectStringFormatterService>();
      BindableObjectProvider provider = new BindableObjectProvider();
      provider.AddService (typeof (IBusinessObjectStringFormatterService), _mockStringFormatterService);
      BusinessObjectProvider.SetProvider(typeof (BindableObjectProviderAttribute), provider);
      
      _businessObject = (IBusinessObject) ObjectFactory.Create<SimpleBusinessObjectClass> ().With();

      _property = _businessObject.BusinessObjectClass.GetPropertyDefinition ("String");
      Assert.That (
          _property, Is.Not.Null, "Property 'String' was not found on BusinessObjectClass '{0}'", _businessObject.BusinessObjectClass.Identifier);

      BusinessObjectProvider.SetProvider (typeof (BindableObjectProviderAttribute), new BindableObjectProvider ());
    }

    [Test]
    public void FromProperty ()
    {
      Expect.Call (_mockStringFormatterService.GetPropertyString (_businessObject, _property, "TheFormatString")).Return ("TheStringValue");
      _mockRepository.ReplayAll();

      string actual = _businessObject.GetPropertyString (_property, "TheFormatString");
      
      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("TheStringValue"));
    }

    [Test]
    public void FromIdentifier ()
    {
      Expect.Call (_mockStringFormatterService.GetPropertyString (_businessObject, _property, null)).Return ("TheStringValue");
      _mockRepository.ReplayAll ();

      string actual = _businessObject.GetPropertyString ("String");

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.EqualTo ("TheStringValue"));
    }
  }
}
