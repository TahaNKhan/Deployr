using Deployr.Web.Contracts.DataContracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Deployr.Processor.Logic
{
	public interface IScriptRunner
	{
		Task RunScript(string scriptPath, DeploymentInformation deployment);
	}
	public class ScriptRunner: IScriptRunner
	{
		
	}
}
