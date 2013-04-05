// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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

namespace Remotion.TypePipe.Configuration
{
  /// <summary>
  /// Defines an interface for classes that provide configuration values.
  /// </summary>
  public interface IConfigurationProvider
  {
    /// <summary>
    /// If <see langword="true"/>, the pipeline signs all generated assemblies or throws an <see cref="InvalidOperationException"/> if that is not
    /// possible.
    /// </summary>
    bool ForceStrongNaming { get; }

    /// <summary>
    /// When <see cref="ForceStrongNaming"/> is enabled, the key file (<c>*.snk</c>) denoted by this property is used to sign generated assemblies.
    /// If this property is <see langword="null"/> a default key file is used instead.
    /// </summary>
    string KeyFilePath { get; }
  }
}