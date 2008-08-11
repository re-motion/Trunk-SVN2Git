using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remotion.Text.Diagnostic
{
  public class OuterProduct
  {
    private int[] _numberElementsPerDimension;
    public int[] NumberElementsPerDimension
    {
      get { return _numberElementsPerDimension; }
      private set { _numberElementsPerDimension = value; }
    }

    private readonly int _numberElementsOverall;
    public int NumberElementsOverall
    {
      get { return _numberElementsOverall; }
    }

    public int Length { get { return NumberElementsOverall; } }

    public OuterProduct (int[] numberElementsPerDimension)
    {
      _numberElementsPerDimension = (int[]) numberElementsPerDimension.Clone ();
      _numberElementsOverall = CalcOuterProductNrElementsOverall (_numberElementsPerDimension);
    }

    public OuterProduct (Array array)
    {
      int numberDimensions = array.Rank;
      _numberElementsPerDimension = new int[numberDimensions];
      for (int iDimension = 0; iDimension < numberDimensions; ++iDimension)
      {
        _numberElementsPerDimension[iDimension] = array.GetLength (iDimension);
      }
      _numberElementsOverall = CalcOuterProductNrElementsOverall (_numberElementsPerDimension);
    }


    public static int CalcOuterProductNrElementsOverall (int[] numberElementsPerDimension)
    {
      if (numberElementsPerDimension.Length <= 0)
      {
        return 0;
      }
      else
      {
        int numberStateCombinations = 1;
        foreach (var numberElements in numberElementsPerDimension)
        {
          numberStateCombinations *= numberElements;
        }
        return numberStateCombinations;
      }
    }


 
    public string VisitedIndicesToString ()
    {

      if (_numberElementsOverall == 0)
      {
        return null;
      }

      int numberDimensions = _numberElementsPerDimension.Length;
      int[] currentDimensionIndices = new int[numberDimensions];

      string result = "";

      for (int iAllElements = 0; iAllElements < _numberElementsOverall; ++iAllElements)
      {
        //result += (iAllElements == 0) ? "" : ",";
        //result += "(";
        //for (int iDimension = 0; iDimension < numberDimensions; ++iDimension)
        //{
        //  result += (iDimension == 0) ? "" : ",";
        //  result += currentDimensionIndices[iDimension];
        //}
        //result += ")";

        CalculateCurrentCombination (ref result, currentDimensionIndices, iAllElements);

        PrepareNextCombination (currentDimensionIndices);
      }

      return result;
    }

    private void PrepareNextCombination (int[] currentDimensionIndices)
    {
      //for (int iDimension = 0; iDimension < currentDimensionIndices.Length; ++iDimension)
      for (int iDimension = currentDimensionIndices.Length - 1; iDimension >= 0; --iDimension)
      {
        ++currentDimensionIndices[iDimension];
        if (currentDimensionIndices[iDimension] < _numberElementsPerDimension[iDimension])
        {
          break;
        }
        else
        {
          currentDimensionIndices[iDimension] = 0;
        }
      }
    }

    private void CalculateCurrentCombination (ref string result, int[] currentDimensionIndices, int iAllElements)
    {
      result += (iAllElements == 0) ? "" : ",";
      result += "(";
      for (int iDimension = 0; iDimension < currentDimensionIndices.Length; ++iDimension)
      {
        result += (iDimension == 0) ? "" : ",";
        result += currentDimensionIndices[iDimension];
      }
      result += ")";
    }



  }
}
