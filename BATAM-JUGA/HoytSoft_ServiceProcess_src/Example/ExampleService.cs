using System;
using HoytSoft.ServiceProcess.Attributes;

namespace HoytSoft.Example {
	///<summary>A service for testing out the base service class.</summary>
	[Service(
	"HoytSoft_ExampleService", 
	DisplayName				= "HoytSoft Example Service",
	Description				= "Isn't this just absolutely amazing?",
	ServiceType				= ServiceType.Default,
	ServiceAccessType		= ServiceAccessType.AllAccess,
	ServiceStartType		= ServiceStartType.AutoStart,
	ServiceErrorControl		= ServiceErrorControl.Normal,
	ServiceControls			= ServiceControls.Default
	)]
	public class ExampleService : HoytSoft.ServiceProcess.ServiceBase {
		public static new void Main(string[] Args) {
			HoytSoft.ServiceProcess.ServiceBase.RunService(Args, typeof(ExampleService));
		}

		protected override bool Initialize(string[] Arguments) {
			this.Log("Example service initialized correctly, starting up...");
			return true;
		}

		protected override void Start() {
			this.Log("Service started");
		}

		protected override void Stop() {
			this.Log("Service stopped");
		}

		protected override void Pause() {
			this.Log("Service paused");
		}

		protected override void Continue() {
			this.Log("Service continued");
		}

		protected override void Interrogate() {
			this.Log("Service interrogated");
		}

		protected override void Shutdown() {
			this.Log("Service shutdown");
		}

		protected override bool Install() {
			this.Log("Service installed");
			return true;
		}

		protected override bool Uninstall() {
			this.Log("Service uninstalled");
			return true;
		}
	}
}
