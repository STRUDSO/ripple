﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using FubuCore;

namespace ripple.New.Model
{
	public interface IDependencyReader
	{
		bool Matches(Project project, string projectDir);
		IEnumerable<Dependency> Read(Project project, string projectDir);
	}

	public class NuGetDependencyReader : IDependencyReader
	{
		public const string PackagesConfig = "packages.config";

		private readonly IFileSystem _fileSystem = new FileSystem();

		public bool Matches(Project project, string projectDir)
		{
			return _fileSystem.FileExists(projectDir, PackagesConfig);
		}

		public IEnumerable<Dependency> Read(Project project, string projectDir)
		{
			var document = new XmlDocument();
			document.Load(Path.Combine(projectDir, PackagesConfig));

			return ReadFrom(document);
		}

		public static IEnumerable<Dependency> ReadFrom(XmlDocument document)
		{
			foreach (XmlElement element in document.SelectNodes("//package"))
			{
				yield return ReadFrom(element);
			}
		}

		public static Dependency ReadFrom(XmlElement element)
		{
			return new Dependency(element.GetAttribute("id"), element.GetAttribute("version"));
		}
	}

	public class RippleDependencyReader : IDependencyReader
	{
		public const string RippleDependenciesConfig = "ripple.dependencies.config";

		private readonly IFileSystem _fileSystem = new FileSystem();

		public bool Matches(Project project, string projectDir)
		{
			return _fileSystem.FileExists(projectDir, RippleDependenciesConfig);
		}

		public IEnumerable<Dependency> Read(Project project, string projectDir)
		{
			var dependencies = new List<Dependency>();
			_fileSystem.ReadTextFile(Path.Combine(projectDir, RippleDependenciesConfig), line =>
			{
				if (line.IsEmpty()) return;

				var values = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				var version = string.Empty;

				if (values.Length == 2)
				{
					version = values[1].Trim();
				}

				dependencies.Add(new Dependency(values[0].Trim(), version));
			});
			
			return dependencies;
		}
	}
}