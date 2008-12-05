// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
