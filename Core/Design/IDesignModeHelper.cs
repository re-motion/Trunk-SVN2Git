using System.ComponentModel;
using System.ComponentModel.Design;

namespace Remotion.Design
{
  /// <summary>
  /// The <see cref="IDesignModeHelper"/> interface defines methods to encapsulate the access to various design-mode properties of a project.
  /// It is intended to be used by components offering design-time support.
  /// </summary>
  public interface IDesignModeHelper
  {
    IDesignerHost DesignerHost { get; }
    string GetProjectPath();
    System.Configuration.Configuration GetConfiguration();
  }
}