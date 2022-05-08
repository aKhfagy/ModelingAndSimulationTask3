using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryModels
{
    public class SimulationSystem
    {
        public SimulationSystem()
        {
            DemandDistribution = new List<Distribution>();
            LeadDaysDistribution = new List<Distribution>();
            SimulationCases = new List<SimulationCase>();
            PerformanceMeasures = new PerformanceMeasures();
        }

        ///////////// INPUTS /////////////

        public int OrderUpTo { get; set; }
        public int ReviewPeriod { get; set; }
        public int NumberOfDays { get; set; }
        public int StartInventoryQuantity { get; set; }
        public int StartLeadDays { get; set; }
        public int StartOrderQuantity { get; set; }
        public int OrderQuantity { get; set; }
        public List<Distribution> DemandDistribution { get; set; }
        public List<Distribution> LeadDaysDistribution { get; set; }
        public static string PATH = "";
        ///////////// OUTPUTS /////////////

        public List<SimulationCase> SimulationCases { get; set; }
        public PerformanceMeasures PerformanceMeasures { get; set; }

        public void ReadInput()
        {
            string[] lines = File.ReadAllLines(PATH);
            this.OrderUpTo = int.Parse(lines[1]); // 2
            this.ReviewPeriod = int.Parse(lines[4]); // 5
            this.StartInventoryQuantity = int.Parse(lines[7]); // 8
            this.StartLeadDays = int.Parse(lines[10]); // 11
            this.StartOrderQuantity = int.Parse(lines[13]); // 14
            this.NumberOfDays = int.Parse(lines[16]); // 17
            Distribution prev = null;
            for (int i = 19; i < lines.Length && lines[i].Length > 0; i++)
            {
                string[] temp = lines[i].Split(',');
                int value = int.Parse(temp[0]);
                decimal prob = decimal.Parse(temp[1]);

                Distribution dist = new Distribution();
                dist.Value = value;
                dist.Probability = prob;

                if(DemandDistribution.Count == 0)
                {
                    dist.MinRange = 1;
                    dist.MaxRange = (int)(prob * 100);
                    dist.CummProbability = prob;
                }
                else
                {
                    dist.MinRange = prev.MaxRange + 1;
                    dist.MaxRange = dist.MinRange + (int)(prob * 100) - 1;
                    dist.CummProbability = prev.CummProbability + prob;
                }
                prev = dist;
                DemandDistribution.Add(dist);
            }

            prev = null;

            for (int i = 26; i < lines.Length && lines[i].Length > 0; i++)
            {
                string[] temp = lines[i].Split(',');
                int value = int.Parse(temp[0]);
                decimal prob = decimal.Parse(temp[1]);

                Distribution dist = new Distribution();
                dist.Value = value;
                dist.Probability = prob;

                if (LeadDaysDistribution.Count == 0)
                {
                    dist.MinRange = 1;
                    dist.MaxRange = (int)(prob * 10);
                    dist.CummProbability = prob;
                }
                else
                {
                    dist.MinRange = prev.MaxRange + 1;
                    dist.MaxRange = dist.MinRange + (int)(prob * 10) - 1;
                    dist.CummProbability = prev.CummProbability + prob;
                }
                prev = dist;
                LeadDaysDistribution.Add(dist);
            }
        }

        public void CalculatePerformanceMeasures()
        {
            decimal endingQn = 0;
            decimal shortageQn = 0;
            foreach (SimulationCase simulation in this.SimulationCases)
            {
                endingQn += simulation.EndingInventory;
                shortageQn += simulation.ShortageQuantity;
            }

            this.PerformanceMeasures.EndingInventoryAverage = endingQn / this.NumberOfDays;
            this.PerformanceMeasures.ShortageQuantityAverage = shortageQn / this.NumberOfDays;
        }

        public void Simulate()
        {
            Random random = new Random();
            SimulationCase prev = new SimulationCase();
            prev.OrderQuantity = this.StartOrderQuantity;
            this.OrderQuantity = prev.OrderQuantity;
            prev.EndingInventory = this.StartInventoryQuantity;
            prev.LeadDays = this.StartLeadDays;
            for (int day = 1; day <= this.NumberOfDays; day++)
            {
                SimulationCase simulationCase = new SimulationCase();
                simulationCase.Day = day;
                simulationCase.RandomDemand = random.Next(1, 100);

                simulationCase.Cycle = ((day - 1) / this.ReviewPeriod) + 1;
                simulationCase.DayWithinCycle = ((day - 1) % this.ReviewPeriod) + 1;

                if(simulationCase.DayWithinCycle == 1)
                {
                    if(simulationCase.Cycle > 1)
                    {
                        simulationCase.RandomLeadDays = random.Next(1, 10);
                        foreach (Distribution distribution in this.LeadDaysDistribution)
                        {
                            if(simulationCase.RandomLeadDays >= distribution.MinRange 
                                && simulationCase.RandomLeadDays <= distribution.MaxRange)
                            {
                                simulationCase.LeadDays = distribution.Value;
                                break;
                            }
                        }
                        simulationCase.OrderQuantity = this.OrderUpTo - prev.EndingInventory + prev.ShortageQuantity;
                        this.OrderQuantity = simulationCase.OrderQuantity;
                        prev.ShortageQuantity = 0;
                    }
                }

                if(prev.LeadDays >= 1)
                {
                    simulationCase.LeadDays = prev.LeadDays - 1;
                    simulationCase.BeginningInventory = prev.EndingInventory;
                }
                else if(prev.LeadDays == 0)
                {
                    simulationCase.LeadDays = 0;
                    simulationCase.BeginningInventory = prev.EndingInventory + this.OrderQuantity;
                }

                foreach (Distribution distribution in this.DemandDistribution)
                {
                    if (simulationCase.RandomDemand >= distribution.MinRange
                         && simulationCase.RandomDemand <= distribution.MaxRange)
                    {
                        simulationCase.Demand = distribution.Value;
                        break;
                    }
                }

                if(simulationCase.Demand <= simulationCase.BeginningInventory)
                {
                    simulationCase.EndingInventory = simulationCase.BeginningInventory - simulationCase.Demand;
                    simulationCase.ShortageQuantity = prev.ShortageQuantity;
                }
                else
                {
                    simulationCase.EndingInventory = 0;
                    simulationCase.ShortageQuantity = simulationCase.ShortageQuantity + (simulationCase.Demand - simulationCase.BeginningInventory);
                }

                SimulationCases.Add(simulationCase);
                prev = simulationCase;

            }
        }

    }
}
