using HealthCheck.Devices;
using System.Text;

namespace HealthCheck
{
    public partial class FormMain : System.Windows.Forms.Form
    {
        private String workingDirPath = @"C:\Users\chua_s\Documents\Rohde-Schwarz HealthCheck";
        private String configFilename = "test configuration.csv";

        private Boolean demoMode = true;
        private Boolean logging = false;
        private Boolean pathCal = false;
        private Boolean initialised = false;     
        private Boolean resetInstruments = true;

        private SpectrumAnalyser sA;
        private SignalGenerator sG;
        private Dut dut;

        public FormMain()
        {
            InitializeComponent();
            this.buttonStart.Enabled = false;
        }

        private void WriteTrace(String message, String traceFilename)
        {
            if (this.logging)
            {
                using (StreamWriter writer = new StreamWriter(traceFilename, true))
                {
                    StringBuilder s = new StringBuilder(message);
                    writer.WriteLine(s);
                    writer.Dispose();
                }
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
               this.buttonStart.Text = "Testing...";

                DateTime dt = this.GetDateTime();
                String traceFilename = this.workingDirPath + "\\logs" + "\\" + "Log_" + dt.ToString("ddMMyyyy") + "_" + dt.ToString("hhmmss") + ".txt";
                String resultFilename = this.workingDirPath + "\\results" + "\\" + "result_" + dt.ToString("ddMMyyyy") + "_" + dt.ToString("hhmmss") + ".csv";
             
                StringBuilder s = new StringBuilder("SA Resource: " + this.textBoxSA.Text);
                s.AppendLine();
                s.AppendLine("SG Resource: " + this.textBoxSG.Text);
                s.AppendLine("DUT Resource: " + this.textBoxDUT.Text);
                s.AppendLine("Power sensor Resource: " + this.textBoxPS.Text);
                s.AppendLine("Path Calibration: " + this.checkBoxPathCal.Checked);
                s.AppendLine("Config filename: " + this.workingDirPath + "\\" + this.configFilename);
                s.AppendLine("Result filename: " + resultFilename);
                s.AppendLine("Demo Mode: " + this.demoMode);
                s.AppendLine("Instruments Reset: " + this.resetInstruments);

                this.WriteTrace(s.ToString(), traceFilename);

                this.sA.SetTraceFilename(traceFilename);
                this.sG.SetTraceFilename(traceFilename);
                this.dut.SetTraceFilename(traceFilename);

                Testplan testplan = new Testplan(this.logging, traceFilename, this.sG, this.sA, this.dut);
                testplan.PopulateTests(workingDirPath + "\\" + configFilename);
                testplan.SetResults(this.richTextBoxDisplay, resultFilename);

                this.textBoxDate.Text = dt.ToString("dd/MM/yyyy");
                this.textBoxTime.Text = dt.ToString("HH:mm:ss");

                this.richTextBoxDisplay.Clear();

                testplan.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception");
            }

            this.buttonStart.Text = "Start";
        }          

        private void buttonInitialise_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder s = new StringBuilder("SA Resource: " + this.textBoxSA.Text);
                s.AppendLine();
                s.AppendLine("SG Resource: " + this.textBoxSG.Text);
                s.AppendLine("DUT Resource: " + this.textBoxDUT.Text);
                s.AppendLine("Power sensor Resource: " + this.textBoxPS.Text);
                s.AppendLine("Path Calibration: " + this.checkBoxPathCal.Checked);
                s.AppendLine("Config filename: " + this.workingDirPath + "\\" + this.configFilename);
                s.AppendLine("Demo Mode: " + this.demoMode);
                s.AppendLine("Instruments Reset: " + this.resetInstruments);

                this.buttonStart.Enabled = false;
                this.initialised = false;

                this.pathCal = this.checkBoxPathCal.Checked;
                this.logging = this.checkBoxLogging.Checked;
              
                DateTime dt = this.GetDateTime();
                String traceFilename = this.workingDirPath + "\\logs" + "\\" + "Init_log_" + dt.ToString("ddMMyyyy") + "_" + dt.ToString("hhmmss") + ".txt";

                this.WriteTrace(s.ToString(), traceFilename);

                this.sA = new SpectrumAnalyser(this.textBoxSA.Text, "SA", this.demoMode, this.resetInstruments, this.logging, traceFilename);
                this.sG = new SignalGenerator(this.textBoxSG.Text, "SG", this.demoMode, this.resetInstruments, this.logging, traceFilename);
                this.dut = new Dut(this.textBoxDUT.Text, "DUT", this.demoMode, this.resetInstruments, this.logging, traceFilename);

                Testplan testplan = new Testplan(this.logging, traceFilename, this.sG, this.sA, this.dut);              
                testplan.PopulateTests(this.workingDirPath + "\\" + this.configFilename);

                this.textBoxManufacturer.Text = this.dut.Manufacturer();
                this.textBoxModel.Text = this.dut.Model();
                this.textBoxSerialNumber.Text = this.dut.SerialNumber();

                this.textBoxDate.Text = dt.ToString("dd/MM/yyyy");
                this.textBoxTime.Text = dt.ToString("HH:mm:ss");

                this.richTextBoxDisplay.Clear();
                this.buttonStart.Enabled = true;
                this.initialised = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception");
            }
        }

        private DateTime GetDateTime()
        {
            return DateTime.Now;
        }
    }
}