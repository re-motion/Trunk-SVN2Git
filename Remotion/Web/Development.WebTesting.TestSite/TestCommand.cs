using System;
using System.Web.UI;
using Remotion.ServiceLocation;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public class TestCommand : Control
  {
    private readonly IRenderingFeatures _renderingFeatures;

    public string Text { get; set; }
    public CommandType CommandType { get; set; }
    public Command.EventCommandInfo EventCommandInfo { get; set; }
    public Command.HrefCommandInfo HrefCommandInfo { get; set; }
    public string ItemID { get; set; }

    public TestCommand ()
    {
      _renderingFeatures = SafeServiceLocator.Current.GetInstance<IRenderingFeatures>();
      EventCommandInfo = new Command.EventCommandInfo();
      HrefCommandInfo = new Command.HrefCommandInfo();
    }

    protected override void Render (HtmlTextWriter writer)
    {
      base.Render (writer);

      var command = new Command (CommandType)
                    {
                        EventCommand = EventCommandInfo,
                        HrefCommand = HrefCommandInfo,
                        ItemID = ItemID
                    };

      writer.AddAttribute (HtmlTextWriterAttribute.Id, ClientID);
      var postBackEvent = Page.ClientScript.GetPostBackEventReference (this, "additional") + ";";
      command.RenderBegin (writer, _renderingFeatures, postBackEvent, new string[0], null, null);

      writer.Write (Text);

      command.RenderEnd (writer);
    }
  }
}