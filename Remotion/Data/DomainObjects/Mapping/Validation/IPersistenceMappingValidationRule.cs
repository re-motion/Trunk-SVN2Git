namespace Remotion.Data.DomainObjects.Mapping.Validation
{
  /// <summary>
  /// Defines the API for the persistence mapping validation rules.
  /// </summary>
  public interface IPersistenceMappingValidationRule
  {
    MappingValidationResult Validate (ClassDefinition classDefinition);
  }
}