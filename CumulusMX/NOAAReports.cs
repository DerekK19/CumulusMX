﻿using System;
using System.Collections.Generic;
using System.IO;

namespace CumulusMX
{
	public class NOAAReports
	{
		private readonly Cumulus cumulus;
		//private WeatherStation station;
		private List<string> report;
		//private string[] report;
		private string noaafile;

		public NOAAReports(Cumulus cumulus)
		//public NOAAReports()
		{
			this.cumulus = cumulus;
			//this.station = station;
		}

		public List<string> GenerateNoaaYearReport(int year)
		{
			NOAA noaa = new NOAA(cumulus);
			DateTime noaats = new DateTime(year, 1, 1);

			cumulus.LogMessage("Creating NOAA yearly report");
			report = noaa.CreateYearlyReport(noaats);
			// If not using UTF, then we have to convert the character set
			var utf8WithoutBom = new System.Text.UTF8Encoding(false);
			var encoding = cumulus.NOAAUseUTF8 ? utf8WithoutBom : System.Text.Encoding.GetEncoding("iso-8859-1");
			var reportName = noaats.ToString(cumulus.NOAAYearFileFormat);
			noaafile = cumulus.ReportPath + reportName;
			cumulus.LogMessage("Saving yearly NOAA report as " + noaafile);
			File.WriteAllLines(noaafile, report, encoding);

			return report;
		}

		public List<string> GenerateNoaaMonthReport(int year, int month)
		{
			NOAA noaa = new NOAA(cumulus);
			DateTime noaats = new DateTime(year, month, 1);

			cumulus.LogMessage("Creating NOAA monthly report");
			var report = noaa.CreateMonthlyReport(noaats);
			// If not using UTF, then we have to convert the character set
			var utf8WithoutBom = new System.Text.UTF8Encoding(false);
			var encoding = cumulus.NOAAUseUTF8 ? utf8WithoutBom : System.Text.Encoding.GetEncoding("iso-8859-1");
			var reportName = noaats.ToString(cumulus.NOAAMonthFileFormat);
			noaafile = cumulus.ReportPath + reportName;
			cumulus.LogMessage("Saving monthly NOAA report as " + noaafile);
			File.WriteAllLines(noaafile, report, encoding);

			return report;
		}

		public List<string> GetNoaaYearReport(int year)
		{
			DateTime noaats = new DateTime(year, 1, 1);

			try
			{
				var reportName = noaats.ToString(cumulus.NOAAYearFileFormat);
				noaafile = cumulus.ReportPath + reportName;
				report = File.Exists(noaafile) ? new List<string>(File.ReadAllLines(noaafile)) : new List<String> { "That report does not exist" };
			}
			catch
			{
				report = new List<string> { "Something went wrong!" };
			}
			return report;
		}

		public List<string> GetNoaaMonthReport(int year, int month)
		{
			DateTime noaats = new DateTime(year, month, 1);

			try
			{
				var reportName = noaats.ToString(cumulus.NOAAMonthFileFormat);
				noaafile = cumulus.ReportPath + reportName;
				report = File.Exists(noaafile) ? new List<string> (File.ReadAllLines(noaafile)) : new List<string> { "That report does not exist" };
			}
			catch
			{
				report = new List<string> { "Something went wrong!" };
			}
			return report;
		}
	}
}
