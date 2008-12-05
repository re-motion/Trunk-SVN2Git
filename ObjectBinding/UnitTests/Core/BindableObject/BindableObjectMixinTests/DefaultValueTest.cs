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
