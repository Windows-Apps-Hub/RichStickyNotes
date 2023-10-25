using ColorCode;
using Get.RichTextKit;
using Get.RichTextKit.Editor.DataStructure.Table;
using Get.RichTextKit.Editor.Paragraphs;
using Get.RichTextKit.Editor.Paragraphs.Panel;
using Get.RichTextKit.Styles;
using SkiaSharp;
using System;
using System.Linq;
using System.Text.Json.Nodes;
using Style = Get.RichTextKit.Style;

namespace WAH.NoteSystem.UI;

static class GRTKLoader
{
    static TEnum ParseEnum<TEnum>(JsonNode node) where TEnum : struct
    {
        return (TEnum)Enum.Parse(typeof(TEnum), node.ToString());
    }
    static SKColor? ReadColor(JsonNode? node)
    {
        if (node is null) return null;
        return new((byte)node["R"], (byte)node["G"], (byte)node["B"], (byte)node["A"]);
    }
    public static Paragraph? Load(JsonNode? obj)
    {
        if (obj is null) return null;
        switch (obj["Type"].ToString())
        {
            case nameof(TextParagraph):
                IStyle ReadStyle(JsonNode node)
                {
                    return new Style
                    {
                        FontFamily = "Segoe UI",//(string)node["FontFamily"],
                        FontSize = 32,//(float)node["FontSize"],
                        FontWeight = (int)node["FontWeight"],
                        FontWidth = ParseEnum<SkiaSharp.SKFontStyleWidth>(node["FontWidth"]),
                        FontItalic = (bool)node["FontItalic"],
                        Underline = ParseEnum<UnderlineStyle>(node["Underline"]),
                        StrikeThrough = ParseEnum<StrikeThroughStyle>(node["StrikeThrough"]),
                        LineHeight = (float)node["LineHeight"],
                        TextColor = ReadColor(node["TextColor"]),
                        BackgroundColor = ReadColor(node["BackgroundColor"]),
                        HaloColor = ReadColor(node["HaloColor"]) ?? SKColor.Empty,
                        HaloWidth = (float)node["HaloWidth"],
                        HaloBlur = (float)node["HaloBlur"],
                        LetterSpacing = (float)node["LetterSpacing"],
                        FontVariant = ParseEnum<FontVariant>(node["FontVariant"]),
                        TextDirection = ParseEnum<TextDirection>(node["TextDirection"]),
                        ReplacementCharacter = (char)node["ReplacementCharacter"]
                    };
                }
                {
                    var para = new TextParagraph(ReadStyle(obj["Data"][0]));
                    para.TextBlock.Clear();
                    foreach (var data in (JsonArray)obj["Data"])
                    {
                        para.TextBlock.AddText((string)data["Text"], ReadStyle(data));
                    }
                    return para;
                }
            case nameof(VerticalParagraph):
                return new VerticalParagraph(
                    from data in (JsonArray)obj["Data"]
                    select Load(data)
                );
            case nameof(TableParagraph):
                TableLength ReadTableLength(JsonNode n)
                {
                    return new((float)n["Length"], ParseEnum<TableLengthMode>(n["Mode"]));
                }
                return new TableParagraph(
                    from x in (JsonArray)obj["ColumnInfo"]
                    select ReadTableLength(x),
                    from y in (JsonArray)obj["Rows"]
                    select (
                        from z in (JsonArray)obj["Cells"]
                        select Load(z),
                        ReadTableLength(y["Height"])
                    )
                );
            case nameof(CodeParagraph):
                {
                    var style = new Style() { FontSize = 32, FontFamily = "Cascadia Mono" };
                    var para = new CodeParagraph(style);
                    para.Language = Languages.FindById((string)obj["LanguageId"]);
                    para.TextBlock.Clear();
                    para.TextBlock.AddText((string)obj["Code"], style);
                    (para as ITextParagraph).OnTextBlockChanged();
                    return para;
                }
            default:
                throw new NotImplementedException();
        }
    }
}