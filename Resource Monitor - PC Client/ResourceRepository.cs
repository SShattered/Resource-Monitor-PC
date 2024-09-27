using LibreHardwareMonitor.Hardware;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Resource_Monitor___PC_Client
{
    public class ResourceRepository
    {
        static float cpuTemp;
        static float cpuUsage;
        static float cpuPowerDrawPackage;
        static float cpuFrequency;
        static float gpuTemp;
        static float gpuUsage;
        static float gpuCoreFrequency;
        static float gpuMemoryFrequency;
        static float ramUsage;
        static float vRamUsage;
        static float gpuPower;
        
        Computer computer;
        public ResourceRepository()
        {
            computer = new Computer()
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true
            };

            computer.Open();
            computer.Accept(new UpdateVisitor());
        }

        public void Close()
        {
            computer.Close();
        }

        public void GetSystemInfo()
        {
            foreach (var hardware in computer.Hardware)
            {

                if (hardware.HardwareType == HardwareType.Cpu)
                {
                    hardware.Update();
                    foreach (var sensor in hardware.Sensors)
                    {
                        //Debug.WriteLine(sensor.Name + "-" + sensor.SensorType + "-" + sensor.Value);
                        if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("Core (Tctl/Tdie)"))
                        {
                            cpuTemp = sensor.Value.GetValueOrDefault();
                            //Debug.WriteLine(cpuTemp);
                        }
                        else if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("CPU Total"))
                        {
                            cpuUsage = sensor.Value.GetValueOrDefault();
                        }
                        else if (sensor.SensorType == SensorType.Power && sensor.Name.Contains("Package"))
                        {
                            cpuPowerDrawPackage = sensor.Value.GetValueOrDefault();
                        }
                        else if (sensor.SensorType == SensorType.Clock && sensor.Name.Contains("Core #1"))
                        {
                            cpuFrequency = sensor.Value.GetValueOrDefault();
                            // Debug.WriteLine(cpuFrequency);
                        }
                    }
                }

                if (hardware.HardwareType == HardwareType.Memory)
                {
                    hardware.Update();
                    foreach (var sensor in hardware.Sensors)
                    {
                        Debug.WriteLine(sensor.Name + "-" + sensor.SensorType + "-" + sensor.Value);
                        if (sensor.SensorType == SensorType.Data && sensor.Name.Equals("Memory Used"))
                            ramUsage = sensor.Value.GetValueOrDefault(); ;
                    }
                }


                // GPUs
                if (hardware.HardwareType == HardwareType.GpuAmd || hardware.HardwareType == HardwareType.GpuNvidia)
                {
                    hardware.Update();

                    foreach (var sensor in hardware.Sensors)
                    {
                        //Debug.WriteLine(sensor.Name + "-" + sensor.SensorType + "-" + sensor.Value);
                        if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("GPU Core"))
                        {
                            gpuTemp = sensor.Value.GetValueOrDefault();
                        }
                        else if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("GPU Core"))
                        {
                            gpuUsage = sensor.Value.GetValueOrDefault();
                        }
                        else if (sensor.SensorType == SensorType.Clock && sensor.Name.Contains("GPU Core"))
                        {
                            gpuCoreFrequency = sensor.Value.GetValueOrDefault();
                        }
                        else if (sensor.SensorType == SensorType.Clock && sensor.Name.Contains("GPU Memory"))
                        {
                            gpuMemoryFrequency = sensor.Value.GetValueOrDefault();
                        }
                        else if (sensor.SensorType == SensorType.SmallData && sensor.Name.Contains("GPU Memory Used"))
                        {
                            vRamUsage = sensor.Value.GetValueOrDefault();
                        }
                        else if (sensor.SensorType == SensorType.Power && sensor.Name.Contains("GPU Package"))
                        {
                            gpuPower = sensor.Value.GetValueOrDefault();
                        }
                    }
                }
            }
        }

        public void Poll()
        {
            GetSystemInfo();
        }

        public string GetDashboardJson()
        {
            var model = new DashboardModel()
            {
                CpuUsage = cpuUsage,
                GpuUsage = gpuUsage,
                CpuTemperature = cpuTemp,
                GpuTemperature = gpuTemp,
                Frequency = cpuFrequency,
                RamUsage = ramUsage,
                VRamUsage = vRamUsage,
                GpuPower = gpuPower
            };
            return JsonConvert.SerializeObject(model);
        }
    }
}
