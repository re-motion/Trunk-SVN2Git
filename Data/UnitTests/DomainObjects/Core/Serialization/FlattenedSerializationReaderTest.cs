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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class FlattenedSerializationReaderTest
  {
    [Test]
    public void ReadValue ()
    {
      FlattenedSerializationReader<int> reader = new FlattenedSerializationReader<int> (new int[] { 1, 2, 3 });
      Assert.AreEqual (1, reader.ReadValue());
    }

    [Test]
    public void ReadValue_MultipleTimes ()
    {
      FlattenedSerializationReader<int> reader = new FlattenedSerializationReader<int> (new int[] { 1, 2, 3 });
      Assert.AreEqual (1, reader.ReadValue ());
      Assert.AreEqual (2, reader.ReadValue ());
      Assert.AreEqual (3, reader.ReadValue ());
    }

    [Test]
    public void ReadPosition ()
    {
      FlattenedSerializationReader<int> reader = new FlattenedSerializationReader<int> (new int[] { 1, 2, 3 });
      Assert.AreEqual (0, reader.ReadPosition);
      reader.ReadValue();
      Assert.AreEqual (1, reader.ReadPosition);
      reader.ReadValue ();
      Assert.AreEqual (2, reader.ReadPosition);
      reader.ReadValue ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no more data in the serialization stream at position 3.")]
    public void ReadValue_TooOften ()
    {
      FlattenedSerializationReader<int> reader = new FlattenedSerializationReader<int> (new int[] { 1, 2, 3 });
      Assert.AreEqual (1, reader.ReadValue ());
      Assert.AreEqual (2, reader.ReadValue ());
      Assert.AreEqual (3, reader.ReadValue ());
      reader.ReadValue ();
    }
  }
}
