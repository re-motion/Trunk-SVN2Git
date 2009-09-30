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
using Remotion.Mixins.Context;
using Remotion.Utilities;
using Remotion.Logging;

namespace Remotion.Mixins
{
  public partial class MixinConfiguration
  {
    private static MixinConfiguration s_masterConfiguration = null;
    private static readonly object s_masterConfigurationLock = new object ();

    /// <summary>
    /// Gets the master <see cref="MixinConfiguration"/>. The master configuration is the default <see cref="MixinConfiguration"/> used whenever a 
    /// thread first accesses its <see cref="ActiveConfiguration"/>. If no master configuration has been set via <see cref="SetMasterConfiguration"/>,
    /// a default configuration will be built by <see cref="DeclarativeConfigurationBuilder.BuildDefaultConfiguration"/>.
    /// </summary>
    /// <returns>The master <see cref="MixinConfiguration"/>.</returns>
    /// <seealso cref="SetMasterConfiguration"/>
    public static MixinConfiguration GetMasterConfiguration ()
    {
      lock (s_masterConfigurationLock)
      {
        if (s_masterConfiguration == null)
        {
          s_log.Info ("Building mixin master configuration...");
          using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Time needed to build mixin master configuration: {0}."))
          {
            s_masterConfiguration = DeclarativeConfigurationBuilder.BuildDefaultConfiguration();
          }
        }
        return s_masterConfiguration;
      }
    }

    /// <summary>
    /// Sets the master <see cref="MixinConfiguration"/>. The master configuration is the default <see cref="MixinConfiguration"/> used whenever a 
    /// thread first accesses its <see cref="ActiveConfiguration"/>. If the master configuration is set to <see langword="null" />, the next call
    /// to <see cref="GetMasterConfiguration"/> (or the next thread first accessing its <see cref="ActiveConfiguration"/>) will trigger a new
    /// default configuration to be built.
    /// </summary>
    /// <param name="newMasterConfiguration">The <see cref="MixinConfiguration"/> to be used as the new master configuration.</param>
    /// <remarks>
    /// Changes made to the master configuration will affect any thread accessing its mixin configuration for the
    /// first time after this method has been called.
    /// </remarks>
    /// <seealso cref="GetMasterConfiguration"/>
    public static void SetMasterConfiguration (MixinConfiguration newMasterConfiguration)
    {
      lock (s_masterConfigurationLock)
      {
        s_masterConfiguration = newMasterConfiguration;
      }
    }

    /// <summary>
    /// Causes the master mixin configuration to be rebuilt from scratch the next time a thread accesses its mixin configuration for the first time.
    /// </summary>
    /// <remarks>
    /// The master mixin configuration is the default mixin configuration used whenever a thread first accesses
    /// <see cref="ActiveConfiguration"/>. Changes made to it will affect any thread accessing its mixin configuration for the
    /// first time after this method has been called.
    /// </remarks>
    public static void ResetMasterConfiguration ()
    {
      lock (s_masterConfigurationLock)
      {
        s_masterConfiguration = null;
      }
    }
  }
}
