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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class FlattenedSerializationWriterTest
  {
    [Test]
    public void InitialWriter ()
    {
      FlattenedSerializationWriter<int> writer = new FlattenedSerializationWriter<int>();
      int[] data = writer.GetData();
      Assert.IsNotNull (data);
      Assert.IsEmpty (data);
    }

    [Test]
    public void AddSimpleValue ()
    {
      FlattenedSerializationWriter<int> writer = new FlattenedSerializationWriter<int> ();
      writer.AddSimpleValue (1);
      int[] data = writer.GetData ();
      Assert.IsNotNull (data);
      Assert.That (data, Is.EqualTo (new int[] { 1 }));
    }

    [Test]
    public void AddSimpleValue_Twice ()
    {
      FlattenedSerializationWriter<int> writer = new FlattenedSerializationWriter<int> ();
      writer.AddSimpleValue (1);
      writer.AddSimpleValue (2);
      int[] data = writer.GetData ();
      Assert.IsNotNull (data);
      Assert.That (data, Is.EqualTo (new int[] { 1, 2 }));
    }
  }
}
