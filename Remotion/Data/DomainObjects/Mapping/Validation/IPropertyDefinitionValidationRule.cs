namespace Remotion.Data.DomainObjects.Mapping.Validation
{
  /// <summary>
  /// Defines the API for the property definition mapping validation rules.
  /// </summary>
  public interface IPropertyDefinitionValidationRule
  {
    MappingValidationResult Validate (ClassDefinition classDefinition);
  }
}