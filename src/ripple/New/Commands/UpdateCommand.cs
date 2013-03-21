﻿using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using ripple.New.Model;
using ripple.New.Steps;

namespace ripple.New.Commands
{
	public class UpdateInput : SolutionInput
	{
		[Description("Only update a specific nuget by name")]
		public string NugetFlag { get; set; }

		[Description("Only show what would be updated")]
		public bool PreviewFlag { get; set; }

		[Description("Forces the update command to override all dependencies even if they are locked")]
		[FlagAlias("force", 'f')]
		public bool ForceFlag { get; set; }

		public override string DescribePlan(Solution solution)
		{
			return "Updating dependencies for solution {0}".ToFormat(solution.Name);
		}
	}

	public class UpdateCommand : FubuCommand<UpdateInput>
	{
		public override bool Execute(UpdateInput input)
		{
			return RippleOperation
				.For<UpdateInput>(input)
				.Step<DownloadUpdatedDependencies>()
				.Step<ExplodeDownloadedNugets>()
				.Step<UpdateReferences>()
				.Execute();
		}
	}
}