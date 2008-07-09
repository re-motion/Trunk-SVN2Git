using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Globalization;
using $DOMAIN_ROOTNAMESPACE$;
using $PROJECT_ROOTNAMESPACE$.Classes;

namespace $PROJECT_ROOTNAMESPACE$.UI
{
  public partial class Edit$DOMAIN_CLASSNAME$Control : BaseControl
  {
    protected void Page_Load (object sender, EventArgs e)
    {
    }

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    $REPEAT_FOREACHREFERENCEDPROPERTY_BEGIN$(isList=true)
    protected void $DOMAIN_PROPERTYNAME$Field_MenuItemClick (object sender, Remotion.Web.UI.Controls.WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "AddMenuItem")
      {
        $DOMAIN_REFERENCEDCLASSNAME$ row = $DOMAIN_REFERENCEDCLASSNAME$.NewObject();
        $DOMAIN_PROPERTYNAME$Field.AddAndEditRow (row);
      }
    }

    $REPEAT_FOREACHREFERENCEDPROPERTY_END$
  }
}