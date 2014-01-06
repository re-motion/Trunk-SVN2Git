// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System;
using System.ComponentModel;
using JetBrains.Annotations;
using Remotion.ServiceLocation;

namespace Remotion.Utilities
{
  /// <summary>
  /// Creates a <see cref="TypeConverter"/> if the type is supported by the factory.
  /// </summary>
  [ConcreteImplementation (
      "Remotion.ExtensibleEnums.Infrastructure.ExtensibleEnumTypeConverterFactory, Remotion.ExtensibleEnums, Version=<version>, Culture=neutral, PublicKeyToken=<publicKeyToken>",
      ignoreIfNotFound: true,
      Lifetime = LifetimeKind.Singleton, Position = 2)]
  [ConcreteImplementation (typeof (EnumTypeConverterFactory), Lifetime = LifetimeKind.Singleton, Position = 1)]
  [ConcreteImplementation (typeof (AttributeBasedTypeConverterFactory), Lifetime = LifetimeKind.Singleton, Position = 0)]
  public interface ITypeConverterFactory
  {
    [CanBeNull]
    TypeConverter CreateTypeConverterOrDefault ([NotNull]Type type);
  }
}