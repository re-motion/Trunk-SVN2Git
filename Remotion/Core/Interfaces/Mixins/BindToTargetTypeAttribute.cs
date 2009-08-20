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

namespace Remotion.Mixins
{
  /// <summary>
  /// Indicates that a generic parameter of a mixin should be bound to the mixin's target type (unless the generic parameter type is explicitly
  /// specified when the mixin is configured).
  /// </summary>
  /// <remarks>
  /// <para>
  /// Apply this attribute to a generic parameter of a generic mixin when the mixin engine should be able to automatically close the mixin type.
  /// Without the attribute, the generic parameter will be bound to the target type's generic parameter, and the mixin engine will throw an exception
  /// if the target type has no parameter.
  /// </para>
  /// <para>
  /// For example, consider the following code:
  /// <code>
  /// public class TargetClass&lt;T&gt; { }
  /// 
  /// [Extends (typeof (TargetClass&lt;&gt;))]
  /// public class MyMixin&lt;T&gt; { }
  /// </code>
  /// In this example, MyMixin's <c>T</c> will be bound to the TargetClass' <c>T</c> parameter.
  /// </para>
  /// <para>
  /// To bind <c>T</c> to <c>TargetClass&lt;T&gt;</c> instead, use the following code:
  /// <code>
  /// public class TargetClass&lt;T&gt; { }
  /// 
  /// [Extends (typeof (TargetClass&lt;&gt;))]
  /// public class MyMixin&lt;[BindToTargetType] T&gt; { }
  /// </code>
  /// </para>
  /// <note type="inotes">When a type parameter is reused for the generic parameter of the <see cref="Mixin{TThis,TBase}"/>
  /// or <see cref="Mixin{TThis}"/> base classes, the type parameter must satisfy several constraints. See <see cref="Mixin{TThis,TBase}"/> and
  /// <see cref="Mixin{TThis}"/> for more information.</note>
  /// </remarks>
  /// <seealso cref="BindToConstraintsAttribute"/>
  [AttributeUsage (AttributeTargets.GenericParameter, Inherited = false)]
  public class BindToTargetTypeAttribute : Attribute
  {
  }
}