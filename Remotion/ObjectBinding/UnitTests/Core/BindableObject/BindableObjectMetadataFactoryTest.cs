// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectMetadataFactoryTest
  {
    public class TestClass
    {
      public int Property
      {
        get { return 0; }
      }
    }

    [Test]
    public void Instantiate_WithMixin ()
    {
      using (MixinConfiguration.BuildNew().ForClass (typeof (BindableObjectMetadataFactory)).AddMixin<MixinStub>().EnterScope())
      {
        BindableObjectMetadataFactory factory = BindableObjectMetadataFactory.Create();
        Assert.That (factory, Is.InstanceOfType (typeof (BindableObjectMetadataFactory)));
        Assert.That (factory, Is.InstanceOfType (typeof (IMixinTarget)));
      }
    }

    [Test]
    public void CreateClassReflector ()
    {
      BindableObjectProvider provider = new BindableObjectProvider();
      IClassReflector classReflector = BindableObjectMetadataFactory.Create().CreateClassReflector (typeof (TestClass), provider);
      Assert.That (classReflector.TargetType, Is.SameAs (typeof (TestClass)));
      Assert.That (classReflector.BusinessObjectProvider, Is.SameAs (provider));
    }

    [Test]
    public void CreatePropertyFinder ()
    {
      IPropertyFinder finder = BindableObjectMetadataFactory.Create().CreatePropertyFinder (typeof (TestClass));
      Assert.AreSame (typeof (ReflectionBasedPropertyFinder), finder.GetType());
      Assert.AreSame (typeof (TestClass), new List<IPropertyInformation> (finder.GetPropertyInfos())[0].DeclaringType);
    }

    [Test]
    public void CreatePropertyReflector ()
    {
      using (MixinConfiguration.BuildNew ().EnterScope ())
      {
        PropertyInfo propertyInfo = typeof (TestClass).GetProperty ("Property");
        IPropertyInformation property = new PropertyInfoAdapter (propertyInfo);
        PropertyReflector propertyReflector =
            BindableObjectMetadataFactory.Create().CreatePropertyReflector (typeof (TestClass), property, new BindableObjectProvider());
        Assert.AreSame (typeof (PropertyReflector), propertyReflector.GetType());
        Assert.AreSame (property, propertyReflector.PropertyInfo);
      }
    }
  }
}
