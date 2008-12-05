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
  [DBTable]
  [Instantiable]
  [Serializable]
  public abstract class DOImplementingISerializable : DomainObject, ISerializable
  {
    public static DOImplementingISerializable NewObject (string memberHeldAsField)
    {
      return NewObject<DOImplementingISerializable> ().With (memberHeldAsField);
    }

    private string _memberHeldAsField;

    protected DOImplementingISerializable (string memberHeldAsField)
    {
      _memberHeldAsField = memberHeldAsField;
    }

    protected DOImplementingISerializable (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
      _memberHeldAsField = info.GetString ("_memberHeldAsField") + "-Ctor";
    }

    public abstract int PropertyWithGetterAndSetter { get; set; }

    public string MemberHeldAsField
    {
      get { return _memberHeldAsField; }
      set { _memberHeldAsField = value; }
    }

    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("_memberHeldAsField", _memberHeldAsField + "-GetObjectData");
      BaseGetObjectData (info, context);
    }
  }
}
