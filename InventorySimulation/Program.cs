﻿using InventoryModels;
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
            Application.Run(new FileNameForum());
            if (SimulationSystem.PATH.Length > 0)
            {
                SimulationSystem system = new SimulationSystem();
                system.ReadInput();
                system.Simulate();
                system.CalculatePerformanceMeasures();

                string result = TestingManager.Test(system, Constants.FileNames.TestCase1);
                MessageBox.Show(result);
                Application.Run(new DataView(system));
            }
        }
    }
}
