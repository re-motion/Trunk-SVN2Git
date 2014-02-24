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
using Remotion.ExtensibleEnums.Infrastructure;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ExtensibleEnums.UnitTests.Infrastructure
{
  [TestFixture]
  public class ITypeConverterFactoryTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
    }

    [Test]
    public void GetAllInstances_Once ()
    {
      var instances = _serviceLocator.GetAllInstances<ITypeConverterFactory>().ToArray();

      Assert.That (instances.First(), Is.TypeOf<AttributeBasedTypeConverterFactory>());
      Assert.That (instances.Last(), Is.TypeOf<ExtensibleEnumTypeConverterFactory>());
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