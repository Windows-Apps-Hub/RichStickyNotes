using CommunityToolkit.Mvvm.Input;
using WAH.NoteSystem.Core;
using WAH.NoteSystem.Core.Notes;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Get.XAMLTools;
using EasyCSharp;
using System.ComponentModel;
using System;
using Windows.UI;
using Microsoft.UI;
using Get.TextEditor;
using Get.RichTextKit.Editor;
using Get.RichTextKit.Editor.Paragraphs;
using Get.RichTextKit.Editor.Paragraphs.Panel;
using Get.RichTextKit.Data;
using Windows.Foundation;

namespace WAH.NoteSystem.UI;
[DependencyProperty<Visibility>("BackButtonVisibility")]
[DependencyProperty<Note>("Note", GenerateLocalOnPropertyChangedMethod = true, UseNullableReferenceType = true)]
public sealed partial class NoteEditPage : Page, INotifyPropertyChanged
{
    static NoteEditPage()
    {
        DocumentDataGenerator.ParagraphProcessor.Register<CodeParagraph>(ProcessCodeParagraph);
    }
    static ProcessGeneratorInfo ProcessCodeParagraph(CodeParagraph para, (DataInfo data, ProcessGeneratorInfo info) param)
    {
        var (info, procInfo) = param;
        
        var code = para.GetText(procInfo.Range.Start, procInfo.Range.Length);
        info.HTML.Append($"""<pre langid="{para.Language.Id}">""");
        info.Rtf.Body.Append($$$"""{{\*\code {{{para.Language.Id}}}}\pard""");
        bool hasEndPara = false;
        ExternalHelper.AppendText(info, code, ref hasEndPara);
        if (!procInfo.RTFEndLineImplicit && hasEndPara)
            info.Rtf.Body.Append("\\par}\n");
        else
            info.Rtf.Body.Append("}\n");

        info.HTML.Append($"""</pre>""");
        return procInfo;
    }
    public NoteEditPage()
    {
        InitializeComponent();
        //EditorTextBox.Resources["TextControlBorderThemeThicknessFocused"] = default(Thickness);
        TextEditor.AllowedClipboardfFormatting = new(false)
        {
            Alignment = true,
            Bold = true,
            Italic = true,
            SubScript = true,
            SuperScript = true,
            Underline = true,
            Strikethrough = true,
        };
        TextEditor.UIConfigParagraphTemplateSelector = new Editor.Controls.UIParagraphSettingTemplate().GetDataTemplateSelector();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        Note = (Note)e.Parameter;
        base.OnNavigatedTo(e);
    }
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        var Note = this.Note;
        if (Note is not null && HasTextChanged)
        {
            _ = Note.SaveNewVersionAsync(TextEditor);
        }
        base.OnNavigatedFrom(e);
    }
    async partial void OnNoteChanged(Note? oldValue, Note? newValue)
    {
        if (oldValue is not null && HasTextChanged)
        {
            await oldValue.SaveNewVersionAsync(TextEditor);
        }
        if (newValue is not null)
        {
            await newValue.CurrentVersion.LoadDataToAsync(TextEditor);
            //HasTextChanged = false;
        }
    }
    [RelayCommand]
    void NavigateBack()
    {
        Frame.GoBack();
    }
    bool HasTextChanged = true;

    public event PropertyChangedEventHandler? PropertyChanged;

    //private void EditorTextBox_TextChanged(object sender, RoutedEventArgs e)
    //{
    //    HasTextChanged = true;
    //}
    [RelayCommand]
    async void Save()
    {

        var Note = this.Note;
        if (Note is not null && HasTextChanged)
        {
            //HasTextChanged = false;
            await Note.SaveNewVersionAsync(TextEditor);
        }
    }
    [Event<RoutedEventHandler>]
    void Undo()
    {
        TextEditor.DocumentView.Undo();
    }
    [Event<RoutedEventHandler>]
    void Redo()
    {
        TextEditor.DocumentView.Redo();
    }
}
static class Extension
{
    public static async Task SaveNewVersionAsync(this Note n, RichTextEditor retd)
    {
        var rootPara = retd.DocumentView.OwnerDocument.Paragraphs.UnsafeGetRootParagraph();
        await n.SaveNewVersionAsync(new()
        {
            Cells =
            {
                new GRTKCell()
                {
                    Node = GRTKSaver.Save(rootPara)
                }
            }
        });
    }
    static Document defaultDoc = new(RichTextEditor.GetDefaultStyle());
    public static async Task LoadDataToAsync(this NoteVersion n, RichTextEditor ret)
    {
        var data = await n.GetDataAsync();
        var rootPara = (VerticalParagraph)ret.DocumentView.OwnerDocument.Paragraphs.UnsafeGetRootParagraph();
        rootPara.Children.Clear();
        var actualNode = ((GRTKCell)data.Cells[0]).Node;
        if (actualNode is null)
        {
            actualNode = GRTKSaver.Save(defaultDoc.Paragraphs.UnsafeGetRootParagraph());
        }
        
        {
            var node = (VerticalParagraph)GRTKLoader.Load(actualNode)!;
            rootPara.Children.Clear();
            foreach (var child in node.Children)
                ret.DocumentView.OwnerDocument.Paragraphs.Add(child);
            ret.DocumentView.OwnerDocument.Layout.InvalidateAndValid();
            ret.DocumentView.OwnerDocument.RequestRedraw();
        }
    }
}
class Null2TransparentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return ((Color?)value) ?? Colors.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
}