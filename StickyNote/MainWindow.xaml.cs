using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Diagnostics;
using WAH.NoteSystem.Core;
using WAH.NoteSystem.Core.Notes;
using WAH.NoteSystem.UI;
using Windows.UI.Text.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StickyNote
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(TitleBar);
            SystemBackdrop = new MicaBackdrop();

            NoteSelectionFrame.Navigated += delegate
            {
                if (NoteSelectionFrame.Content is NoteSelectionPage ns)
                    ns.SelectedNoteChanged += SelectedNoteChanged;
            };
            NoteEditFrame.Navigated += delegate
            {
                if (NoteEditFrame.Content is NoteEditPage ne)
                    ne.BackButtonVisibility = Visibility.Collapsed;
            };
            InitAsync();
        }
        async void InitAsync()
        {
            await NoteSystem.Current.EnsureSystemInitializedAsync();
            Debug.WriteLine(NoteSystem.Current.RootFolder.Path);
            NoteSelectionFrame.Navigate(typeof(NoteSelectionPage));
        }

        private void SelectedNoteChanged(Note note)
        {
            NoteEditFrame.Navigate(typeof(NoteEditPage), note);
        }
    }
}
