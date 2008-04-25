using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Design.BindableObject;
using Remotion.ObjectBinding.Web.UI.Design;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  [Designer (typeof (BindableObjectDataSourceDesigner))]
  public class BindableObjectDataSourceControl : BusinessObjectDataSourceControl
  {
    private readonly BindableObjectDataSource _dataSource = new BindableObjectDataSource();

    public BindableObjectDataSourceControl ()
    {
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Data")]
    [DefaultValue (null)]
    [Editor (typeof (BindableObjectTypePickerEditor), typeof (UITypeEditor))]
    [TypeConverter (typeof (TypeNameConverter))]
    public Type Type
    {
      get { return _dataSource.Type; }
      set { _dataSource.Type = value; }
    }

    protected override IBusinessObjectDataSource GetDataSource ()
    {
      return _dataSource;
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      _dataSource.Site = Site;
    }
  }
}