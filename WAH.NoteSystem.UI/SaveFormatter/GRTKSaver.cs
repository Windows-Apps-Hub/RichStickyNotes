using Get.RichTextKit.Editor.DataStructure.Table;
using Get.RichTextKit.Editor.Paragraphs;
using Get.RichTextKit.Editor.Paragraphs.Panel;
using Get.RichTextKit.Styles;
using Get.TextEditor;
using MethodInjector;
using SkiaSharp;
using System;
using System.Linq;
using System.Text.Json.Nodes;

namespace WAH.NoteSystem.UI;

static class GRTKSaver
{
    struct Void { }
    static ExternalMethodManager<Paragraph, Void, JsonNode> Saver { get; set; } = new();
    public static JsonNode Save(Paragraph para) => Saver.Call(para, default);
    static GRTKSaver()
    {
        Saver.Register<TextParagraph>(SaveTextParagraph);
        Saver.Register<VerticalParagraph>(SaveVerticalParagraph);
        Saver.Register<CodeParagraph>(SaveCodeParagraph);
    }
    static JsonObject ToJSON(SKColor color)
        => new()
        {
            ["R"] = color.Red,
            ["G"] = color.Green,
            ["B"] = color.Blue,
            ["A"] = color.Alpha
        };
    static JsonObject ToJSON(SKColor? color)
        => color is null ? null : ToJSON(color.Value);
    static JsonObject SaveTextParagraph(TextParagraph Paragraph, Void arg)
    {
        return new JsonObject()
        {
            ["Type"] = nameof(TextParagraph),
            ["Data"] =
                new JsonArray(
                    (
                    from styleRun in Paragraph.GetStyles(0, Paragraph.CodePointLength)
                    select new JsonObject
                    {
                        ["Text"] = Paragraph.GetText(styleRun.Start, styleRun.Length).ToString(),
                        [nameof(IStyle.FontFamily)] = styleRun.Style.FontFamily,
                        [nameof(IStyle.FontSize)] = styleRun.Style.FontSize,
                        [nameof(IStyle.FontWeight)] = styleRun.Style.FontWeight,
                        [nameof(IStyle.FontWidth)] = styleRun.Style.FontWidth.ToString(),
                        [nameof(IStyle.FontItalic)] = styleRun.Style.FontItalic,
                        [nameof(IStyle.Underline)] = styleRun.Style.Underline.ToString(),
                        [nameof(IStyle.StrikeThrough)] = styleRun.Style.StrikeThrough.ToString(),
                        [nameof(IStyle.LineHeight)] = styleRun.Style.LineHeight,
                        [nameof(IStyle.TextColor)] = ToJSON(styleRun.Style.TextColor),
                        [nameof(IStyle.BackgroundColor)] = ToJSON(styleRun.Style.BackgroundColor),
                        [nameof(IStyle.HaloColor)] = ToJSON(styleRun.Style.HaloColor),
                        [nameof(IStyle.HaloWidth)] = styleRun.Style.HaloWidth,
                        [nameof(IStyle.HaloBlur)] = styleRun.Style.HaloBlur,
                        [nameof(IStyle.LetterSpacing)] = styleRun.Style.LetterSpacing,
                        [nameof(IStyle.FontVariant)] = styleRun.Style.FontVariant.ToString(),
                        [nameof(IStyle.TextDirection)] = styleRun.Style.TextDirection.ToString(),
                        [nameof(IStyle.ReplacementCharacter)] = styleRun.Style.ReplacementCharacter
                    }
                    ).ToArray()
                )
        };
    }
    static JsonObject SaveVerticalParagraph(VerticalParagraph Paragraph, Void arg)
    {
        return new JsonObject()
        {
            ["Type"] = nameof(VerticalParagraph),
            ["Data"] = new JsonArray((
                from para in Paragraph.Children
                select Saver.Call(para, default)
            ).ToArray())
        };
    }
    static JsonObject ToJSON(TableLength len)
        => new()
        {
            ["Length"] = len.Length,
            ["Mode"] = len.Mode.ToString()
        };
    static JsonObject SaveTableParagraph(TableParagraph Paragraph, Void arg)
    {
        return new JsonObject
        {
            ["Type"] = nameof(TableParagraph),
            ["ColumnInfo"] = new JsonArray((
                from col in Paragraph.Columns
                select ToJSON(col.Width)
            ).ToArray())
            ["Rows"] = new JsonArray((
                from row in Paragraph.Rows
                select new JsonObject
                {
                    ["Height"] = ToJSON(row.Height),
                    ["Cells"] = new JsonArray((
                        from col in row
                        select Saver.Call(col, default)
                    ).ToArray())
                }
            ).ToArray())
        };
    }
    static JsonObject SaveCodeParagraph(CodeParagraph Paragraph, Void arg)
    {
        return new JsonObject
        {
            ["Type"] = nameof(CodeParagraph),
            ["LanguageId"] = Paragraph.Language.Id,
            ["Code"] = Paragraph.GetText(0, Paragraph.CodePointLength).ToString()
        };
    }
    //static JsonObject SaveImageParagraph(ImageParagraph Paragraph, Void arg)
    //{

        //Access pixel buffer in such a way
        //byte[] bytes = pixelData.DetachPixelData();
        //return new JsonObject
        //{
        //    ["Type"] = nameof(ImageParagraph),
        //    ["Data"] = Paragraph.UnsafeGetImage().save
        //};
    //}
}