using System;
using System.Collections;
using System.Linq.Expressions;

namespace Remotion.Diagnostics.ToText
{
  public interface IToTextBuilderBase
  {
    //ToTextBuilderSettings Settings { get; }
    bool UseMultiLine { get; set; }

    bool Enabled { get; set; }
    bool IsInSequence { get; }

    //SequenceStateHolder SequenceState { get; protected set; }
    ToTextBuilderBase.ToTextBuilderOutputComplexityLevel OutputComplexity { get; }
    ToTextProvider ToTextProvider { get; set; }
    IToTextBuilderBase cSkeleton { get; }
    IToTextBuilderBase cBasic { get; }
    IToTextBuilderBase cMedium { get; }
    IToTextBuilderBase cComplex { get; }
    IToTextBuilderBase cFull { get; }
    IToTextBuilderBase AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderBase.ToTextBuilderOutputComplexityLevel complexityLevel);


    string CheckAndConvertToString ();
    IToTextBuilderBase Flush ();


    void AppendRawElementBegin ();
    void AppendRawElementEnd ();
    
    IToTextBuilderBase AppendNewLine ();
    IToTextBuilderBase nl ();

    IToTextBuilderBase AppendRawString (string s);
    IToTextBuilderBase AppendRawEscapedString (string s);
    IToTextBuilderBase sEsc (string s);

    IToTextBuilderBase AppendRawChar (char c);

    //IToTextBuilderBase ToTextString (string s);
    void OutputDisable ();
    void OutputSkeleton ();
    void OutputBasic ();
    void OutputMedium ();
    void OutputComplex ();
    void OutputFull ();


    IToTextBuilderBase s (string s);

    IToTextBuilderBase ts (object obj);

    IToTextBuilderBase Append (Object obj);
    IToTextBuilderBase Append (string s);


    /// <summary>
    /// <para>Applies <see cref="ToText"/> to the passed argument and emits the result to the TextBuilder.</para>
    /// <para>Shorthand notation: <see cref="tt(object)"/>.</para>
    /// </summary>
    IToTextBuilderBase EmitToText (object obj);
    IToTextBuilderBase AppendToText (Object obj);
    IToTextBuilderBase tt (Object obj);
    IToTextBuilderBase tt (Object obj, bool honorSequence);
    IToTextBuilderBase AppendToTextNonSequence (Object obj);



    IToTextBuilderBase AppendMember (string name, Object obj);
    IToTextBuilderBase AppendMemberNonSequence (string name, Object obj);
    IToTextBuilderBase AppendMember<T> (Expression<Func<object, T>> expression);
    IToTextBuilderBase m (Object obj);
    IToTextBuilderBase m (string name, Object obj, bool honorSequence);
    IToTextBuilderBase m<T> (Expression<Func<object, T>> expression);
    IToTextBuilderBase m (string name, Object obj);
    


    IToTextBuilderBase AppendSequenceBegin (string name, string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix);
    IToTextBuilderBase sb ();
    IToTextBuilderBase sb (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix);
    IToTextBuilderBase sb (string sequencePrefix, string separator, string sequencePostfix);
    IToTextBuilderBase sb (string sequencePrefix, string sequencePostfix);
    IToTextBuilderBase AppendSequenceEnd ();
    IToTextBuilderBase se ();
    
    IToTextBuilderBase AppendSequenceElement (object obj);
    IToTextBuilderBase e (object obj);

    IToTextBuilderBase AppendSequenceElements (params object[] sequenceElements);
    IToTextBuilderBase elements (params object[] sequenceElements);
    IToTextBuilderBase elementsNumbered (string s1, int i0, int i1);

    IToTextBuilderBase beginInstance (Type type);
    IToTextBuilderBase endInstance ();

    IToTextBuilderBase AppendArray (Array array);
    IToTextBuilderBase array (Array array);
    
    IToTextBuilderBase AppendEnumerable (IEnumerable collection);
    IToTextBuilderBase collection (IEnumerable collection);
  }
}