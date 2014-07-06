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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class NullAccessTypeFilterTest
  {
    [Test]
    public void Filter_ReturnsSequenceUnmodified ()
    {
      IEnumerable<AccessType> accessTypes = new AccessType[0];
      Assert.That (
          NullAccessTypeFilter.Instance.Filter (
              accessTypes,
              MockRepository.GenerateStub<ISecurityContext>(),
              MockRepository.GenerateStub<ISecurityPrincipal>()),
          Is.SameAs (accessTypes));
    }

    [Test]
    public void Serialization ()
    {
      var deserializedObject = Serializer.SerializeAndDeserialize (NullAccessTypeFilter.Instance);
      Assert.That (deserializedObject, Is.SameAs (NullAccessTypeFilter.Instance));
    }

    [Test]
    public void IsNull_ReturnsTrue ()
    {
      Assert.That (((IAccessTypeFilter) NullAccessTypeFilter.Instance).IsNull, Is.True);
    }
  }
}