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
            throw new NotImplementedException();
        }

        public void Simulate()
        {
            throw new NotImplementedException();
        }

    }
}
