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
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;

namespace Remotion.Web.UnitTests.Core.UI.Controls
{
  [TestFixture]
  public class RendererTestBase
  {
    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      ServiceLocator.SetLocatorProvider (() => new StubServiceLocator());
    }

    [TestFixtureTearDown]
    public void TestFixtureTearDown ()
    {
      ServiceLocator.SetLocatorProvider (null);
    }
  }
}