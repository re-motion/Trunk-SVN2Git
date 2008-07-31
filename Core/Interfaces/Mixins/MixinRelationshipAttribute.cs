/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Implementation;
using System.Linq;

namespace Remotion.Mixins
{
  public abstract class MixinRelationshipAttribute : Attribute
  {
    private Type[] _additionalDependencies = Type.EmptyTypes;
    private Type[] _suppressedMixins = Type.EmptyTypes;
    private MemberVisibility _introducedMemberVisibility = MemberVisibility.Private;

    /// <summary>
    /// Gets or sets additional explicit base call dependencies for the applied mixin type. This can be used to establish an ordering when
    /// combining unrelated mixins on a class which override the same methods.
    /// </summary>
    /// <value>The additional dependencies of the mixin. The validity of the dependency types is not checked until the configuration is built.</value>
    /// <exception cref="ArgumentNullException">The <paramref name="value"/> argument is <see langword="null"/>.</exception>
    public Type[] AdditionalDependencies
    {
      get { return _additionalDependencies; }
      set
      {
        _additionalDependencies = ArgumentUtility.CheckNotNull ("value", value);
      }
    }

    /// <summary>
    /// Gets or sets the mixins suppressed by the applied mixin.
    /// </summary>
    /// <value>The mixins suppressed by the applied mixins.</value>
    /// <remarks>Use this attribute to actively remove a mixin from the attribute's target type. The list of suppressed mixins cannot contain 
    /// the applied mixin itself, but it can contain mixins which themselves suppress this mixin. Such circular suppressions result in both mixins
    /// being removed from the configuration.</remarks>
    public Type[] SuppressedMixins
    {
      get { return _suppressedMixins; }
      set
      {
        _suppressedMixins = ArgumentUtility.CheckNotNull ("value", value);
      }
    }

    /// <summary>
    /// Gets or sets the default visibility of members introduced by the mixin to the target class. The default is <see cref="MemberVisibility.Private"/>.
    /// </summary>
    /// <value>The introduced member visibility.</value>
    public MemberVisibility IntroducedMemberVisibility
    {
      get { return _introducedMemberVisibility; }
      set { _introducedMemberVisibility = value; }
    }

    public override bool Equals (object obj)
    {
      var other = obj as MixinRelationshipAttribute;
      return !object.ReferenceEquals (other, null)
          && IntroducedMemberVisibility == other.IntroducedMemberVisibility
          && SuppressedMixins.SequenceEqual (other.SuppressedMixins)
          && AdditionalDependencies.SequenceEqual (other.AdditionalDependencies);
    }

    public override int GetHashCode ()
    {
      int hc = IntroducedMemberVisibility.GetHashCode ()
          ^ SuppressedMixins.Aggregate (0, (acc, t) => acc ^ t.GetHashCode())
          ^ AdditionalDependencies.Aggregate (0, (acc, t) => acc ^ t.GetHashCode ());
      return hc;
    }
  }
}
