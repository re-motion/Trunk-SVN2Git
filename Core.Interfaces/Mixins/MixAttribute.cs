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

namespace Remotion.Mixins
{
  /// <summary>
  /// Configures that a class and a mixin should be mixed together.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This attribute is effective for the declarative mixin configuration, which is in effect by default when an application is started.
  /// </para>
  /// <para> 
  /// The <see cref="MixAttribute"/> is an alternative to <see cref="UsesAttribute"/> and <see cref="ExtendsAttribute"/> allowing assembly-level mixin
  /// configuration. Therefore, it is suitable for transparently putting mixins and classes together, with neither mixin nor target class explicitly
  /// referencing the other side of the relationship.
  /// </para>
  /// </remarks>
  [AttributeUsage (AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
  public class MixAttribute : MixinRelationshipAttribute
  {
    private readonly Type _targetType;
    private readonly Type _mixinType;
    private MixinKind _mixinKind;

    /// <summary>
    /// Initializes a new instance of the <see cref="MixAttribute"/> class.
    /// </summary>
    /// <param name="targetType">The target type to be mixed.</param>
    /// <param name="mixinType">The mixin type to be mixed with the target type.</param>
    public MixAttribute (Type targetType, Type mixinType)
    {
      _targetType = ArgumentUtility.CheckNotNull ("targetType", targetType);
      _mixinType = ArgumentUtility.CheckNotNull ("mixinType", mixinType);
    }

    /// <summary>
    /// Gets or sets the kind of relationship between the mixin and its target class. For more information see <see cref="Mixins.MixinKind"/>.
    /// </summary>
    /// <value>The mixin kind.</value>
    public MixinKind MixinKind
    {
      get { return _mixinKind; }
      set { _mixinKind = value; }
    }


    /// <summary>
    /// Gets the target type to be mixed.
    /// </summary>
    /// <value>The mixed type.</value>
    public Type TargetType
    {
      get { return _targetType; }
    }

    /// <summary>
    /// Gets the mixin type mixed with the target class.
    /// </summary>
    /// <value>The mixin type.</value>
    public Type MixinType
    {
      get { return _mixinType; }
    }
  }
}
