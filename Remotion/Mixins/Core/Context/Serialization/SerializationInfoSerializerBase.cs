// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.Serialization
{
  /// <summary>
  /// Provides functionality for classes serializing data into a <see cref="SerializationInfo"/> object in such a way that deserialization can occur in
  /// a single pass (required by mixed object serialization - <see cref="SerializationHelper"/>).
  /// </summary>
  public class SerializationInfoSerializerBase
  {
    private readonly SerializationInfo _info;
    private readonly string _prefix;

    protected SerializationInfoSerializerBase (SerializationInfo info, string prefix)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      ArgumentUtility.CheckNotNull ("prefix", prefix);

      _info = info;
      _prefix = prefix;
    }

    public SerializationInfo Info
    {
      get { return _info; }
    }

    public string Prefix
    {
      get { return _prefix; }
    }

    protected void AddTypes (string key, IEnumerable<Type> types)
    {
      var typeNames = types.Select (t => t.AssemblyQualifiedName).ToArray ();
      _info.AddValue (_prefix + "." + key + ".AssemblyQualifiedNames", typeNames);
    }

    protected void AddType (string key, Type type)
    {
      // Type must be serialized as a string; otherwise, SerializationHelper won't be able to use it immediately after deserialization.
      AddValue (key + ".AssemblyQualifiedName", type.AssemblyQualifiedName);
    }

    protected void AddValue (string key, object value)
    {
      _info.AddValue (_prefix + "." + key, value);
    }
  }
}