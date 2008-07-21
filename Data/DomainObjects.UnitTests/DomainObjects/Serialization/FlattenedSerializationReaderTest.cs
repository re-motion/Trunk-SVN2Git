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
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Serialization
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
