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
using System.Text;

namespace Remotion.Mixins.Samples.PhotoStuff.Variant2
{
  public class Document : IDocument
  {
    private DateTime _createdAt = DateTime.Now;

    public DateTime CreatedAt
    {
      get { return _createdAt; }
      set { _createdAt = value; }
    }

    public void Extend ()
    {
      Console.WriteLine ("Extending");
    }

    public void Save ()
    {
      Console.WriteLine ("Saving");
    }

    public void Print ()
    {
      Console.WriteLine ("Printing");
    }
  }
}
