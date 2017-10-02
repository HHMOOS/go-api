﻿using System;
using System.Linq;
using GoApi;
using GoApi.Core;
using GoApi.TimeTracking;

namespace TimeTracking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var authorizationSettings = new AuthorizationSettings
            {
                ApplicationKey = "<You Application Key Here>",
                ClientKey = "<PowerOffice Go Client Key Here>",
                TokenStore = new BasicTokenStore(@"my.tokenstore")
            };

            // Initialize the PowerOffice Go API and request authorization
            var api = new Go(authorizationSettings);

            //Get and print out all hour types available on the Client
            Console.WriteLine("Hour types:");
            var hourTypes = api.TimeTracking.HourType.Get().ToArray();
            foreach (var hourType in hourTypes)
            {
                Console.WriteLine($"{hourType.Description} | PayItemCode: {hourType.PayItemCode} | IsActive: {hourType.IsActive}");
            }

            Console.WriteLine();

            //Get and print all Activities available on the Client.
            Console.WriteLine("Activities:");
            var activities = api.TimeTracking.Activity.Get().ToArray();
            foreach (var activity in activities)
            {
                Console.WriteLine($"{activity.Name} ({activity.Code}) | Type: {activity.ActivityType} | IsActive: {activity.IsActive}");
            }

            Console.WriteLine();

            //Query time tracking entries this month
            Console.WriteLine("Time tracking entries:");
            var currentDate = DateTime.Today;
            var monthStart = new DateTime(currentDate.Year, currentDate.Month, 1);
            var timeTrackingEntries = api.TimeTracking.TimeTrackingEntry.Get().Where(t => t.Date >= monthStart).OrderBy(t => t.Date).ToArray();
            foreach (var timeTrackingEntry in timeTrackingEntries)
            {
                Console.WriteLine($"{timeTrackingEntry.EmployeeCode} - {timeTrackingEntry.ActivityCode} - {timeTrackingEntry.Date} - {timeTrackingEntry.Hours}");
            }

            Console.WriteLine();

            //Get entry for a given employee, project, activity and hour type, or create new with the same dimensions
            var date = new DateTime(2017, 7, 3);
            var employeeCode = 1;
            var activityCode = "802";
            var projectCode = "2";
            var hourTypeToUse = "Overtid 100 %";
            var numberOfHours = 12;


            var timeTrackingEntryToEdit =
                timeTrackingEntries.FirstOrDefault(t => t.Date == date && t.EmployeeCode == employeeCode && t.ActivityCode == activityCode && t.ProjectCode == projectCode && t.HourType == hourTypeToUse);

            if (timeTrackingEntryToEdit != null)
            {
                timeTrackingEntryToEdit.Hours += numberOfHours;
                timeTrackingEntryToEdit.Comment = "A comment from edit";
            }
            else
            {
                timeTrackingEntryToEdit = new TimeTrackingEntry()
                {
                    ActivityCode = activityCode,
                    Date = date,
                    Comment = "A comment",
                    Hours = numberOfHours,
                    EmployeeCode = employeeCode,
                    ProjectCode = projectCode,
                    HourType = hourTypeToUse
                };
            }

            //Save the time tracking entry
            api.TimeTracking.TimeTrackingEntry.Save(timeTrackingEntryToEdit);

            //Query again and print
            Console.WriteLine("Time tracking entries:");
            timeTrackingEntries = api.TimeTracking.TimeTrackingEntry.Get().Where(t => t.Date >= monthStart).OrderBy(t => t.Date).ToArray();
            foreach (var timeTrackingEntry in timeTrackingEntries)
            {
                Console.WriteLine($"{timeTrackingEntry.EmployeeCode} - {timeTrackingEntry.ActivityCode} - {timeTrackingEntry.Date} - {timeTrackingEntry.Hours}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();


        }
    }
}
