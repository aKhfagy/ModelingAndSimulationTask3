﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventoryModels;
using InventoryTesting;
using System.IO;

namespace InventorySimulation
{
    public partial class FileNameForum : Form
    {
        public FileNameForum()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = textBox1.Text;
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("Please enter the path to the test file");
            }
            else
            {
                if (!File.Exists(path))
                {
                    MessageBox.Show("Please enter a valid path");
                }
                else
                {
                    SimulationSystem.PATH = path;
                    this.Close();
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
