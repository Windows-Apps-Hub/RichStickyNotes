using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Specialized;
using System;
using WAH.NoteSystem.Core.Tags;

namespace WAH.NoteSystem.UI.Helpers;
class ObservableCollectionMimicker<T> : ObservableCollectionMimicker<T, T>
{
    public ObservableCollectionMimicker(ObservableCollection<T> source, IList<T> dest) : base(source, dest) { }

    protected override T CreateFrom(T source) => source;
}
abstract class ObservableCollectionMimicker<TSource, TDest> : ObservableCollectionMimicker<ObservableCollection<TSource>, TSource, TDest>
{
    public ObservableCollectionMimicker(ObservableCollection<TSource> source, IList<TDest> dest) : base(source, dest) { }
}
abstract class ObservableCollectionMimicker<TSourceCollection, TSource, TDest> : IDisposable where TSourceCollection : INotifyCollectionChanged, IList<TSource>
{
    public readonly TSourceCollection SourceObservableCollection;
    public readonly IList<TDest> DestinationList;
    public ObservableCollectionMimicker(TSourceCollection source, IList<TDest> dest)
    {
        SourceObservableCollection = source;
        DestinationList = dest;
        source.CollectionChanged += CollectionChanged;
    }

    private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        var newindex = e.NewStartingIndex;
        var oldindex = e.OldStartingIndex;
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (SourceObservableCollection.Count - 1 != DestinationList.Count)
                {
                    ResetAndReadd();
                    break;
                }
                var lp = SourceObservableCollection[newindex];
                DestinationList.Insert(newindex, CreateFrom(lp));
                break;
            case NotifyCollectionChangedAction.Remove:
                {
                    if (SourceObservableCollection.Count != DestinationList.Count - 1)
                    {
                        ResetAndReadd();
                        break;
                    }
                    var oldItem = DestinationList[oldindex];
                    DestinationList.RemoveAt(oldindex);
                    Recycle(oldItem);
                }
                break;
            case NotifyCollectionChangedAction.Move:
                if (SourceObservableCollection.Count != DestinationList.Count)
                {
                    ResetAndReadd();
                    break;
                }
                (DestinationList[oldindex], DestinationList[newindex]) = (DestinationList[newindex], DestinationList[oldindex]);
                break;
            case NotifyCollectionChangedAction.Replace:
                {
                    if (SourceObservableCollection.Count != DestinationList.Count)
                    {
                        ResetAndReadd();
                        break;
                    }
                    var oldItem = DestinationList[oldindex];
                    DestinationList[oldindex] = CreateFrom(SourceObservableCollection[oldindex]);
                    Recycle(oldItem);
                }
                break;
            case NotifyCollectionChangedAction.Reset:
                foreach (var item in DestinationList) Recycle(item);
                DestinationList.Clear();
                break;
        }
    }
    public void ResetAndReadd()
    {
        foreach (var item in DestinationList) Recycle(item);
        DestinationList.Clear();
        foreach (var item in SourceObservableCollection)
        {
            DestinationList.Add(CreateFrom(item));
        }
    }

    protected abstract TDest CreateFrom(TSource source);
    protected virtual void Recycle(TDest dest) { }

    public void Dispose()
    {
        SourceObservableCollection.CollectionChanged -= CollectionChanged;
        GC.SuppressFinalize(this);
    }
}