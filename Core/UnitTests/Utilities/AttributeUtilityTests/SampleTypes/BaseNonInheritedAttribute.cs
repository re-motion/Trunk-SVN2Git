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

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests.SampleTypes
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class BaseNonInheritedAttribute : Attribute
  {
    private readonly string _context;

    public BaseNonInheritedAttribute (string context)
    {
      _context = context;
    }

    public string Context
    {
      get { return _context; }
    }

    public override bool Equals (object obj)
    {
      if (obj.GetType () != this.GetType())
        return false;
      return ((BaseNonInheritedAttribute) obj).Context == Context;
    }

    public override int GetHashCode ()
    {
      return Context.GetHashCode ();
    }

    public override string ToString ()
    {
      return GetType () + " (\"" + Context + "\")";
    }
  }
}