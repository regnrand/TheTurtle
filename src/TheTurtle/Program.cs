// Copyright (C) 2010-2016 Randy Regnier. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.

using System;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Windows.Forms;
using Chorus.VcsDrivers.Mercurial;
using SIL.Reporting;
using SIL.Windows.Forms.HotSpot;
using TheTurtle.Properties;

namespace TheTurtle
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// This is a kludge to make sure we have a real reference to SIL.Windows.Forms.
			// Without this call, although SIL.Windows.Forms is listed in the References of this project,
			// since we don't actually use it directly, it does not show up when calling GetReferencedAssemblies on this assembly.
			// But we need it to show up in that list so that ExceptionHandler.Init can install the intended SIL.Windows.Forms
			// exception handler.
			using (new HotSpotProvider())
			{}

			if (TheTurtleSettings.Default.CallUpgrade)
			{
				TheTurtleSettings.Default.Upgrade();
				TheTurtleSettings.Default.CallUpgrade = false;
			}

			SetUpErrorHandling();

			// Is mercurial set up?
			var readinessMessage = HgRepository.GetEnvironmentReadinessMessage("en");
			if (!string.IsNullOrEmpty(readinessMessage))
			{
				MessageBox.Show(readinessMessage, Resources.kTheTurtle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			} 

			if (TheTurtleUtilities.FwAssemblyPath == null)
			{
				MessageBox.Show(Resources.kFlexNotFound, Resources.kTheTurtle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}

			using (var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly()))
			using (var container = new CompositionContainer(catalog))
			using (var turtleModel = container.GetExportedValue<Model.TheTurtle>())
			{
				Application.Run(turtleModel.MainWindow);
			}
			TheTurtleSettings.Default.Save();
		}

		private static void SetUpErrorHandling()
		{
			ErrorReport.EmailAddress = "rbregnier@gmail.com";
			ErrorReport.AddStandardProperties();
			ExceptionHandler.Init();
		}
	}
}
