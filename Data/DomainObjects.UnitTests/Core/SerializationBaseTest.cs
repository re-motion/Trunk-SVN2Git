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

namespace Remotion.Data.DomainObjects.UnitTests.Core
{
  public class SerializationBaseTest : ClientTransactionBaseTest
  {
    protected object SerializeAndDeserialize (object graph)
    {
      using (MemoryStream stream = new MemoryStream ())
      {
        Serialize (stream, graph);
        return Deserialize (stream);
      }
    }

    protected void Serialize (Stream stream, object graph)
    {
      BinaryFormatter serializationFormatter = new BinaryFormatter ();
      serializationFormatter.Serialize (stream, graph);
    }

    protected object Deserialize (Stream stream)
    {
      stream.Position = 0;

      BinaryFormatter deserializationFormatter = new BinaryFormatter ();
      return deserializationFormatter.Deserialize (stream);
    }
  }
}
