using HealthCheck.Devices;
using System.Text;

namespace HealthCheck
{
    internal class Testplan
    {
        private String testNameVsaPowerAccuracy = "VSA Power Accuracy";
        private List<VsaPowerAccuracy> vsaPowerAccuracyList = new List<VsaPowerAccuracy>();

        private SpectrumAnalyser sA = null;
        private SignalGenerator sG = null;
        private Dut dut = null;

        private RichTextBox displayPanel = null;
        private String resultFilename = String.Empty;

        private String traceFilename = String.Empty;
        private Boolean logging = false;
        
        private VsaPowerAccuracy PopulateVsaPowerAccuracy(String line, String portName)
        {
            // Points_MHz_InputdBm_RefdBm_LL_HL
            String[] items = line.Split(",");

            VsaPowerAccuracy test = new VsaPowerAccuracy(this.logging, this.traceFilename);
            test.SetInstruments(this.sG, this.sA, this.dut);
            test.SetParamters(Double.Parse(items[1]), Double.Parse(items[2]), Double.Parse(items[3]), Double.Parse(items[4]), Double.Parse(items[5]), portName);
            return test;
        }

        public Testplan(Boolean logging, String traceFilename, SignalGenerator sG, SpectrumAnalyser sA, Dut dut)
        {
            this.traceFilename = traceFilename;
            this.logging = logging;

            this.sG = sG;
            this.sA = sA;
            this.dut = dut;
        }

        public void SetResults(RichTextBox displayPanel, String resultFilename)
        {
            this.displayPanel = displayPanel;
            this.resultFilename = resultFilename;
        }

        public void PopulateTests(String configFilename)
        {
            StreamReader file = new StreamReader(configFilename);
            String line = String.Empty;
            String portName = String.Empty;

            try
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains(this.testNameVsaPowerAccuracy))
                    {
                        while ((line = file.ReadLine()) != null)
                        {
                            if (line.Contains("Test_End")) 
                                break;
                            else if (line.Contains("Ports="))
                            {
                                String[] items = line.Split(",");
                                portName = items[0].Substring("Ports=".Length);
                            }
                                
                            else
                                this.vsaPowerAccuracyList.Add(this.PopulateVsaPowerAccuracy(line, portName));
                        }
                    }
                    else
                    {
                    }
                }                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            file.Close();
        }

        public void Run()
        {
            String textPrint = String.Empty;
            String textSaved = String.Empty;

            this.displayPanel.SelectionTabs = new int[] {90, 180, 270, 360};
            this.displayPanel.AppendText(" " + "Test" + "\t\t" + "Settings" + "\t\t\t\t\t\t" + "LLimit" + "\t\t" +"HLimit" + "\t\t" + "Measured" + "\t" + "Verdict" + "\n");

            foreach (var item in this.vsaPowerAccuracyList)
            {
                item.Run(ref textPrint, ref textSaved);
                this.displayPanel.AppendText(textPrint);
                this.displayPanel.Refresh();
            }
        }
    }
}
