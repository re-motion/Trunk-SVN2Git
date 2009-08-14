// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Context;
using Remotion.Mixins.Context;

namespace Remotion.Mixins
{
  public partial class MixinConfiguration
  {
    private static readonly SafeContextSingleton<MixinConfiguration> s_activeConfiguration =
        new SafeContextSingleton<MixinConfiguration> ("Remotion.Mixins.MixinConfiguration.s_activeConfiguration",
        CopyMasterConfiguration);

    /// <summary>
    /// Gets a value indicating whether this thread has an active mixin configuration.
    /// </summary>
    /// <value>
    ///   True if there is an active configuration for the current thread (actually <see cref="SafeContext"/>); otherwise, false.
    /// </value>
    /// <remarks>
    /// The <see cref="ActiveConfiguration"/> property will always return a non-<see langword="null"/> configuration, no matter whether one was
    /// set for the current thread or not. If none was set, a default one is built using <see cref="DeclarativeConfigurationBuilder.BuildDefaultConfiguration"/>.
    /// In order to check whether a configuration has been set (or a default one has been built), use this property.
    /// </remarks>
    public static bool HasActiveConfiguration
    {
      get { return s_activeConfiguration.HasCurrent; }
    }

    /// <summary>
    /// Gets the active mixin configuration for the current thread (actually <see cref="SafeContext"/>).
    /// </summary>
    /// <value>The active mixin configuration for the current thread (<see cref="SafeContext"/>).</value>
    /// <remarks>
    /// The <see cref="ActiveConfiguration"/> property will always return a non-<see langword="null"/> configuration, no matter whether one was
    /// set for the current thread or not. If none was set, a default one is built using <see cref="DeclarativeConfigurationBuilder.BuildDefaultConfiguration"/>.
    /// In order to check whether a configuration has been set (or a default one has been built), use the <see cref="HasActiveConfiguration"/> property.
    /// </remarks>
    public static MixinConfiguration ActiveConfiguration
    {
      get { return s_activeConfiguration.Current; }
    }

    private static MixinConfiguration PeekActiveConfiguration
    {
      get { return HasActiveConfiguration ? ActiveConfiguration : null; }
    }

    /// <summary>
    /// Sets the active mixin configuration configuration for the current thread.
    /// </summary>
    /// <param name="configuration">The configuration to be set, can be <see langword="null"/>.</param>
    public static void SetActiveConfiguration (MixinConfiguration configuration)
    {
      s_activeConfiguration.SetCurrent (configuration);
    }
  }
}
