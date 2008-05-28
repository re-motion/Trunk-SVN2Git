using System;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// The <see cref="BindableDomainObjectMixin"/> applies the <see cref="IBusinessObjectWithIdentity"/> implementation for bindable types derived 
  /// from <see cref="DomainObject"/>.
  /// </summary>
  /// <remarks>
  /// If you do not wish to cast to <see cref="IBusinessObject"/> and <see cref="IBusinessObjectWithIdentity"/>, you can use the default 
  /// implementation provided by <see cref="BindableDomainObject"/> type. This type exposes the aforementioned interfaces and delegates their 
  /// implementation to the mixin.
  /// </remarks>
  [Serializable]
  [BindableDomainObjectProvider]
  public class BindableDomainObjectMixin : BindableObjectMixinBase<BindableDomainObjectMixin.IDomainObject>, IBusinessObjectWithIdentity
  {
    public interface IDomainObject
    {
      Type GetPublicDomainObjectType ();
      ObjectID ID { get; }
      PropertyIndexer Properties { get; }
      StateType State { get; }
    }

    protected override BindableObjectClass InitializeBindableObjectClass ()
    {
      return BindableObjectProvider.GetBindableObjectClassFromProvider (This.GetPublicDomainObjectType());
    }

    public string UniqueIdentifier
    {
      get { return This.ID.ToString(); }
    }

    public string BaseDisplayName
    {
      get { return base.DisplayName; }
    }

    protected override bool IsDefaultValue (PropertyBase property, object nativeValue)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      if (This.State != StateType.New)
        return false;
      else
      {
        string propertyIdentifier = ReflectionUtility.GetPropertyName (property.PropertyInfo.GetOriginalDeclaringType(), property.PropertyInfo.Name);
        if (This.Properties.Contains (propertyIdentifier))
          return !This.Properties[propertyIdentifier].HasBeenTouched;
        else
          return base.IsDefaultValue (property, nativeValue);
      }
    }
  }
}
