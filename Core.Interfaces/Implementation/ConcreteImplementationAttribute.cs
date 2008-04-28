using System;

namespace Remotion.Implementation
{
  [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
  public class ConcreteImplementationAttribute : Attribute
  {
    private readonly string _partialTypeNameTemplate;

    public ConcreteImplementationAttribute (string partialTypeNameTemplate)
    {
      _partialTypeNameTemplate = ArgumentUtility.CheckNotNull ("partialTypeNameTemplate", partialTypeNameTemplate);
    }

    public string PartialTypeNameTemplate
    {
      get { return _partialTypeNameTemplate; }
    }

    public string GetPartialTypeName()
    {
      return _partialTypeNameTemplate.Replace ("<version>", FrameworkVersion.Value.ToString());
    }

    public Type ResolveType ()
    {
      return Type.GetType (GetPartialTypeName(), true);
    }

    public object InstantiateType ()
    {
      return Activator.CreateInstance (ResolveType());
    }
  }
}