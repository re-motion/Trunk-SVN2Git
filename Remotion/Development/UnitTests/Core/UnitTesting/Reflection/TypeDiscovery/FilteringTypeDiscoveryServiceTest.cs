// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Reflection.TypeDiscovery
{
  [TestFixture]
  public class FilteringTypeDiscoveryServiceTest
  {
    
    [Test]
    public void CreateFromNamespaceWhitelist ()
    {
      var types = new[]
                  {
                      typeof (Color),
                      typeof (Brush),
                      typeof (DateTime),
                  };

      var mockRepository = new MockRepository();
      var decoratedTypeDiscoveryServiceMock = mockRepository.Stub<ITypeDiscoveryService>();

      decoratedTypeDiscoveryServiceMock.Stub (service => service.GetTypes (Arg<Type>.Is.Anything, Arg<bool>.Is.Anything)).Return (types);
      mockRepository.ReplayAll();

      var filteringTypeDiscoveryService = FilteringTypeDiscoveryService.CreateFromNamespaceWhitelist (decoratedTypeDiscoveryServiceMock, typeof (Color).Namespace);
      var filteredTypes = filteringTypeDiscoveryService.GetTypes (null, false);

      Assert.AreEqual (
          new[]
          {
              typeof (Color),
              typeof (Brush),
          },
          filteredTypes.Cast<Type>().ToArray());
    }

    [Test]
    public void CreateFromNamespaceWhitelist_IncludesSubNamespaces ()
    {
      var types = new[]
                  {
                      typeof (Color),
                      typeof (Brush),
                      typeof (DateTime),
                      typeof (ImageFlags)
                  };

      var mockRepository = new MockRepository ();
      var decoratedTypeDiscoveryServiceMock = mockRepository.Stub<ITypeDiscoveryService> ();

      decoratedTypeDiscoveryServiceMock.Stub (service => service.GetTypes (Arg<Type>.Is.Anything, Arg<bool>.Is.Anything)).Return (types);
      mockRepository.ReplayAll ();

      var filteringTypeDiscoveryService = FilteringTypeDiscoveryService.CreateFromNamespaceWhitelist (decoratedTypeDiscoveryServiceMock, typeof (Color).Namespace);
      var filteredTypes = filteringTypeDiscoveryService.GetTypes (null, false);

      Assert.AreEqual (
          new[]
          {
              typeof (Color),
              typeof (Brush),
              typeof (ImageFlags)
          },
          filteredTypes.Cast<Type> ().ToArray ());
    }

    [Test]
    public void CreateFromNamespaceBlacklist ()
    {
      var types = new[]
                  {
                      typeof (Color),
                      typeof (Brush),
                      typeof (DateTime),
                  };

      var mockRepository = new MockRepository ();
      var decoratedTypeDiscoveryServiceMock = mockRepository.Stub<ITypeDiscoveryService> ();

      decoratedTypeDiscoveryServiceMock.Stub (service => service.GetTypes (Arg<Type>.Is.Anything, Arg<bool>.Is.Anything)).Return (types);
      mockRepository.ReplayAll ();

      var filteringTypeDiscoveryService = FilteringTypeDiscoveryService.CreateFromNamespaceBlacklist (decoratedTypeDiscoveryServiceMock, typeof (Color).Namespace);
      var filteredTypes = filteringTypeDiscoveryService.GetTypes (null, false);

      Assert.AreEqual (
          new[]
          {
              typeof (DateTime),
          },
          filteredTypes.Cast<Type> ().ToArray ());
    }

    [Test]
    public void CreateFromNamespaceBlacklist_IncludesSubNamespaces ()
    {
      var types = new[]
                  {
                      typeof (Color),
                      typeof (Brush),
                      typeof (DateTime),
                      typeof (ImageFlags)
                  };

      var mockRepository = new MockRepository ();
      var decoratedTypeDiscoveryServiceMock = mockRepository.Stub<ITypeDiscoveryService> ();

      decoratedTypeDiscoveryServiceMock.Stub (service => service.GetTypes (Arg<Type>.Is.Anything, Arg<bool>.Is.Anything)).Return (types);
      mockRepository.ReplayAll ();

      var filteringTypeDiscoveryService = FilteringTypeDiscoveryService.CreateFromNamespaceBlacklist (decoratedTypeDiscoveryServiceMock, typeof (Color).Namespace);
      var filteredTypes = filteringTypeDiscoveryService.GetTypes (null, false);

      Assert.AreEqual (
          new[]
          {
              typeof (DateTime),
          },
          filteredTypes.Cast<Type> ().ToArray ());
    }
  }
}