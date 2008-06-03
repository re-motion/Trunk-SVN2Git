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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.BindableObjectMixinTests
{
  [TestFixture]
  public class DefaultValueTest
  {
    public class DefaultValueTrueMixin : Mixin<BindableObjectMixin>
    {
      [OverrideTarget]
      public bool IsDefaultValue (PropertyBase property, object nativeValue)
      {
        return true;
      }
    }

    [Test]
    public void GetProperty_NormallyReturnsNonNull ()
    {
      ClassWithValueType<int> instance = ObjectFactory.Create<ClassWithValueType<int>> ().With ();
      IBusinessObject instanceAsIBusinessObject = (IBusinessObject) instance;

      Assert.IsNotNull (instanceAsIBusinessObject.GetProperty ("Scalar"));
      Assert.AreEqual (instance.Scalar, instanceAsIBusinessObject.GetProperty ("Scalar"));
    }

    [Test]
    public void GetProperty_ReturnsNull_WhenDefaultValueTrue ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (BindableObjectMixin)).AddMixins (typeof (DefaultValueTrueMixin)).EnterScope())
      {
        ClassWithValueType<int> instance = ObjectFactory.Create<ClassWithValueType<int>>().With();
        IBusinessObject instanceAsIBusinessObject = (IBusinessObject) instance;

        Assert.IsNull (instanceAsIBusinessObject.GetProperty ("Scalar"));
      }
    }

    [Test]
    public void GetProperty_ReturnsNonNull_WhenDefaultValueTrueOnList ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (BindableObjectMixin)).AddMixins (typeof (DefaultValueTrueMixin)).EnterScope())
      {
        ClassWithValueType<int> instance = ObjectFactory.Create<ClassWithValueType<int>> ().With ();
        IBusinessObject instanceAsIBusinessObject = (IBusinessObject) instance;

        Assert.IsNotNull (instanceAsIBusinessObject.GetProperty ("List"));
        Assert.AreEqual (instance.List, instanceAsIBusinessObject.GetProperty ("List"));
      }
    }
  }
}
