using System.Diagnostics;
using WAH.NoteSystem.Core.Notes;
using WAH.NoteSystem.UI;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StickyNote.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ApplicationView.GetForCurrentView().Title = "Sticky Note";
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
            NoteSelectionFrame.Navigate(typeof(NoteSelectionPage));
        }
        private void SelectedNoteChanged(Note note)
        {
            NoteEditFrame.Navigate(typeof(NoteEditPage), note);
        }
    }
}
