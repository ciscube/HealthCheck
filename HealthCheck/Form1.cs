using HealthCheck.Devices;

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

        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                this.buttonStart.Text = "Testing...";

                String dateNow = this.GetDateTime();
                String resultFilename = this.workingDirPath + "\\results" + "\\" + "result_" + dateNow + ".csv";
                String traceFilename = this.workingDirPath + "\\logs" + "\\" + "log_" + dateNow + ".txt";

                this.sA.SetTraceFilename(traceFilename);
                this.sG.SetTraceFilename(traceFilename);
                this.dut.SetTraceFilename(traceFilename);

                Testplan testplan = new Testplan(this.logging, traceFilename, this.sG, this.sA, this.dut);
                testplan.PopulateTests(workingDirPath + "\\" + configFilename);
                testplan.SetResults(this.richTextBoxDisplay, resultFilename);

                this.richTextBoxDisplay.Clear();
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
                this.buttonStart.Enabled = false;
                this.initialised = false;

                this.pathCal = this.checkBoxPathCal.Checked;
                this.logging = this.checkBoxLogging.Checked;
              
                String dateNow = this.GetDateTime();
                String traceFilename = this.workingDirPath + "\\logs" + "\\" + "Init_log_" + dateNow + ".txt";

                this.sA = new SpectrumAnalyser(this.textBoxSA.Text, "SA", this.demoMode, this.resetInstruments, this.logging, traceFilename);
                this.sG = new SignalGenerator(this.textBoxSG.Text, "SG", this.demoMode, this.resetInstruments, this.logging, traceFilename);
                this.dut = new Dut(this.textBoxDUT.Text, "DUT", this.demoMode, this.resetInstruments, this.logging, traceFilename);

                Testplan testplan = new Testplan(this.logging, traceFilename, this.sG, this.sA, this.dut);              
                testplan.PopulateTests(this.workingDirPath + "\\" + this.configFilename);

                this.textBoxManufacturer.Text = this.dut.Identity();
                this.textBoxModel.Text = this.dut.Identity();
                this.textBoxSerialNumber.Text = this.dut.Identity();

                this.richTextBoxDisplay.Clear();
                this.buttonStart.Enabled = true;
                this.initialised = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception");
            }
        }

        private String GetDateTime()
        {
            DateTime dt = DateTime.Now;
            return dt.ToString("ddMMyyyy") + "_" + dt.ToString("hhmmss");
        }
    }
}