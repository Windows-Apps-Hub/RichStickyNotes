using Get.XAMLTools;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI;

namespace WAH.NoteSystem.UI.Controls;

public sealed partial class RichEditBoxEx : RichEditBox
{
    partial void OnSelectionBoldChanged(FormatEffect oldValue, FormatEffect newValue)
    {
        Document.Selection.CharacterFormat.Bold = newValue;
    }
    partial void OnSelectionItalicChanged(FormatEffect oldValue, FormatEffect newValue)
    {
        Document.Selection.CharacterFormat.Italic = newValue;
    }
    partial void OnSelectionStrikethroughChanged(FormatEffect oldValue, FormatEffect newValue)
    {
        Document.Selection.CharacterFormat.Strikethrough = newValue;
    }
    partial void OnSelectionSuperscriptChanged(FormatEffect oldValue, FormatEffect newValue)
    {
        Document.Selection.CharacterFormat.Superscript = newValue;
    }
    partial void OnSelectionSubscriptChanged(FormatEffect oldValue, FormatEffect newValue)
    {
        Document.Selection.CharacterFormat.Subscript = newValue;
    }
    partial void OnSelectionUnderlineChanged(FormatEffect oldValue, FormatEffect newValue)
    {
        if (oldValue == newValue) return;
        var format = Document.Selection.CharacterFormat;
        switch (newValue)
        {
            case FormatEffect.On:
                if (format.Underline is not UnderlineType.Single)
                    format.Underline = UnderlineType.Single;
                break;
            case FormatEffect.Off:
                if (format.Underline is not UnderlineType.None)
                    format.Underline = UnderlineType.None;
                break;
            case FormatEffect.Toggle:
                if (format.Underline is UnderlineType.None)
                    goto case FormatEffect.On;
                else
                    goto case FormatEffect.Off;
        }
    }
    partial void OnSelectionBulletListChanged(FormatEffect oldValue, FormatEffect newValue)
    {
        var format = Document.Selection.ParagraphFormat;
        switch (newValue)
        {
            case FormatEffect.Toggle:
                if (format.ListType is MarkerType.Bullet)
                    goto case FormatEffect.Off;
                else
                    goto case FormatEffect.On;
            case FormatEffect.On:
                if (format.ListType is not MarkerType.Bullet) format.ListType = MarkerType.Bullet;
                break;
            case FormatEffect.Off:
                if (format.ListType is MarkerType.Bullet) format.ListType = MarkerType.None;
                break;
        }
    }
    partial void OnSelectionNumberedListChanged(FormatEffect oldValue, FormatEffect newValue)
    {
        var format = Document.Selection.ParagraphFormat;
        switch (newValue)
        {
            case FormatEffect.Toggle:
                if (format.ListType is MarkerType.Arabic)
                    goto case FormatEffect.Off;
                else
                    goto case FormatEffect.On;
            case FormatEffect.On:
                if (format.ListType is not MarkerType.Arabic) format.ListType = MarkerType.Arabic;
                if (format.ListStart is not 1) format.ListStart = 1;
                break;
            case FormatEffect.Off:
                if (format.ListType is MarkerType.Arabic) format.ListType = MarkerType.None;
                break;
        }
    }
    partial void OnSelectionForegroundColorChanged(Color? oldValue, Color? newValue)
    {
        var format = Document.Selection.CharacterFormat;
        if (newValue is not null && format.ForegroundColor != newValue.Value)
            format.ForegroundColor = newValue.Value;
    }
    partial void OnSelectionBackgroundColorChanged(Color? oldValue, Color? newValue)
    {
        var format = Document.Selection.CharacterFormat;
        if (newValue is not null && format.BackgroundColor != newValue.Value)
            format.BackgroundColor = newValue.Value;
    }
}
