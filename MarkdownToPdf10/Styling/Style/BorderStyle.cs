using MigraDoc.DocumentObjectModel;

namespace MarkdownToPdf10.Styling.Style;

/// <summary>
/// Part of <see cref="CascadingStyle"/> defining borders of a leaf block, table or table cell
/// </summary>
/// <remarks>If used for table, only the common properties are used for all sides, different styling cannot be used for each side</remarks>
public class BorderStyle
{
    private MigraDoc.DocumentObjectModel.BorderStyle? _lineStyle;
    private Color _color;
    private Dimension _width;

    public MigraDoc.DocumentObjectModel.BorderStyle? LineStyle { get => _lineStyle; set => SetLineStyle(value); }
    public Dimension Width { get => _width; set => SetWidth(value); }
    public Color Color { get => _color; set => SetColor(value); }
    public SingleBorderStyle Top { get; set; }
    public SingleBorderStyle Bottom { get; set; }
    public SingleBorderStyle Left { get; set; }
    public SingleBorderStyle Right { get; set; }

    public SingleBorderStyle this[BoxSide key]
    {
        get => GetSide(key);
    }

    internal BorderStyle()
    {
        _width = new Dimension();
        Top = new SingleBorderStyle();
        Bottom = new SingleBorderStyle();
        Left = new SingleBorderStyle();
        Right = new SingleBorderStyle();
    }

    /// <summary>
    /// Tests if main border properties are set, ignoring single sides settings
    /// </summary>
    public bool HasValue()
    {
        if (Width.IsEmpty) return false;
        return true;
    }

    internal BorderStyle MergeWith(BorderStyle baseStyle)
    {
        var res = new BorderStyle
        {
            Top = Top.MergeWith(baseStyle.Top),
            Bottom = Bottom.MergeWith(baseStyle.Bottom),
            Left = Left.MergeWith(baseStyle.Left),
            Right = Right.MergeWith(baseStyle.Right)
        };

        res._lineStyle = _lineStyle ?? baseStyle._lineStyle;
        res._color = _color.IsEmpty ? baseStyle._color : _color;
        res._width = _width.IsEmpty ? baseStyle._width : _width;

        return res;
    }

    internal Borders MergeWithBorders(Borders borders, double fontSize, double containerWidth)
    {
        var res = borders.Clone();
        if (HasValue())
        {
            res.Color = !Color.IsEmpty ? Color : Colors.Black;
            res.Width = Width.Eval(fontSize, containerWidth);
            res.Style = LineStyle ?? MigraDoc.DocumentObjectModel.BorderStyle.Single;
        }

        // Migradoc bug - if border is just read, it is set as existing and overrides the main border
        if (Bottom.HasValue()) Bottom.WriteTo(res.Bottom, fontSize, containerWidth);
        if (Top.HasValue()) Top.WriteTo(res.Top, fontSize, containerWidth);
        if (Left.HasValue()) Left.WriteTo(res.Left, fontSize, containerWidth);
        if (Right.HasValue()) Right.WriteTo(res.Right, fontSize, containerWidth);

        return res;
    }

    private void SetWidth(Dimension value)
    {
        _width = value;
        Left.Width = Left.Width.IsEmpty ? value : Left.Width;
        Right.Width = Right.Width.IsEmpty ? value : Right.Width;
        Top.Width = Top.Width.IsEmpty ? value : Top.Width;
        Bottom.Width = Bottom.Width.IsEmpty ? value : Bottom.Width;
    }

    private void SetColor(Color value)
    {
        _color = value;
        Left.Color = Left.Color.IsEmpty ? value : Left.Color;
        Right.Color = Right.Color.IsEmpty ? value : Right.Color;
        Top.Color = Top.Color.IsEmpty ? value : Top.Color;
        Bottom.Color = Bottom.Color.IsEmpty ? value : Bottom.Color;
    }

    private void SetLineStyle(MigraDoc.DocumentObjectModel.BorderStyle? value)
    {
        _lineStyle = value;
        Left.LineStyle = Left.LineStyle.HasValue ? value : Left.LineStyle;
        Right.LineStyle = Right.LineStyle.HasValue ? value : Right.LineStyle;
        Top.LineStyle = Top.LineStyle.HasValue ? value : Top.LineStyle;
        Bottom.LineStyle = Bottom.LineStyle.HasValue ? value : Bottom.LineStyle;
    }

    private SingleBorderStyle GetSide(BoxSide key)
    {
        switch (key)
        {
            case BoxSide.Left: return Left;
            case BoxSide.Right: return Right;
            case BoxSide.Top: return Top;
            case BoxSide.Bottom: return Bottom;
        }
        throw new ArgumentException("Unknown side key");
    }
}
