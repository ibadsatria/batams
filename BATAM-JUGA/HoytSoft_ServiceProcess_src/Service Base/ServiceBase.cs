using System;
using System.Reflection;
using System.Runtime.InteropServices;
/*
 Copyright (c) 2005, 2006 David Hoyt
 
 A huge thank-you to C.V Anish for his article: http://www.codeproject.com/system/windows_nt_service.asp
 It paved the way for this class. Although I used his article to better understand the internals of Win32
 services, almost everything here is original except where noted.
*/
namespace HoytSoft.ServiceProcess {
	#region Public Enums
	public enum ServiceState : byte {
		Running,
		Stopped,
		Paused,
		ShuttingDown,
		Interrogating
	}
	#endregion

	///<summary>A base class for installing, starting, pausing, etc. a Windows service.</summary>
	public abstract class ServiceBase : IDisposable {
		#region Entry Point
		///<summary>If you do not want to specify your own main entry point, you can use this one.</summary>
		public static void Main(string[] Args) {
			Assembly a = Assembly.GetExecutingAssembly();
			if (a == null) throw new ServiceException("No currently executing assembly.");
			RunServices(Args, a.GetTypes());
		}

		///<summary>Executes your service. If multiple services are defined in the assembly, it will run them all in separate threads.</summary>
		/// <param name="Args">The arguments passed in from the command line.</param>
		/// <param name="ServiceType">The class' type you want to inspect for services.</param>
		public static void RunService(string[] Args, Type ServiceType) {
			RunServices(Args, new Type[] { ServiceType });
		}

