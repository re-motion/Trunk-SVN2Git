// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
  public class ClassWithDeserializationEvents : BaseClassWithDeserializationEvents, IDeserializationCallback
  {
    [NonSerialized]
    public bool OnDeserializationCalled;
    [NonSerialized]
    public bool OnDeserializedCalled;
    [NonSerialized]
    public bool OnDeserializingCalled;

    public void OnDeserialization (object sender)
    {
      OnDeserializationCalled = true;
    }

    [OnDeserializing]
    private void OnDeserializing (StreamingContext context)
    {
      OnDeserializingCalled = true;
    }

    [OnDeserialized]
    private void OnDeserialized (StreamingContext context)
    {
      OnDeserializedCalled = true;
    }
  }
}
