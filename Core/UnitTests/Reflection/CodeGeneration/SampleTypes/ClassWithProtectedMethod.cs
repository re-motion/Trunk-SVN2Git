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

namespace Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes
{
  public class ClassWithProtectedMethod
  {
    protected string GetSecret ()
    {
      return "The secret is to be more provocative and interesting than anything else in [the] environment.";
    }
  }
}
