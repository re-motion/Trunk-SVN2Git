using System;
using Remotion.Globalization;

namespace Remotion.ObjectBinding.BindableObject.Properties
{
  public class BooleanProperty : PropertyBase, IBusinessObjectBooleanProperty, IBusinessObjectEnumerationProperty
  {
    private readonly BooleanToEnumPropertyConverter _booleanToEnumPropertyConverter;


    public BooleanProperty (Parameters parameters)
        : base (parameters)
    {
      _booleanToEnumPropertyConverter = new BooleanToEnumPropertyConverter (this);
    }

    /// <summary> Returns the human readable value of the boolean property. </summary>
    /// <param name="value"> The <see cref="bool"/> value to be formatted. </param>
    /// <returns> The human readable string value of the boolean property. </returns>
    /// <remarks> The value of this property may depend on the current culture. </remarks>
    public string GetDisplayName (bool value)
    {
      IBindableObjectGlobalizationService globalizationService = BusinessObjectProvider.GetService<IBindableObjectGlobalizationService> ();
      if (globalizationService == null)
        return value.ToString ();
      return globalizationService.GetBooleanValueDisplayName (value);
    }

    /// <summary> Returns the default value to be assumed if the boolean property returns <see langword="null"/>. </summary>
    /// <param name="objectClass"> The <see cref="IBusinessObjectClass"/> for which to get the property's default value. </param>
    /// <remarks> 
    ///   If <see langword="null"/> is returned, the object model does not define a default value. In case the 
    ///   caller requires a default value, the selection of the appropriate value is left to the caller.
    /// </remarks>
    public bool? GetDefaultValue (IBusinessObjectClass objectClass)
    {
      if (IsNullable)
        return null;
      return false;
    }

    /// <summary> Returns a list of all the enumeration's values. </summary>
    /// <returns> 
    ///   A list of <see cref="IEnumerationValueInfo"/> objects encapsulating the values defined in the enumeration. 
    /// </returns>
    public IEnumerationValueInfo[] GetAllValues (IBusinessObject businessObject)
    {
      return _booleanToEnumPropertyConverter.GetValues();
    }

    /// <summary> Returns a list of the enumeration's values that can be used in the current context. </summary>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> used to determine the enabled enum values. </param>
    /// <returns>A list of <see cref="IEnumerationValueInfo"/> objects encapsulating the enabled values in the enumeration. </returns>
    /// <remarks> CLS type enums do not inherently support the disabling of its values. </remarks>
    public IEnumerationValueInfo[] GetEnabledValues (IBusinessObject businessObject)
    {
      return _booleanToEnumPropertyConverter.GetValues();
    }

    /// <overloads> Returns a specific enumeration value. </overloads>
    /// <summary> Returns a specific enumeration value. </summary>
    /// <param name="value"> The enumeration value to return the <see cref="IEnumerationValueInfo"/> for. </param>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> used to determine whether the enum value is enabled. </param>
    /// <returns> The <see cref="IEnumerationValueInfo"/> object for the provided <paramref name="value"/>. </returns>
    public IEnumerationValueInfo GetValueInfoByValue (object value, IBusinessObject businessObject)
    {
      return _booleanToEnumPropertyConverter.GetValueInfoByValue (value);
    }

    /// <summary> Returns a specific enumeration value. </summary>
    /// <param name="identifier">The string identifying the  enumeration value to return the <see cref="IEnumerationValueInfo"/> for.</param>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> used to determine whether the enum value is enabled. </param>
    /// <returns> The <see cref="IEnumerationValueInfo"/> object for the provided <paramref name="identifier"/>. </returns>
    public IEnumerationValueInfo GetValueInfoByIdentifier (string identifier, IBusinessObject businessObject)
    {
      return _booleanToEnumPropertyConverter.GetValueInfoByIdentifier (identifier);
    }
  }
}