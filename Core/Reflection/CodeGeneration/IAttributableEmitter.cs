using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Reflection.Emit;

namespace Remotion.Reflection.CodeGeneration
{
  public interface IAttributableEmitter
  {
    void AddCustomAttribute (CustomAttributeBuilder customAttribute);
  }
}