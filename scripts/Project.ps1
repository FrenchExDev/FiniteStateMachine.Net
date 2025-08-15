[CmdletBinding()]
param(
	[validateset("core")][string[]] $Project,
	[validateset("pack", "build", "push")][string[]] $Action,
	[validateset("Release", "Debug")][string] $Configuration = "Release",
	[string] $NugetSource = "https://api.nuget.org/v3/index.json",
	[string] $Version
)

function Get-ProjectCsProjPath {
		param(
		[string] $ProjectName
	)
	switch($ProjectName) {
		("core") {
			return "$(Get-ProjectSourcePath -ProjectName $projectName)/FrenchExDev.Net.FiniteStateMachine.Core.csproj"
		}
		(default) {
			throw "Unknown project name: $ProjectName"
		}
	}
}

function Get-ProjectSourcePath {
	param(
		[string] $ProjectName
	)

	switch($ProjectName) {
		("core") {
			return "src/FrenchExDev.Net.FiniteStateMachine.Core/"
		}
		(default) {
			throw "Unknown project name: $ProjectName"
		}
	}
}

function Get-ProjectNugetPackage {
	param(
		[string] $ProjectName,
		[string] $Version
	)

	switch($ProjectName) {
		("core") {
			return "FrenchExDev.Net.FiniteStateMachine.Core.$Version.nupkg"
		}
		(default) {
			throw "Unknown project name: $ProjectName"
		}
	}
}

Import-Env -Type:Environment

foreach($projectName in $Project) {
	foreach($currentAction in $Action){
		switch ($currentAction) {
			("pack") {
				dotnet pack $(Get-ProjectCsProjPath -ProjectName $projectName) -c $Configuration
			}
			("build") {
				dotnet build $(Get-ProjectCsProjPath -ProjectName $projectName) -c $Configuration
			}
			("push") {
				$projectPath = "$(Get-ProjectSourcePath -ProjectName $projectName)\bin\$Configuration"
				$latestNuget = Get-ChildItem "$projectPath" -Filter "*.nupkg"  | select-object  -Last 1
				dotnet nuget push $($latestNuget.Fullname) --api-key $env:NUGET_API_KEY --source "$NugetSource"
			}
			default {
				throw "Unknown action: $currentAction"
			}
		}
	}
}
