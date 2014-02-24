// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 

using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Security;
using Remotion.SecurityManager.GlobalAccessTypeCache;
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class IGlobalAccessTypeCacheTest
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
      var obj = _serviceLocator.GetAllInstances<IGlobalAccessTypeCache>().ToArray();

      Assert.That (obj.Length, Is.EqualTo (2));
      Assert.That (obj[0], Is.TypeOf (typeof (RevisionBasedGlobalAccessTypeCache)));
      Assert.That (obj[1], Is.TypeOf (typeof (NullGlobalAccessTypeCache)));
    }

    [Test]
    public void GetAllInstances_Twice_ReturnsSameInstance ()
    {
      var obj1 = _serviceLocator.GetAllInstances<IGlobalAccessTypeCache>().ToArray();
      var obj2 = _serviceLocator.GetAllInstances<IGlobalAccessTypeCache>().ToArray();

      Assert.That (obj1[0], Is.SameAs (obj2[0]));
      Assert.That (obj1[1], Is.SameAs (obj2[1]));
    }
  }
}