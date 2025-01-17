
using System;

namespace ConsoleApp1

{
    public static class Program
    {
        static void Main(string[] args)
        {
            Test(new[] { "12:12:12" }, new[] { "few seconds ago" }, "12:12:12");
            Test(new[] { "23:23:23", "23:23:23" }, new[] { "59 minutes ago", "59 minutes ago" }, "00:22:23");
            Test(new[] { "00:10:10", "00:10:10" }, new[] { "59 minutes ago", "1 hours ago" }, "impossible");
            Test(new[] { "11:59:13", "11:13:23", "12:25:15" }, new[] { "few seconds ago", "46 minutes ago", "23 hours ago" }, "11:59:23");
            Console.ReadKey(true);
        }

        private static void Test(string[] postTimes, string[] showTimes, string expected)
        {
            var result = GetCurrentTime(postTimes, showTimes).Equals(expected) ? "PASS" : "FAIL";
            var postTimesCsv = string.Join(", ", postTimes);
            var showTimesCsv = string.Join(", ", showTimes);
            Console.WriteLine($"[{postTimesCsv}], [{showTimesCsv}] => {result}");
        }



        public static string GetCurrentTime(string[] exactPostTime, string[] showPostTime)
        {
            //validations
            if (exactPostTime.Length < 0 && exactPostTime.Length > 50)
            {
                return "exactPostTime will contain between 0 and 50 elements, inclusive.";
            }

            foreach (string timeString in exactPostTime){
                if (!DateTime.TryParse(timeString, out DateTime result)) return "incorrect time format or incorrect time";
            }

            if(exactPostTime.Length != showPostTime.Length)
            {
                return "exactPostTime and showPostTime will contain same number of elements.";
            }

            foreach(string postTimeString in showPostTime)
            {
                if (!validateString(postTimeString)) return "invalid format of postTime";
            }






            //checking the condition whether there is a comflict in two post's showPostTime
            for (int i = 0; i < exactPostTime.Length; i++)
            {
                for (int j = i + 1; j < exactPostTime.Length; j++)
                {
                    if (exactPostTime[i] == exactPostTime[j])
                        if (showPostTime[i] != showPostTime[j])
                            return "impossible";
                }
            }

            //xx:xx:xx + time_span = candidateString
            string[] candidateStrings = new string[exactPostTime.Length];

            //taking every string from exactPostTime_string_array one at a time,
            //and calculate candidateString generated from that string and add it to candidateString array
            for (int i = 0; i < exactPostTime.Length; i++)
            {

                //1996,25,7 is just any date which will be same for every time_string.
                string[] split_hour_min_sec = exactPostTime[i].Split(":");
                DateTime dateTime = new DateTime(1996, 7, 25, Int32.Parse(split_hour_min_sec[0]), Int32.Parse(split_hour_min_sec[1]), Int32.Parse(split_hour_min_sec[2]));


                //checking what should be added to the time(raw_exact_time)
                if (showPostTime[i].Contains("seconds"))
                {

                    candidateStrings[i] = exactPostTime[i];

                }
                else if (showPostTime[i].Contains("minutes"))
                {

                    string minutes = showPostTime[i].Split(" ")[0];                //taking that x minute
                    TimeSpan timeSpan = new TimeSpan(0, Int32.Parse(minutes), 0);  //creating a timeSpan object, stores above x minutes
                    candidateStrings[i] = dateTime.Add(timeSpan).ToString().Split(" ")[1];  // adding the timespan to input_raw_exactTime and spliting as well

                }
                else if (showPostTime[i].Contains("hours"))
                {

                    string hours = showPostTime[i].Split(" ")[0];
                    TimeSpan timeSpan = new TimeSpan(Int32.Parse(hours), 0, 0);
                    candidateStrings[i] = dateTime.Add(timeSpan).ToString().Split(" ")[1];

                }
            }

            Array.Sort(candidateStrings);
            return candidateStrings[(exactPostTime.Length - 1)];



        }

        private static bool validateString(string postTimeString)
        {
            string[] arr = postTimeString.Split(" ");
            if (arr.Length > 3) return false;
            else if (arr[2] != "ago") return false;
            else if (arr[1] != "minutes" || arr[1] != "seconds" || arr[1] != "hours") return false;
            
            if(arr[1] == "seconds")
            {
                int.TryParse(arr[0], out int noOfSeconds);
                if (noOfSeconds! <= 59 && noOfSeconds! >= 0) return false;
            }

            if (arr[1] == "minutes")
            {
                int.TryParse(arr[0], out int noOfMinutes);
                if (noOfMinutes! <= 59 && noOfMinutes! >= 0) return false;
            }

            if (arr[1] == "hours")
            {
                int.TryParse(arr[0], out int noOfHours);
                if (noOfHours! <= 23 && noOfHours! >= 0) return false;
            }            

        }
    }
}
