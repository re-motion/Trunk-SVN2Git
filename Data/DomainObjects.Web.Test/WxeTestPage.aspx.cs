using System;
using System.Web.UI.WebControls;
using Remotion.Data.DomainObjects.Web.Test.Domain;
using Remotion.Data.DomainObjects.Web.Test.WxeFunctions;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Controls;

namespace Remotion.Data.DomainObjects.Web.Test
{
  public class WxeTestPage: WxePage
  {
    protected Button WxeTransactedFunctionCreateNewButton;
    protected Label ResultLabel;
    protected Button WxeTransactedFunctionNoneButton;
    protected Button WxeTransactedFunctionCreateNewAutoCommitButton;
    protected Button WxeTransactedFunctionCreateNewNoAutoCommitButton;
    protected Button WxeTransactedFunctionNoneAutoCommitButton;
    protected Button WxeTransactedFunctionNoneNoAutoCommitButton;
    protected Button WxeTransactedFunctionWithPageStepButton;

    protected WxeTestPageFunction CurrentWxeTestPageFunction
    {
      get { return (WxeTestPageFunction) CurrentFunction; }
    }

    protected HtmlHeadContents HtmlHeadContents;

    private void Page_Load (object sender, EventArgs e)
    {
      ResultLabel.Visible = false;
    }

    #region Web Form Designer generated code

    protected override void OnInit (EventArgs e)
    {
      //
      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      //
      InitializeComponent();
      base.OnInit (e);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.WxeTransactedFunctionCreateNewButton.Click += new System.EventHandler (this.WxeTransactedFunctionCreateNewButton_Click);
      this.WxeTransactedFunctionNoneButton.Click += new System.EventHandler (this.WxeTransactedFunctionNoneButton_Click);
      this.WxeTransactedFunctionCreateNewAutoCommitButton.Click += new System.EventHandler (this.WxeTransactedFunctionCreateNewAutoCommitButton_Click);
      this.WxeTransactedFunctionNoneAutoCommitButton.Click += new System.EventHandler (this.WxeTransactedFunctionNoneAutoCommitButton_Click);
      this.WxeTransactedFunctionCreateNewNoAutoCommitButton.Click +=
          new System.EventHandler (this.WxeTransactedFunctionCreateNewNoAutoCommitButton_Click);
      this.WxeTransactedFunctionNoneNoAutoCommitButton.Click += new System.EventHandler (this.WxeTransactedFunctionNoneNoAutoCommitButton_Click);
      this.WxeTransactedFunctionWithPageStepButton.Click += new EventHandler (WxeTransactedFunctionWithPageStepButton_Click);
      this.Load += new System.EventHandler (this.Page_Load);
    }

    #endregion

    private void WxeTransactedFunctionCreateNewButton_Click (object sender, EventArgs e)
    {
      // TODO: cheange to Remember/CheckActiveClientTransactionScope
      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
      {
        RememberCurrentClientTransaction();

        new CreateRootTestTransactedFunction (ClientTransactionScope.CurrentTransaction).Execute();

        CheckCurrentClientTransactionRestored();
      }

      ShowResultText ("Test WxeTransactedFunction (CreateNew) executed successfully.");
    }

    private void WxeTransactedFunctionNoneButton_Click (object sender, EventArgs e)
    {
      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
      {
        RememberCurrentClientTransaction();

        new CreateNoneTestTransactedFunction (ClientTransactionScope.CurrentTransaction).Execute();
        CheckCurrentClientTransactionRestored();
      }
      ShowResultText ("Test WxeTransactedFunction (None) executed successfully.");
    }

    private void WxeTransactedFunctionCreateNewAutoCommitButton_Click (object sender, EventArgs e)
    {
      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
      {
        RememberCurrentClientTransaction();
        SetInt32Property (5, ClientTransaction.NewRootTransaction());

        new AutoCommitTestTransactedFunction (WxeTransactionMode.CreateRoot, DomainObjectIDs.ObjectWithAllDataTypes1).Execute ();
        CheckCurrentClientTransactionRestored();

        if (GetInt32Property (ClientTransaction.NewRootTransaction()) != 10)
          throw new TestFailureException ("The WxeTransactedFunction wrongly did not properly commit or set the property value.");
      }

        ShowResultText ("Test WxeTransactedFunction (TransactionMode = CreateNew, AutoCommit = true) executed successfully.");
    }

