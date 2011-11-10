// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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

namespace Remotion.Security
{
  [Serializable]
  public class SecurityException : Exception
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SecurityException () : this ("An error occurred in the security system.") {}
    public SecurityException (string message) : base (message) {}
    public SecurityException (string message, Exception inner) : base (message, inner) {}
    protected SecurityException (SerializationInfo info, StreamingContext context) : base (info, context) { }

    // methods and properties
  }
}