		///<summary>Executes your service. If multiple services are defined in the assembly, it will run them all in separate threads.</summary>
		/// <param name="Args">The arguments passed in from the command line.</param>
		/// <param name="Types">An array of types we want to inspect for services.</param>
		public static void RunServices(string[] Args, Type[] Types) {
			//Reads in all the classes in the assembly and finds one that derives
			//from this class. If it finds one, it checks the attributes to see
			//if we should run it. If we should, we create an instance of it and 
			//start it on its way...

			//Type[] types = a.GetTypes();
			
			System.Collections.ArrayList alDispatchTables = new System.Collections.ArrayList();
			System.Collections.ArrayList alServices = new System.Collections.ArrayList();
			foreach(Type t in Types) {
				if (t.IsClass && t.BaseType != null && t.BaseType.Equals(typeof(ServiceBase))) {
					//Gets all the custom attributes of type ServiceAttribute in the class.
					object[] attributes = t.GetCustomAttributes(typeof(Attributes.ServiceAttribute), true);
					foreach(Attributes.ServiceAttribute info in attributes) {
						if (info.Run) {
							ServiceBase s = (ServiceBase)Activator.CreateInstance(t);
							alServices.Add(s);

							//Make sure we have a name set for this guy...
							if (s.Name == null || s.Name.Trim() == "")
								throw new ServiceRuntimeException("A service was created without a name.");

							if (Args.Length > 0 && (Args[0].ToLower() == "u" || Args[0].ToLower() == "uninstall")) {
								//Nothing to uninstall if it's not installed...
								if (!s.baseIsInstalled(info.Name)) break;
								if (!s.baseUninstall(info.Name))
									throw new ServiceUninstallException("Unable to remove service \"" + info.DisplayName + "\"");
								if (!s.Uninstall())
									throw new ServiceUninstallException("Service \"" + info.DisplayName + "\" was unable to uninstall itself correctly.");
							} else if (Args.Length > 0 && (Args[0].ToLower() == "i" || Args[0].ToLower() == "install")) {
								//Just install the service if we pass in "i" or "install"...
								//Always check to see if the service is installed and if it isn't,
								//then go ahead and install it...
								if (!s.baseIsInstalled(info.Name)) {
									string[] envArgs = Environment.GetCommandLineArgs();
									if (envArgs.Length > 0) {
										System.IO.FileInfo fi = new System.IO.FileInfo(envArgs[0]);
										if (!s.baseInstall(fi.FullName, info.Name, info.DisplayName, info.Description, info.ServiceType, info.ServiceAccessType, info.ServiceStartType, info.ServiceErrorControl))
											throw new ServiceInstallException("Unable to install service \"" + info.DisplayName + "\"");
										if (!s.Install())
											throw new ServiceInstallException("Service was not able to install itself correctly.");
									}
								}
							} else {
								//Always check to see if the service is installed and if it isn't,
								//then go ahead and install it...
								//Always check to see if the service is installed and if it isn't,
								//then go ahead and install it...
								if (!s.baseIsInstalled(info.Name)) {
									string[] envArgs = Environment.GetCommandLineArgs();
									if (envArgs.Length > 0) {
										System.IO.FileInfo fi = new System.IO.FileInfo(envArgs[0]);
										if (!s.baseInstall(fi.FullName, info.Name, info.DisplayName, info.Description, info.ServiceType, info.ServiceAccessType, info.ServiceStartType, info.ServiceErrorControl))
											throw new ServiceInstallException("Unable to install service \"" + info.DisplayName + "\"");
										if (!s.Install())
											throw new ServiceInstallException("Service was not able to install itself correctly.");
									}
								}
								
								ServicesAPI.SERVICE_TABLE_ENTRY entry = new ServicesAPI.SERVICE_TABLE_ENTRY();
								entry.lpServiceName = info.Name;
								entry.lpServiceProc = new ServicesAPI.ServiceMainProc(s.baseServiceMain);
								alDispatchTables.Add(entry);
								s.debug = false;
							}
						}
						break; //We can break b/c we only allow ONE instance of this attribute per object...
					}
				}
			}

			if (alDispatchTables.Count > 0) {
				//Add a null entry to tell the API it's the last entry in the table...
				ServicesAPI.SERVICE_TABLE_ENTRY entry = new ServicesAPI.SERVICE_TABLE_ENTRY();
				entry.lpServiceName = null;
				entry.lpServiceProc = null;
				alDispatchTables.Add(entry);

				ServicesAPI.SERVICE_TABLE_ENTRY[] table = (ServicesAPI.SERVICE_TABLE_ENTRY[])alDispatchTables.ToArray(typeof(ServicesAPI.SERVICE_TABLE_ENTRY));
				if (ServicesAPI.StartServiceCtrlDispatcher(table) == 0) {
					//There was an error. What was it?
					switch(Marshal.GetLastWin32Error()) {
						case ServicesAPI.ERROR_INVALID_DATA:
							throw new ServiceStartupException("The specified dispatch table contains entries that are not in the proper format.");
						case ServicesAPI.ERROR_SERVICE_ALREADY_RUNNING:
							throw new ServiceStartupException("A service is already running.");
						case ServicesAPI.ERROR_FAILED_SERVICE_CONTROLLER_CONNECT:
							//"A service is being run as a console application. Try setting the Service attribute's \"Debug\" property to true if you're testing an application."
							//If we've started up as a console/windows app, then we'll get this error in which case we treat the program
							//like a normal app instead of a service and start it up in "debug" mode...
							foreach(ServiceBase s in alServices) {
								s.debug = true;
								s.args = Args;
								System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(s.baseDebugStart));
								t.Name = "Main - " + s.displayName;
								t.Start();
							}/**/
							break;
						default:
							throw new ServiceStartupException("An unknown error occurred while starting up the service(s).");
					}
				}
			}
			//alDispatchTables.Clear();
		}
		#endregion

		#region Private Variables
		private string name;
		private string displayName;
		private string description;
		private string logName;
		private bool run;
		private bool debug;
		private string[] args;
		private ServiceState servState;
		private Attributes.ServiceType servType;
		private Attributes.ServiceAccessType servAccessType;
		private Attributes.ServiceStartType servStartType;
		private Attributes.ServiceErrorControl servErrorControl;
		private Attributes.ServiceControls servControls;
		private ServicesAPI.SERVICE_STATUS servStatus;
		private IntPtr servStatusHandle;
		private System.Diagnostics.EventLog log;
		private bool disposed = false;
		private ServicesAPI.ServiceCtrlHandlerProc servCtrlHandlerProc;
		#endregion

