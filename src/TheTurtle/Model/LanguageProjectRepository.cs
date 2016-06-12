﻿// Copyright (C) 2010-2016 Randy Regnier. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using TheTurtle.Properties;

namespace TheTurtle.Model
{
	/// <summary>
	/// A repository that gets the available FieldWorks projects on a computer
	/// as represented by the class LanguageProject
	/// (not the FielwdWorks internal language project class).
	/// </summary>
	[Export(typeof(LanguageProjectRepository))]
	internal sealed class LanguageProjectRepository
	{
		private readonly HashSet<LanguageProject> _projects = new HashSet<LanguageProject>();

		[ImportingConstructor]
		internal LanguageProjectRepository(IProjectPathLocator pathLocator)
		{
			if (pathLocator == null)
				throw new ArgumentNullException("pathLocator");
			if (pathLocator.BaseFolderPaths.Count == 0)
				throw new ArgumentOutOfRangeException("pathLocator", Resources.kNoPathsGiven);

			var baseFolderPaths = pathLocator.BaseFolderPaths;
			foreach (var fwdataFiles in
				baseFolderPaths.SelectMany(baseFolderPath => Directory.
					GetDirectories(baseFolderPath).
					Select(dir => Directory.
						GetFiles(dir, "*" + TheTurtleUtilities.FwXmlExtension)).
						Where(fwdataFiles => fwdataFiles.Length > 0)))
			{
				_projects.Add(new LanguageProject(fwdataFiles[0]));
			}
		}

		/// <summary>
		/// Return all of the FieldWorks projects on a computer.
		/// </summary>
		internal IList<LanguageProject> AllLanguageProjects
		{
			get { return _projects.ToList(); }
		}

		internal LanguageProject GetProject(string projectName)
		{
			return (from project in _projects
					where project.Name == projectName
			        select project).FirstOrDefault();
		}
	}
}