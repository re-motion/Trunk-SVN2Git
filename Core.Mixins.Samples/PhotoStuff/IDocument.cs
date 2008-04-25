using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Mixins.Samples.PhotoStuff
{
  public interface IDocument
  {
    DateTime CreatedAt { get; }
    void Extend ();
    void Save ();
    void Print ();
  }
}
