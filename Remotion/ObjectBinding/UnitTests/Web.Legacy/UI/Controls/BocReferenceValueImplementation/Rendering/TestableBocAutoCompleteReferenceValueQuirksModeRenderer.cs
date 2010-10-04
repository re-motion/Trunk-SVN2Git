// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.Web;

namespace Remotion.ObjectBinding.UnitTests.Web.Legacy.UI.Controls.BocReferenceValueImplementation.Rendering
{
  public class TestableBocAutoCompleteReferenceValueQuirksModeRenderer : BocAutoCompleteReferenceValueQuirksModeRenderer
  {
    public TestableBocAutoCompleteReferenceValueQuirksModeRenderer (IResourceUrlFactory resourceUrlFactory)
        : this ( resourceUrlFactory, () => new TextBox())
    {
    }

    public TestableBocAutoCompleteReferenceValueQuirksModeRenderer (IResourceUrlFactory resourceUrlFactory, Func<TextBox> textBoxFactory)
        : base (resourceUrlFactory, textBoxFactory)
    {
      
    }
  }
}