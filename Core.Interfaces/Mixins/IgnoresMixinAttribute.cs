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

namespace Remotion.Mixins
{
  /// <summary>
  /// Defines that a specific mixin is not applied to a class, even when it is explicitly or implicitly configured for that class via
  /// the declarative configuration attributes <see cref="UsesAttribute"/>, <see cref="ExtendsAttribute"/>, and <see cref="MixAttribute"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Use this attribute to exclude a mixin that is configured to be applied to a base class. This attribute is not inherited, so the mixin
  /// exclusion will only work for the exact mixin to which the attribute is applied.
  /// </para>
  /// <para>
  /// Note that when a generic type definition (e.g. <c>C&lt;&gt;</c>) excludes a mixin, a corresponding closed generic class (<c>C&lt;int&gt;</c>) can
  /// still inherit the mixin from its base class. This is by design due to the rule that a closed generic type inherits mixins from both 
  /// its base class and its generic type definition.
  /// </para>
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class IgnoresMixinAttribute : Attribute
  {
    private readonly Type _mixinToIgnore;

    /// <summary>
    /// Initializes a new instance of the <see cref="IgnoresClassAttribute"/> class, specifying the mixin to be ignored by this class.
    /// </summary>
    /// <param name="mixinToIgnore">The mixin to be ignored in declarative configuration. Subclasses of this class will not inherit the mixin either.</param>
    public IgnoresMixinAttribute (Type mixinToIgnore)
    {
      _mixinToIgnore = mixinToIgnore;
    }

    /// <summary>
    /// Gets the mixin to be ignored by this class.
    /// </summary>
    /// <value>The mixin to be ignored.</value>
    public Type MixinToIgnore
    {
      get { return _mixinToIgnore; }
    }
  }
}
