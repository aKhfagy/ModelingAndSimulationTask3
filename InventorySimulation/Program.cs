using InventoryModels;
using InventoryTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventorySimulation
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SimulationSystem.PATH = System.Environment.CurrentDirectory + @"\TestCases\TestCase1.txt";
            if (SimulationSystem.PATH.Length > 0)
            {
                SimulationSystem system = new SimulationSystem();

                //foreach (var simulation in system.SimulationCases)
                //{
                //    MessageBox.Show(simulation.Day.ToString() + simulation.Cycle.ToString() + simulation.DayWithinCycle.ToString() + simulation.BeginningInventory.ToString() +
                //        simulation.Demand.ToString() + simulation.EndingInventory.ToString() + simulation.ShortageQuantity.ToString() + simulation.OrderQuantity.ToString() +
                //        simulation.LeadDays.ToString() + simulation.RandomDemand.ToString() + simulation.RandomLeadDays.ToString());
                //}

                //for (int i = 0; i < 20; i++)
                //{
                //    var s = new SimulationCase();
                //    s.RandomDemand = 1;
                //    s.RandomLeadDays = 1;
                //    system.SimulationCases.Add(s);
                //}
                
                string result = TestingManager.Test(system, "TestCase1.txt");
                MessageBox.Show(result);
                Application.Run(new DataView(system));
            }
        }
    }
}
