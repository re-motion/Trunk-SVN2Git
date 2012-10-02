﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using Remotion.Collections;

namespace Remotion.TypePipe.MutableReflection
{
  /// <summary>
  /// Represents the TypePipe counterpart of <see cref="CustomAttributeData"/>.
  /// </summary>
  public interface ICustomAttributeData
  {
    /// <summary>
    /// Gets the type of the attribute.
    /// </summary>
    /// <value>
    /// The attribute type.
    /// </value>
    Type Type { get; }

    /// <summary>
    /// Gets the constructor that was used to instantiate the <see cref="Attribute"/>.
    /// </summary>
    /// <value>
    /// The constructor.
    /// </value>
    ConstructorInfo Constructor { get; }

    /// <summary>
    /// Gets a <b>fresh copy</b> of the constructor arguments.
    /// </summary>
    /// <value>
    /// The constructor arguments.
    /// </value>
    ReadOnlyCollection<object> ConstructorArguments { get; }

    /// <summary>
    /// Gets a <b>fresh copy</b> the named arguments.
    /// </summary>
    /// <value>
    /// The named arguments.
    /// </value>
    ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> NamedArguments { get; }
  }
}