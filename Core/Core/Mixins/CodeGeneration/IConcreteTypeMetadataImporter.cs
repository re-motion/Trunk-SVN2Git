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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Assists in importing pre-generated concrete mixed and mixin types by analyzing the types and returning the respective metadata they were
  /// generated for.
  /// </summary>
  public interface IConcreteTypeMetadataImporter
  {
    IEnumerable<TargetClassDefinition> GetMetadataForMixedType (Type concreteMixedType, ITargetClassDefinitionCache targetClassDefinitionCache);
    IEnumerable<MixinDefinition> GetMetadataForMixinType (Type concreteMixinType, ITargetClassDefinitionCache targetClassDefinitionCache);
  }
}