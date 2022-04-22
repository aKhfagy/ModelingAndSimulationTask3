using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
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
