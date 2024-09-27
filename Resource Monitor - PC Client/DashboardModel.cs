using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Resource_Monitor___PC_Client
{
    class DashboardModel
    {
        [JsonProperty("cpuusage")]
        public double CpuUsage { get; set; }
        [JsonProperty("frequency")]
        public double Frequency { get; set; }
        [JsonProperty("cputemperature")]
        public double CpuTemperature { get; set; }
        [JsonProperty("ramusage")]
        public double RamUsage { get; set; }
        [JsonProperty("gpuusage")]
        public double GpuUsage { get; set; }
        [JsonProperty("vramusage")]
        public double VRamUsage { get; set; }
        [JsonProperty("gputemperature")]
        public double GpuTemperature { get; set; }
        [JsonProperty("gpupower")]
        public double GpuPower { get; set; }
    }
}
