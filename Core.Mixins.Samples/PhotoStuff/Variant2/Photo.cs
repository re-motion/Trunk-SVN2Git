using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Mixins;

namespace Remotion.Mixins.Samples.PhotoStuff.Variant2
{
  [Uses (typeof (DocumentMixin))]
  public class Photo
  {
    [Stored]
    public IDocument Document
    {
      get { return (IDocument) this; }
    }
  }
}