    private void WxeTransactedFunctionCreateNewNoAutoCommitButton_Click (object sender, EventArgs e)
    {
      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
      {
        RememberCurrentClientTransaction();
        SetInt32Property (5, ClientTransaction.NewRootTransaction());

        new NoAutoCommitTestTransactedFunction (WxeTransactionMode.CreateRoot, DomainObjectIDs.ObjectWithAllDataTypes1).Execute ();

        CheckCurrentClientTransactionRestored();

        if (GetInt32Property (ClientTransaction.NewRootTransaction()) != 5)
          throw new TestFailureException ("The WxeTransactedFunction wrongly did set and commit the property value.");
      }
      ShowResultText ("Test WxeTransactedFunction (TransactionMode = CreateNew, AutoCommit = false) executed successfully.");
    }

    private void WxeTransactedFunctionNoneAutoCommitButton_Click (object sender, EventArgs e)
    {
      SetInt32Property (5, ClientTransaction.NewRootTransaction());
      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
      {
        RememberCurrentClientTransaction();

        new AutoCommitTestTransactedFunction (WxeTransactionMode.None, DomainObjectIDs.ObjectWithAllDataTypes1).Execute ();

        CheckCurrentClientTransactionRestored();

        if (GetInt32Property (ClientTransactionScope.CurrentTransaction) != 10)
          throw new TestFailureException ("The WxeTransactedFunction wrongly did not set property value.");
      }

      if (GetInt32Property (ClientTransaction.NewRootTransaction()) != 5)
        throw new TestFailureException ("The WxeTransactedFunction wrongly committed the property value.");

      ShowResultText ("Test WxeTransactedFunction (TransactionMode = None, AutoCommit = true) executed successfully.");
    }

    private void WxeTransactedFunctionNoneNoAutoCommitButton_Click (object sender, EventArgs e)
    {
      SetInt32Property (5, ClientTransaction.NewRootTransaction());
      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
      {
        RememberCurrentClientTransaction();

        new NoAutoCommitTestTransactedFunction (WxeTransactionMode.None, DomainObjectIDs.ObjectWithAllDataTypes1).Execute ();

        CheckCurrentClientTransactionRestored();

        if (GetInt32Property (ClientTransactionScope.CurrentTransaction) != 10)
          throw new TestFailureException ("The WxeTransactedFunction wrongly did not set the property value.");
      }

      if (GetInt32Property (ClientTransaction.NewRootTransaction()) != 5)
        throw new TestFailureException ("The WxeTransactedFunction wrongly committed the property value.");

      ShowResultText ("Test WxeTransactedFunction (TransactionMode = None, AutoCommit = false) executed successfully.");
    }

    private void WxeTransactedFunctionWithPageStepButton_Click (object sender, EventArgs e)
    {
      if (!IsReturningPostBack)
      {
        using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
        {
          RememberCurrentClientTransaction();
          ExecuteFunction (new ParentPageStepTestTransactedFunction());
        } // we must dispose the scope on the original thread
      }
      else
      {
        // the WXE must copy the original scope to the new thread
        if (ClientTransactionScope.ActiveScope == null)
          throw new TestFailureException ("The function did not restore the original scope.");

        CheckCurrentClientTransactionRestored();
        ClientTransactionScope.ActiveScope.Leave (); // dispose the scope on the new thread
        ShowResultText ("Test WxeTransactedFunction with nested PageStep executed successfully.");
      }
    }
    
    private void SetInt32Property (int value, ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterDiscardingScope ())
      {
        ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ObjectWithAllDataTypes1);

        objectWithAllDataTypes.Int32Property = value;

        clientTransaction.Commit();
      }
    }

    private int GetInt32Property (ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterDiscardingScope ())
      {
        ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ObjectWithAllDataTypes1);

        return objectWithAllDataTypes.Int32Property;
      }
    }

    private void RememberCurrentClientTransaction()
    {
      CurrentWxeTestPageFunction.CurrentClientTransaction = ClientTransactionScope.CurrentTransaction;
    }

    private void CheckCurrentClientTransactionRestored()
    {
      if (CurrentWxeTestPageFunction.CurrentClientTransaction != ClientTransactionScope.CurrentTransaction)
        throw new TestFailureException (
            "ClientTransactionScope.CurrentTransaction was not properly restored to the state before the WxeTransactedFunction was called.");
    }

    private void ShowResultText (string text)
    {
      ResultLabel.Visible = true;
      ResultLabel.Text = text;
    }
  }
}