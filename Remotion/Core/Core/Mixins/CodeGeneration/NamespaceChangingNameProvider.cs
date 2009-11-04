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

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Returns names for concrete mixed types by extending the type name of the target class with an additional namespace. This name provider
  /// cannot be used to return names for concrete mixin types those types wouldn't be uniquely identifiable if only the namespace was changed. 
  /// Use <see cref="GuidNameProvider"/> instead.
  /// </summary>
  /// <remarks>
  /// This provider can lead to name conflicts if it is used to generate multiple concrete types for the same target type, for example because
  /// the mixin configuration has changed. In such scenarios, a unique name provider, such as <see cref="GuidNameProvider"/>, should be preferred.
  /// </remarks>
  public class NamespaceChangingNameProvider : IConcreteMixedTypeNameProvider
  {
    public static readonly NamespaceChangingNameProvider Instance = new NamespaceChangingNameProvider ();

    private NamespaceChangingNameProvider ()
    {
    }

    public string GetNameForConcreteMixedType (TargetClassDefinition configuration)
    {
      string originalNamespace = configuration.Type.Namespace;
      int restStart = originalNamespace.Length > 0 ? originalNamespace.Length + 1 : 0;
      string originalRest = configuration.Type.FullName.Substring (restStart);

      string maskedRest = originalRest.Replace ("[[", "{");
      maskedRest = maskedRest.Replace ("]]", "}");
      maskedRest = maskedRest.Replace (", ", "/");
      maskedRest = maskedRest.Replace (",", "/");
      maskedRest = maskedRest.Replace ('.', '_');

      return string.Format ("{0}.MixedTypes.{1}", originalNamespace, maskedRest);
    }
  }
}
