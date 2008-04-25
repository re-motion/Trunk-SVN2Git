using System;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace OBWTest.IndividualControlTests
{

  public partial class BocLiteralUserControl : BaseUserControl
  {
    protected override void RegisterEventHandlers ()
    {
      base.RegisterEventHandlers ();

      this.CVTestSetNullButton.Click += new EventHandler (this.CVTestSetNullButton_Click);
      this.CVTestSetNewValueButton.Click += new EventHandler (this.CVTestSetNewValueButton_Click);
    }

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    override protected void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      Person person = (Person) CurrentObject.BusinessObject;

      UnboundCVField.Property = (IBusinessObjectStringProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition ("CVString");
      UnboundCVField.LoadUnboundValue (person.CVString, IsPostBack);
    }

    override protected void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
    }

    private void CVTestSetNullButton_Click (object sender, EventArgs e)
    {
      CVField.Value = null;
    }

    private void CVTestSetNewValueButton_Click (object sender, EventArgs e)
    {
      CVField.Value = "Foo<br/>Bar";
    }
  }

}
