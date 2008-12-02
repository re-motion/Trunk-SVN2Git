using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;
using $PROJECT_ROOTNAMESPACE$;
using $PROJECT_ROOTNAMESPACE$.Classes;
using $DOMAIN_ROOTNAMESPACE$;

namespace $PROJECT_ROOTNAMESPACE$.UI
{
  // <WxeFunction>
  //   <Parameter name="obj" type="$DOMAIN_CLASSNAME$" />
  // </WxeFunction>
  public partial class Edit$DOMAIN_CLASSNAME$Form : EditFormPage
  {
    protected void Page_Load (object sender, EventArgs e)
    {
      Title = ResourceManagerUtility.GetResourceManager(this).GetString("Edit~$DOMAIN_CLASSNAME$");
      if (!IsPostBack)
      {
        if (obj == null)
          obj = $DOMAIN_CLASSNAME$.NewObject();
      }

      LoadObject (obj);
    }

    protected override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected override TabbedMultiView UserControlMultiView
    {
      get { return MultiView; }
    }

    protected void SaveButton_Click (object sender, EventArgs e)
    {
      if (SaveObject())
        Return();
    }

    protected void CancelButton_Click (object sender, EventArgs e)
    {
      throw new WxeUserCancelException ();
    }
  }
}