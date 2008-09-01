using System;

namespace Remotion.Diagnostics.ToText.Handlers
{
  /// <summary>
  /// Settings class for the <see cref="ToTextProvider"/> class.
  /// </summary>
  public class ToTextProviderSettings
  {
    public ToTextProviderSettings ()
    {
      UseAutomaticObjectToText = true;
      EmitPublicProperties = true;
      EmitPublicFields = true;
      EmitPrivateProperties = true;
      EmitPrivateFields = true;

      UseAutomaticStringEnclosing = true;
      UseAutomaticCharEnclosing = true;

      UseInterfaceHandlers = true;

      ParentHandlerSearchDepth = 0;
      ParentHandlerSearchUpToRoot = true;
      UseParentHandlers = false;
    }

    public bool UseAutomaticObjectToText { get; set; }
    public bool EmitPublicProperties { get; set; }
    public bool EmitPublicFields { get; set; }
    public bool EmitPrivateProperties { get; set; }
    public bool EmitPrivateFields { get; set; }

    public bool UseAutomaticStringEnclosing { get; set; }
    public bool UseAutomaticCharEnclosing { get; set; }

    public int ParentHandlerSearchDepth { get; set; }
    public bool ParentHandlerSearchUpToRoot { get; set; }
    public bool UseParentHandlers { get; set; }

    public bool UseInterfaceHandlers { get; set; }

    public void SetAutomaticObjectToTextEmit (bool emitPublicProperties, bool emitPublicFields, bool emitPrivateProperties, bool emitPrivateFields)
    {
      EmitPublicProperties = emitPublicProperties;
      EmitPublicFields = emitPublicFields;
      EmitPrivateProperties = emitPrivateProperties;
      EmitPrivateFields = emitPrivateFields;
    }    
  }
}