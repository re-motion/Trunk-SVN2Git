using System;
using System.Collections;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Sample
{
  public class PersonTreeView: BocTreeView
  {
    public PersonTreeView()
    {
    }

    protected override BusinessObjectPropertyTreeNodeInfo[] GetPropertyNodes (IBusinessObjectWithIdentity businessObject)
    { 
      BusinessObjectPropertyTreeNodeInfo[] nodeInfos;
      if (businessObject is Person)
      {
        nodeInfos = new BusinessObjectPropertyTreeNodeInfo[2];
        nodeInfos[0] = new BusinessObjectPropertyTreeNodeInfo (
            "Children", 
            "ToolTip: Children", 
            new IconInfo(null, Unit.Empty, Unit.Empty),
            (IBusinessObjectReferenceProperty) businessObject.BusinessObjectClass.GetPropertyDefinition ("Children"));
        nodeInfos[1] = new BusinessObjectPropertyTreeNodeInfo (
            "Jobs", 
            "ToolTip: Jobs",
            new IconInfo(null, Unit.Empty, Unit.Empty),
            (IBusinessObjectReferenceProperty) businessObject.BusinessObjectClass.GetPropertyDefinition ("Jobs"));
      }
      else
      {
        nodeInfos = new BusinessObjectPropertyTreeNodeInfo[0];
      }

      return nodeInfos;                                
    }

    protected override IBusinessObjectWithIdentity[] GetBusinessObjects(IBusinessObjectWithIdentity parent, IBusinessObjectReferenceProperty property)
    {
      if (parent.UniqueIdentifier == new Guid (0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1).ToString())
      {
        IList children = (IList) parent.GetProperty (property);
        ArrayList childrenList = new ArrayList();
        for (int i = 0; i < children.Count; i++)
        {
          if (i != 1)
            childrenList.Add (children[i]);
        }
        return  (IBusinessObjectWithIdentity[]) childrenList.ToArray (typeof (IBusinessObjectWithIdentity));
      }
      return base.GetBusinessObjects (parent, property);
    }

  }
}