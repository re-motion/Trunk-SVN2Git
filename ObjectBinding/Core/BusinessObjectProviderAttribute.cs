using System;
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// Use the <see cref="BusinessObjectProviderAttribute"/> to associate a <see cref="IBusinessObjectProvider"/> with a business object implementation.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
  public abstract class BusinessObjectProviderAttribute : Attribute
  {
    private readonly Type _businessObjectProviderType;

    protected BusinessObjectProviderAttribute (Type businessObjectProviderType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("businessObjectProviderType", businessObjectProviderType, typeof (IBusinessObjectProvider));
      _businessObjectProviderType = businessObjectProviderType;
    }

    public Type BusinessObjectProviderType
    {
      get { return _businessObjectProviderType; }
    }
  }
}