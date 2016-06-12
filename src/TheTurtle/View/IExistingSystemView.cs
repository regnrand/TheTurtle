// Copyright (C) 2010-2016 Randy Regnier. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.

using Chorus;
using Chorus.UI.Sync;
using TheTurtle.Model;

namespace TheTurtle.View
{
	internal interface IExistingSystemView
	{
		void SetSystem(ChorusSystem chorusSystem, LanguageProject project);
		void UpdateDisplay(bool projectIsInUse);
		SyncControlModel Model { get; }
		bool Enabled { get; set; }
	}
}