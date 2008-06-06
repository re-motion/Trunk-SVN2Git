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
  /// Indicates that a mixin member overrides a virtual member of the mixin's target class.
  /// </summary>
  /// <remarks>
  /// <para>
  /// An overriding member and its base member must both be public or protected, and they must have the same name and signature.
  /// </para>
  /// <para>
  /// This attribute is inherited (i.e. if the overriding member is replaced in a subclass, the subclass' member is now the overriding member) and
  /// can only be applied once per member.
  /// </para>
  /// </remarks>
  [AttributeUsage (AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = false, Inherited = true)]
  public class OverrideTargetAttribute : Attribute, IOverrideAttribute
  {
    Type IOverrideAttribute.OverriddenType
    {
      get { return null; }
    }
  }
}
