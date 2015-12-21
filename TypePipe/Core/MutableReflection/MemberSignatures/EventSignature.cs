﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection.MemberSignatures
{
  /// <summary>
  /// Represents an event signature and allows signatures to be compared to each other.
  /// </summary>
  public class EventSignature : IMemberSignature, IEquatable<EventSignature>
  {
    public static EventSignature Create (EventInfo eventInfo)
    {
      ArgumentUtility.CheckNotNull ("eventInfo", eventInfo);
      return new EventSignature (eventInfo.EventHandlerType);
    }

    private readonly Type _eventHandlerType;

    public EventSignature (Type eventHandlerType)
    {
      ArgumentUtility.CheckNotNull ("eventHandlerType", eventHandlerType);
      _eventHandlerType = eventHandlerType;
    }

    public Type EventHandlerType
    {
      get { return _eventHandlerType; }
    }

    public override string ToString ()
    {
      return _eventHandlerType.ToString();
    }

    public virtual bool Equals (EventSignature other)
    {
      return !ReferenceEquals (other, null) 
          && EventHandlerType == other.EventHandlerType;
    }

    public sealed override bool Equals (object obj)
    {
      if (obj == null || obj.GetType() != GetType())
        return false;

      var other = (EventSignature) obj;
      return Equals(other);
    }

    bool IEquatable<IMemberSignature>.Equals (IMemberSignature other)
    {
      return Equals (other);
    }

    public override int GetHashCode ()
    {
      return EventHandlerType.GetHashCode();
    }
  }
}