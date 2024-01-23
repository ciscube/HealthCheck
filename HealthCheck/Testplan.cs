using HealthCheck.Devices;
using System.Text;

namespace HealthCheck
{
    internal class Testplan
    {
        private String testNameVsaPowerAccuracy = "Vsa Power Accuracy";
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
            test.SetParamters(Double.Parse(items[1]), Double.Parse(items[2]), Double.Parse(items[3]), Double.Parse(items[1]), Double.Parse(items[5]), portName);
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
                            if (line == "Test_End") 
                                break;
                            else if (line.Contains("Ports="))
                                portName = line.Substring("Test_End".Length);
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
            Boolean passResult = false;
            Double measuredResult = 0;

            foreach(var item in this.vsaPowerAccuracyList)
            {
                item.Run(passResult, measuredResult);
            }
        }
    }
}
