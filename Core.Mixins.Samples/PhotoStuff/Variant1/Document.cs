using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Mixins;

namespace Remotion.Mixins.Samples.PhotoStuff.Variant1
{
  public class Document : Mixin<object>, IDocument
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
