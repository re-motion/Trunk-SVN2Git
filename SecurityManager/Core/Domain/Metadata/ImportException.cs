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

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  public class ImportException : Exception
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public ImportException () : this ("A metadata import exception occurred.") { }
    public ImportException (string message) : base (message) { }
    public ImportException (string message, Exception inner) : base (message, inner) { }
    protected ImportException (SerializationInfo info, StreamingContext context) : base (info, context) { }

    // methods and properties

  }
}
