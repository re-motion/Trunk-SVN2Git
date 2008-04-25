using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Mixins;

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