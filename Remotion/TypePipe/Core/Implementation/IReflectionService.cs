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

namespace Remotion.TypePipe.Implementation
{
  // TODO 5519: Docs
  public interface IReflectionService
  {
    // TODO 5519: Docs
    bool IsAssembledType (Type type);

    // TODO 5519: Docs
    Type GetRequestedType (Type assembledType);

    // TODO 5519: Docs
    /// <summary>
    /// Gets the assembled type for the requested type.
    /// </summary>
    /// <param name="requestedType">The requested type.</param>
    /// <returns>The generated type for the requested type.</returns>
    Type GetAssembledType (Type requestedType);
  }
}