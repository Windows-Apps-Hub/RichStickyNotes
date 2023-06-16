using WAH.NoteSystem.Core.Notes;
using WAH.NoteSystem.UI;
using Microsoft.UI.Xaml;
namespace NoteFlyout;

public sealed partial class FlyoutContent
{
    public FlyoutContent()
    {
        InitializeComponent();

        Frame.Navigated += delegate
        {
            switch (Frame.Content)
            {
                case NoteSelectionPage ns:
                    ns.SelectedNoteChanged += SelectedNoteChanged;
                    break;
                case NoteEditPage ne:
                    ne.BackButtonVisibility = Visibility.Visible;
                    break;
            }
        };
        Frame.Navigating += delegate
        {
            switch (Frame.Content)
            {
                case NoteSelectionPage ns:
                    ns.SelectedNoteChanged -= SelectedNoteChanged;
                    break;
            }
        };
        Frame.Navigate(typeof(NoteSelectionPage));
    }

    private void SelectedNoteChanged(Note obj)
    {
        Frame.Navigate(typeof(NoteEditPage), obj);
    }

    public void OpenFlyout()
    {

    }
    public void CloseFlyout()
    {

    }
}
