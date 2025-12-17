// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using MigraDoc.DocumentObjectModel.Tables;

namespace VectorAi.MarkdownToPdf.Styling.Style;

/// <summary>
/// Part of <see cref="CascadingStyle"/> defining table style
/// </summary>

public class TableStyle
{
    private List<TableColumnStyle> _columns;

    /// <summary>
    /// Alignnment of the entire table
    /// </summary>
    public RowAlignment? HorizontalAlignment { get; set; }

    /// <summary>
    /// Horizontal alignment of cell content. Can be directtly applied to a single cell as well
    /// </summary>

    public VerticalAlignment? VerticalCellAlignment { get; set; }

    public CellSpacingStyle CellSpacing { get; set; }

    /// <summary>
    /// Table width. If not defined, it is calucated from column widths
    /// </summary>
    public Dimension Width { get; set; }

    /// <summary>
    /// Collection of column definitions
    /// </summary>
    public IReadOnlyList<TableColumnStyle> Columns { get => _columns.AsReadOnly(); }

    public TableStyle()
    {
        CellSpacing = new CellSpacingStyle();
        Width = new Dimension();
        _columns = new List<TableColumnStyle>();
    }

    /// <summary>
    /// Adds column definition
    /// </summary>
    /// <returns></returns>
    public TableColumnStyle AddColumn()
    {
        var c = new TableColumnStyle();
        _columns.Add(c);
        return c;
    }

    internal TableStyle MergeWith(TableStyle baseStyle)
    {
        var res = new TableStyle
        {
            HorizontalAlignment = HorizontalAlignment.HasValue ? HorizontalAlignment : baseStyle.HorizontalAlignment,
            VerticalCellAlignment = VerticalCellAlignment.HasValue ? VerticalCellAlignment : baseStyle.VerticalCellAlignment,
            Width = !Width.IsEmpty ? Width : baseStyle.Width,
        };

        res.CellSpacing = CellSpacing.MergeWith(baseStyle.CellSpacing);

        var colCount = Math.Max(baseStyle._columns.Count, _columns.Count);
        res._columns = new List<TableColumnStyle>();
        for (var i = 0; i < colCount; i++)
        {
            if (i < baseStyle.Columns.Count && i < _columns.Count)
            {
                var t = Columns[i].MergeWith(baseStyle.Columns[i]);
                res._columns.Add(t);
            }
            else if (i < Columns.Count)
            {
                var t = Columns[i].Clone();
                res._columns.Add(t);
            }
            else if (i < baseStyle.Columns.Count)
            {
                var t = baseStyle.Columns[i].Clone();
                res._columns.Add(t);
            }
        }

        return res;
    }

    internal TableColumnStyle GetColumnStyle(int index)
    {
        if (!_columns.Any()) return new TableColumnStyle();

        return index >= _columns.Count ? _columns.Last() : _columns[index];
    }
}
