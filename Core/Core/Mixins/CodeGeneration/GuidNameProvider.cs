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
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.CodeGeneration
{
  public class GuidNameProvider : INameProvider
  {
    public static readonly GuidNameProvider Instance = new GuidNameProvider ();

    private GuidNameProvider ()
    {
    }
    
    public string GetNewTypeName (ClassDefinitionBase configuration)
    {
      return string.Format ("{0}_Mixed_{1}", configuration.FullName, Guid.NewGuid ().ToString ("N"));
    }
  }
}
