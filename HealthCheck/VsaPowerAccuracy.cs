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

        private Double measuredResult = 0;
        private Boolean passResult = false;

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

        public void SetParamters(Double freqMHz, Double inputPowerDBm, Double referenceLevelDBm, Double lowLimitDBm, Double highLimitDBm, String rxPortName)
        {
            this.freqMHz = freqMHz;
            this.inputPowerDBm = inputPowerDBm;
            this.referenceLevelDBm = referenceLevelDBm;            
            this.lowLimitDBm = this.inputPowerDBm + lowLimitDBm;
            this.highLimitDBm = this.inputPowerDBm + highLimitDBm;
            this.rxPortName = rxPortName;
        }

        public void Run(ref String textPrint, ref String textSaved)
        {
            Double measuredPower = 0;

            // Set DUT
            this.WriteTrace("Setting DUT Rx port: " + this.rxPortName);
            this.dut.SetRxPort(this.rxPortName);
            this.WriteTrace("Setting DUT reference level dBm: " + this.referenceLevelDBm);
            this.dut.SetRFLevel(this.referenceLevelDBm);
            this.WriteTrace("Setting DUT output frequency MHz: " + this.freqMHz);
            this.dut.SetFrequency(this.freqMHz);

            // Set SG 
            this.WriteTrace("Setting SG output power dBm: " + this.inputPowerDBm);
            this.sG.SetRFLevel(this.inputPowerDBm);
            this.WriteTrace("Setting SG output frequency MHz: " + this.freqMHz);
            this.sG.SetFrequency(this.freqMHz);

            // Measure DUT RF level
            this.WriteTrace("Reading DUT measured power dBm: " + measuredPower);
            this.dut.GetMeasuredPower(measuredPower);            

            // Results
            this.measuredResult = measuredPower;
            this.passResult = (measuredResult < this.lowLimitDBm || measuredResult > this.highLimitDBm) ? false : true;
            this.WriteTrace("Measured: " + this.measuredResult + ". Result: " +  ((this.passResult)? "Pass" : "Fail" ));

            String settings = "Freq:" + this.freqMHz + "MHz, Power:" + this.inputPowerDBm + "dBm, Ref:" + this.referenceLevelDBm + "dBm";
            StringBuilder s = new StringBuilder(" " + this.testName + "\t" + settings + "\t\t\t" + this.lowLimitDBm + "\t\t" + this.highLimitDBm + "\t\t");
            s.AppendLine(this.measuredResult + "\t\t" + this.passResult);

            textPrint = s.ToString();
        } 
        

    }
}
