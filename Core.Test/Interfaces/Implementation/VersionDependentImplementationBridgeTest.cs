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
using Remotion.Implementation;
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Interfaces.Implementation
{
  [TestFixture]
  public class VersionDependentImplementationBridgeTest
  {
    [ConcreteImplementation ("Remotion.UnitTests.Interfaces.Implementation.VersionDependentImplementationBridgeTest+VersionDependentImplementation, " 
        + "Remotion.UnitTests, Version = <version>")]
    public interface IVersionIndependentInterface { }
    public class VersionDependentImplementation : IVersionIndependentInterface { }

    [ConcreteImplementation ("Remotion.UnitTests.Interfaces.Implementation.VersionDependentImplementationBridgeTest+VersionDependentImplementation, " 
        + "Remotion.UnitTests, Version = <version>")]
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
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot get a version-dependent implementation of type "
        + "'System.IServiceProvider': Expected one ConcreteImplementationAttribute applied to the type, but found 0.")]
    public void Implementation_WithoutAttribute ()
    {
      try
      {
        Dev.Null = VersionDependentImplementationBridge<IServiceProvider>.Implementation;
      }
      catch (TypeInitializationException ex)
      {
        throw ex.InnerException;
      }
      catch
      {
        Assert.Fail ("Expected TypeInitializationException.");
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidCastException), ExpectedMessage = "Unable to cast object of type 'VersionDependentImplementation' to type " 
        + "'IVersionIndependentInterface2'.")]
    public void Implementation_WithInvalidAttributeType ()
    {
      try
      {
        Dev.Null = VersionDependentImplementationBridge<IVersionIndependentInterface2>.Implementation;
      }
      catch (TypeInitializationException ex)
      {
        throw ex.InnerException;
      }
      catch
      {
        Assert.Fail ("Expected TypeInitializationException.");
      }
    }
  }
}
