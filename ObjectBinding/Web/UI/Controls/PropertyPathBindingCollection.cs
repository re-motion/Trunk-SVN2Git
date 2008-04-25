using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using Remotion.ObjectBinding.Web.UI.Design;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
 
/// <summary> A collection of <see cref="PropertyPathBinding"/> objects. </summary>
[Editor (typeof (PropertyPathBindingCollectionEditor), typeof (UITypeEditor))]
public class PropertyPathBindingCollection : BusinessObjectControlItemCollection
{
  public PropertyPathBindingCollection (IBusinessObjectBoundWebControl ownerControl)
    : base (ownerControl, new Type[] {typeof (PropertyPathBinding)})
  {
  }

  public new PropertyPathBinding[] ToArray()
  {
    ArrayList arrayList = new ArrayList (List);
    return (PropertyPathBinding[]) arrayList.ToArray (typeof (PropertyPathBinding));
  }

  //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
  protected internal new PropertyPathBinding this[int index]
  {
    get { return (PropertyPathBinding) List[index]; }
    set { List[index] = value; }
  }
}

}
