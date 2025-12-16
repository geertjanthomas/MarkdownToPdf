using Markdig.Extensions.Tables;
using Markdig.Syntax;
using MarkdownToPdf10.Styling;
using MarkdownToPdf10.Utils;
using MigraDoc.DocumentObjectModel.Tables;

namespace MarkdownToPdf10.Converters.ContainerConverters;

internal class TableRowConverter : ContainerBlockConverter
{
    public TableRow CurrentBlock { get; set; }
    private int _currentColumn;

    public Row OutputTableRow { get; private set; }

    internal TableRowConverter(TableRow block, ContainerBlockConverter parent)
        : base(block, parent)
    {
        ElementDescriptor = new SingleElementDescriptor
        {
            Attributes = Attributes,
            Type = Block.GetIndex() % 2 == 1 ? ElementType.TableRowOdd : ElementType.TableRowEven,
            Position = new ElementPosition(Block)
        };
        CurrentBlock = Block as TableRow;

        if (CurrentBlock.IsHeader) ElementDescriptor.Type = ElementType.TableHeader;
    }

    protected override void PrepareStyling()
    {
        base.PrepareStyling();
        EvaluatedStyle.Table.VerticalCellAlignment = EvaluatedStyle.Table.VerticalCellAlignment.HasValue ? EvaluatedStyle.Table.VerticalCellAlignment : Parent.EvaluatedStyle.Table.VerticalCellAlignment;
    }

    protected override bool CreateOutput()
    {
        OutputTableRow = (Parent as TableBlockConverter).OutputTable.Rows.AddRow();
        return true;
    }

    protected override void ApplyStyling()
    {
        if (ElementDescriptor.Type == ElementType.TableHeader) OutputTableRow.HeadingFormat = true;
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
            var conv = new TableCellConverter(block as TableCell, this)
            {
                Column = (block as TableCell).ColumnIndex >= 0 ? (block as TableCell).ColumnIndex : _currentColumn
            };
            conv.Convert();
            _currentColumn++;
            return true;
        }

        return false;
    }
}
