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
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Interception.TestDomain
{
  [Serializable]
  [DBTable]
  public class SerializableClassImplementingISerializable : DomainObject, ISerializable
  {
    public bool ISerializableCtorCalled = false;
    public int I;

    public SerializableClassImplementingISerializable ()
    {
    }

    protected SerializableClassImplementingISerializable (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
      ISerializableCtorCalled = true;
      I = info.GetInt32 ("I");
    }

    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("I", I);
      BaseGetObjectData (info, context);
    }
  }
}