		#region Constructors
		public ServiceBase() {
			object[] attributes = this.GetType().GetCustomAttributes(typeof(Attributes.ServiceAttribute), true);
			foreach(Attributes.ServiceAttribute info in attributes) {
				this.name = info.Name;
				this.displayName = info.DisplayName;
				this.description = info.Description;
				this.run = info.Run;
				this.servType = info.ServiceType;
				this.servAccessType = info.ServiceAccessType;
				this.servStartType = info.ServiceStartType;
				this.servErrorControl = info.ServiceErrorControl;
				this.servControls = info.ServiceControls;
				this.logName = info.LogName;
				this.debug = false;
			}
			this.servCtrlHandlerProc = new ServicesAPI.ServiceCtrlHandlerProc(this.baseServiceControlHandler);
			this.servStatus = new ServicesAPI.SERVICE_STATUS();
			this.servState = ServiceState.Stopped;
			this.args = null;
		}
		#endregion

		#region Properties
		///<summary>The name of the service used in the service database.</summary>
		public string Name { get { return this.name; } }
		///<summary>The name of the service that will be displayed in the services snap-in.</summary>
		public string DisplayName { get { return this.displayName; } }
		///<summary>The description of the service that will be displayed in the service snap-in.</summary>
		public string Description { get { return this.description; } }
		///<summary>Indicates if you want the service to run or not on program startup.</summary>
		public bool Run { get { return this.run; } }
		///<summary>Indicates the type of service you want to run.</summary>
		public Attributes.ServiceType ServiceType { get { return this.servType; } }
		///<summary>Access to the service. Before granting the requested access, the system checks the access token of the calling process.</summary>
		public Attributes.ServiceAccessType ServiceAccessType { get { return this.servAccessType; } }
		///<summary>Service start options.</summary>
		public Attributes.ServiceStartType ServiceStartType { get { return this.servStartType; } }
		///<summary>Severity of the error, and action taken, if this service fails to start.</summary>
		public Attributes.ServiceErrorControl ServiceErrorControl { get { return this.servErrorControl; } }
		///<summary>The controls or actions the service responds to.</summary>
		public Attributes.ServiceControls ServiceControls { get { return this.servControls; } }
		///<summary>The current state of the service.</summary>
		public ServiceState ServiceState { get { return this.servState; } }
		///<summary>Treats the service as a console application instead of a normal service.</summary>
		public bool Debug { get { return this.debug; } }
		///<summary>Tells if the service has been disposed of already.</summary>
		public bool Disposed { get { return this.disposed; } }
		#endregion

		#region Override Methods
		protected virtual bool Install() { return true; }
		protected virtual bool Uninstall() { return true; }
		protected virtual bool Initialize(string[] Arguments) { return true; }
		protected virtual void Start() { }
		protected virtual void Pause() { }
		protected virtual void Stop() { }
		protected virtual void Continue() { }
		protected virtual void Shutdown() { }
		protected virtual void Interrogate() { }
		#endregion

		#region API Methods
		[DllImport("advapi32.dll")]
		public static extern IntPtr CreateService(IntPtr scHandle, 
			//[MarshalAs(UnmanagedType.LPTStr)]
			string lpSvcName, 
			//[MarshalAs(UnmanagedType.LPTStr)]
			string lpDisplayName, 
			Attributes.ServiceAccessType dwDesiredAccess, 
			Attributes.ServiceType dwServiceType, 
			Attributes.ServiceStartType dwStartType, 
			Attributes.ServiceErrorControl dwErrorControl, 
			//[MarshalAs(UnmanagedType.LPTStr)]
			string lpPathName, 
			//[MarshalAs(UnmanagedType.LPTStr)]
			string lpLoadOrderGroup, 
			int lpdwTagId, 
			//[MarshalAs(UnmanagedType.LPTStr)]
			string lpDependencies, 
			//[MarshalAs(UnmanagedType.LPTStr)]
			string lpServiceStartName, 
			//[MarshalAs(UnmanagedType.LPTStr)]
			string lpPassword);

