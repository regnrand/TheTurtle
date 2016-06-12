// Copyright (C) 2010-2016 Randy Regnier. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.

using System.Collections.Generic;
using TheTurtle.Model;

namespace TheTurtle.View
{
	internal interface ITurtleView
	{
		event ProjectSelectedEventHandler ProjectSelected;

		void SetProjects(IList<LanguageProject> allLanguageProjects, LanguageProject currentLanguageProject);
		IProjectView ProjectView { get; }
		void EnableSendReceiveControls(bool makeWarningsVisible);
	}
}