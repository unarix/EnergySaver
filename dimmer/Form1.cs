using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Newtonsoft.Json;
using RestSharp;

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

            txtResult.AppendText("> Gama seteada en:" + txtBrillo.Text);
            txtResult.AppendText(Environment.NewLine);

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtResult.AppendText("> Modo energia minimo seteado");
            txtResult.AppendText(Environment.NewLine);
            setmin();
            //Brightness.Brightness.SetBrightness(0); // Seteo el brillo en cero
        }

        private void setmin()
        {
            // paso al So a modo full 
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "CMD.EXE";
            process.StartInfo = startInfo;

            // Corriente alterna
            startInfo.Arguments = "/C powercfg -setacvalueindex 079aa309-89cf-4675-88fc-67e4428a4cf1 SUB_VIDEO aded5e82-b909-4619-9949-f5d71dac0bcb 0";
            process.Start();
            process.WaitForExit();

            startInfo.Arguments = "/C powercfg -setdcvalueindex 079aa309-89cf-4675-88fc-67e4428a4cf1 SUB_VIDEO aded5e82-b909-4619-9949-f5d71dac0bcb 0";
            process.Start();
            process.WaitForExit();

            if (chkAp.Checked)
            {
                //Corriente alterna Tiempo de apagado monitor
                startInfo.Arguments = "/C powercfg -setacvalueindex 079aa309-89cf-4675-88fc-67e4428a4cf1 SUB_VIDEO VIDEOIDLE 20"; // en 20 segundos
                process.Start();
                process.WaitForExit();

                startInfo.Arguments = "/C powercfg -setdcvalueindex 079aa309-89cf-4675-88fc-67e4428a4cf1 SUB_VIDEO VIDEOIDLE 20"; // en 20 segundos
                process.Start();
                process.WaitForExit();
            }
            else
            {
                //Corriente alterna Tiempo de apagado monitor
                startInfo.Arguments = "/C powercfg -setacvalueindex 079aa309-89cf-4675-88fc-67e4428a4cf1 SUB_VIDEO VIDEOIDLE 0"; // en 20 segundos
                process.Start();
                process.WaitForExit();

                startInfo.Arguments = "/C powercfg -setdcvalueindex 079aa309-89cf-4675-88fc-67e4428a4cf1 SUB_VIDEO VIDEOIDLE 0"; // en 20 segundos
                process.Start();
                process.WaitForExit();
            }

            // Activar esquema
            startInfo.Arguments = "/C powercfg -setactive 079aa309-89cf-4675-88fc-67e4428a4cf1";
            process.Start();
            process.WaitForExit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            txtResult.AppendText("> Modo energia maximo seteado");
            txtResult.AppendText(Environment.NewLine);
            setmax();
            //Brightness.Brightness.SetBrightness(120); // Seteo la gama en valor normal
        }

        private void setmax()
        {
            // paso al So a modo full 
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "CMD.EXE";
            process.StartInfo = startInfo;

            // Corriente alterna Brillo
            startInfo.Arguments = "/C powercfg -setacvalueindex a98efcdb-7c9a-4ed8-abbc-f0d3fdea31c3 SUB_VIDEO aded5e82-b909-4619-9949-f5d71dac0bcb 100";
            process.Start();
            process.WaitForExit();
            // Corriente alterna Tiempo de apagado monitor
            startInfo.Arguments = "/C powercfg -setacvalueindex a98efcdb-7c9a-4ed8-abbc-f0d3fdea31c3 SUB_VIDEO VIDEOIDLE 0"; // nunca
            process.Start();
            process.WaitForExit();
            // Activar esquema
            startInfo.Arguments = "/C powercfg -setactive a98efcdb-7c9a-4ed8-abbc-f0d3fdea31c3";
            process.Start();
            process.WaitForExit();
        }

        private void cmdReport_Click(object sender, EventArgs e)
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("dimmer.exe", "");

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "CMD.EXE";
            psi.Arguments = "/C powercfg -energy -output " + path + "energy.html";
            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                psi.Verb = "runas";
            }
            Process p = Process.Start(psi);
            
            lblReport.Text = path + "energy.html";
            
            p.WaitForExit();
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

        private void button4_Click(object sender, EventArgs e)
        {
            txtResult.AppendText("> Iniciando consulta a la API");
            txtResult.AppendText(Environment.NewLine);
            if (txtKey.Text != "")
            {
                List<data> dta = auto(txtKey.Text, txtParams.Text, int.Parse(txtMinutos.Text));
                //imprimo en la list
                llenarLst(dta, int.Parse(txtMinutos.Text));
                autodim(dta, int.Parse(txtMinutos.Text));
            }
            else
                MessageBox.Show("Debe ingresar una KEY");
        }


        private List<data> auto(string key, string sparams, int minutos)
        {
            List<data> dta = null;

            if (key != "")
            {
                var client = new RestClient("http://api.aa2000.com.ar/api/vuelos?" + sparams);
                var request = new RestRequest(Method.GET);

                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Host", "api.aa2000.com.ar");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Accept", "*/*");
                request.AddHeader("key", key);
                IRestResponse response = client.Execute(request);

                // Convierto el json a data
                dta = JsonConvert.DeserializeObject<List<data>>(response.Content);

                // Convierto las fechas en date
                foreach (data d in dta)
                    if (!d.stda.Equals(""))
                        d.fecha = DateTime.ParseExact(d.stda, "dd/MM HH:mm", CultureInfo.InvariantCulture);

                // Ordeno el data por fecha
                dta = dta.OrderBy(c => c.fecha).ToList();
            }

            return dta;
        }

        private void autodim(List<data> dta, int minutos)
        {
            foreach (data d in dta)
            {
                if (!d.stda.Equals(""))
                    if (d.fecha > DateTime.Now)
                        if (DateTime.Now.AddMinutes(minutos) < d.fecha)
                        {
                            button2_Click(null, null);
                            break;
                        }
                        else
                        {
                            button3_Click(null, null);
                            break;
                        }
            }
        }

        private void llenarLst(List<data> dta, int minutos)
        {
            // ESTE METODO ME DIBUJA EN UN LISTBOX, CUALES SON LOS VUELOS QUE ENTRAN EN LA VENTANA HORARIA. 
            // SI EXISTEN DATOS DENTRO DE LA VENTANA, ENTONCES ME PONGO EN ENERGIA MAXIMA.
            foreach (data d in dta)
            {
                if (!d.stda.Equals(""))
                {
                    if (d.fecha > DateTime.Now)
                    {
                        if (DateTime.Now.AddMinutes(minutos) < d.fecha)
                        {
                            // Debo ponerme en reposo
                            txtResult.AppendText(" X " + d.arpt + " " + d.nro + " " + d.stda);
                            txtResult.AppendText(Environment.NewLine);
                        }
                        else
                        {
                            // Debo ponerme en modo MAX
                            txtResult.AppendText(" --> " + d.arpt + " " + d.nro + " " + d.stda);
                            txtResult.AppendText(Environment.NewLine);
                            txtResult.AppendText("      CONSUMO DE ENERGIA MAXIMO");
                            txtResult.AppendText(Environment.NewLine);
                        }
                    }
                }
            }


        }

        private void chkAp_CheckedChanged(object sender, EventArgs e)
        {
            // do nothign
        }

        private void btnAuto_Click(object sender, EventArgs e)
        {

            txtResult.AppendText("> Automatico: ya no puedes salir de este modo. Para frenar debes cerrar el programa.");
            txtResult.AppendText(Environment.NewLine);

            btnAuto.Enabled = false;
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(20);
            var timer = new System.Threading.Timer((ef) => { test(); }, null, startTimeSpan, periodTimeSpan);
        }

        static bool flag = false;

        private void test()
        {
            if (flag)
            {
                setmin();
                flag = false;
            }
            else
            {
                setmax();
                flag = true;
            }

        }

    }
}
