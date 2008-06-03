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
  /// Indicates that an ordinal name comparison can be used to determine the order between two mixins when these override the same methods
  /// on a target object.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Ordering between mixins is important when two mixins override the same methods on a target object, because without a defined ordering,
  /// it wouldn't be deterministic which of the overrides would be executed first. Usually, orderings between mixins are expressed via dependencies.
  /// Either implicitly, because the mixin has a base call dependency (second type argument of the <see cref="Mixin{TThis,TBase}"/> base class) to 
  /// an interface implemented by another mixin, or explicitly via  <see cref="MixinRelationshipAttribute.AdditionalDependencies"/>.
  /// </para>
  /// <para>
  /// In some situations, however, a mixin cannot and does not need to specify a specific ordering simply because any override call order would
  /// be sufficient for its purpose. Such a mixin can opt into alphabetic ordering by having this attribute applied to it. Alphabetic ordering is
  /// only applied after all implicit or explicit dependencies have been analyzed. It is also ruled out if more than one of the mixins involved
  /// do not accept alphabetic ordering.
  /// </para>
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class AcceptsAlphabeticOrderingAttribute : Attribute
  {
  }
}
