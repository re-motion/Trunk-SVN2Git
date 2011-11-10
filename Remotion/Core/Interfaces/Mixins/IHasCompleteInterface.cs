// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
namespace Remotion.Mixins
{
  /// <summary>
  /// Defines <typeparamref name="TInterface"/> as a complete interface for the target class implementing the <see cref="IHasCompleteInterface{TInterface}"/> interface.
  /// </summary>
  /// <typeparam name="TInterface">The complete interface type.</typeparam>
  /// <remarks>
  /// <para>
  /// Complete interfaces are interfaces that comprise members implemented on a target class as well as members added by mixins to that target class.
  /// They can be used to access all members on a mixed instance without casting to mixin interfaces.
  /// </para>
  /// <para>
  /// For more information, see <see cref="CompleteInterfaceAttribute"/>. Implementing <see cref="IHasCompleteInterface{TInterface}"/> on a target 
  /// class has the same effect as marking the respective <typeparamref name="TInterface"/> with the <see cref="CompleteInterfaceAttribute"/>.
  /// </para>
  /// <para>
  /// <see cref="IHasCompleteInterface{TInterface}"/> is automatically taken into account when the declarative mixin configuration is analyzed.
  /// When building a mixin configuration using the fluent mixin building APIs (<see cref="M:Remotion.Mixins.MixinConfiguration.BuildNew()"/> and 
  /// similar), it is not automatically taken into account. Register the interface by hand using 
  /// <see cref="M:Remotion.Mixins.Context.FluentBuilders.ClassContextBuilder.AddCompleteInterface{TInterface}()"/>.
  /// </para>
  /// </remarks>
  public interface IHasCompleteInterface<TInterface> where TInterface : class
  {
  }
}