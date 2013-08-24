﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using TriboroughBridge_ChorusPlugin;
using TriboroughBridge_ChorusPlugin.Infrastructure;

namespace FLEx_ChorusPlugin.Infrastructure.ActionHandlers
{
	/// <summary>
	/// This IBridgeActionTypeHandler implementation handles everything needed for a normal 'undo export' for a Flex repo.
	/// </summary>
	/// <remarks>
	/// This option is not supported at the moment.
	/// </remarks>
	[Export(typeof(IBridgeActionTypeHandler))]
	internal sealed class UndoExportActionHandler : IBridgeActionTypeHandler
	{
		#region IBridgeActionTypeHandler impl

		/// <summary>
		/// Start doing whatever is needed for the supported type of action.
		/// </summary>
		/// <returns>'true' if the caller expects the main window to be shown, otherwise 'false'.</returns>
		public void StartWorking(Dictionary<string, string> commandLineArgs)
		{
			throw new NotSupportedException("The Undo Export handler is not supported");
		}

		/// <summary>
		/// Get the type of action supported by the handler.
		/// </summary>
		public ActionType SupportedActionType
		{
			get { return ActionType.UndoExport; }
		}

		#endregion IBridgeActionTypeHandler impl
	}
}
