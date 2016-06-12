// --------------------------------------------------------------------------------------------
// Copyright (C) 2010-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Xml.Linq;

namespace TriboroughBridge_ChorusPlugin
{
	/// <summary>
	/// This class holds constants and methods that are relevant to common bridge operations.
	/// A lot of what it had held earlier, was moved into places like Flex Bridge's LibTriboroughBridgeConstants class or
	/// into Lift Bridge's LiftUtilties class, when the stuff was only used by one bridge.
	///
	/// Some of the remaining constants could yet be moved at the cost of having duplciates in each bridge's project.
	/// It may be worth that to be rid of bridge-specific stuff in this project.
	/// </summary>
	public static class TriboroughBridgeUtilities
	{
// ReSharper disable InconsistentNaming
		public const string hg = ".hg";
		public const string FlexBridgeEmailAddress = "flex_errors@sil.org";
// ReSharper restore InconsistentNaming

		/// <summary>
		/// Strips file URI prefix from the beginning of a file URI string, and keeps
		/// a beginning slash if in Linux.
		/// eg "file:///C:/Windows" becomes "C:/Windows" in Windows, and
		/// "file:///usr/bin" becomes "/usr/bin" in Linux.
		/// Returns the input unchanged if it does not begin with "file:".
		///
		/// Does not convert the result into a valid path or a path using current platform
		/// path separators.
		/// fileString does not neet to be a valid URI. We would like to treat it as one
		/// but since we import files with file URIs that can be produced by other
		/// tools (eg LIFT) we can't guarantee that they will always be valid.
		///
		/// File URIs, and their conversation to paths, are more complex, with hosts,
		/// forward slashes, and escapes, but just stripping the file URI prefix is
		/// what's currently needed.
		/// Different places in code need "file://', or "file:///" removed.
		///
		/// See uri.LocalPath, http://en.wikipedia.org/wiki/File_URI , and
		/// http://blogs.msdn.com/b/ie/archive/2006/12/06/file-uris-in-windows.aspx .
		/// </summary>
		public static string StripFilePrefix(string fileString)
		{
			if (String.IsNullOrEmpty(fileString))
				return fileString;

			var prefix = Uri.UriSchemeFile + ":";

			if (!fileString.StartsWith(prefix))
				return fileString;

			var path = fileString.Substring(prefix.Length);
			// Trim any number of beginning slashes
			path = path.TrimStart('/');
			// Prepend slash on Linux
			if (IsUnix)
				path = '/' + path;

			return path;
		}

		public static XElement CreateFromBytes(byte[] xmlData)
		{
			using (var memStream = new MemoryStream(xmlData))
			{
				// This loads the MemoryStream as Utf8 xml. (I checked.)
				return XElement.Load(memStream);
			}
		}

		/// <summary>
		/// Returns <c>true</c> if we're running on Unix, otherwise <c>false</c>.
		/// </summary>
		public static bool IsUnix
		{
			get { return Environment.OSVersion.Platform == PlatformID.Unix; }
		}

		/// <summary>
		/// Returns <c>true</c> if we're running on Windows NT or later, otherwise <c>false</c>.
		/// </summary>
		public static bool IsWindows
		{
			get { return Environment.OSVersion.Platform == PlatformID.Win32NT; }
		}

		public static string HgDataFolder(string path)
		{
			return Path.Combine(path, hg, "store", "data");
		}

		public static bool FolderIsEmpty(string folder)
		{
			return Directory.GetDirectories(folder).Length == 0 && Directory.GetFiles(folder).Length == 0;
		}
	}
}
