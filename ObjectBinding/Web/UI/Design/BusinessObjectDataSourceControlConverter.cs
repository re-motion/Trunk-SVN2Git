using System;
using System.Collections;
using System.ComponentModel;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Design
{
public class BusinessObjectDataSourceControlConverter : StringConverter
{
  public BusinessObjectDataSourceControlConverter()
  {
  }

  private object[] GetDataSourceControls (object instance, IContainer container)
  {
    ComponentCollection components = container.Components;
    ArrayList dataSources = new ArrayList();

    ICollection controls = null;
    if (instance is Array)
      controls = (Array) instance;
    else
      controls = new IBusinessObjectBoundWebControl[] {(IBusinessObjectBoundWebControl) instance};

    for (int idxComponents = 0; idxComponents < components.Count; idxComponents++)
    {
      IComponent component = (IComponent) components[idxComponents];
      IBusinessObjectDataSourceControl dataSource = component as IBusinessObjectDataSourceControl;
      if (dataSource != null && ! StringUtility.IsNullOrEmpty (dataSource.ID))
      {
        bool hasSelfReference = false;
        foreach (IBusinessObjectBoundWebControl control in controls)
        {
          if (dataSource == control)
          {
            hasSelfReference = true;
            break;
          }
        }
        if (! hasSelfReference)
          dataSources.Add (dataSource.ID);
      }
    }

    dataSources.Sort(Comparer.Default);
    return dataSources.ToArray();
  }

  public override TypeConverter.StandardValuesCollection GetStandardValues (ITypeDescriptorContext context)
  {
    if ((context != null) && (context.Container != null))
    {
      object[] dataSources = GetDataSourceControls (context.Instance, context.Container);
      if (dataSources != null)
        return new TypeConverter.StandardValuesCollection (dataSources);
    }
    return null;
  }

  public override bool GetStandardValuesExclusive (ITypeDescriptorContext context)
  {
    return false;
  }

  public override bool GetStandardValuesSupported (ITypeDescriptorContext context)
  {
    return true;
  }
}
}
