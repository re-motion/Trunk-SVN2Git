﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Data.DomainObjects.UberProfIntegration;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.UberProfIntegration
{
  public abstract class UberProfIntegrationTestBase : StandardMappingTest
  {
    private LinqToSqlAppenderProxy _appenderProxy;
    private LinqToSqlAppenderProxy _originalAppender;
    private DoubleCheckedLockingContainer<LinqToSqlAppenderProxy> _container;

    public override void SetUp ()
    {
      base.SetUp();

      _appenderProxy = (LinqToSqlAppenderProxy) PrivateInvoke.CreateInstanceNonPublicCtor (
          typeof (LinqToSqlAppenderProxy),
          "Test",
          typeof (FakeLinqToSqlProfiler),
          typeof (MockableLinqToSqlAppender));

      _container = (DoubleCheckedLockingContainer<LinqToSqlAppenderProxy>)
                   PrivateInvoke.GetNonPublicStaticField (typeof (LinqToSqlAppenderProxy), "s_instance");
      if (_container.HasValue)
        _originalAppender = _container.Value;
      else
        _originalAppender = null;

      _container.Value = _appenderProxy;
    }

    public override void TearDown()
    {
      _container.Value = _originalAppender;

      base.TearDown();
    }

    public LinqToSqlAppenderProxy AppenderProxy
    {
      get { return _appenderProxy; }
    }
  }
}