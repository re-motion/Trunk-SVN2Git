// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System;
using System.Linq;
using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class ITypeConverterFactoryTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = new DefaultServiceLocator();
    }

    [Test]
    public void GetAllInstances_Once ()
    {
      var instances = _serviceLocator.GetAllInstances<ITypeConverterFactory>();

      Assert.That (instances, Is.Not.Null);
      Assert.That (
          instances.Select (i => i.GetType()),
          Is.EqualTo (new[] { typeof (AttributeBasedTypeConverterFactory), typeof (EnumTypeConverterFactory) }));
    }

    [Test]
    public void GetAllInstances_Twice_ReturnsSameInstances ()
    {
      var instances1 = _serviceLocator.GetAllInstances<ITypeConverterFactory>();
      var instances2 = _serviceLocator.GetAllInstances<ITypeConverterFactory>();

      Assert.That (instances1, Is.EqualTo (instances2));
    }
  }
}