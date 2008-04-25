using System;
using System.Xml.Serialization;

namespace Remotion.ObjectBinding
{
  // TODO FS: Move to OB.dll
  /// <summary> The <see langword="abstract"/> default implementation of the <see cref="IBusinessObject"/> interface. </summary>
  [Serializable]
  public abstract class BusinessObject : IBusinessObject
  {
    /// <summary> Returns the <see cref="IBusinessObjectProperty"/> for the passed <paramref name="propertyIdentifier"/>. </summary>
    /// <param name="propertyIdentifier"> 
    ///   A <see cref="String"/> uniquely identifying an <see cref="IBusinessObjectProperty"/> in this
    ///   business object.
    /// </param>
    /// <returns> Returns the <see cref="IBusinessObjectProperty"/> or <see langword="null"/>. </returns>
    /// <remarks> 
    ///   <para>
    ///     It is not specified wheter an exception is thrown or <see langword="null"/> is returned if the 
    ///     <see cref="IBusinessObjectProperty"/> could not be found.
    ///   </para><para>
    ///     The default implementation uses the <see cref="BusinessObjectClass"/>'s 
    ///     <see cref="IBusinessObjectClass.GetPropertyDefinition"/> method for finding the 
    ///     <see cref="IBusinessObjectProperty"/>.
    ///   </para>
    /// </remarks>
    public virtual IBusinessObjectProperty GetBusinessObjectProperty (string propertyIdentifier)
    {
      return BusinessObjectClass.GetPropertyDefinition (propertyIdentifier);
    }

    /// <overloads> Gets the value accessed through the specified property. </overloads>
    /// <summary> Gets the value accessed through the specified <see cref="IBusinessObjectProperty"/>. </summary>
    public abstract object GetProperty (IBusinessObjectProperty property);

    /// <summary>
    ///   Gets the value accessed through the <see cref="IBusinessObjectProperty"/> identified by the passed 
    ///   <paramref name="propertyIdentifier"/>. 
    /// </summary>
    public object GetProperty (string propertyIdentifier)
    {
      return GetProperty (GetBusinessObjectProperty (propertyIdentifier));
    }

    /// <overloads> Sets the value accessed through the specified property. </overloads>
    /// <summary> Sets the value accessed through the specified <see cref="IBusinessObjectProperty"/>. </summary>
    public abstract void SetProperty (IBusinessObjectProperty property, object value);

    /// <summary>
    ///   Sets the value accessed through the <see cref="IBusinessObjectProperty"/> identified by the passed 
    ///   <paramref name="propertyIdentifier"/>. 
    /// </summary>
    public void SetProperty (string propertyIdentifier, object value)
    {
      SetProperty (GetBusinessObjectProperty (propertyIdentifier), value);
    }

    /// <overloads> Gets the string representation of the value accessed through the specified property.  </overloads>
    /// <summary> 
    ///   Gets the string representation of the value accessed through the specified 
    ///   <see cref="IBusinessObjectProperty"/>.
    /// </summary>
    public string GetPropertyString (IBusinessObjectProperty property)
    {
      return GetPropertyString (property, null);
    }

    /// <summary> 
    ///   Gets the formatted string representation of the value accessed through the specified 
    ///   <see cref="IBusinessObjectProperty"/>.
    /// </summary>
    public virtual string GetPropertyString (IBusinessObjectProperty property, string format)
    {
      return StringFormatterService.GetPropertyString (this, property, format);
    }

    /// <summary> 
    ///   Gets the string representation of the value accessed through the <see cref="IBusinessObjectProperty"/> 
    ///   identified by the passed <paramref name="propertyIdentifier"/>.
    /// </summary>
    public string GetPropertyString (string propertyIdentifier)
    {
      return GetPropertyString (GetBusinessObjectProperty (propertyIdentifier));
    }

    /// <summary> Gets the <see cref="IBusinessObjectClass"/> of this business object. </summary>
    // Not to be serialized.
    [XmlIgnore]
    public abstract IBusinessObjectClass BusinessObjectClass { get; }

    /// <summary> Gets the human readable representation of this <see cref="IBusinessObject"/>. </summary>
    /// <value> A <see cref="string"/> identifying this object to the user. </value>
    public abstract string DisplayName { get; }

    /// <summary>
    ///   Gets the value of <see cref="DisplayName"/> if it is accessible and otherwise falls back to the <see cref="string"/> returned by
    ///   <see cref="IBusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder"/>.
    /// </summary>
    string IBusinessObject.DisplayNameSafe
    {
      get
      {
        IBusinessObjectClass businessObjectClass = BusinessObjectClass;
        IBusinessObjectProperty displayNameProperty = businessObjectClass.GetPropertyDefinition ("DisplayName");
        if (displayNameProperty.IsAccessible (businessObjectClass, this))
          return DisplayName;

        return businessObjectClass.BusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder ();
      }
    }

    /// <summary>Gets the <see cref="BusinessObjectStringFormatterService"/> used to convert the property values to strings.</summary>
    protected abstract IBusinessObjectStringFormatterService StringFormatterService { get; }
  }
}