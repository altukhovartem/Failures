using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incapsulation.Failures
{
    public enum FailureTypes
    {
        Unexpected = 0,
        Short,
        Hardware,
        Connection
    }

    public class Failure
    {
        public FailureTypes failure;
        public int device;
        public DateTime date;

        public Failure(FailureTypes failure, int device, DateTime date)
        {
            this.failure = (FailureTypes)failure;
            this.device = device;
            this.date = date;
        }

        public static bool IsFailureSerious(int failureType)
        {
            if (failureType % 2 == 0)
                return true;
            else
                return false;
        }

        public bool Earlier(DateTime currentDatetime)
        {

            if (this.date < currentDatetime)
                return true;
            else
                return false;
        }
    }
    

    public class Device
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public Device(int id, string name)
        {
            this.ID = id;
            this.Name = name;
        }
    }


    public class Common
    {
        public static int IsFailureSerious(int failureType)
        {
            if (failureType%2==0) return 1;
            return 0;
        }


        public static int Earlier(object[] v, int day, int month, int year)
        {
            int vYear = (int)v[2];
            int vMonth = (int)v[1];
            int vDay = (int)v[0];
            if (vYear < year) return 1;
            if (vYear > year) return 0;
            if (vMonth < month) return 1;
            if (vMonth > month) return 0;
            if (vDay < day) return 1;
            return 0;
        }
    }

    public class ReportMaker
    {
        /// <summary>
        /// </summary>
        /// <param name="day"></param>
        /// <param name="failureTypes">
        /// 0 for unexpected shutdown, 
        /// 1 for short non-responding, 
        /// 2 for hardware failures, 
        /// 3 for connection problems
        /// </param>
        /// <param name="deviceId"></param>
        /// <param name="times"></param>
        /// <param name="devices"></param>
        /// <returns></returns>
        public static List<string> FindDevicesFailedBeforeDateObsolete(
            int day,
            int month,
            int year,
            int[] failureTypes, 
            int[] deviceId, 
            object[][] times,
            List<Dictionary<string, object>> devices)
        {

            var problematicDevices = new HashSet<int>();
            for (int i = 0; i < failureTypes.Length; i++)
                if (Common.IsFailureSerious(failureTypes[i]) == 1 && Common.Earlier(times[i], day, month, year) == 1)
                    problematicDevices.Add(deviceId[i]);

            var result = new List<string>();
            foreach (var device in devices)
                if (problematicDevices.Contains((int)device["DeviceId"]))
                    result.Add(device["Name"] as string);

            //////////////////////////////////////////////////////////////
            DateTime currentDatetime = new DateTime(year, month, day);
            //////////////////////////////////////////////////////////////
            List<Failure> failures = new List<Failure>();

            for (int i = 0; i < failureTypes.Length; i++)
            {
                Failure currentFailure = new Failure
                (
                    (FailureTypes)failureTypes[i],
                    deviceId[i],
                    new DateTime(Convert.ToInt32(times[i][2]), Convert.ToInt32(times[i][1]), Convert.ToInt32(times[i][0]))
                );
                failures.Add(currentFailure);
            }
            //////////////////////////////////////////////////////////////
            List<Device> listOfDevices = new List<Device>(); 
            for (int i = 0; i < devices.Count; i++)
            {
                Device currentDevice = new Device(Convert.ToInt32(devices[i].Values.First()), devices[i].Values.Last().ToString());
                listOfDevices.Add(currentDevice);
            }
            //////////////////////////////////////////////////////////////

            return FindDevicesFailedBeforeDate(currentDatetime, failures, listOfDevices);

            //List<string> resultX = new List<string>();
            //for (int i = 0; i < failures.Count; i++)
            //{
            //    if (Failure.IsFailureSerious(Convert.ToInt32(failures[i].failure)) && failures[i].Earlier(currentDatetime))
            //    {
            //        foreach (var item in listOfDevices)
            //        {
            //            if (item.ID == failures[i].device)
            //                resultX.Add(item.Name);
            //        }
            //    }
            //}

            //return resultX;
        }

        public static List<string> FindDevicesFailedBeforeDate(DateTime currentDatetime, List<Failure> failures, List<Device> listOfDevices)
        {

            List<string> resultX = new List<string>();
            for (int i = 0; i < failures.Count; i++)
            {
                if (Failure.IsFailureSerious(Convert.ToInt32(failures[i].failure)) && failures[i].Earlier(currentDatetime))
                {
                    foreach (var item in listOfDevices)
                    {
                        if (item.ID == failures[i].device)
                            resultX.Add(item.Name);
                    }
                }
            }

            return resultX;
        }



    }
}
