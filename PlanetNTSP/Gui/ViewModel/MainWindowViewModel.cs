﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using TSP;

namespace Gui.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        private readonly object lockObject = new object();

        private TspResult bestTspResult;
        private string filename;
        private ICommand openFileCommand;
        private ICommand startCommand;
        private int solutionCount;
        private bool enabledUI = true;
        private int phaseOneTimeInSeconds = 10;
        private int phaseTwoTimeInSeconds = 10;
        private uint workersCount = 8;
        private List<CancellationTokenSource> tasksToken = new List<CancellationTokenSource>();
        private List<Location> startTour;

        public TspResult BestTspResult
        {
            get { return bestTspResult; }
            set
            {
                bestTspResult = value;
                OnPropertyChanged();
            }
        }

        public string Filename
        {
            get { return filename; }
            set
            {
                filename = value;
                OnPropertyChanged();
            }
        }

        public bool EnabledUI
        {
            get { return enabledUI; }
            set
            {
                enabledUI = value;
                OnPropertyChanged();
            }
        }

        public int SolutionCount
        {
            get { return solutionCount; }
            set
            {
                solutionCount = value;
                OnPropertyChanged();
            }
        }

        public int PhaseOneInSeconds
        {
            get { return phaseOneTimeInSeconds;}
            set
            {
                phaseOneTimeInSeconds = value;
                OnPropertyChanged();
            }
        }

        public int PhaseTwoInSeconds
        {
            get { return phaseTwoTimeInSeconds; }
            set
            {
                phaseTwoTimeInSeconds = value;
                OnPropertyChanged();
            }
        }

        public uint WorkersCount
        {
            get { return workersCount; }
            set
            {
                if (!this.EnabledUI)
                {
                    int diff = (int)(value - workersCount);
                    if (diff > 0)
                    {
                        InvokeWorkers((uint)diff);
                    }
                    else if (diff < 0)
                    {
                        KillWorkers((uint) Math.Abs(diff));
                    } 
                }
                workersCount = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenFileCommand => openFileCommand ?? (openFileCommand = new RelayCommand(Open));
        public ICommand StartCommand => startCommand ?? (startCommand = new RelayCommand(Start));

        private void Open()
        {
            var openFileDialog = new OpenFileDialog { Multiselect = false };
            if (openFileDialog.ShowDialog() == true)
            {
                this.Filename = openFileDialog.FileName;
            }
        }

        private void Start()
        {
            this.EnabledUI = false;

            var dataModel = new DataModel(this.Filename);
            startTour = new List<Location>(dataModel.Data);
            var startResult = new TspResult() { BestTour = startTour, BestTourEdges = startTour.ConvertToEdgeList(), Distance = double.MaxValue };
            this.BestTspResult = startResult;

            CalculateScale(startTour);

            uint count = workersCount;

            InvokeWorkers(count);
        }

        private void InvokeWorkers(uint count)
        {
            for (var i = 0; i < count; i++)
            {
                var tokenSource = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    var copy = new List<Location>(startTour);
                    while (!tokenSource.Token.IsCancellationRequested)
                    {
                        TspResult result = NtspRun(copy);
                        result.BestTourEdges = result.BestTour.ConvertToEdgeList();
                        UpdateResult(result);
                    }
                });
                tasksToken.Add(tokenSource);
            }
        }

        private void KillWorkers(uint count)
        {
            for (var i = 0; i < count; i++)
            {
                var token = tasksToken.FirstOrDefault();
                tasksToken.Remove(token);
                token.Cancel();
            }
        }

        private void UpdateResult(TspResult result)
        {
            lock (lockObject)
            {
                if (bestTspResult == null
                    || result.Distance < bestTspResult.Distance)
                {
                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            this.BestTspResult = result;
                        });
                }

                SolutionCount += 1;
            }
        }

        private TspResult NtspRun(List<Location> tempTour)
        {
            var bestTspRes = new TspResult();
            bestTspRes.Distance = Double.MaxValue;
            double tempDistance = 0;
            double counter = 0;
            var sw = new Stopwatch();
            sw.Start();
            while (sw.Elapsed.TotalSeconds < phaseOneTimeInSeconds)
            {
                counter++;
                tempTour.Shuffle();
                if ((tempDistance = Utils.DistanceSum(tempTour)) < bestTspRes.Distance)
                {
                    bestTspRes.BestTour = new List<Location>(tempTour);
                    bestTspRes.Distance = tempDistance;
                }
            }

            while (sw.Elapsed.TotalSeconds < phaseTwoTimeInSeconds)
            {
                counter++;
                tempTour.SwapEdges();
                if ((tempDistance = Utils.DistanceSum(tempTour)) < bestTspRes.Distance)
                {
                    bestTspRes.BestTour = new List<Location>(tempTour);
                    bestTspRes.Distance = tempDistance;
                }
            }

            return bestTspRes;
        }

        private static void CalculateScale(List<Location> tour)
        {
            var temp = tour[0];
            XSizeConverter.MinX = temp.X;
            YSizeConverter.MinY = temp.Y;

            foreach (var location in tour)
            {
                XSizeConverter.MinX = Math.Min(XSizeConverter.MinX, location.X);
                XSizeConverter.MaxX = Math.Max(XSizeConverter.MaxX, location.X);

                YSizeConverter.MinY = Math.Min(YSizeConverter.MinY, location.Y);
                YSizeConverter.MaxY = Math.Max(YSizeConverter.MaxY, location.Y);
            }
        }
    }
}