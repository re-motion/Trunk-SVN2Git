using System;

namespace Remotion.Web.Test.MultiplePostBackCatching
{
  public partial class UpdatePanelSutForm : SutBasePage
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      SutGenerator.GenerateSut (this, SutPlaceHolder.Controls);
    }
  }
}