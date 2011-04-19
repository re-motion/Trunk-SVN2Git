using System;
using System.Collections.Generic;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="ColumnDefinitionFinder"/> checks if the visited column is available in the specified column definitions and if yes, the visited
  /// column is returned. Otherwise a <see cref="NullColumnDefinition"/> is returned.
  /// </summary>
  public class ColumnDefinitionFinder : IColumnDefinitionVisitor
  {
    private readonly HashSet<IColumnDefinition> _availableColumns;

    private IColumnDefinition _foundColumn;

    public ColumnDefinitionFinder (IEnumerable<IColumnDefinition> availableColumns)
    {
      _availableColumns = new HashSet<IColumnDefinition> (availableColumns);
    }

    public IColumnDefinition FindColumn (IColumnDefinition columnDefinition)
    {
      _foundColumn = null;
      if(columnDefinition!=null)
        columnDefinition.Accept (this);
      return _foundColumn;
    }

    void IColumnDefinitionVisitor.VisitSimpleColumnDefinition (SimpleColumnDefinition simpleColumnDefinition)
    {
      if (_availableColumns.Contains (simpleColumnDefinition))
        _foundColumn = simpleColumnDefinition;
      else
        _foundColumn = new NullColumnDefinition ();
    }

    public void VisitSqlIndexedColumnDefinition (SqlIndexedColumnDefinition indexedColumnDefinition)
    {
      if (_availableColumns.Contains (indexedColumnDefinition))
      {
        _foundColumn = indexedColumnDefinition;
      }
      else
      {
        var innerColumn = FindColumn (indexedColumnDefinition.Columnn);
        _foundColumn = new SqlIndexedColumnDefinition (innerColumn, indexedColumnDefinition.IndexOrder);
      }
    }

    void IColumnDefinitionVisitor.VisitIDColumnDefinition (IDColumnDefinition idColumnDefinition)
    {
      if (_availableColumns.Contains (idColumnDefinition))
      {
        _foundColumn = idColumnDefinition;
      }
      else
      {
        var objectIDColumn = FindColumn (idColumnDefinition.ObjectIDColumn);
        var classIDColumn = FindColumn (idColumnDefinition.ClassIDColumn);
        _foundColumn = new IDColumnDefinition (objectIDColumn, classIDColumn);
      }
    }

    void IColumnDefinitionVisitor.VisitNullColumnDefinition (NullColumnDefinition nullColumnDefinition)
    {
      _foundColumn = new NullColumnDefinition ();
    }
  }
}