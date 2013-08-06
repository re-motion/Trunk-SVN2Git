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
using Remotion.Utilities;

namespace Remotion.Mixins.Context.Serialization
{
  // TODO 5370: Remove this class and its subclasses.
  /// <summary>
  /// Provides functionality for deserializing data serialized via <see cref="SerializationInfoSerializerBase"/>.
  /// </summary>
  public class SerializationInfoDeserializerBase
  {
    private readonly SerializationInfo _info;
    private readonly string _prefix;

    protected SerializationInfoDeserializerBase (SerializationInfo info, string prefix)
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

    protected Type GetType (string key)
    {
      var typeName = GetValue<string> (key + ".AssemblyQualifiedName");
      return Type.GetType (typeName);
    }

    protected T GetValue<T> (string key)
    {
      return (T) _info.GetValue (_prefix + "." + key, typeof (T));
    }

    protected IEnumerable<Type> GetTypes (string key)
    {
      var typeNames = GetValue<string[]> (key + ".AssemblyQualifiedNames");
      return typeNames.Select (Type.GetType);
    }
  }
}