using System;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace OBWTest.IndividualControlTests
{

public class BaseUserControl : 
    DataEditUserControl, 
    IObjectWithResources //  Provides the UserControl's ResourceManager via GetResourceManager() 
{
  protected virtual void RegisterEventHandlers ()
  {
  }
  
	override protected void OnInit(EventArgs e)
	{
    RegisterEventHandlers ();
		base.OnInit(e);
	}

  protected override void OnPreRender(EventArgs e)
  {
    //  A call to the ResourceDispatcher to get have the automatic resources dispatched
    ResourceDispatcher.Dispatch (this, ResourceManagerUtility.GetResourceManager (this));

    base.OnPreRender (e);
  }

  protected virtual IResourceManager GetResourceManager()
  {
    Type type = GetType();
    if (MultiLingualResources.ExistsResource (type))
      return MultiLingualResources.GetResourceManager (type, true);
    else
      return null;
  }

  IResourceManager IObjectWithResources.GetResourceManager()
  {
    return GetResourceManager();
  }
}

}
