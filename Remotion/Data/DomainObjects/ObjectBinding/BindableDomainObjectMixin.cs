// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Mixins.CodeGeneration;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Mixins;

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
      return BindableObjectProvider.GetBindableObjectClass (This.GetPublicDomainObjectType());
    }

    public string UniqueIdentifier
    {
      get { return This.ID.ToString(); }
    }

    protected override bool IsDefaultValue (PropertyBase property, object nativeValue)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      if (This.State != StateType.New)
        return false;
      else
      {
        string propertyIdentifier = GetMappingPropertyIdentifier(property);
        if (This.Properties.Contains (propertyIdentifier))
          return !This.Properties[propertyIdentifier].HasBeenTouched;
        else
          return base.IsDefaultValue (property, nativeValue);
      }
    }

    public virtual string GetMappingPropertyIdentifier (PropertyBase property)
    {
      Type originalDeclaringType = property.PropertyInfo.GetOriginalDeclaringType();
      var interfacePropertyInfo = property.PropertyInfo.InterfacePropertyInfo;

      if (interfacePropertyInfo != null && Mixins.MixinTypeUtility.IsGeneratedConcreteMixedType (originalDeclaringType))
      {
        // this property was added by a mixin, get the correct mapping property name

        var introducedMemberAttribute = property.PropertyInfo.GetCustomAttribute<IntroducedMemberAttribute> (true);
        return MappingConfiguration.Current.NameResolver.GetPropertyName (introducedMemberAttribute.Mixin, introducedMemberAttribute.MixinMemberName);
      }

      return MappingConfiguration.Current.NameResolver.GetPropertyName (originalDeclaringType, property.PropertyInfo.Name);
    }
  }
}
