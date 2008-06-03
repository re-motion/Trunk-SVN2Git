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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.DomainObjects.UnitTests.Core.DataManagement
{
  [TestFixture]
  public class ObjectDeletedExceptionTest : StandardMappingTest
  {
    [Test]
    public void Serialization ()
    {
      ObjectDeletedException exception = new ObjectDeletedException (DomainObjectIDs.Order1);

      using (MemoryStream memoryStream = new MemoryStream ())
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        formatter.Serialize (memoryStream, exception);
        memoryStream.Seek (0, SeekOrigin.Begin);

        formatter = new BinaryFormatter ();

        exception = (ObjectDeletedException) formatter.Deserialize (memoryStream);

        Assert.AreEqual (DomainObjectIDs.Order1, exception.ID);
      }
    }
  }
}
