using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Brightness;

namespace dimmer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Brightness.Brightness.SetBrightness(short.Parse(txtBrillo.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Brightness.Brightness.SetBrightness(0); // Seteo el brillo en cero

            // paso al SO a modo economizador.
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C powercfg -s scheme_max";
            process.StartInfo = startInfo;
            process.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Brightness.Brightness.SetBrightness(120); // Seteo la gama en valor normal

            // paso al So a modo full 
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C powercfg -s scheme_min -c -";
            process.StartInfo = startInfo;
            process.Start();
        }

        private void cmdReport_Click(object sender, EventArgs e)
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("dimmer.exe","");

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "CMD.EXE";
            psi.Arguments = "/C powercfg -energy -output " + path + "energy.html ";
            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                psi.Verb = "runas";
            }
            Process p = Process.Start(psi);

            p.WaitForExit();

            MessageBox.Show("Esta operacion tomara 60 segundos, luego encontrara su reporte en 'C:\\Windows\\system32\\energy.html'");
        }

        private void btnOpenReport_Click(object sender, EventArgs e)
        {
            try
            {
                string path = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("dimmer.exe", "");
                System.Diagnostics.Process.Start(path + "energy.html");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
    }
    }
}
