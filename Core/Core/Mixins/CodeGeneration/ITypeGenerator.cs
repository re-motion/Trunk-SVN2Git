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
using System.Reflection;
using System.Reflection.Emit;
using Remotion.Collections;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.CodeGeneration
{
  public interface ITypeGenerator
  {
    TypeBuilder TypeBuilder { get; }
    bool IsAssemblySigned { get; }

    Type GetBuiltType ();
    IEnumerable<Tuple<MixinDefinition, Type>> GetBuiltMixinTypes ();
    MethodInfo GetPublicMethodWrapper (MethodDefinition methodToBeWrapped);
  }
}
