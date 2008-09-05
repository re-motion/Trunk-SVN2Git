using System;
using System.Collections;
using System.Linq.Expressions;

namespace Remotion.Diagnostics.ToText
{
  public interface IToTextBuilderBase
  {
    ////SequenceStateHolder SequenceState { get; protected set; }
    //ToTextBuilderBase.ToTextBuilderOutputComplexityLevel OutputComplexity { get; }
    //bool IsInSequence { get; }
    //ToTextProvider ToTextProvider { get; set; }
    //IToTextBuilderBase cSkeleton { get; }
    //IToTextBuilderBase cBasic { get; }
    //IToTextBuilderBase cMedium { get; }
    //IToTextBuilderBase cComplex { get; }
    //IToTextBuilderBase cFull { get; }
    //void OutputDisable ();
    //void OutputSkeleton ();
    //void OutputBasic ();
    //void OutputMedium ();
    //void OutputComplex ();
    //void OutputFull ();
    //IToTextBuilderBase AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderBase.ToTextBuilderOutputComplexityLevel complexityLevel);
    //IToTextBuilderBase Flush ();
    //IToTextBuilderBase ts (object obj);
    //IToTextBuilderBase AppendSequenceBegin (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix);
    
    //IToTextBuilderBase sb ();
    //IToTextBuilderBase sb (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix);
    //IToTextBuilderBase sb (string sequencePrefix, string separator, string sequencePostfix);
    //IToTextBuilderBase sb (string sequencePrefix, string sequencePostfix);

    //IToTextBuilderBase tt (Object obj);
    //IToTextBuilderBase tt (Object obj, bool honorSequence);

    //IToTextBuilderBase AppendString (string s);
    //IToTextBuilderBase s (string s);
    //IToTextBuilderBase AppendChar (char c);
    //IToTextBuilderBase AppendMember (string name, Object obj);
    //IToTextBuilderBase AppendMember<T> (Expression<Func<object, T>> expression);
    //IToTextBuilderBase AppendMemberNonSequence (string name, Object obj);
    //IToTextBuilderBase m (Object obj);
    //IToTextBuilderBase m (string name, Object obj, bool honorSequence);
    //IToTextBuilderBase m<T> (Expression<Func<object, T>> expression);
    //IToTextBuilderBase m (string name, Object obj);
    //IToTextBuilderBase AppendEnumerable (IEnumerable collection);
    //IToTextBuilderBase collection (IEnumerable collection);
    //IToTextBuilderBase AppendArray (Array array);
    //IToTextBuilderBase AppendToText (Object obj);
    //IToTextBuilderBase Append (string s);
    //IToTextBuilderBase Append (Object obj);
    //IToTextBuilderBase beginInstance (Type type);
    //IToTextBuilderBase endInstance ();
    //IToTextBuilderBase AppendSequenceEnd ();
    //IToTextBuilderBase se ();
    //IToTextBuilderBase AppendSequenceElement (object obj);
    //IToTextBuilderBase e (object obj);
    //IToTextBuilderBase AppendSequenceElements (params object[] sequenceElements);
    //IToTextBuilderBase elements (params object[] sequenceElements);
    //IToTextBuilderBase elementsNumbered (string s1, int i0, int i1);



    //ToTextBuilderSettings Settings { get; }
    bool UseMultiLine { get; set; }
    bool Enabled { get; set; }
    IToTextBuilderBase seperator { get; }
    IToTextBuilderBase comma { get; }
    IToTextBuilderBase colon { get; }
    IToTextBuilderBase semicolon { get; }
    //SequenceStateHolder SequenceState { get; protected set; }
    ToTextBuilderBase.ToTextBuilderOutputComplexityLevel OutputComplexity { get; }
    bool IsInSequence { get; }
    ToTextProvider ToTextProvider { get; set; }
    IToTextBuilderBase cSkeleton { get; }
    IToTextBuilderBase cBasic { get; }
    IToTextBuilderBase cMedium { get; }
    IToTextBuilderBase cComplex { get; }
    IToTextBuilderBase cFull { get; }
    IToTextBuilderBase AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderBase.ToTextBuilderOutputComplexityLevel complexityLevel);
    string CheckAndConvertToString ();
    ToTextBuilder ToText (object obj);
    IToTextBuilderBase Flush ();
    IToTextBuilderBase sf (string format, params object[] paramArray);
    ToTextBuilder AppendNewLine ();
    ToTextBuilder nl ();
    ToTextBuilder AppendSpace ();
    IToTextBuilderBase space ();
    ToTextBuilder AppendTabulator ();
    ToTextBuilder tab ();
    ToTextBuilder AppendSeperator ();
    ToTextBuilder AppendComma ();
    ToTextBuilder AppendColon ();
    ToTextBuilder AppendSemiColon ();
    IToTextBuilderBase AppendArray (Array array);
    IToTextBuilderBase AppendString (string s);
    ToTextBuilder AppendEscapedString (string s);
    ToTextBuilder sEsc (string s);
    IToTextBuilderBase AppendChar (char c);
    IToTextBuilderBase AppendMember (string name, Object obj);
    IToTextBuilderBase AppendEnumerable (IEnumerable collection);
    IToTextBuilderBase array (Array array);
    IToTextBuilderBase Append (Object obj);
    IToTextBuilderBase ToTextString (string s);
    void OutputDisable ();
    void OutputSkeleton ();
    void OutputBasic ();
    void OutputMedium ();
    void OutputComplex ();
    void OutputFull ();
    IToTextBuilderBase ts (object obj);
    IToTextBuilderBase AppendSequenceBegin (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix);
    IToTextBuilderBase sb ();
    IToTextBuilderBase sb (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix);
    IToTextBuilderBase sb (string sequencePrefix, string separator, string sequencePostfix);
    IToTextBuilderBase sb (string sequencePrefix, string sequencePostfix);
    IToTextBuilderBase s (string s);
    IToTextBuilderBase AppendMember<T> (Expression<Func<object, T>> expression);
    IToTextBuilderBase AppendMemberNonSequence (string name, Object obj);
    IToTextBuilderBase m (Object obj);
    IToTextBuilderBase m (string name, Object obj, bool honorSequence);
    IToTextBuilderBase m<T> (Expression<Func<object, T>> expression);
    IToTextBuilderBase m (string name, Object obj);
    IToTextBuilderBase collection (IEnumerable collection);
    IToTextBuilderBase AppendToText (Object obj);
    IToTextBuilderBase tt (Object obj);
    IToTextBuilderBase tt (Object obj, bool honorSequence);
    IToTextBuilderBase AppendToTextNonSequence (Object obj);
    IToTextBuilderBase Append (string s);
    IToTextBuilderBase beginInstance (Type type);
    IToTextBuilderBase endInstance ();
    IToTextBuilderBase AppendSequenceEnd ();
    IToTextBuilderBase se ();
    IToTextBuilderBase AppendSequenceElement (object obj);
    IToTextBuilderBase e (object obj);
    IToTextBuilderBase AppendSequenceElements (params object[] sequenceElements);
    IToTextBuilderBase elements (params object[] sequenceElements);
    IToTextBuilderBase elementsNumbered (string s1, int i0, int i1);
  }
}