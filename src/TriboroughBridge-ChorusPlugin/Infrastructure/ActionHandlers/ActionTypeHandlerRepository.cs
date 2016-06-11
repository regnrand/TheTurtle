// --------------------------------------------------------------------------------------------
// Copyright (C) 2010-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace TriboroughBridge_ChorusPlugin.Infrastructure.ActionHandlers
{
	/// <summary>
	/// This is the central repository (collection) of IBridgeActionTypeHandler implementations.
	///
	/// When an implementation of the IBridgeActionTypeHandler is marked for export. MEF then makes sure
	/// it is included in this class.
	///
	/// Depending on the command line option for "-v", the startup code selects a matching hanlder to process
	/// the action specified in the "-v" option.
	/// </summary>
	[Export(typeof(ActionTypeHandlerRepository))]
	public class ActionTypeHandlerRepository
	{
		internal const string obtain = "obtain";						// -p <$fwroot>
		internal const string send_receive = "send_receive";			// -p <$fwroot>\foo\foo.fwdata
		internal const string view_notes = "view_notes";				// -p <$fwroot>\foo\foo.fwdata
		internal const string check_for_updates = "check_for_updates";	// -p <$fwroot>\foo where 'foo' is the project folder name
		internal const string about_flex_bridge = "about_flex_bridge";	// -p <$fwroot>\foo where 'foo' is the project folder name

		private static readonly Dictionary<string, ActionType> VOptionToActionTypeMap = new Dictionary<string, ActionType>
			{
				{obtain, ActionType.Obtain},
				{send_receive, ActionType.SendReceive},
				{view_notes, ActionType.ViewNotes},
				{check_for_updates, ActionType.CheckForUpdates},
				{about_flex_bridge, ActionType.AboutFlexBridge}
			};

		[ImportMany]
		public IEnumerable<IBridgeActionTypeHandler> Handlers { get; private set; }

		public IBridgeActionTypeHandler GetHandler(Dictionary<string, string> commandLineArgs)
		{
			return Handlers.First(handler => handler.SupportedActionType == VOptionToActionTypeMap[commandLineArgs["-v"]]);
		}
	}
}