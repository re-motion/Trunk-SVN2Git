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

namespace Remotion.Mixins
{
  public partial class MixinConfiguration
  {
    private static MixinConfiguration _masterConfiguration = null;
    private static readonly object _masterConfigurationLock = new object ();

    private static MixinConfiguration GetMasterConfiguration ()
    {
      lock (_masterConfigurationLock)
      {
        if (_masterConfiguration == null)
          _masterConfiguration = DeclarativeConfigurationBuilder.BuildDefaultConfiguration ();
        return _masterConfiguration;
      }
    }

    private static MixinConfiguration CopyMasterConfiguration ()
    {
      lock (_masterConfigurationLock)
      {
        MixinConfiguration masterConfiguration = GetMasterConfiguration ();
        return new MixinConfiguration (masterConfiguration);
      }
    }

    /// <summary>
    /// Locks access to the application's master mixin configuration and accepts a delegate to edit the configuration while it is locked.
    /// </summary>
    /// <param name="editor">A delegate performing changes to the master configuration.</param>
    /// <remarks>
    /// The master mixin configuration is the default mixin configuration used whenever a thread first accesses
    /// <see cref="ActiveConfiguration"/>. Changes made to it will affect any thread accessing its mixin configuration for the
    /// first time after this method has been called. If a thread attempts to access its mixin configuration for the first time while
    /// a change is in progress, it will block until until that process has finished (i.e. until <paramref name="editor"/> has returned).
    /// </remarks>
    public static void EditMasterConfiguration (Action<MixinConfiguration> editor)
    {
      lock (_masterConfigurationLock)
      {
        editor (GetMasterConfiguration ());
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
      lock (_masterConfigurationLock)
      {
        _masterConfiguration = null;
      }
    }
  }
}
