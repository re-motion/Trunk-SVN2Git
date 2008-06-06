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
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes
{
  public class ClassWithAllKindsOfMembers
  {
    public virtual void Method ()
    {
      if (Event != null)
        Event (null, null);
    }

    public virtual void MethodWithOutRef (out string outP, ref int refP)
    {
      outP = refP.ToString ();
      ++refP;
    }

    public virtual int Property
    {
      get { return 0; }
      set { Dev.Null = value; }
    }

    public virtual string this[int i]
    {
      get { return i.ToString(); }
      set { Dev.Null = value; }
    }

    public virtual event EventHandler Event;
  }
}
