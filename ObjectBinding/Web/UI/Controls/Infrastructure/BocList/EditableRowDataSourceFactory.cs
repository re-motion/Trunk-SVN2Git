using System;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList
{
public class EditableRowDataSourceFactory
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public EditableRowDataSourceFactory ()
  {
  }

  // methods and properties

  public virtual IBusinessObjectReferenceDataSource Create (IBusinessObject businessObject)
  {
    ArgumentUtility.CheckNotNull ("businessObject", businessObject);

    BusinessObjectReferenceDataSource dataSource = new BusinessObjectReferenceDataSource();
    dataSource.BusinessObject = businessObject;
    
    return dataSource;
  }
}

}
