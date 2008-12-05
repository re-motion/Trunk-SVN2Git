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
using System.Runtime.Serialization;

namespace Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes
{
  [Serializable]
  public class SerializableClass : ISerializable
  {
    private StreamingContext _Context;
    private SerializationInfo _Info;

    public SerializableClass ()
    {
    }

    protected SerializableClass (SerializationInfo info, StreamingContext context)
    {
      Info = info;
      Context = context;
    }

    public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      Info = info;
      Context = context;
    }

    public SerializationInfo Info
    {
      get { return _Info; }
      set { _Info = value; }
    }

    public StreamingContext Context
    {
      set { _Context = value; }
      get { return _Context; }
    }
  }
}
