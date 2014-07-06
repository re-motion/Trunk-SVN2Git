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
using Remotion.Data.DomainObjects.Security;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  [TestFixture]
  public class BindableObjectWithSecurityTest : BindableObjectTestBase
  {
    private ServiceLocatorScope _serviceLocatorScope;

    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp ()
    {
      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterMultiple<IObjectSecurityAdapter> (() => new ObjectSecurityAdapter());
      _serviceLocatorScope = new ServiceLocatorScope (serviceLocator);
    }

    [SetUp]
    public void SetUp ()
    {
      SecurityConfiguration.Current.SecurityProvider = new StubSecurityProvider();
      SecurityConfiguration.Current.PrincipalProvider = new ThreadPrincipalProvider();
      var clientTransaction = new SecurityClientTransactionFactory().CreateRootTransaction();
      clientTransaction.To<ClientTransaction>().EnterDiscardingScope();
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfiguration.Current.SecurityProvider = null;
      SecurityConfiguration.Current.PrincipalProvider = null;
      ClientTransactionScope.ResetActiveScope();
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
          "Expected average duration of BindableObjectWithSecurityTest for BusinessObject_Property_IsAccessible on reference system: ~1.3 �s (release build), ~2.7 �s (debug build)");

      base.BusinessObject_Property_IsAccessible();

      Console.WriteLine ();
    }

    [Test]
    public override void BusinessObject_GetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithSecurityTest for BusinessObject_GetProperty on reference system: ~3.3 �s (release build), ~7.3 �s (debug build)");

      base.BusinessObject_GetProperty ();

      Console.WriteLine ();
    }

    [Test]
    public override void DynamicMethod_GetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithSecurityTest for DynamicMethod_GetProperty on reference system: ~2.4 �s (release build), ~5.2 �s (debug build)");

      base.DynamicMethod_GetProperty ();

      Console.WriteLine ();
    }

    [Test]
    public override void DomainObject_GetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithSecurityTest for DomainObject_GetProperty on reference system: ~2.4 �s (release build), ~5.2 �s (debug build)");

      base.DomainObject_GetProperty ();

      Console.WriteLine ();
    }

    [Test]
    public override void BusinessObject_SetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithSecurityTest for BusinessObject_SetProperty on reference system: ~3.1 �s (release build), ~6.4 �s (debug build)");

      base.BusinessObject_SetProperty ();

      Console.WriteLine ();
    }

    [Test]
    public override void DomainObject_SetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithSecurityTest for DomainObject_SetProperty on reference system: 3.0 �s (release build), ~6.2 �s (debug build)");

      base.DomainObject_SetProperty ();

      Console.WriteLine ();
    }
  }
}