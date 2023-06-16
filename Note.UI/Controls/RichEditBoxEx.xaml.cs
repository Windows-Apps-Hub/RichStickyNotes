using CommunityToolkit.Mvvm.Input;
using Get.XAMLTools;
using Microsoft.UI.Input;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI;
using WinRT.Interop;

namespace WAH.NoteSystem.UI.Controls;

[DependencyProperty<FormatEffect>("SelectionBold", CheckForChanges = true, GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<FormatEffect>("SelectionItalic", CheckForChanges = true, GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<FormatEffect>("SelectionUnderline", CheckForChanges = true, GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<FormatEffect>("SelectionStrikethrough", CheckForChanges = true, GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<FormatEffect>("SelectionSuperscript", CheckForChanges = true, GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<FormatEffect>("SelectionSubscript", CheckForChanges = true, GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<FormatEffect>("SelectionBulletList", CheckForChanges = true, GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<FormatEffect>("SelectionNumberedList", CheckForChanges = true, GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<Color?>("SelectionForegroundColor", CheckForChanges = true, GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<Color?>("SelectionBackgroundColor", CheckForChanges = true, GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<bool>("CanUndo", CheckForChanges = true, GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<bool>("CanRedo", CheckForChanges = true, GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<bool>("CanCopy", CheckForChanges = true, GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<bool>("CanPaste", CheckForChanges = true, GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<bool>("CanInsertLink", CheckForChanges = true, GenerateLocalOnPropertyChangedMethod = true)]
public sealed partial class RichEditBoxEx : RichEditBox
{
    public RichEditBoxEx()
    {
        InitializeComponent();
        TextChanged += (_, _) => UpdateProperties();
        SelectionChanged += (_, _) => UpdateProperties();
        FocusEngaged += (_, _) =>
        {
            CanPaste = Document.CanPaste();
        };
        KeyDown += ThisKeyDown;
        PointerMoved += ThisPointerMoved;
        Paste += delegate
        {
            Document.BeginUndoGroup();
            PasteStartPosition = Document.Selection.StartPosition;
            Pasted = true;
        };
        //var format = Document.GetDefaultCharacterFormat();
        //this.Background
        //Document.SetDefaultCharacterFormat(
        //    format
        //);
    }
    bool Pasted = false;
    int PasteStartPosition = 0;
    private void ThisPointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var range = Document.GetRangeFromPoint(e.GetCurrentPoint(this).Position, PointOptions.None);
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) Debugger.Break();
        
    }

    private void ThisKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key is VirtualKey.Tab)
        {
            Document.Selection.TypeText("\t");
            e.Handled = true;
        }
    }

    void UpdateProperties()
    {
        if (Pasted)
        {
            //Document.GetText(TextGetOptions.None, out var a);
            var range = Document.GetRange(PasteStartPosition, Document.Selection.StartPosition);
            range.CharacterFormat.Name = FontFamily.Source;
            range.CharacterFormat.Size = (float)FontSize;
            range = Document.Selection;
            range.CharacterFormat.Name = FontFamily.Source;
            range.CharacterFormat.Size = (float)FontSize;
            PasteStartPosition = 0;
            Pasted = false;
            Document.EndUndoGroup();
        }
        var selection = Document.Selection;
        var textFormat = selection.CharacterFormat;
        var paraFormat = selection.ParagraphFormat;
        SelectionBold = textFormat.Bold;
        SelectionItalic = textFormat.Italic;
        SelectionStrikethrough = textFormat.Strikethrough;
        SelectionSuperscript = textFormat.Superscript;
        SelectionSubscript = textFormat.Subscript;
        CanUndo = Document.CanUndo();
        CanRedo = Document.CanRedo();
        CanCopy = Document.CanCopy();
        CanPaste = Document.CanPaste();
        CanInsertLink = selection.Length > 0;
        SelectionUnderline = textFormat.Underline switch
        {
            UnderlineType.None => FormatEffect.Off,
            UnderlineType.Undefined => FormatEffect.Undefined,
            _ => FormatEffect.On,
        };
        if (textFormat.ForegroundColor is { A: 0, R: 0, B: 2, G: 0})
        {
            SelectionForegroundColor = null;
        } else
        {
            SelectionForegroundColor = textFormat.ForegroundColor;
        }
        if (textFormat.BackgroundColor is { A: 0, R: 0, B: 2, G: 0 })
        {
            SelectionBackgroundColor = null;
        }
        else
        {
            SelectionBackgroundColor = textFormat.BackgroundColor;
        }
        switch (paraFormat.ListType)
        {
            case MarkerType.None:
                SelectionBulletList = FormatEffect.Off;
                SelectionNumberedList = FormatEffect.Off;
                break;
            case MarkerType.Bullet:
                SelectionBulletList = FormatEffect.On;
                SelectionNumberedList = FormatEffect.Off;
                break;
            case MarkerType.Arabic:
                SelectionBulletList = FormatEffect.Off;
                SelectionNumberedList = FormatEffect.On;
                break;
        }
    }
    [RelayCommand]
    public void Undo() => Document.Undo();
    [RelayCommand]
    public void Redo() => Document.Redo();
    [RelayCommand]
    public void Cut() => Document.Selection.Cut();
    [RelayCommand]
    public void Copy() => Document.Selection.Copy();
    [RelayCommand]
    public void DoPaste() => Document.Selection.Paste(default);
    [RelayCommand]
    public void InsertTable() => Document.Selection.SetText(
        TextSetOptions.FormatRtf,
        CreateTableString(5, 5)
    );
    FileOpenPicker ImageFilePicker = new()
    {
        ViewMode = PickerViewMode.Thumbnail,
        SuggestedStartLocation = PickerLocationId.PicturesLibrary,
        FileTypeFilter = { ".BMP", ".DDS", ".DNG", ".GIF", ".ICO", ".JPEG", ".JPG", ".PNG", ".TIFF" }
    };
    [RelayCommand]
    public async void PromptForImageAndInsertPhoto()
    {
        InitializeWithWindow.Initialize(ImageFilePicker, WinWrapper.Windowing.Window.ActiveWindow.Root);
        var file = await ImageFilePicker.PickSingleFileAsync();
        var stream = await file.OpenReadAsync();
        var bmp = new BitmapImage();
        await bmp.SetSourceAsync(stream);
        stream.Dispose();
        stream = await file.OpenReadAsync();
        Document.Selection.InsertImage(bmp.PixelWidth, bmp.PixelHeight, 10, VerticalCharacterAlignment.Bottom, "Image", stream);
        stream.Dispose();
    }
    string CreateTableString(int row, int column)
    {
        return $$"""
            {\rtf1\ansi\deff0{{string.Join("",
                from _0 in Enumerable.Range(0, row+1)
                select $$"""
                \trowd{{string.Join("",
                    from j in Enumerable.Range(0, column)
                    select $@"\cellx{(j + 1)
                        * 1000 // Cell Width
                    } "
                )}}{{string.Join("",
                    from j in Enumerable.Range(0, column)
                    select $@"{"" // Cell Content
                    }\intbl\cell "
                )}}\row
                """
                )}}
            """;
    }

    [RelayCommand]
    public void RequestSetLinkForCurrentSelection()
    {
        var selection = Document.Selection;
        var link = selection.Link;
        LinkFlyoutTextBox.Text = link.Length >= 2 ? link[1..^1] : link;
        selection.GetRect(PointOptions.None, out var rect, out int hit);
        LinkSetterFlyout.ShowAt(this, new()
        {
            Position = new((rect.Left + rect.Right) / 2, (rect.Bottom + rect.Top) / 2),
            ExclusionRect = rect,
            Placement = FlyoutPlacementMode.Bottom
        });
    }

    private void LinkFlyoutConfirmButtonClick(object sender, RoutedEventArgs e)
    {
        ConfirmLinkOnFlyout();
    }
    void ConfirmLinkOnFlyout()
    {
        LinkSetterFlyout.Hide();
        if (LinkFlyoutTextBox.Text is "")
            Document.Selection.Link = "";
        else
            Document.Selection.Link = $"\"{LinkFlyoutTextBox.Text}\"";
    }

    private void LinkFlyoutTextBox_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key is not VirtualKey.Enter) return;
        e.Handled = true;
        ConfirmLinkOnFlyout();
    }
}