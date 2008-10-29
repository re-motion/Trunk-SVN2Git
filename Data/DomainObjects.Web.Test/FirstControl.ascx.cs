using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Data.DomainObjects.Web.Test.Domain;
using Remotion.Data.DomainObjects.Web.Test.WxeFunctions;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test
{
  public partial class FirstControl : WxeUserControl2
  {
    public WxeUserControlTestPageFunction MyFunction
    {
      get
      {
        return (WxeUserControlTestPageFunction) CurrentPageStep.ParentFunction;
      }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
      if (CurrentPageStep.IsReturningPostBack)
      {
        var function = (ShowSecondUserControlFunction) CurrentPageStep.ReturningFunction;
        if (function.ExceptionHandler.Exception != null)
          throw function.ExceptionHandler.Exception;
        MyFunction.ObjectReadFromSecondControl = function.ReturnedObjectWithAllDataTypes;
      }
      RefreshText();
    }

    private void RefreshText()
    {
      GuidInLabel.Text = GetObjectString (MyFunction.ObjectPassedIntoSecondControl);
      var outObject = MyFunction.ObjectReadFromSecondControl;
      GuidOutLabel.Text = GetObjectString (outObject);
      ClientTransactionLabel.Text = ClientTransaction.Current.ToString ();
    }

    private string GetObjectString(ClassWithAllDataTypes obj)
    {
      return obj == null ? "<null>" : obj.ID + " (byte property: " + obj.ByteProperty + ", State: " + obj.State + ")";
    }

    protected void NonTransactionUserControlStepButton_Click (object sender, EventArgs e)
    {
      ExecuteSecondControl (WxeTransactionMode.None);
      RefreshText ();
    }

    protected void SubTransactionUserControlStepButton_Click (object sender, EventArgs e)
    {
      ExecuteSecondControl (WxeTransactionMode.CreateChildIfParent);
      RefreshText ();
    }

    private void ExecuteSecondControl (ITransactionMode transactionMode)
    {
      // Note: assigning return value doesn't work
      MyFunction.ObjectReadFromSecondControl = SecondControl.Call (WxePage, this, this, transactionMode, MyFunction.ObjectPassedIntoSecondControl);
    }

    protected void SaveButton_Click (object sender, EventArgs e)
    {
      ClientTransaction.Current.Commit ();
      RefreshText ();
    }

    protected void ReturnButton_Click (object sender, EventArgs e)
    {
      ExecuteNextStep ();
    }
  }
}