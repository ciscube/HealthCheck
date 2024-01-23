using HealthCheck.Devices;
using System.Text;

namespace HealthCheck
{
    internal class VsaPowerAccuracy
    {
        private String testName = "Vsa Power Accuracy";
        private Double freqMHz = 0;
        private Double inputPowerDBm = 0;
        private Double referenceLevelDBm = 0;
        private Double highLimitDBm = 0;
        private Double lowLimitDBm = 0;
        private String rxPortName = String.Empty;

        private SpectrumAnalyser sA;
        private SignalGenerator sG;
        private Dut dut;

        private String traceFilename = String.Empty;
        private Boolean logging = false;        

        private void WriteTrace(String message)
        {
            try
            {
                if (this.logging)
                {
                    using (StreamWriter writer = new StreamWriter(this.traceFilename, true))
                    {
                        StringBuilder s = new StringBuilder(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo));
                        s.Append(" [" + this.testName + "] -> ");
                        s.Append(message);
                        writer.WriteLine(s);
                        writer.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                this.ThrowException("Writing to trace. " + ex.Message);
            }
        }

        private void ThrowException(String message)
        {
            throw new Exception(this.testName + ": Test Exception -> " + message);
        }

        public VsaPowerAccuracy(Boolean logging, String traceFilename)
        {
            this.traceFilename = traceFilename;
            this.logging = logging;
        }

        public void SetInstruments(SignalGenerator sG, SpectrumAnalyser sA, Dut dut)
        {
            this.sG = sG;
            this.sA = sA;
            this.dut = dut;
        }

        public void SetParamters(Double freqMHz, Double inputPowerDBm, Double referenceLevelDBm, Double highLimitDBm, Double lowLimitDBm, String rxPortName)
        {
            this.freqMHz = freqMHz;
            this.inputPowerDBm = inputPowerDBm;
            this.referenceLevelDBm = referenceLevelDBm;
            this.highLimitDBm = highLimitDBm;
            this.lowLimitDBm = lowLimitDBm;
            this.rxPortName = rxPortName;
        }

        public void Run(Boolean passResult, Double measuredResult)
        {
            Double measuredPower = 0;

            // Set DUT RX Port, Reference level
            this.dut.SetRxPort(this.rxPortName);
            this.dut.SetRFLevel(this.referenceLevelDBm);

            // Set SG RF level, Frequency
            this.sG.SetRFLevel(this.inputPowerDBm);
            this.sG.SetFrequency(this.freqMHz);

            // Measure DUT RF level
            this.dut.GetMeasuredPower(measuredPower);

            // Results
        }        
    }
}
