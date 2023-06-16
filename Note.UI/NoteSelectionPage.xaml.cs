using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using WAH.NoteSystem.Core;
using WAH.NoteSystem.Core.Notes;
using WAH.NoteSystem.Core.Tags;

namespace WAH.NoteSystem.UI;

public sealed partial class NoteSelectionPage : Page
{
    public event Action<Note>? SelectedNoteChanged;
    public NoteSelectionPage()
    {
        InitializeComponent();
    }

    private void ListView_SelectionChanged(object _1, SelectionChangedEventArgs _2)
    {
        var note = (Note)NoteListView.SelectedItem;
        SelectedNoteChanged?.Invoke(note);
        //Frame.Navigate(typeof(NoteEditPage), note);

    }
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is Action<NoteSelectionPage> a) { a(this); }
        base.OnNavigatedTo(e);
    }
    [RelayCommand]
    async void AddNoteAndEdit()
    {
        var note = await Core.NoteSystem.Current.CreateNewNoteAsync("Untitled Note");
        NoteListView.SelectedItem = note;
        //SelectedNoteChanged?.Invoke(note);// Frame.Navigate(typeof(NoteEditPage), note);
    }
    [RelayCommand]
    void ClearFilter()
    {
        Filter.SelectedItem = null;
    }

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Filter.SelectedItem is Tag tag)
        {
            NoteListView.ItemsSource = tag.Notes;
        } else
        {
            NoteListView.ItemsSource = Core.NoteSystem.Current.Notes;
        }
    }
}
