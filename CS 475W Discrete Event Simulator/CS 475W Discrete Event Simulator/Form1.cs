﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CS_475W_Discrete_Event_Simulator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Simulation newsim = new Simulation("test.txt");
            newsim.Run();
        }
    }
}
