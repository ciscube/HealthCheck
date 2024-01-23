using RohdeSchwarz.Devices;

namespace HealthCheck.Devices
{
    internal class SignalGenerator
    {
        private String errorMessage = String.Empty;
        private VisaSession session = null;
        private String identity = String.Empty;
        private String option = String.Empty;

        private Double currentRFLevelDBm = 0;
        private Double currentFreqMHz = 0;

        public SignalGenerator(String resourceName, String aliasName, Boolean demoMode, Boolean reset, Boolean scpiLogging, String traceFilename)
        {
            this.errorMessage = String.Empty;
            this.session = new VisaSession(resourceName, aliasName, demoMode, scpiLogging, traceFilename);
            this.identity = this.session.Query("*IDN?", "I'm a virtual signal generator");
            this.option = this.session.Query("*OPT?", "B1");

            if (reset)
            {
                this.session.Reset();
            }
        }

        public void SetTraceFilename(String tracefilename)
        {
            this.session.SetTraceFilename(tracefilename);
        }

        public String ErrorMessage()
        {
            return this.errorMessage;
        }

        public void CheckStatus()
        {
            this.session.CheckStatus();
        }

        public String Identity() 
        { 
            return this.identity; 
        }

        public void Close()
        {
            this.session.Close();
        }

        public void SetRFLevel(Double level_dBm)
        {
            this.session.Write("DISPlay:TRACe1 ON");
            this.session.Write("DISPlay:TRACe1:MODE MAXHold");
        }

        public void SetFrequency(Double freq_MHz)
        {
            this.session.Write("DISPlay:TRACe1 ON");
            this.session.Write("DISPlay:TRACe1:MODE MAXHold");
        }
    }
}
