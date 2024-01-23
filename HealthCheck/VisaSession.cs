using NationalInstruments.VisaNS;
using System.Text;

namespace RohdeSchwarz.Devices
{
    public class VisaSession
    {
        private MessageBasedSession mbSession = null;
        private String resourceString = String.Empty;
        private String scpiTraceFilename = String.Empty;
        private Boolean scpiLogging = false;
		private Boolean useStb = false;
		private String lastCommand = String.Empty;
        private String aliasName = String.Empty;
        private Boolean demoRun = false;

        private void WriteScpiTrace(String message, Boolean writeCommand)
        {
            try
            {
                if (this.scpiLogging)
                {
                    using (StreamWriter writer = new StreamWriter(this.scpiTraceFilename, true))
                    {
                        // 2015-12-23 23:02:01.112 [CMW] -> *IDN?
                        // 2015-12-23 23:02:01.212 [CMW] <- Virtual CMW

                        StringBuilder s = new StringBuilder(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo));
                        s.Append(" [" + this.aliasName + "]");
                        s.Append(writeCommand ? " -> " : " <- ");
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
            throw new Exception(this.aliasName + ": VisaSession Exception -> " + message);
        }

        public VisaSession(String resourceName, String aliasName, Boolean demoRun, Boolean scpiLogging, String traceFilename)
        {
            this.useStb = false;
            this.aliasName = aliasName;
            this.demoRun = demoRun;
            this.scpiLogging = scpiLogging;
            this.scpiTraceFilename = traceFilename;

            if (this.demoRun)
            {
                return;
            }

            try
            {
                this.mbSession = (MessageBasedSession)ResourceManager.GetLocalManager().Open(resourceString);
            }
            catch (InvalidCastException)
            {
                this.ThrowException("Unable to cast resource object to MessageBasedSession.");
            }
            catch (Exception ex)
            {
                this.ThrowException("Opening VISA resource " + resourceName.ToString() + "." + ex.Message);
            }
        }

        public void SetTraceFilename(String traceFilename)
        {
            this.scpiTraceFilename = traceFilename;
        }

        public void CheckStatus()
        {
            String result = String.Empty;
            String scpiError = String.Empty;

            if (this.useStb)
            {
                result = this.Query("*STB?", "0");

                if (result != "0")
                {
                    scpiError = this.Query("*SYST:ERR?", "0");
                    this.ThrowException(String.Format(
                        ": \"{0}\" failed with errorcode {1}, {2}", this.lastCommand, result, scpiError));
                }
            }
        }

		public void Close()
        {
            if (this.mbSession != null)
            {
                try
                {
                    this.mbSession.Dispose(); ;
                }
                catch (Exception)
                {
                    this.ThrowException("Closing VISA resource.");
                }
            }
        }

        public void Write(String command)
        {
            if (command.EndsWith("?"))
            {
                this.ThrowException("Use SCPI Query for quering instrument.");
            }

            this.lastCommand = command;
            this.WriteScpiTrace(command, true);

            if (this.demoRun)
            {
                return;
            }

            try
            {
                this.mbSession.Write(command);
            }
            catch (Exception ex)
            {
                this.ThrowException("Exception in write " + command + ". " + ex.Message);
            }
        }

        public String Read(String virtualResult)
        {
            String scpiResult = String.Empty;

            if (this.demoRun)
            {
                scpiResult = virtualResult;
            }
            else
            {
                try
                {
                    scpiResult = this.mbSession.ReadString();
                }
                catch (Exception ex)
                {
                    this.ThrowException("Exception in read. " + ex.Message);
                }
            }

            this.WriteScpiTrace(scpiResult, false);
            return scpiResult;
        }

		public String Query(String command, String virtualResult)
        {
            String scpiResult = String.Empty;

            if (!command.Contains("?"))
            {
                this.ThrowException("Not a query: " + command);
            }

            this.lastCommand = command;
            this.WriteScpiTrace(command, true);

            if (this.demoRun)
            {
                scpiResult = virtualResult;
            }
            else
            {
                try
                {
                    scpiResult = this.mbSession.Query(command);
                }
                catch (Exception ex)
                {
                    this.ThrowException("Exception in query " + command + ". " + ex.Message);
                }
            }

            this.WriteScpiTrace(scpiResult, false);
            return scpiResult;
        }

		public void WriteBinary(byte[] command)
        {
            this.ThrowException("The method or operation is not implemented.");
        }

		public byte[] ReadBinary()
        {
            byte[] result = new byte[1];
            this.ThrowException("The method or operation is not implemented.");

            return result;
        }

		public byte[] QueryBinary(String command)
        {
            byte[] result = new byte[1];
            this.ThrowException("The method or operation is not implemented.");

            return result;
        }

		public void WaitOpc()
        {
            this.Query("*OPC?", "0");
        }

		public void Reset()
        {
            this.Query("*CLS;*RST;*OPC?", "0");
        }
    }
}


