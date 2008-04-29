using System;
using System.Web;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList
{

public class EditableRowControlFactory
{
  public EditableRowControlFactory ()
  {
  }

  public virtual IBusinessObjectBoundEditableWebControl Create (BocSimpleColumnDefinition column, int columnIndex)
  {
    ArgumentUtility.CheckNotNull ("column", column);
    if (columnIndex < 0) throw new ArgumentOutOfRangeException ("columnIndex");

    IBusinessObjectBoundEditableWebControl control = column.CreateEditModeControl();

    if (control == null)
    {
      IBusinessObjectPropertyPath propertyPath = column.GetPropertyPath ();
      control = (IBusinessObjectBoundEditableWebControl) ControlFactory.CreateControl (propertyPath.LastProperty, ControlFactory.EditMode.InlineEdit);
    }

    return control;
  }

  public virtual void RegisterHtmlHeadContents (HttpContext context)
  {
    BocBooleanValue bocBooleanValue = new BocBooleanValue();
    bocBooleanValue.RegisterHtmlHeadContents (context);

    BocDateTimeValue bocDateTimeValue = new BocDateTimeValue();
    bocDateTimeValue.RegisterHtmlHeadContents (context);

    BocEnumValue bocEnumValue = new BocEnumValue();
    bocEnumValue.RegisterHtmlHeadContents (context);

    BocMultilineTextValue bocMultilineTextValue = new BocMultilineTextValue ();
    bocMultilineTextValue.RegisterHtmlHeadContents (context);

    BocReferenceValue bocReferenceValue = new BocReferenceValue();
    bocReferenceValue.RegisterHtmlHeadContents (context);

    BocTextValue bocTextValue = new BocTextValue();
    bocTextValue.RegisterHtmlHeadContents (context);
  }
}

}
