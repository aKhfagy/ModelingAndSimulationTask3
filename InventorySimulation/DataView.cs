using InventoryModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventorySimulation
{
    public partial class DataView : Form
    {
        public DataView(SimulationSystem system)
        {
            InitializeComponent();
            DataTable dt = new DataTable();
            object[] obj;
            dt.Columns.Add("Day", typeof(int));
            dt.Columns.Add("Cycle", typeof(int));
            dt.Columns.Add("Day Withing Cycle", typeof(int));
            dt.Columns.Add("Begining Inventory", typeof(int));
            dt.Columns.Add("Random Digit For Demand", typeof(int));
            dt.Columns.Add("Demand", typeof(int));
            dt.Columns.Add("Ending Inventory", typeof(int));
            dt.Columns.Add("Shortage Quantity", typeof(int));
            dt.Columns.Add("Order Quantity", typeof(int));
            dt.Columns.Add("Random Digit For Lead Days", typeof(int));
            dt.Columns.Add("Lead Time (Days)", typeof(int));
            dt.Columns.Add("Days until order arrives", typeof(int));
            foreach (SimulationCase simulationCase in system.SimulationCases)
            {
                obj = new object[dt.Columns.Count];
                obj[0] = simulationCase.Day;
                obj[1] = simulationCase.Cycle;
                obj[2] = simulationCase.DayWithinCycle;
                obj[3] = simulationCase.BeginningInventory;
                obj[4] = simulationCase.RandomDemand;
                obj[5] = simulationCase.Demand;
                obj[6] = simulationCase.EndingInventory;
                obj[7] = simulationCase.ShortageQuantity;
                obj[8] = simulationCase.OrderQuantity;
                obj[9] = simulationCase.RandomLeadDays;
                if(simulationCase.DayWithinCycle == 1 && simulationCase.Cycle > 1)
                    obj[10] = simulationCase.LeadDays;
                else 
                    obj[10] = 0;
                obj[11] = simulationCase.LeadDays;
                dt.Rows.Add(obj);
            }

            dataGridView1.DataSource = dt;

            dt = new DataTable();
            dt.Columns.Add("Ending Inventory Average", typeof(decimal));
            dt.Columns.Add("Shortage Quantity Average", typeof(decimal));
            obj = new object[dt.Columns.Count];
            obj[0] = system.PerformanceMeasures.EndingInventoryAverage;
            obj[1] = system.PerformanceMeasures.ShortageQuantityAverage;
            dt.Rows.Add(obj);
            dataGridView2.DataSource = dt;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void DataView_Load(object sender, EventArgs e)
        {

        }
    }
}
