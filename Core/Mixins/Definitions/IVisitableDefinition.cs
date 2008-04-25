using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Mixins.Definitions
{
  public interface IVisitableDefinition
  {
    void Accept (IDefinitionVisitor visitor);
    string FullName { get; }
    IVisitableDefinition Parent { get; }
  }
}
