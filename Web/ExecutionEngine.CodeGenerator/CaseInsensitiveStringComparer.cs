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

namespace Remotion.Web.ExecutionEngine.CodeGenerator
{
  public class CaseInsensitiveStringComparer: StringComparer
  {
    public override int Compare (string x, string y)
    {
      return string.Compare (x, y, true);
    }

    public override bool Equals (string x, string y)
    {
      return string.Compare (x, y, true) == 0;
    }

    public override int GetHashCode (string obj)
    {
      return obj.ToLowerInvariant().GetHashCode();
    }
  }
}