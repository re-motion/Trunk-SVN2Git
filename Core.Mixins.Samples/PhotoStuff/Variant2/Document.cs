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
