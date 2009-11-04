// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Returns names for concrete mixed or mixin types by extending the type name of the target or mixin class with a <see cref="Guid"/>. That way,
  /// unique names are generated.
  /// </summary>
  public class GuidNameProvider : IConcreteMixedTypeNameProvider, IConcreteMixinTypeNameProvider
  {
    public static readonly GuidNameProvider Instance = new GuidNameProvider ();

    private GuidNameProvider ()
    {
    }
    
    public string GetNameForConcreteMixedType (TargetClassDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      return string.Format ("{0}_Mixed_{1}", configuration.FullName, Guid.NewGuid ().ToString ("N"));
    }

    public string GetNameForConcreteMixinType (ConcreteMixinTypeIdentifier identifier)
    {
      var mixinTypeName = identifier.MixinType.IsGenericType ? identifier.MixinType.GetGenericTypeDefinition().FullName : identifier.MixinType.FullName;
      return string.Format ("{0}_GeneratedMixin_{1}", mixinTypeName, Guid.NewGuid ().ToString ("N"));
    }
  }
}
