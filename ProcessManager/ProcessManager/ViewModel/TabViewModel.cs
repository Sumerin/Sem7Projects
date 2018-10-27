using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace ProcessManager.ViewModel
{
    public class TabViewModel : NotifyPropertyChanged
    {
        private ICommand sortCommand;
        private object processLock;
        private ListCollectionView procesesView;
        private ObservableCollection<ProcessViewModel> proceses;
        private string lastSort;
        private string filterText;
        private ICommand selectionChangedCommand;
        private ProcessViewModel selectedItem;

        public string FilterText
        {
            get { return filterText; }
            set
            {
                filterText = value;
                ProcesesView.Filter = Filter;
                OnPropertyChanged();
            }
        }
        public ICommand SortCommand
        {
            get { return sortCommand ?? (sortCommand = new RelayCommand(SortProcess)); }
        }
        public ICollectionView ProcesesView
        {
            get
            {
                return this.procesesView;
            }
        }
        public ICommand SelectionChangedCommand
        {
            get { return selectionChangedCommand ?? (selectionChangedCommand = new RelayCommand(Select)); }
        }

        public ProcessViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                this.selectedItem = value;
                OnPropertyChanged();
            }
        }

        private void Select(object obj)
        {
            var process = obj as ProcessViewModel;
            Task.Run(() =>
            {
                process?.StartWatching();
            });
            SelectedItem?.StopWatching();
            SelectedItem = process;
        }


        private bool Filter(object obj)
        {
            var item = obj as ProcessViewModel;
            return item.Name.Contains(FilterText);
        }
        private void SortProcess(object obj)
        {
            var sortOption = obj as string;
            this.ProcesesView.SortDescriptions.Clear();
            if (lastSort == sortOption)
            {
                this.ProcesesView.SortDescriptions.Add(new SortDescription(sortOption, ListSortDirection.Descending));
                lastSort = "";
            }
            else
            {
                this.ProcesesView.SortDescriptions.Add(new SortDescription(sortOption, ListSortDirection.Ascending));
                lastSort = sortOption;
            }
        }

        public TabViewModel()
        {
            this.proceses = new ObservableCollection<ProcessViewModel>();
            this.procesesView = new ListCollectionView(this.proceses);
            this.processLock = new object();
        }

        public ProcessViewModel Find(int pid)
        {
            lock (this.processLock)
            {
                foreach (ProcessViewModel process in this.proceses)
                {
                    if (process.PID == pid)
                    {
                        return process;
                    }
                }
            }
            return null;
        }

        internal void AddProcess(ProcessViewModel processItem)
        {
            lock (this.processLock)
            {
                this.proceses.Add(processItem);
            }
        }

        internal void RemoveProcess(ProcessViewModel processItem)
        {
            lock (this.processLock)
            {
                if (this.proceses.Contains(processItem))
                {
                    this.proceses.Remove(processItem);
                }
            }
        }

        public void ClearContext()
        {
            lock (this.processLock)
            {
                foreach (ProcessViewModel viewModel in this.proceses)
                {
                    viewModel.Dispose();
                }
                this.proceses.Clear();
            }
        }
    }
}
