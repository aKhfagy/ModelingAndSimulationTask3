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

            this.ReadInput();
            this.Simulate();
            this.CalculatePerformanceMeasures();
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
            this.DemandDistribution = new List<Distribution>();
            this.LeadDaysDistribution = new List<Distribution>();
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
                LeadDaysDistribution.Add(dist);
            }
        }

        public void CalculatePerformanceMeasures()
        {
            this.PerformanceMeasures = new PerformanceMeasures();
            this.PerformanceMeasures.EndingInventoryAverage = 0;
            this.PerformanceMeasures.ShortageQuantityAverage = 0;
            foreach (SimulationCase simulation in this.SimulationCases)
            {
                this.PerformanceMeasures.EndingInventoryAverage += simulation.EndingInventory;
                this.PerformanceMeasures.ShortageQuantityAverage += simulation.ShortageQuantity;
            }

            this.PerformanceMeasures.EndingInventoryAverage /= this.NumberOfDays;
            this.PerformanceMeasures.ShortageQuantityAverage /= this.NumberOfDays;
        }

        public void Simulate()
        {
            this.SimulationCases = new List<SimulationCase>();

            this.OrderQuantity = this.StartOrderQuantity;
            int lastShortage = 0;
            Random random = new Random();

            SimulationCase simulationCase = new SimulationCase();
            simulationCase.Day = 1;
            simulationCase.Cycle = ((0) / this.ReviewPeriod) + 1;
            simulationCase.DayWithinCycle = ((0) % this.ReviewPeriod) + 1;
            simulationCase.BeginningInventory = this.StartInventoryQuantity;
            simulationCase.DayUntillArrival = this.StartLeadDays - 1;
            simulationCase.RandomDemand = random.Next(1, 100);
            simulationCase.RandomLeadDays = random.Next(1, 10);

            foreach (Distribution distribution in this.DemandDistribution)
            {
                if (simulationCase.RandomDemand >= distribution.MinRange 
                    && simulationCase.RandomDemand <= distribution.MaxRange)
                {
                    simulationCase.Demand = distribution.Value;
                    break;
                }
            }
            if(simulationCase.BeginningInventory > 0)
            {
                int required = simulationCase.Demand;
                int inventory = simulationCase.BeginningInventory;

                if(required <= inventory)
                {
                    inventory -= required;
                    simulationCase.ShortageQuantity = 0;
                    simulationCase.EndingInventory = inventory;
                }
                else
                {
                    simulationCase.EndingInventory = 0;
                    simulationCase.ShortageQuantity = required - inventory;
                    lastShortage = simulationCase.ShortageQuantity;
                }
            }
            else
            {
                simulationCase.EndingInventory = 0;
                simulationCase.ShortageQuantity = simulationCase.Demand;
            }
            this.SimulationCases.Add(simulationCase);
            for (int day = 1; day < this.NumberOfDays; day++)
            {
                simulationCase = new SimulationCase();

                simulationCase.Day = day + 1;
                simulationCase.Cycle = ((day) / this.ReviewPeriod) + 1;
                simulationCase.DayWithinCycle = ((day) % this.ReviewPeriod) + 1;
                simulationCase.RandomDemand = random.Next(1, 100);
                simulationCase.RandomLeadDays = random.Next(1, 100);

                simulationCase.BeginningInventory = this.SimulationCases[this.SimulationCases.Count - 1].EndingInventory;

                if(this.SimulationCases[this.SimulationCases.Count - 1].DayUntillArrival >= 1)
                {
                    simulationCase.DayUntillArrival = this.SimulationCases[this.SimulationCases.Count - 1].DayUntillArrival - 1;
                }
                else if(this.SimulationCases[this.SimulationCases.Count - 1].DayUntillArrival == 0 && this.OrderQuantity > 0)
                {
                    simulationCase.DayUntillArrival = 0;
                    simulationCase.BeginningInventory += this.OrderQuantity;
                    this.OrderQuantity = 0;
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

                if (simulationCase.BeginningInventory > 0)
                {
                    int required = simulationCase.Demand + lastShortage;
                    int inventory = simulationCase.BeginningInventory;

                    if (required <= inventory)
                    {
                        inventory -= required;
                        simulationCase.ShortageQuantity = 0;
                        simulationCase.EndingInventory = inventory;
                        lastShortage = 0;
                    }
                    else
                    {
                        simulationCase.EndingInventory = 0;
                        simulationCase.ShortageQuantity = required - inventory;
                        lastShortage = simulationCase.ShortageQuantity;
                    }
                }
                else
                {
                    simulationCase.EndingInventory = 0;
                    simulationCase.ShortageQuantity = simulationCase.Demand + lastShortage;
                    lastShortage = simulationCase.ShortageQuantity;
                }

                if (simulationCase.DayWithinCycle == 5)
                {
                    foreach (Distribution distribution in this.LeadDaysDistribution)
                    {
                        if (simulationCase.RandomLeadDays >= distribution.MinRange
                            && simulationCase.RandomLeadDays <= distribution.MaxRange)
                        {
                            simulationCase.LeadDays = distribution.Value;
                            simulationCase.DayUntillArrival = distribution.Value;
                            break;
                        }
                    }

                    simulationCase.OrderQuantity = this.OrderUpTo - simulationCase.EndingInventory + lastShortage;

                    this.OrderQuantity = simulationCase.OrderQuantity;

                }

                this.SimulationCases.Add(simulationCase);
            }


        }

    }
}
