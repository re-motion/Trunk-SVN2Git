using System;
using System.Runtime.Remoting.Messaging;
using Remotion.Mixins.Context;

namespace Remotion.Mixins
{
  public partial class MixinConfiguration
  {
    private static readonly CallContextSingleton<MixinConfiguration> s_activeConfiguration =
        new CallContextSingleton<MixinConfiguration> ("Remotion.Mixins.MixinConfiguration.s_activeConfiguration",
        delegate { return CopyMasterConfiguration (); });

    /// <summary>
    /// Gets a value indicating whether this thread has an active mixin configuration.
    /// </summary>
    /// <value>
    ///   True if there is an active configuration for the current thread (actually <see cref="CallContext"/>); otherwise, false.
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
    /// Gets the active mixin configuration for the current thread (actually <see cref="CallContext"/>).
    /// </summary>
    /// <value>The active mixin configuration for the current thread (<see cref="CallContext"/>).</value>
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