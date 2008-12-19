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
using Remotion.Development.UnitTesting;
using Remotion.Implementation;

namespace Remotion.UnitTests.Interfaces.Implementation
{
  [TestFixture]
  public class VersionDependentImplementationBridgeTest
  {
    [ConcreteImplementation ("Remotion.UnitTests.Interfaces.Implementation.VersionDependentImplementationBridgeTest+VersionDependentImplementation, "
        + "Remotion.UnitTests, Culture = neutral, Version = <version>")]
    public interface IVersionIndependentInterface { }
    public class VersionDependentImplementation : IVersionIndependentInterface { }

    [ConcreteImplementation ("Remotion.UnitTests.Interfaces.Implementation.VersionDependentImplementationBridgeTest+VersionDependentImplementation, "
        + "Remotion.UnitTests, Culture = neutral, Version = <version>")]
    public interface IVersionIndependentInterface2 { }

    [SetUp]
    public void SetUp ()
    {
      FrameworkVersion.Reset();
      FrameworkVersion.Value = typeof (INullObject).Assembly.GetName().Version;
    }

    [TearDown]
    public void TearDown ()
    {
      FrameworkVersion.Reset ();
    }

    [Test]
    public void Implementation ()
    {
      Assert.IsInstanceOfType (typeof (VersionDependentImplementation), VersionDependentImplementationBridge<IVersionIndependentInterface>.Implementation);
    }

    [Test]
    [ExpectedException (typeof (VersionDependentImplementationException), ExpectedMessage = "The initialization of type 'System.IServiceProvider' "
        + "threw an exception of type 'InvalidOperationException': Cannot get a version-dependent implementation of type "
        + "'System.IServiceProvider': Expected one ConcreteImplementationAttribute applied to the type, but found 0.")]
    public void Implementation_WithoutAttribute ()
    {
      Dev.Null = VersionDependentImplementationBridge<IServiceProvider>.Implementation;
    }

    [Test]
    [ExpectedException (typeof (VersionDependentImplementationException), ExpectedMessage = "The initialization of type "
        + "'Remotion.UnitTests.Interfaces.Implementation.VersionDependentImplementationBridgeTest+IVersionIndependentInterface2' "
        + "threw an exception of type 'InvalidCastException': Unable to cast object of type 'VersionDependentImplementation' to type " 
        + "'IVersionIndependentInterface2'.")]
    public void Implementation_WithInvalidAttributeType ()
    {
      Dev.Null = VersionDependentImplementationBridge<IVersionIndependentInterface2>.Implementation;
    }
  }
}
