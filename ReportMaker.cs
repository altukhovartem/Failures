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

    public class ListOfFailure
    {
        public List<Failure> listOfFailured = null;
        public int Length { get; private set; }

        public ListOfFailure()
        {
            listOfFailured = new List<Failure>();
        }

        public Failure this[int index]
        {
            set
            {
                listOfFailured.Add(new Failure(value.failure, value.device, value.date));
                Length++;
            }
            get
            {
                return listOfFailured[index];
            }
        }

        

    }

    public class Device
    {
        public Dictionary<string, object> dictionaryOfDevices = null;

        public Device()
        {
            dictionaryOfDevices = new Dictionary<string, object>();
        }

        public KeyValuePair<string, object> this[int index]
        {
            set
            {
                dictionaryOfDevices.Add(value.Key, value.Value);
            }
            get
            {
                foreach (var item in dictionaryOfDevices)
                {
                    if (item.Key == index.ToString())
                    {
                        return new KeyValuePair<string, object>(item.Key, item.Value);
                    }
                }
                throw new Exception("Нет такого девайса");
            }
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
            ListOfFailure failures = new ListOfFailure();

            for (int i = 0; i < failureTypes.Length; i++)
            {

                Failure currentFailure = new Failure
                (
                    (FailureTypes)failureTypes[i],
                    deviceId[i],
                    new DateTime(Convert.ToInt32(times[i][2]), Convert.ToInt32(times[i][1]), Convert.ToInt32(times[i][0]))
                );
                failures[i] = currentFailure;
            }
            //////////////////////////////////////////////////////////////
            Device listOfDevices = new Device();
            for (int i = 0; i < devices.Count; i++)
            {
                KeyValuePair<string, object> pair = new KeyValuePair<string, object>(devices[i].Values.First().ToString(), devices[i].Values.Last());
                listOfDevices[i] = pair; 
            }
            //////////////////////////////////////////////////////////////

            Device issueDevices = new Device();
            int j = 0;
            for (int i = 0; i < failures.Length; i++)
            {
                Failure currentFailure = failures[i];
                if (Failure.IsFailureSerious((int)currentFailure.failure) && currentFailure.Earlier(currentDatetime))
                {
                    issueDevices[j] = listOfDevices[1];
                    j++;
                } 
            }   

            return issueDevices.dictionaryOfDevices.Values;
        }

        public static void FindDevicesFailedBeforeDate(DateTime currentDate, Failure failures)
        {
            
        }



    }
}
