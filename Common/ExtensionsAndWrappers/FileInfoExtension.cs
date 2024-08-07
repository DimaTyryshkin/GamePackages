using System;
using System.IO;

namespace GamePackages.Core
{
	public static class FileInfoExtension
	{
		/// <summary>
		/// Возвращает всю часть имени файла, идущую после первой точки. Например "file.xml.zip" => ".xml.zip" 
		/// </summary>  
		public static string GetFullExtension(this FileInfo fileInfo)
		{ 
			string name          = fileInfo.Name;
			int    starExtension = name.IndexOf(".", StringComparison.Ordinal);
			string extension = starExtension > 0 ?
				name.Substring(starExtension) :
				name;

			return extension;
		}
	}
}