		[DllImport("advapi32.dll")]
		public static extern IntPtr OpenService(IntPtr hSCManager, 
			string lpServiceName, 
			Attributes.ServiceAccessType dwDesiredAccess);

		[DllImport("advapi32.dll")]
		public static extern IntPtr OpenService(IntPtr hSCManager, 
			string lpServiceName, 
			int dwDesiredAccess);

		[DllImport("advapi32.dll")]
		public static extern int DeleteService(IntPtr svHandle);
		#endregion

		#region IDisposable Members
		public void Dispose() {
			if (!this.disposed) {
				this.disposed = true;
				if (this.log != null) {
					this.log.Close();
					this.log.Dispose();
					this.log = null;
				}
			}
		}
		#endregion

		#region Helper Methods
		private void checkLog() {
			if (this.log == null) {
				this.log = new System.Diagnostics.EventLog(this.logName);
				this.log.Source = this.displayName;
			}
		}

		public void Log(string Message) {
			try {
				checkLog();
				this.log.WriteEntry(Message);
				this.log.Close();
			} catch(System.ComponentModel.Win32Exception) {
				//In case the event log is full....
			}
		}
		public void Log(string Message, System.Diagnostics.EventLogEntryType EntryType) {
			try {
				checkLog();
				this.log.WriteEntry(Message, EntryType);
			} catch(System.ComponentModel.Win32Exception) {
			}
		}
		public void Log(string Message, System.Diagnostics.EventLogEntryType EntryType, int EventID) {
			try {
				checkLog();
				this.log.WriteEntry(Message, EntryType, EventID);
			} catch(System.ComponentModel.Win32Exception) {
			}
		}
		public void Log(string Message, System.Diagnostics.EventLogEntryType EntryType, short Category, int EventID) {
			try {
				checkLog();
				this.log.WriteEntry(Message, EntryType, EventID, Category);
			} catch(System.ComponentModel.Win32Exception) {
			}
		}

