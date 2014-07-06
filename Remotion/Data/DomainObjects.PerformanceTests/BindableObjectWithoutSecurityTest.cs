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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  [TestFixture]
  public class BindableObjectWithoutSecurityTest : BindableObjectTestBase
  {
    private ServiceLocatorScope _serviceLocatorScope;

    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp ()
    {
      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterMultiple<IObjectSecurityAdapter>();
      _serviceLocatorScope = new ServiceLocatorScope (serviceLocator);
    }

    [SetUp]
    public void SetUp ()
    {
      SecurityConfiguration.Current.SecurityProvider = null;
      ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ();
    }

    [TearDown]
    public void TearDown ()
    {
      ClientTransactionScope.ResetActiveScope ();
    }

    [TestFixtureTearDown]
    public virtual void TestFixtureTearDown ()
    {
      _serviceLocatorScope.Dispose();
    }

    [Test]
    public override void BusinessObject_Property_IsAccessible ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithoutSecurityTest for BusinessObject_Property_IsAccessible on reference system: ~0.33탎 (was ~0.08 탎) (release build), ~0.66 탎 (debug build)");

      base.BusinessObject_Property_IsAccessible ();

      Console.WriteLine ();
    }

    [Test]
    public override void BusinessObject_GetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithoutSecurityTest for BusinessObject_GetProperty on reference system: ~1.5 탎 (release build), ~3.8 탎 (debug build)");

      base.BusinessObject_GetProperty ();
      
      Console.WriteLine ();
    }

    [Test]
    public override void DynamicMethod_GetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithoutSecurityTest for DynamicMethod_GetProperty on reference system: ~0.9 탎 (release build), ~2.0 탎 (debug build)");

      base.DynamicMethod_GetProperty ();

      Console.WriteLine ();
    }

    [Test]
    public override void DomainObject_GetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithoutSecurityTest for DomainObject_GetProperty on reference system: ~0.9 탎 (release build), ~2.0 탎 (debug build)");

      base.DomainObject_GetProperty ();

      Console.WriteLine ();
    }
    [Test]
    public override void BusinessObject_SetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithoutSecurityTest for BusinessObject_SetProperty on reference system: ~1.5 탎 (release build), ~3.3 탎 (debug build)");

      base.BusinessObject_SetProperty ();

      Console.WriteLine ();
    }

    [Test]
    public override void DomainObject_SetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithoutSecurityTest for DomainObject_SetProperty on reference system: ~1.4 탎 (release build), ~3.0 탎 (debug build)");

      base.DomainObject_SetProperty ();

      Console.WriteLine ();
    }
  }
}