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
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Remotion.Utilities;

namespace Remotion.Mixins.Context
{
  /// <summary>
  /// Describes the code artifact (custom attribute, method, etc.) a <see cref="MixinContext"/> was configured by.
  /// </summary>
  public class MixinContextOrigin
  {
    public static MixinContextOrigin CreateForCustomAttribute (Attribute attribute, MemberInfo target)
    {
      ArgumentUtility.CheckNotNull ("attribute", attribute);
      ArgumentUtility.CheckNotNull ("target", target);

      return new MixinContextOrigin (attribute.GetType ().Name, target.Module.Assembly, target.ToString ());
    }

    public static MixinContextOrigin CreateForCustomAttribute (Attribute attribute, Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("attribute", attribute);
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      return new MixinContextOrigin (attribute.GetType ().Name, assembly, "assembly");
    }

    public static MixinContextOrigin CreateForMethod (MethodBase methodBase)
    {
      ArgumentUtility.CheckNotNull ("methodBase", methodBase);

      var location = string.Format ("{0}, declaring type: {1}", methodBase, methodBase.DeclaringType);
      return new MixinContextOrigin ("Method", methodBase.Module.Assembly, location);
    }

    private readonly string _kind; // e.g., UsesAttribute or Imperative
    private readonly Assembly _assembly;
    private readonly string _location; // e.g., a type name, or a fully qualified method name

    public MixinContextOrigin (string kind, Assembly assembly, string location)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("kind", kind);
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNullOrEmpty ("location", location);

      _kind = kind;
      _assembly = assembly;
      _location = location;
    }

    public string Kind
    {
      get { return _kind; }
    }

    public Assembly Assembly
    {
      get { return _assembly; }
    }

    public string Location
    {
      get { return _location; }
    }

    public override string ToString ()
    {
      var assemblyName = Assembly.GetName (false);
      return string.Format ("{0}, Location: '{1}' (Assembly: '{2}', code base: {3})", Kind, Location, assemblyName.Name, assemblyName.CodeBase);
    }
  }
}