		#region Testing Methods
		private bool inTestAction = false;
		protected void TestInstall() {
			System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(this.baseDebugInstall));
			t.Name = "Install - " + this.displayName;
			t.Start();
		}

		protected void TestUninstall() {
			System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(this.baseDebugUninstall));
			t.Name = "Uninstall - " + this.displayName;
			t.Start();
		}

		protected void TestStop() {
			System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(this.baseDebugStop));
			t.Name = "Stop - " + this.displayName;
			t.Start();
		}

		protected void TestPause() {
			System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(this.baseDebugPause));
			t.Name = "Pause - " + this.displayName;
			t.Start();
		}

		protected void TestContinue() {
			System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(this.baseDebugContinue));
			t.Name = "Continue - " + this.displayName;
			t.Start();
		}

		protected void TestShutdown() {
			System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(this.baseDebugShutdown));
			t.Name = "Shutdown - " + this.displayName;
			t.Start();
		}

		protected void TestInterrogate() {
			System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(this.baseDebugInterrogate));
			t.Name = "Interrogate - " + this.displayName;
			t.Start();
		}
		#endregion

		#region Debug Mode Methods
		private void baseDebugInstall() {
			while(this.servState == ServiceState.Interrogating)
				;
			if (!this.Install())
				throw new ServiceInstallException("Service was not able to install itself correctly.");
		}

		private void baseDebugUninstall() {
			while(this.servState == ServiceState.Interrogating)
				;
			if (!this.Uninstall())
				throw new ServiceUninstallException("Service \"" + this.displayName + "\" was unable to uninstall itself correctly.");
		}

		private void baseDebugStop() {
			while(this.inTestAction || this.servState == ServiceState.Interrogating)
				;
			this.inTestAction = true;
			this.servState = ServiceState.Stopped;
			this.Stop();
			this.inTestAction = false;
		}

		private void baseDebugPause() {
			if (this.servState != ServiceState.ShuttingDown) {
				while(this.inTestAction || this.servState == ServiceState.Interrogating)
					;
				this.inTestAction = true;
				this.servState = ServiceState.Paused;
				this.Pause();
				this.inTestAction = false;
			}
		}

		private void baseDebugContinue() {
			while(this.inTestAction || this.servState == ServiceState.Interrogating)
				;
			this.inTestAction = true;
			this.servState = ServiceState.Running;
			this.Continue();
			this.inTestAction = false;
		}

		private void baseDebugShutdown() {
			while(this.inTestAction || this.servState == ServiceState.Interrogating)
				;
			this.inTestAction = true;
			this.servState = ServiceState.ShuttingDown;
			this.Shutdown();
			this.inTestAction = false;
		}

		private void baseDebugInterrogate() {
			while(this.inTestAction)
				;
			this.inTestAction = true;
			this.servState = ServiceState.Interrogating;
			this.Interrogate();
			this.servState = ServiceState.Running;
			this.inTestAction = false;
		}
		#endregion
		#endregion

		private void baseDebugStart() {
			if (this.Initialize(this.args)) {
				this.servState = ServiceState.Running;
				this.Start();
				//Wait for all tests to finish...
				//while(this.inTestAction)
				//	;
				//this.TestStop();
			}
		}

		private void baseServiceMain(int argc, string[] argv) {
			if (this.ServiceType != Attributes.ServiceType.FileSystemDriver && this.ServiceType != Attributes.ServiceType.KernelDriver)
				this.servStatus.dwServiceType = ServicesAPI.SERVICE_WIN32;
			else
				this.servStatus.dwServiceType = (int)this.ServiceType;
			this.servStatus.dwCurrentState = ServicesAPI.SERVICE_START_PENDING;
			this.servStatus.dwControlsAccepted = (int)this.ServiceControls;
			this.servStatus.dwWin32ExitCode = 0;
			this.servStatus.dwServiceSpecificExitCode = 0;
			this.servStatus.dwCheckPoint = 0;
			this.servStatus.dwWaitHint = 0;

			this.servStatusHandle = ServicesAPI.RegisterServiceCtrlHandler(this.Name, this.servCtrlHandlerProc); 
			if (servStatusHandle == IntPtr.Zero) return;
			this.servStatus.dwCurrentState = ServicesAPI.SERVICE_RUNNING;
			this.servStatus.dwCheckPoint = 0;
			this.servStatus.dwWaitHint = 0;
			if (ServicesAPI.SetServiceStatus(this.servStatusHandle, ref this.servStatus) == 0) {
				throw new ServiceRuntimeException("\"" + this.displayName + "\" threw an error. Error Number: " + Marshal.GetLastWin32Error());
			}

			//Call the initialize method...
			if (!this.Initialize(argv)) {
				this.servStatus.dwCurrentState       = ServicesAPI.SERVICE_STOPPED; 
				this.servStatus.dwCheckPoint         = 0; 
				this.servStatus.dwWaitHint           = 0; 
				this.servStatus.dwWin32ExitCode      = 1; 
				this.servStatus.dwServiceSpecificExitCode = 1;
 
				ServicesAPI.SetServiceStatus(this.servStatusHandle, ref this.servStatus); 
				return; 
			}

			//Initialization complete - report running status. 
			this.servStatus.dwCurrentState       = ServicesAPI.SERVICE_RUNNING; 
			this.servStatus.dwCheckPoint         = 0; 
			this.servStatus.dwWaitHint           = 0; 
 
			if (ServicesAPI.SetServiceStatus(this.servStatusHandle, ref this.servStatus) == 0) {
				throw new ServiceRuntimeException("\"" + this.displayName + "\" threw an error. Error Number: " + Marshal.GetLastWin32Error());
			}

			this.servState = ServiceState.Running;
			/*System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(this.Start));
			if (this.debug)
				t.Name = "Start - " + this.displayName;
			t.Start();/**/
			this.Start();
		}

		//This is called whenever a service control event happens such as pausing, stopping, etc...
		private void baseServiceControlHandler(int Opcode) {
			switch(Opcode) {
				case ServicesAPI.SERVICE_CONTROL_PAUSE: 
					this.servState = ServiceState.Paused;
					this.servStatus.dwCurrentState = ServicesAPI.SERVICE_PAUSED;
					try {
						this.Pause();
					} catch(Exception e) {
						this.Log("An exception occurred while trying to pause the service:" + e);
					}
					ServicesAPI.SetServiceStatus(this.servStatusHandle, ref this.servStatus);
					break;

				case ServicesAPI.SERVICE_CONTROL_CONTINUE:
					this.servState = ServiceState.Running;
					this.servStatus.dwCurrentState = ServicesAPI.SERVICE_RUNNING;
					ServicesAPI.SetServiceStatus(this.servStatusHandle, ref this.servStatus);
					try {
						this.Continue();
					} catch(Exception e) {
						this.Log("An exception occurred while trying to continue the service:" + e);
					}
					break;

				case ServicesAPI.SERVICE_CONTROL_STOP:
					this.servState = ServiceState.Stopped;
					this.servStatus.dwWin32ExitCode = 0;
					this.servStatus.dwCurrentState = ServicesAPI.SERVICE_STOPPED;
					this.servStatus.dwCheckPoint = 0;
					this.servStatus.dwWaitHint = 0;
					ServicesAPI.SetServiceStatus(this.servStatusHandle, ref this.servStatus);
					try {
						this.Stop();
					} catch(Exception e) {
						this.Log("An exception occurred while trying to stop the service:" + e);
					}
					break;

				case ServicesAPI.SERVICE_CONTROL_SHUTDOWN:
					this.servState = ServiceState.ShuttingDown;
					this.servStatus.dwCurrentState = ServicesAPI.SERVICE_STOPPED;
					ServicesAPI.SetServiceStatus(this.servStatusHandle, ref this.servStatus);
					try {
						this.Shutdown();
					} catch(Exception e) {
						this.Log("An exception occurred while trying to shut down the service:" + e);
					}
					break;

				case ServicesAPI.SERVICE_CONTROL_INTERROGATE:
					this.servState = ServiceState.Interrogating;
					this.servStatus.dwCurrentState = ServicesAPI.SERVICE_INTERROGATE;
					ServicesAPI.SetServiceStatus(this.servStatusHandle, ref this.servStatus);
					try {
						this.Interrogate();
					} catch(Exception e) {
						this.Log("An exception occurred while trying to interrogate the service:" + e);
					}
					break; 
			}
		}

		private bool baseIsInstalled(string Name) {
			if (Name.Length > 256) throw new ServiceStartupException("The maximum length for a service name is 256 characters.");
			bool ret = false;
			IntPtr sc_handle = IntPtr.Zero;
			IntPtr sv_handle = IntPtr.Zero;
			try {
				sc_handle = ServicesAPI.OpenSCManagerA(null, null, ServicesAPI.ServiceControlManagerType.SC_MANAGER_CREATE_SERVICE);
				if (sc_handle == IntPtr.Zero) return false;

				sv_handle = OpenService(sc_handle, Name, ServicesAPI.GENERIC_READ);
				ret = (sv_handle != IntPtr.Zero);
			} catch {
				ret = false;
			} finally {
				if (sv_handle != IntPtr.Zero) ServicesAPI.CloseServiceHandle(sv_handle);
				if (sc_handle != IntPtr.Zero) ServicesAPI.CloseServiceHandle(sc_handle);
			}
			return ret;
		}

		private bool baseUninstall(string Name) {
			if (Name.Length > 256) throw new ServiceInstallException("The maximum length for a service name is 256 characters.");
			try {
				IntPtr sc_hndl = ServicesAPI.OpenSCManagerA(null, null, ServicesAPI.GENERIC_WRITE);
 
				if(sc_hndl != IntPtr.Zero) {
					IntPtr svc_hndl = OpenService(sc_hndl, Name, ServicesAPI.DELETE);
					if(svc_hndl != IntPtr.Zero) { 
						int i = DeleteService(svc_hndl);
						if (i != 0) {
							ServicesAPI.CloseServiceHandle(sc_hndl);
							return true;
						} else {
							ServicesAPI.CloseServiceHandle(sc_hndl);
							return false;
						}
					} else return false;
				} else return false;
			} catch {
				return false;
			}
		}

		private bool baseInstall(string ServicePath, string Name, string DisplayName, string Description, Attributes.ServiceType ServType, Attributes.ServiceAccessType ServAccessType, Attributes.ServiceStartType ServStartType, Attributes.ServiceErrorControl ServErrorControl) {
			if (Name.Length > 256) throw new ServiceInstallException("The maximum length for a service name is 256 characters.");
			if (Name.IndexOf(@"\") >= 0 || Name.IndexOf(@"/") >= 0) throw new ServiceInstallException(@"Service names cannot contain \ or / characters.");
			if (DisplayName.Length > 256) throw new ServiceInstallException("The maximum length for a display name is 256 characters.");
			//The spec says that if a service's path has a space in it, then we must quote it...
			//if (ServicePath.IndexOf(" ") >= 0)
			//	ServicePath = "\"" + ServicePath + "\"";
			//ServicePath = ServicePath.Replace(@"\", @"\\");

			try {
				IntPtr  sc_handle = ServicesAPI.OpenSCManagerA(null, null, ServicesAPI.ServiceControlManagerType.SC_MANAGER_CREATE_SERVICE);
				if (sc_handle == IntPtr.Zero) return false;

				IntPtr sv_handle = CreateService(sc_handle, Name, DisplayName, ServAccessType, ServType, ServStartType, ServErrorControl, ServicePath, null, 0, null, null, null);
				//IntPtr sv_handle = ServicesAPI.CreateService(sc_handle, Name, DisplayName, 0xF0000 | 0x0001 | 0x0002 | 0x0004 | 0x0008 | 0x0010 | 0x0020 | 0x0040 | 0x0080 | 0x0100, 0x00000010, 0x00000002, 0x00000001, ServicePath, null, 0, null, null, null);
				if (sv_handle == IntPtr.Zero) {
					ServicesAPI.CloseServiceHandle(sc_handle);
					return false;
				}
				ServicesAPI.CloseServiceHandle(sv_handle);
				ServicesAPI.CloseServiceHandle(sc_handle);

				//Sets a service's description by adding a registry entry for it.
				if (Description != null && Description != "") {
					try {
						using (Microsoft.Win32.RegistryKey serviceKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Services\" + Name, true)) {
							serviceKey.SetValue("Description", Description);
						}
					} catch {
						return false;
					}
				}

				return true;
			} catch {
				return false;
			}
		}
	}

	#region Helper Classes
	#region Exceptions
	public class ServiceRuntimeException : ServiceException {
		public ServiceRuntimeException(string Message) : base(Message) { }
		public ServiceRuntimeException(string Message, Exception InnerException) : base(Message, InnerException) { }
	}

	public class ServiceStartupException : ServiceException {
		public ServiceStartupException(string Message) : base(Message) { }
		public ServiceStartupException(string Message, Exception InnerException) : base(Message, InnerException) { }
	}

	public class ServiceUninstallException : ServiceException {
		public ServiceUninstallException(string Message) : base(Message) { }
		public ServiceUninstallException(string Message, Exception InnerException) : base(Message, InnerException) { }
	}

	public class ServiceInstallException : ServiceException {
		public ServiceInstallException(string Message) : base(Message) { }
		public ServiceInstallException(string Message, Exception InnerException) : base(Message, InnerException) { }
	}

	public class ServiceException : Exception {
		public ServiceException(string Message) : base(Message) { }
		public ServiceException(string Message, Exception InnerException) : base(Message, InnerException) { }
	}
	#endregion
	#endregion
}
