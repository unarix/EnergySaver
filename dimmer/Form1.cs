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
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // paso al So a modo full 
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "CMD.EXE";
            process.StartInfo = startInfo;

            // Corriente alterna
            startInfo.Arguments = "/C powercfg -setacvalueindex SCHEME_MIN SUB_VIDEO aded5e82-b909-4619-9949-f5d71dac0bcb 5";
            process.Start();
            process.WaitForExit();

            if (chkAp.Checked)
            {
                //Corriente alterna Tiempo de apagado monitor
                startInfo.Arguments = "/C powercfg -setacvalueindex SCHEME_MIN SUB_VIDEO VIDEOIDLE 20"; // en 20 segundos
                process.Start();
                process.WaitForExit();
            }
            else
            {
                //Corriente alterna Tiempo de apagado monitor
                startInfo.Arguments = "/C powercfg -setacvalueindex SCHEME_MIN SUB_VIDEO VIDEOIDLE 0"; // en 20 segundos
                process.Start();
                process.WaitForExit();
            }

            // Activar esquema
            startInfo.Arguments = "/C powercfg -setactive SCHEME_MIN";
            process.Start();
            process.WaitForExit();

            //Brightness.Brightness.SetBrightness(0); // Seteo el brillo en cero
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // paso al So a modo full 
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "CMD.EXE";
            process.StartInfo = startInfo;

            // Corriente alterna Brillo
            startInfo.Arguments = "/C powercfg -setacvalueindex SCHEME_MAX SUB_VIDEO aded5e82-b909-4619-9949-f5d71dac0bcb 100";
            process.Start();
            process.WaitForExit();
            // Corriente alterna Tiempo de apagado monitor
            startInfo.Arguments = "/C powercfg -setacvalueindex SCHEME_MAX SUB_VIDEO VIDEOIDLE 0"; // nunca
            process.Start();
            process.WaitForExit();
            // Activar esquema
            startInfo.Arguments = "/C powercfg -setactive SCHEME_MAX";
            process.Start();
            process.WaitForExit();


            //Brightness.Brightness.SetBrightness(120); // Seteo la gama en valor normal
        }

        private void cmdReport_Click(object sender, EventArgs e)
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("dimmer.exe", "");

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "CMD.EXE";
            psi.Arguments = "/C powercfg -energy -output " + path + "energy.html ";
            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                psi.Verb = "runas";
            }
            Process p = Process.Start(psi);

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
            if (txtKey.Text != "")
            {
                try
                {
                    txtResult.AppendText(Environment.NewLine);
                    txtResult.AppendText("> Iniciando consulta a la API");
                    txtResult.AppendText(Environment.NewLine);

                    var client = new RestClient("http://api.aa2000.com.ar/api/vuelos?" + txtParams.Text);
                    var request = new RestRequest(Method.GET);

                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("Connection", "keep-alive");
                    request.AddHeader("Accept-Encoding", "gzip, deflate");
                    request.AddHeader("Host", "api.aa2000.com.ar");
                    request.AddHeader("Cache-Control", "no-cache");
                    request.AddHeader("Accept", "*/*");
                    request.AddHeader("key", txtKey.Text);
                    IRestResponse response = client.Execute(request);

                    txtResult.AppendText("> Respuesta ok.");
                    txtResult.AppendText(Environment.NewLine);

                    // Convierto el json a data
                    List<data> dta = JsonConvert.DeserializeObject<List<data>>(response.Content);
                    
                    // Convierto las fechas en date
                    foreach (data d in dta)
                        if (!d.stda.Equals(""))
                            d.fecha = DateTime.ParseExact(d.stda, "dd/MM HH:mm", CultureInfo.InvariantCulture);

                    // Ordeno el data por fecha
                    dta = dta.OrderBy(c => c.fecha).ToList();

                    //imprimo en la list
                    llenarLst(dta, int.Parse(txtMinutos.Text));

                    //
                    autodim(dta, int.Parse(txtMinutos.Text));

                }
                catch (Exception ex)
                {
                    MessageBox.Show("error:" + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Debe ingresar una KEY");
            }
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

        }
    }
}
