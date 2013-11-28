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
using JetBrains.Annotations;
using Remotion.Globalization.Implementation;
using Remotion.Reflection;
using Remotion.ServiceLocation;

namespace Remotion.Globalization
{
  /// <summary>
  /// Defines an interface for resolving the <see cref="IResourceManager"/> for an <see cref="ITypeInformation"/>.
  /// </summary>
  [ConcreteImplementation (
      "Remotion.Mixins.Globalization.MixinGlobalizationService, Remotion.Mixins, Version=<version>, Culture=neutral, PublicKeyToken=<publicKeyToken>",
      ignoreIfNotFound: true,
      Position = 1, Lifetime = LifetimeKind.Singleton)]
  [ConcreteImplementation (typeof (GlobalizationService), Position = 0, Lifetime = LifetimeKind.Singleton)]
  public interface IGlobalizationService
  {
    /// <summary>
    /// Resolves the <see cref="IResourceManager"/> for the specified <paramref name="typeInformation"/>.
    /// </summary>
    /// <remarks>
    /// If not resource manager can be found for the specified <see cref="ITypeInformation"/> a <see cref="NullResourceManager"/> is returned.
    /// </remarks>
    // TODO AO: extension method accepting Type instead of ITypeInformation > cleanup
    [NotNull]
    IResourceManager GetResourceManager ([NotNull] ITypeInformation typeInformation);
  }
}