using System;

namespace Remotion.Web.UI.Controls.Rendering
{
  public class NullPreRenderer : IPreRenderer
  {
    public static readonly IPreRenderer Instance = new NullPreRenderer();

    private NullPreRenderer ()
    {
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
    }

    public void PreRender ()
    {
    }
  }
}