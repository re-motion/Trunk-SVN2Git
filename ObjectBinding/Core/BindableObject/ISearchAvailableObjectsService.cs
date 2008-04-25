namespace Remotion.ObjectBinding.BindableObject
{
  //TODO: doc
  public interface ISearchAvailableObjectsService : IBusinessObjectService
  {
    bool SupportsIdentity (IBusinessObjectReferenceProperty property);

    IBusinessObject[] Search (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, string searchStatement);
  }
}