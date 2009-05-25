using System;
using System.Web.UI;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList
{
  public interface IBocListNavigationBlockRenderer
  {
    void Render ();

    /// <summary>The <see cref="BocList"/> containing the data to render.</summary>
    IBocList List { get; }

    /// <summary>The <see cref="HtmlTextWriter"/> that is used to render the table cells.</summary>
    HtmlTextWriter Writer { get; }
  }
}
