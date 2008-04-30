using System;
using Remotion.Data.DomainObjects;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// Provides a base class for bindable <see cref="DomainObject"/> classes.
  /// </summary>
  /// <remarks>
  /// Deriving from this base class is equivalent to deriving from the <see cref="DomainObject"/> class and applying the
  /// <see cref="BindableDomainObjectAttribute"/> to the derived class.
  /// </remarks>
  [BindableDomainObject]
  [Serializable]
  public abstract class BindableDomainObject : DomainObject, IBusinessObjectWithIdentity
  {
    /// <summary>
    /// Provides a possibility to override the display name of the bindable domain object.
    /// </summary>
    /// <value>The display name.</value>
    /// <remarks>Override this property to replace the default display name provided by the <see cref="BindableObjectClass"/> with a custom one.
    /// </remarks>
    [OverrideMixin]
    [StorageClassNone]
    public virtual string DisplayName
    {
      get { return Mixin.Get<BindableDomainObjectMixin> (this).BaseDisplayName; }
    }

    [StorageClassNone]
    private IBusinessObjectWithIdentity BindableDomainObjectMixin
    {
      get { return Mixin.Get<BindableDomainObjectMixin> (this); }
    }

    string IBusinessObjectWithIdentity.UniqueIdentifier
    {
      get { return BindableDomainObjectMixin.UniqueIdentifier; }
    }

    object IBusinessObject.GetProperty (IBusinessObjectProperty property)
    {
      return BindableDomainObjectMixin.GetProperty (property);
    }

    object IBusinessObject.GetProperty (string propertyIdentifier)
    {
      return BindableDomainObjectMixin.GetProperty (propertyIdentifier);
    }

    void IBusinessObject.SetProperty (IBusinessObjectProperty property, object value)
    {
      BindableDomainObjectMixin.SetProperty (property, value);
    }

    void IBusinessObject.SetProperty (string propertyIdentifier, object value)
    {
      BindableDomainObjectMixin.SetProperty (propertyIdentifier, value);
    }

    string IBusinessObject.GetPropertyString (IBusinessObjectProperty property, string format)
    {
      return BindableDomainObjectMixin.GetPropertyString (property, format);
    }

    string IBusinessObject.GetPropertyString (string propertyIdentifier)
    {
      return BindableDomainObjectMixin.GetPropertyString (propertyIdentifier);
    }

    string IBusinessObject.DisplayNameSafe
    {
      get { return BindableDomainObjectMixin.DisplayNameSafe; }
    }

    IBusinessObjectClass IBusinessObject.BusinessObjectClass
    {
      get { return BindableDomainObjectMixin.BusinessObjectClass; }
    }
  }
}
