// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Extensions.Tables;
using Markdig.Syntax;
using VectorAi.MarkdownToPdf.Styling;
using VectorAi.MarkdownToPdf.Utils;
using MigraDoc.DocumentObjectModel.Tables;

namespace VectorAi.MarkdownToPdf.Converters.ContainerConverters;

internal class TableRowConverter : ContainerBlockConverter
{
    public TableRow CurrentBlock { get; set; }
    private int _currentColumn;

    public Row? OutputTableRow { get; private set; }

    internal TableRowConverter(TableRow block, ContainerBlockConverter parent)
        : base(block, parent)
    {
        ElementDescriptor = new SingleElementDescriptor
        {
            Attributes = Attributes,
            Type = Block.GetIndex() % 2 == 1 ? ElementType.TableRowOdd : ElementType.TableRowEven,
            Position = new ElementPosition(Block)
        };
        CurrentBlock = (Block as TableRow)!;

        if (CurrentBlock.IsHeader) ElementDescriptor.Type = ElementType.TableHeader;
    }

    protected override void PrepareStyling()
    {
        base.PrepareStyling();
        EvaluatedStyle.Table.VerticalCellAlignment = EvaluatedStyle.Table.VerticalCellAlignment.HasValue ? EvaluatedStyle.Table.VerticalCellAlignment : Parent?.EvaluatedStyle.Table.VerticalCellAlignment;
    }

    protected override bool CreateOutput()
    {
        OutputTableRow = (Parent as TableBlockConverter)!.OutputTable?.Rows.AddRow();
        return true;
    }

    protected override void ApplyStyling()
    {
        if (ElementDescriptor.Type == ElementType.TableHeader) OutputTableRow?.HeadingFormat = true;
    }

    protected override void ConvertContent()
    {
        foreach (var cell in CurrentBlock) ConvertBlock(cell);
    }

    protected override bool ConvertBlock(Block block)
    {
        var type = block.GetType();

        if (type == typeof(TableCell))
        {
            var cell = (block as TableCell)!;
            var conv = new TableCellConverter(cell, this)
            {
                Column = cell.ColumnIndex >= 0 ? cell.ColumnIndex : _currentColumn
            };
            conv.Convert();
            _currentColumn++;
            return true;
        }

        return false;
    }
}
