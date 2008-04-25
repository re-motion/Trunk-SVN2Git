using System;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;

namespace Remotion.Mixins
{
  public partial class MixinConfiguration
  {
    /// <summary>
    /// Returns a <see cref="MixinConfigurationBuilder"/> object to build a new <see cref="MixinConfiguration"/>.
    /// </summary>
    /// <returns>A <see cref="MixinConfigurationBuilder"/> for building a new <see cref="MixinConfiguration"/> with a fluent interface.</returns>
    /// <remarks>
    /// <para>
    /// Use this method to build a new <see cref="MixinConfiguration"/> from scratch.
    /// </para>
    /// <para>
    /// If you want to temporarily make the built
    /// <see cref="MixinConfiguration"/> the <see cref="ActiveConfiguration"/>, call the builder's <see cref="MixinConfigurationBuilder.EnterScope"/>
    /// method from within a <c>using</c> statement.
    /// </para>
    /// </remarks>
    public static MixinConfigurationBuilder BuildNew ()
    {
      return new MixinConfigurationBuilder (null);
    }

    /// <summary>
    /// Returns a <see cref="MixinConfigurationBuilder"/> object to build a new <see cref="MixinConfiguration"/> which inherits data from a 
    /// <paramref name="parentConfiguration"/>.
    /// </summary>
    /// <param name="parentConfiguration">A <see cref="MixinConfiguration"/> whose data should be inherited from the built
    /// <see cref="MixinConfiguration"/>.</param>
    /// <returns>A <see cref="MixinConfigurationBuilder"/> for building a new <see cref="MixinConfiguration"/> with a fluent interface.</returns>
    /// <remarks>
    /// <para>
    /// Use this method to build a new <see cref="MixinConfiguration"/> while taking over the class-mixin bindings from an existing
    /// <see cref="MixinConfiguration"/> object.
    /// </para>
    /// <para>
    /// If you want to temporarily make the built
    /// <see cref="MixinConfiguration"/> the <see cref="ActiveConfiguration"/>, call the builder's <see cref="MixinConfigurationBuilder.EnterScope"/>
    /// method from within a <c>using</c> statement.
    /// </para>
    /// </remarks>
    public static MixinConfigurationBuilder BuildFrom (MixinConfiguration parentConfiguration)
    {
      ArgumentUtility.CheckNotNull ("parentConfiguration", parentConfiguration);
      return new MixinConfigurationBuilder (parentConfiguration);
    }

    /// <summary>
    /// Returns a <see cref="MixinConfigurationBuilder"/> object to build a new <see cref="MixinConfiguration"/> which inherits data from the
    /// <see cref="ActiveConfiguration"/>.
    /// </summary>
    /// <returns>A <see cref="MixinConfigurationBuilder"/> for building a new <see cref="MixinConfiguration"/> with a fluent interface.</returns>
    /// <remarks>
    /// <para>
    /// Use this method to build a new <see cref="MixinConfiguration"/> while taking over the class-mixin bindings from the
    /// <see cref="ActiveConfiguration"/>.
    /// </para>
    /// <para>
    /// If you want to temporarily make the built
    /// <see cref="MixinConfiguration"/> the <see cref="ActiveConfiguration"/>, call the builder's <see cref="MixinConfigurationBuilder.EnterScope"/>
    /// method from within a <c>using</c> statement.
    /// </para>
    /// </remarks>
    public static MixinConfigurationBuilder BuildFromActive ()
    {
      return new MixinConfigurationBuilder (ActiveConfiguration);
    }
  }
}