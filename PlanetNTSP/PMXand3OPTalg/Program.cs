using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP;

namespace PMXand3OPTalg
{

    class Program
    {
        static void Main(string[] args)
        {
            var bestTspRes = new TspResult();
            var dataModel = new DataModel("wi29.tsp");
            double bestDistance = Double.MaxValue;
            List<Location> tempTour = new List<Location>(dataModel.Data);
            double tempDistance = 0;
            double counter = 0;
            int phaseTimeInSeconds = 10;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.Elapsed.TotalSeconds < phaseTimeInSeconds)
            {
                counter++;
                tempTour.Shuffle();
                if ((tempDistance = Utils.DistanceSum(tempTour)) < bestDistance)
                {
                    bestTspRes.BestTour = new List<Location>(tempTour);
                    bestDistance = tempDistance;
                }
            }
            while (sw.Elapsed.TotalSeconds < phaseTimeInSeconds)
            {
                counter++;
                tempTour.SwapEdges();
                if ((tempDistance = Utils.DistanceSum(tempTour)) < bestDistance)
                {
                    bestTspRes.BestTour = new List<Location>(tempTour);
                    bestDistance = tempDistance;
                }
            }
            Console.WriteLine("the shortest found traveling salesman route:");
            foreach(var location in bestTspRes.BestTour)
            {
                Console.WriteLine(location.Id + ". " + location.X + "," + location.Y);
            }
            Console.WriteLine("Distance = " + bestDistance);
            Console.WriteLine("Solution Count = " + counter);
        }
    }
}
