using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using TSP;

namespace PMXand3OPTalg
{

    class Program
    {
        static void Main(string[] args)
        {
            foreach (var s in args)
            {
                Console.WriteLine(s);
            }

            var server = new NamedPipeServerStream(args[1]);
            server.WaitForConnection();

            var bestTspRes = new TspResult();
            var dataModel = new DataModel(args[0]);
            double bestDistance = Double.MaxValue;
            List<Location> tempTour = new List<Location>(dataModel.Data);

            double tempDistance = 0;
            double counter = 0;
            int phaseOneTimeInSeconds = int.Parse(args[2]);
            int phaseTwoTimeInSeconds = int.Parse(args[3]);

            while (true)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (sw.Elapsed.TotalSeconds < phaseOneTimeInSeconds)
                {
                    counter++;
                    tempTour.Shuffle();
                    if ((tempDistance = Utils.DistanceSum(tempTour)) < bestDistance)
                    {
                        bestTspRes.BestTour = new List<Location>(tempTour);
                        bestDistance = tempDistance;
                    }
                }
                while (sw.Elapsed.TotalSeconds < phaseTwoTimeInSeconds)
                {
                    counter++;
                    tempTour.SwapEdges();
                    if ((tempDistance = Utils.DistanceSum(tempTour)) < bestDistance)
                    {
                        bestTspRes.BestTour = new List<Location>(tempTour);
                        bestDistance = tempDistance;
                    }
                }

                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(server, bestTspRes);
            }
        }
    }
}
