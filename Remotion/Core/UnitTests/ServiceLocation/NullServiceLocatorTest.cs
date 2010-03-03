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
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class NullServiceLocatorTest
  {
    [Test]
    public void GetInstance ()
    {
      Assert.That (NullServiceLocator.Instance, Is.Not.Null);
    }

    [Test]
    public void GetInstance_SameInstanceTwice ()
    {
      Assert.That (NullServiceLocator.Instance, Is.SameAs (NullServiceLocator.Instance));
    }

    [Test]
    public void GetService_ReturnsNull ()
    {
      Assert.That (((IServiceProvider) NullServiceLocator.Instance).GetService (typeof (int)), Is.Null);
    }

    [Test]
    public void GetInstance_ReturnsNull ()
    {
      Assert.That (NullServiceLocator.Instance.GetInstance (typeof (int)), Is.Null);
    }

    [Test]
    public void GetInstance_WithKey_ReturnsNull ()
    {
      Assert.That (NullServiceLocator.Instance.GetInstance (typeof (int), "key"), Is.Null);
    }

    [Test]
    public void GetInstanceOfT_ReturnsDefault ()
    {
      Assert.That (NullServiceLocator.Instance.GetInstance<IFormatProvider>(), Is.Null);
    }

    [Test]
    public void GetInstanceOfT_WithKey_ReturnsDefault ()
    {
      Assert.That (NullServiceLocator.Instance.GetInstance<IFormatProvider> ("key"), Is.Null);
    }

    [Test]
    public void GetAllInstances_ReturnsEmpty ()
    {
      Assert.That (NullServiceLocator.Instance.GetAllInstances (typeof (int)), Is.Empty);
    }

    [Test]
    public void GetAllInstancesOfT_ReturnsEmpty ()
    {
      Assert.That (NullServiceLocator.Instance.GetAllInstances<IFormatProvider>(), Is.Empty);
    }
  }
}