// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
// This file is part of re-vision (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectClassWithIdentityTest : TestBase
  {
    private BindableObjectProvider _bindableObjectProvider;
    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObjectProvider = new BindableObjectProvider();
      _mockRepository = new MockRepository();
    }

    [Test]
    public void Initialize ()
    {
      var bindableObjectClass = new BindableObjectClassWithIdentity (
          MixinTypeUtility.GetConcreteMixedType (typeof (ClassWithIdentity)), _bindableObjectProvider, new PropertyBase[0]);

      Assert.That (bindableObjectClass.TargetType, Is.SameAs (typeof (ClassWithIdentity)));
      Assert.That (
          bindableObjectClass.Identifier,
          Is.EqualTo ("Remotion.ObjectBinding.UnitTests.Core.TestDomain.ClassWithIdentity, Remotion.ObjectBinding.UnitTests"));
      Assert.That (bindableObjectClass.RequiresWriteBack, Is.False);
      Assert.That (bindableObjectClass.BusinessObjectProvider, Is.SameAs (_bindableObjectProvider));
    }

    [Test]
    public void GetObject_WithDefaultService ()
    {
      var bindableObjectClass = new BindableObjectClassWithIdentity (
          MixinTypeUtility.GetConcreteMixedType (typeof (ClassWithIdentity)), _bindableObjectProvider, new PropertyBase[0]);
      var mockService = _mockRepository.StrictMock<IGetObjectService>();
      var expected = _mockRepository.Stub<IBusinessObjectWithIdentity>();

      Expect.Call (mockService.GetObject (bindableObjectClass, "TheUniqueIdentifier")).Return (expected);
      _mockRepository.ReplayAll();

      _bindableObjectProvider.AddService (typeof (IGetObjectService), mockService);
      IBusinessObjectWithIdentity actual = bindableObjectClass.GetObject ("TheUniqueIdentifier");

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void GetObject_WithCustomService ()
    {
      var bindableObjectClass = new BindableObjectClassWithIdentity (
          MixinTypeUtility.GetConcreteMixedType (typeof (ClassWithIdentityAndGetObjectServiceAttribute)), _bindableObjectProvider, new PropertyBase[0]);
      var mockService = _mockRepository.StrictMock<ICustomGetObjectService>();
      var expected = _mockRepository.Stub<IBusinessObjectWithIdentity>();

      Expect.Call (mockService.GetObject (bindableObjectClass, "TheUniqueIdentifier")).Return (expected);
      _mockRepository.ReplayAll();

      _bindableObjectProvider.AddService (typeof (ICustomGetObjectService), mockService);
      IBusinessObjectWithIdentity actual = bindableObjectClass.GetObject ("TheUniqueIdentifier");

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
            "The 'Remotion.ObjectBinding.BindableObject.IGetObjectService' required for loading objectes of type "
            + "'Remotion.ObjectBinding.UnitTests.Core.TestDomain.ClassWithIdentity' is not registered with the "
            + "'Remotion.ObjectBinding.BusinessObjectProvider' associated with this type.")]
    public void GetObject_WithoutService ()
    {
      var bindableObjectClass = new BindableObjectClassWithIdentity (
          MixinTypeUtility.GetConcreteMixedType (typeof (ClassWithIdentity)), _bindableObjectProvider, new PropertyBase[0]);

      bindableObjectClass.GetObject ("TheUniqueIdentifier");
    }
  }
}
