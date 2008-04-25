using System;
using System.ComponentModel;
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Reflection.Legacy
{

public class ReflectionBusinessObjectDataSourceControl: BusinessObjectDataSourceControl
{
  private ReflectionBusinessObjectDataSource _dataSource = new ReflectionBusinessObjectDataSource();

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Data")]
  [DefaultValue("")]
  public string TypeName
  {
    get 
    { 
      return _dataSource.TypeName; 
    }
    set
    { 
      _dataSource.TypeName = value; 
    }
  }

  protected override IBusinessObjectDataSource GetDataSource()
  {
    return _dataSource;
  }
}
}
