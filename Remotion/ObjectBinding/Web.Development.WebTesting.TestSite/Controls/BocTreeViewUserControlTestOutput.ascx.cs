using System;
using System.Web.UI;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocTreeViewUserControlTestOutput : UserControl
  {
    public void SetNormalSelectedNodeLabel (string value)
    {
      NormalSelectedNodeLabel.Text = value;
    }

    public void SetNoTopLevelExpanderSelectedNodeLabel (string value)
    {
      NoTopLevelExpanderSelectedNodeLabel.Text = value;
    }

    public void SetNoLookAheadEvaluationSelectedNodeLabel (string value)
    {
      NoLookAheadEvaluationSelectedNodeLabel.Text = value;
    }

    public void SetNoPropertyIdentifierSelectedNodeLabel (string value)
    {
      NoPropertyIdentifierSelectedNodeLabel.Text = value;
    }

    public void SetActionPerformed (string bocTreeViewId, string action, string parameter)
    {
      ActionPerformedSenderLabel.Text = bocTreeViewId;
      ActionPerformedLabel.Text = action;
      ActionPerformedParameterLabel.Text = parameter;
    }
  }
}