using System;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Sample
{
  public class BindableXmlObjectSearchService : ISearchAvailableObjectsService
  {
    public BindableXmlObjectSearchService ()
    {
    }

    public bool SupportsIdentity (IBusinessObjectReferenceProperty property)
    {
      return true;
    }

    public IBusinessObject[] Search (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, string searchStatement)
    {
      ReferenceProperty referenceProperty = ArgumentUtility.CheckNotNullAndType<ReferenceProperty> ("property", property);
      BindableObjectClass bindableObjectClass = (BindableObjectClass) referenceProperty.ReferenceClass;

      return (IBusinessObject[]) ArrayUtility.Convert (
                                     XmlReflectionBusinessObjectStorageProvider.Current.GetObjects (bindableObjectClass.TargetType),
                                     typeof (IBusinessObject));
    }
  }
}