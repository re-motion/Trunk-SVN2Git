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
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.CodeGeneration.SampleTypes
{
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public class InheritableAttribute : Attribute
  {
  }

  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class NonInheritableAttribute : Attribute
  {
  }

  [Inheritable]
  [NonInheritable]
  [CopyCustomAttributes (typeof (CopyTemplate))]
  public class MixinWithProtectedOverriderAndAttributes
  {
    [OverrideTarget]
    protected new string ToString ()
    {
      return "";
    }

    [SampleCopyTemplate]
    public class CopyTemplate {}
  }
}
