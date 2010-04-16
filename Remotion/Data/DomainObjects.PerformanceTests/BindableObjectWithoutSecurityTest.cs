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
using Remotion.Security;
using Remotion.Security.Configuration;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  [TestFixture]
  public class BindableObjectWithoutSecurityTest : BindableObjectTestBase
  {
    [SetUp]
    public void SetUp ()
    {
      SecurityConfiguration.Current.SecurityProvider = null;
      AdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), new ObjectSecurityAdapter());
      ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ();
    }

    [TearDown]
    public void TearDown ()
    {
      ClientTransactionScope.ResetActiveScope ();
    }

    [Test]
    public override void BusinessObject_Property_IsAccessible ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithoutSecurityTest for BusinessObject_Property_IsAccessible on reference system: ~0.8 탎 (release build), ~0.9 탎 (debug build)");

      base.BusinessObject_Property_IsAccessible ();

      Console.WriteLine ();
    }
    [Test]
    public override void BusinessObject_GetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithoutSecurityTest for BusinessObject_GetProperty on reference system: ~17 탎 (release build), ~26.7 탎 (debug build)");

      base.BusinessObject_GetProperty ();
      
      Console.WriteLine ();
    }

    [Test]
    public override void Reflection_GetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithoutSecurityTest for Reflection_GetProperty on reference system: ~12 탎 (release build), ~17 탎 (debug build)");

      base.Reflection_GetProperty ();

      Console.WriteLine ();
    }

    [Test]
    public override void DomainObject_GetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithoutSecurityTest for DomainObject_GetProperty on reference system: ~7.5 탎 (release build), ~12 탎 (debug build)");

      base.DomainObject_GetProperty ();

      Console.WriteLine ();
    }
  }
}