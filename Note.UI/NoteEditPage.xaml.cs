using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WAH.NoteSystem.Core;
using WAH.NoteSystem.Core.Notes;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Get.XAMLTools;
using EasyCSharp;
using System.ComponentModel;
using System;
using Microsoft.UI.Xaml.Documents;
using Windows.UI;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;

namespace WAH.NoteSystem.UI;
[DependencyProperty<Visibility>("BackButtonVisibility")]
[DependencyProperty<Note>("Note", GenerateLocalOnPropertyChangedMethod = true, UseNullableReferenceType = true)]
public sealed partial class NoteEditPage : Page, INotifyPropertyChanged
{
    public NoteEditPage()
    {
        InitializeComponent();
        EditorTextBox.Resources["TextControlBorderThemeThicknessFocused"] = default(Thickness);
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
            _ = Note.SaveNewVersionAsync(EditorTextBox.Document);
        }
        base.OnNavigatedFrom(e);
    }
    async partial void OnNoteChanged(Note? oldValue, Note? newValue)
    {
        if (oldValue is not null && HasTextChanged)
        {
            await oldValue.SaveNewVersionAsync(EditorTextBox.Document);
        }
        if (newValue is not null)
        {
            await newValue.CurrentVersion.LoadDataToAsync(EditorTextBox.Document);
            HasTextChanged = false;
        }
    }
    [RelayCommand]
    void NavigateBack()
    {
        Frame.GoBack();
    }
    bool HasTextChanged = false;

    public event PropertyChangedEventHandler? PropertyChanged;

    void SetForegroundColorBindBack(Color Color) => EditorTextBox.SelectionForegroundColor = Color;

    private void EditorTextBox_TextChanged(object sender, RoutedEventArgs e)
    {
        HasTextChanged = true;
    }
    [RelayCommand]
    async void Save()
    {

        var Note = this.Note;
        if (Note is not null && HasTextChanged)
        {
            HasTextChanged = false;
            await Note.SaveNewVersionAsync(EditorTextBox.Document);
        }
    }

    FormatEffect GetUnderline(ITextSelection selection) =>
        selection is null ? FormatEffect.Undefined :
        selection.CharacterFormat.Underline switch
        {
            UnderlineType.Undefined => FormatEffect.Undefined,
            UnderlineType.Single => FormatEffect.On,
            UnderlineType.None => FormatEffect.Off,
            _ => FormatEffect.Off
        };
    void SetUnderline(FormatEffect fe)
    {
        switch (fe)
        {
            case FormatEffect.On:
                EditorTextBox.Document.Selection.CharacterFormat.Underline = UnderlineType.Single;
                break;
            case FormatEffect.Off:
                EditorTextBox.Document.Selection.CharacterFormat.Underline = UnderlineType.None;
                break;
            case FormatEffect.Toggle:
                EditorTextBox.Document.Selection.CharacterFormat.Underline =
                    EditorTextBox.Document.Selection.CharacterFormat.Underline switch
                    {
                        UnderlineType.Undefined => UnderlineType.None,
                        UnderlineType.None => UnderlineType.Single,
                        UnderlineType.Single => UnderlineType.None,
                        _ => UnderlineType.None
                    };
                break;
        }
    }
    [Event<RoutedEventHandler>]
    void FocusEditBox() => EditorTextBox.Focus(FocusState.Programmatic);
}
static class Extension
{
    public static async Task SaveNewVersionAsync(this Note n, RichEditTextDocument retd)
    {
        retd.GetText(TextGetOptions.FormatRtf, out var rtfText);
        await n.SaveNewVersionAsync(new()
        {
            Cells =
            {
                new RTFCell()
                {
                    RTF = rtfText
                }
            }
        });
    }
    public static async Task LoadDataToAsync(this NoteVersion n, RichEditTextDocument retd)
    {
        var data = await n.GetDataAsync();
        retd.SetText(
            TextSetOptions.FormatRtf,
            ((RTFCell)data.Cells[0]).RTF
        );
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