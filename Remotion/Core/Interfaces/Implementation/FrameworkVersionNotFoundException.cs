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

namespace Remotion.Implementation
{
  /// <summary>
  /// Thrown by <see cref="FrameworkVersionRetriever"/> when the framework version cannot be determined automatically. Use 
  /// <see cref="FrameworkVersion"/> to manually set the version or add a reference to the framework implementation assembly.
  /// </summary>
  [Serializable]
  public class FrameworkVersionNotFoundException : Exception
  {
    public FrameworkVersionNotFoundException (string message)
        : base(message)
    {
    }

    public FrameworkVersionNotFoundException (string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected FrameworkVersionNotFoundException (SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
  }
}
