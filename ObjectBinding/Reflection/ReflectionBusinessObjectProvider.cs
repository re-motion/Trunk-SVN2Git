using System;
using System.Collections;
using Remotion.Collections;
using Remotion.ObjectBinding.Web;

namespace Remotion.ObjectBinding.Reflection.Legacy
{

public class ReflectionBusinessObjectProvider: BusinessObjectProvider
{
  private readonly static ReflectionBusinessObjectProvider s_instance = new ReflectionBusinessObjectProvider();

  public static ReflectionBusinessObjectProvider Instance 
  {
    get { return s_instance; }
  }

  private ReflectionBusinessObjectProvider()
  {
    _serviceCache.Add (typeof (IBusinessObjectWebUIService), new ReflectionBusinessObjectWebUIService ());
  }

  private readonly InterlockedCache<Type, IBusinessObjectService> _serviceCache = new InterlockedCache<Type, IBusinessObjectService> ();

  protected override ICache<Type, IBusinessObjectService> ServiceCache
  {
    get { return _serviceCache; }
  }

}

}
