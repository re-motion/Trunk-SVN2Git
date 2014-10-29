using System;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocTreeViewUserControl : DataEditUserControl
  {
    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      Normal.MenuItemProvider = new TestBocTreeViewContextMenu();
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      SetTestOutput();
    }

    private void SetTestOutput ()
    {
      TestOutput.SetNormalSelectedNodeLabel (Normal.SelectedNode != null ? Normal.SelectedNode.ItemID + "|" + Normal.SelectedNode.Text : "");
      TestOutput.SetNoTopLevelExpanderSelectedNodeLabel (
          NoTopLevelExpander.SelectedNode != null ? NoTopLevelExpander.SelectedNode.ItemID + "|" + NoTopLevelExpander.SelectedNode.Text : "");
      TestOutput.SetNoLookAheadEvaluationSelectedNodeLabel (
          NoLookAheadEvaluation.SelectedNode != null
              ? NoLookAheadEvaluation.SelectedNode.ItemID + "|" + NoLookAheadEvaluation.SelectedNode.Text
              : "");
      TestOutput.SetNoPropertyIdentifierSelectedNodeLabel (
          NoPropertyIdentifier.SelectedNode != null ? NoPropertyIdentifier.SelectedNode.ItemID + "|" + NoPropertyIdentifier.SelectedNode.Text : "");
    }

    private BocTreeViewUserControlTestOutput TestOutput
    {
      get { return (BocTreeViewUserControlTestOutput) ((Layout) Page.Master).GetTestOutputControl(); }
    }
  }
}