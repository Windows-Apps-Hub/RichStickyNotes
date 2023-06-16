using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using WAH.NoteSystem.Core;

namespace WAH.NoteSystem.UI.Helpers;

class SymbolExFontIconExtension : SymbolExExtension
{
    public int FontSize { get; set; } = 20;
    protected override object OnProvideValue()
        => new SymbolExFontIcon
        {
            Glyph = Glyph.ToString(),
            FontSize = FontSize,
            FontFamily = SymbolExFontIcon.SymbolFontIcon
        };
}
class SymbolExFontIcon : FontIcon
{
    public static readonly FontFamily SymbolFontIcon = new("Segoe Fluent Icons,Segoe MDL2 Assets");
    public SymbolExFontIcon()
    {
        FontFamily = SymbolFontIcon;
    }
    public SymbolEx SymbolEx
    {
        get => (SymbolEx)Glyph[0];
        set => Glyph = ((char)value).ToString();
    }
}

class SymbolExIconExtension : SymbolExExtension
{
    protected override object OnProvideValue()
        => new SymbolIcon { Symbol = Symbol };
}
class SymbolExGlyphExtension : SymbolExExtension
{
    protected override object OnProvideValue()
        => Glyph.ToString();
}
class SymbolExExtension : MarkupExtension
{
    public SymbolEx SymbolEx
    {
        get => (SymbolEx)Symbol;
        set => Symbol = (Symbol)value;
    }
    public Symbol Symbol { get; set; }
    public char Glyph
    {
        get => (char)Symbol;
        set => Symbol = (Symbol)value;
    }
    public int GlyphInteger
    {
        get => (int)Symbol;
        set => Symbol = (Symbol)value;
    }

    protected virtual object OnProvideValue() => Symbol;

    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
    {
        return OnProvideValue();
    }
    protected override object ProvideValue()
    {
        return OnProvideValue();
    }
}
static class SymbolExCSharpExtension
{
    public static Symbol ToSymbol(this SymbolEx symbolEx) => (Symbol)symbolEx;
}