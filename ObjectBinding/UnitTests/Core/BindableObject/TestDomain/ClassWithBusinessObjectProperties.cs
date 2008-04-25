using System;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  [BindableObject]
  public class ClassWithBusinessObjectProperties
  {
    private ClassWithSearchServiceTypeAttribute _searchServiceOnType;
    private ClassWithSearchServiceTypeAttribute _searchServiceOnProperty;
    private ClassWithOtherBusinessObjectImplementation _noSearchService;

    public ClassWithBusinessObjectProperties ()
    {
    }

    public ClassWithSearchServiceTypeAttribute SearchServiceFromType
    {
      get { return _searchServiceOnType; }
      set { _searchServiceOnType = value; }
    }

    [SearchAvailableObjectsServiceType (typeof (ISearchServiceOnProperty))]
    public ClassWithSearchServiceTypeAttribute SearchServiceFromProperty
    {
      get { return _searchServiceOnProperty; }
      set { _searchServiceOnProperty = value; }
    }

    public ClassWithOtherBusinessObjectImplementation NoSearchService
    {
      get { return _noSearchService; }
      set { _noSearchService = value; }
    }
  }
}