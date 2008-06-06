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

namespace Remotion.Mixins.Samples.PhotoStuff.Variant2
{
  public class DocumentMixin : Mixin<object>, IDocument
  {
    private Document _document = new Document();

    public DateTime CreatedAt
    {
      get { return _document.CreatedAt; }
    }

    public Document Document
    {
      get { return _document; }
      set { _document = value; }
    }

    public void Extend ()
    {
      Document.Extend();
    }

    public void Save ()
    {
      Document.Save();
    }

    public void Print ()
    {
      Document.Print();
    }
  }
}
