// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Web.Legacy.UI.Controls.BocReferenceValueImplementation.Rendering
{
  public class TestableBocAutoCompleteReferenceValueQuirksModeRenderer : BocAutoCompleteReferenceValueQuirksModeRenderer
  {
    public TestableBocAutoCompleteReferenceValueQuirksModeRenderer ()
        : this (() => new TextBox())
    {
    }

    public TestableBocAutoCompleteReferenceValueQuirksModeRenderer (Func<TextBox> textBoxFactory)
        : base (textBoxFactory)
    {
      
    }
  }
}