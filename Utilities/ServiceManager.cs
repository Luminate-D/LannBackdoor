using System.Runtime.InteropServices;

namespace Utilities;

public class ServiceManager {
	private const int SERVICE_WIN32_OWN_PROCESS = 0x00000010;
	private const int ServiceAllAccess = 0xF01FF;
	private const int ScManagerAllAccess = 0xF003F;
	private const int ServiceConfigFailureActions = 0x2;
	private const int ServiceDescription = 0x1;
	private const int ErrorAccessDenied = 5;

	private const uint ServiceNoChange = 0xFFFFFFFF;

	[StructLayout(LayoutKind.Sequential)]
	private class SERVICE_STATUS {
		public int dwServiceType = 0;
		public ServiceState dwCurrentState = 0;
		public int dwControlsAccepted = 0;
		public int dwWin32ExitCode = 0;
		public int dwServiceSpecificExitCode = 0;
		public int dwCheckPoint = 0;
		public int dwWaitHint = 0;
	}

	[DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
	private static extern IntPtr OpenServiceManager(string machineName, string databaseName, ScmAccessRights dwDesiredAccess);

	[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
	private static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, ServiceAccessRights dwDesiredAccess);

	[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
	private static extern IntPtr CreateService(
		IntPtr hSCManager,
		string lpServiceName,
		string lpDisplayName,
		ServiceAccessRights dwDesiredAccess,
		int dwServiceType,
		ServiceBootFlag dwStartType,
		ServiceError dwErrorControl,
		string lpBinaryPathName,
		string lpLoadOrderGroup,
		IntPtr lpdwTagId,
		string lpDependencies,
		string lp,
		string lpPassword
	);

	[DllImport("advapi32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool CloseServiceHandle(IntPtr hSCObject);

	[DllImport("advapi32.dll")]
	private static extern int QueryServiceStatus(IntPtr hService, SERVICE_STATUS lpServiceStatus);

	[DllImport("advapi32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool DeleteService(IntPtr hService);

	[DllImport("advapi32.dll")]
	private static extern int ControlService(IntPtr hService, ServiceControl dwControl, SERVICE_STATUS lpServiceStatus);

	[DllImport("advapi32.dll", SetLastError = true)]
	private static extern int StartService(IntPtr hService, int dwNumServiceArgs, int lpServiceArgVectors);

	[DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2")]
	private static extern bool SetServiceFailureActions(IntPtr hService, int dwInfoLevel, [MarshalAs(UnmanagedType.Struct)] ref ServiceFailureActions lpInfo);

	[DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2", CharSet = CharSet.Unicode)]
	private static extern bool SetServiceDescription(IntPtr hService, int dwInfoLevel, [MarshalAs(UnmanagedType.Struct)] ref SERVICE_DESCRIPTION lpInfo);

	[DllImport("advapi32.dll", EntryPoint = "QueryServiceConfig2W", SetLastError = true, CharSet = CharSet.Unicode)]
	private static extern bool QueryServiceConfig2(IntPtr hService, uint dwInfoLevel, IntPtr buffer, uint cbBufSize, out uint pcbBytesNeeded);

	[DllImport("advapi32.dll", EntryPoint = "QueryServiceConfigW", CharSet = CharSet.Unicode)]
	private static extern bool QueryServiceConfig(IntPtr hService, IntPtr intPtrQueryConfig, uint cbBufSize, out uint pcbBytesNeeded);

	[DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfigW", SetLastError = true, CharSet = CharSet.Unicode)]
	private static extern bool ChangeServiceConfig(
		IntPtr hService,
		uint nServiceType,
		uint nStartType,
		uint nErrorControl,
		string lpBinaryPathName,
		string lpLoadOrderGroup,
		IntPtr lpdwTagId,
		[In] char[] lpDependencies,
		string lpServiceStartName,
		string lpPassword,
		string lpDisplayName
	);

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	private struct SERVICE_DESCRIPTION {
		public string lpDescription;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public class SERVICE_CONFIG {
		[MarshalAs(UnmanagedType.U4)]
		public uint dwServiceType;
		[MarshalAs(UnmanagedType.U4)]
		public ServiceBootFlag dwStartType;
		[MarshalAs(UnmanagedType.U4)]
		public uint dwErrorControl;
		[MarshalAs(UnmanagedType.LPWStr)]
		public string lpBinaryPathName;
		[MarshalAs(UnmanagedType.LPWStr)]
		public string lpLoadOrderGroup;
		[MarshalAs(UnmanagedType.U4)]
		public uint dwTagID;
		[MarshalAs(UnmanagedType.LPWStr)]
		public string lpDependencies;
		[MarshalAs(UnmanagedType.LPWStr)]
		public string lpServiceStartName;
		[MarshalAs(UnmanagedType.LPWStr)]
		public string lpDisplayName;
	};


	public static async Task Uninstall(string serviceName) {
		IntPtr manager = OpenServiceManager(ScmAccessRights.AllAccess);

		try {
			IntPtr service = OpenService(manager, serviceName, ServiceAccessRights.AllAccess);
			if (service == IntPtr.Zero) throw new InvalidOperationException($"Service '{serviceName}' is not installed");

			try {
				await Stop(service);
				if (!DeleteService(service)) throw new InvalidOperationException($"Failed to delete service '{serviceName}' (error: {Marshal.GetLastWin32Error()})");
			} finally {
				CloseServiceHandle(service);
			}
		} finally {
			CloseServiceHandle(manager);
		}
	}

	public static bool IsInstalled(string serviceName) {
		IntPtr manager = OpenServiceManager(ScmAccessRights.Connect);

		try {
			IntPtr service = OpenService(manager, serviceName, ServiceAccessRights.QueryStatus);

			if (service == IntPtr.Zero) return false;

			CloseServiceHandle(service);

			return true;
		} finally {
			CloseServiceHandle(manager);
		}
	}

	public static void Install(string serviceName, string displayName, string fileName) {
		IntPtr manager = OpenServiceManager(ScmAccessRights.AllAccess);

		try {
			IntPtr service = OpenService(manager, serviceName, ServiceAccessRights.AllAccess);

			if (service == IntPtr.Zero) service = CreateService(
				 manager,
				 serviceName,
				 displayName,
				 ServiceAccessRights.AllAccess,
				 SERVICE_WIN32_OWN_PROCESS,
				 ServiceBootFlag.AutoStart,
				 ServiceError.Normal,
				 fileName,
				 null,
				 IntPtr.Zero,
				 null,
				 null,
				 null
			 );

			if (service == IntPtr.Zero) throw new InvalidOperationException($"Failed to install service '{serviceName}' (error: {Marshal.GetLastWin32Error()})");
		} finally {
			CloseServiceHandle(manager);
		}
	}

	public static void SetDescription(string serviceName, string description) {
		IntPtr manager = OpenServiceManager(ScmAccessRights.Connect);

		try {
			IntPtr service = OpenService(manager, serviceName, ServiceAccessRights.ChangeConfig);
			if (service == IntPtr.Zero) throw new InvalidOperationException($"Failed to open service '{serviceName}' (error: {Marshal.GetLastWin32Error()})");

			try {
				SetDescription(service, description);
			} finally {
				CloseServiceHandle(service);
			}
		} finally {
			CloseServiceHandle(manager);
		}
	}

	public static void SetDescription(IntPtr service, string description) {
		SERVICE_DESCRIPTION info = new SERVICE_DESCRIPTION() {
			lpDescription = description
		};

		if (!SetServiceDescription(service, ServiceDescription, ref info)) {
			throw new InvalidOperationException($"Failed to set service description (pointer: {service}, error: {Marshal.GetLastWin32Error()})");
		}
	}

	public static string GetDescription(string serviceName) {
		IntPtr manager = OpenServiceManager(ScmAccessRights.Connect);

		try {
			IntPtr service = OpenService(manager, serviceName, ServiceAccessRights.QueryConfig);
			if (service == IntPtr.Zero) throw new InvalidOperationException($"Failed to open service '{serviceName}' (error: {Marshal.GetLastWin32Error()})");

			try {
				return GetDescription(service);
			} finally {
				CloseServiceHandle(service);
			}
		} finally {
			CloseServiceHandle(manager);
		}
	}

	public static string GetDescription(IntPtr service) {
		uint descriptionLength = 0;

		QueryServiceConfig2(service, ServiceDescription, IntPtr.Zero, descriptionLength, out descriptionLength);

		IntPtr buffer = Marshal.AllocHGlobal((int)descriptionLength);
		if (!QueryServiceConfig2(service, ServiceDescription, buffer, descriptionLength, out descriptionLength)) {
			throw new InvalidOperationException($"Failed to set service description (pointer: {service}, error: {Marshal.GetLastWin32Error()})");
		}

		SERVICE_DESCRIPTION description = (SERVICE_DESCRIPTION)Marshal.PtrToStructure(buffer, typeof(SERVICE_DESCRIPTION));

		Marshal.FreeHGlobal(buffer);

		return description.lpDescription;
	}

	public static void SetDisplayName(string serviceName, string displayName) {
		IntPtr manager = OpenServiceManager(ScmAccessRights.Connect);

		try {
			IntPtr service = OpenService(manager, serviceName, ServiceAccessRights.ChangeConfig);
			if (service == IntPtr.Zero) throw new InvalidOperationException($"Failed to open service '{serviceName}' (error: {Marshal.GetLastWin32Error()})");

			try {
				SetDisplayName(service, displayName);
			} finally {
				CloseServiceHandle(service);
			}
		} finally {
			CloseServiceHandle(manager);
		}
	}

	public static void SetDisplayName(IntPtr service, string displayName) {
		if (!ChangeServiceConfig(
			service,
			ServiceNoChange,
			ServiceNoChange,
			ServiceNoChange,
			null,
			null,
			IntPtr.Zero,
			null,
			null,
			null,
			displayName
		)) {
			throw new InvalidOperationException($"Failed to set service display name (pointer: {service}, error: {Marshal.GetLastWin32Error()})");
		}
	}

	public static string GetDisplayName(string serviceName) {
		IntPtr manager = OpenServiceManager(ScmAccessRights.Connect);

		try {
			IntPtr service = OpenService(manager, serviceName, ServiceAccessRights.QueryConfig);
			if (service == IntPtr.Zero) throw new InvalidOperationException($"Failed to open service '{serviceName}' (error: {Marshal.GetLastWin32Error()})");

			try {
				return GetDisplayName(service);
			} finally {
				CloseServiceHandle(service);
			}
		} finally {
			CloseServiceHandle(manager);
		}
	}

	public static string GetDisplayName(IntPtr service) {
		return GetConfig(service).lpDisplayName;
	}

	public static SERVICE_CONFIG GetConfig(string serviceName) {
		IntPtr manager = OpenServiceManager(ScmAccessRights.Connect);

		try {
			IntPtr service = OpenService(manager, serviceName, ServiceAccessRights.QueryConfig);
			if (service == IntPtr.Zero) throw new InvalidOperationException($"Failed to open service '{serviceName}' (error: {Marshal.GetLastWin32Error()})");

			try {
				return GetConfig(service);
			} finally {
				CloseServiceHandle(service);
			}
		} finally {
			CloseServiceHandle(manager);
		}
	}

	public static SERVICE_CONFIG GetConfig(IntPtr service) {
		uint configLength = 0;

		QueryServiceConfig(service, IntPtr.Zero, configLength, out configLength);

		IntPtr buffer = Marshal.AllocHGlobal((int)configLength);
		if (!QueryServiceConfig(service, buffer, configLength, out configLength)) {
			throw new InvalidOperationException($"Failed to query service config (pointer: {service}, error: {Marshal.GetLastWin32Error()})");
		}

		SERVICE_CONFIG config = (SERVICE_CONFIG)Marshal.PtrToStructure(buffer, typeof(SERVICE_CONFIG));

		Marshal.FreeHGlobal(buffer);

		return config;
	}

	public static async Task Start(string serviceName) {
		IntPtr manager = OpenServiceManager(ScmAccessRights.Connect);

		try {
			IntPtr service = OpenService(manager, serviceName, ServiceAccessRights.QueryStatus | ServiceAccessRights.Start);
			if (service == IntPtr.Zero) throw new InvalidOperationException($"Failed to open service '{serviceName}' (error: {Marshal.GetLastWin32Error()})");

			try {
				await Start(service);
			} finally {
				CloseServiceHandle(service);
			}
		} finally {
			CloseServiceHandle(manager);
		}
	}

	public static async Task Start(IntPtr service) {
		StartService(service, 0, 0);
		bool changedStatus = await WaitForStatus(service, ServiceState.StartPending, ServiceState.Running);
		if (!changedStatus) throw new InvalidOperationException($"Failed to start service (pointer: {service}, error: {Marshal.GetLastWin32Error()})");
	}

	public static async Task Stop(string serviceName) {
		IntPtr manager = OpenServiceManager(ScmAccessRights.Connect);

		try {
			IntPtr service = OpenService(manager, serviceName, ServiceAccessRights.QueryStatus | ServiceAccessRights.Stop);
			if (service == IntPtr.Zero) throw new InvalidOperationException($"Failed to open service '{serviceName}' (error: {Marshal.GetLastWin32Error()})");

			try {
				await Stop(service);
			} finally {
				CloseServiceHandle(service);
			}
		} finally {
			CloseServiceHandle(manager);
		}
	}

	public static async Task Stop(IntPtr service) {
		SERVICE_STATUS status = new SERVICE_STATUS();
		ControlService(service, ServiceControl.Stop, status);
		bool changedStatus = await WaitForStatus(service, ServiceState.StopPending, ServiceState.Stopped);
		if (!changedStatus) throw new InvalidOperationException($"Failed to stop service (pointer: {service}, error: {Marshal.GetLastWin32Error()})");
	}

	public static ServiceState GetStatus(string serviceName) {
		IntPtr manager = OpenServiceManager(ScmAccessRights.Connect);

		try {
			IntPtr service = OpenService(manager, serviceName, ServiceAccessRights.QueryStatus);
			if (service == IntPtr.Zero) return ServiceState.NotFound;

			try {
				return GetStatus(service);
			} finally {
				CloseServiceHandle(service);
			}
		} finally {
			CloseServiceHandle(manager);
		}
	}

	private static ServiceState GetStatus(IntPtr service) {
		SERVICE_STATUS status = new SERVICE_STATUS();

		if (QueryServiceStatus(service, status) == 0) throw new InvalidOperationException($"Failed to query service status (pointer: {service}, error: {Marshal.GetLastWin32Error()})");

		return status.dwCurrentState;
	}

	private static async Task<bool> WaitForStatus(IntPtr service, ServiceState waitStatus, ServiceState desiredStatus) {
		SERVICE_STATUS status = new SERVICE_STATUS();

		QueryServiceStatus(service, status);
		if (status.dwCurrentState == desiredStatus) return true;

		int dwStartTickCount = Environment.TickCount;
		int dwOldCheckPoint = status.dwCheckPoint;

		while (status.dwCurrentState == waitStatus) {
			int dwWaitTime = status.dwWaitHint / 10;

			if (dwWaitTime < 1000) dwWaitTime = 1000;
			else if (dwWaitTime > 10000) dwWaitTime = 10000;

			await Task.Delay(dwWaitTime);

			if (QueryServiceStatus(service, status) == 0) break;

			if (status.dwCheckPoint > dwOldCheckPoint) {
				// The service is making progress.
				dwStartTickCount = Environment.TickCount;
				dwOldCheckPoint = status.dwCheckPoint;
			} else {
				if (Environment.TickCount - dwStartTickCount > status.dwWaitHint) {
					// No progress made within the wait hint
					break;
				}
			}
		}

		return status.dwCurrentState == desiredStatus;
	}

	private static IntPtr OpenServiceManager(ScmAccessRights rights) {
		IntPtr manager = OpenServiceManager(null, null, rights);
		if (manager == IntPtr.Zero) throw new InvalidOperationException($"Failed to connect to service control manager (error: {Marshal.GetLastWin32Error()})");

		return manager;
	}

	public static void SetFailureActions(
		string serviceName,
		FailureAction firstAction,
		FailureAction secondAction,
		FailureAction subsequentAction,
		int resetPeriod
	) {
		IntPtr manager = OpenServiceManager(ScmAccessRights.Connect);

		try {
			IntPtr service = OpenService(manager, serviceName, ServiceAccessRights.AllAccess);
			if (service == IntPtr.Zero) throw new InvalidOperationException($"Failed to open service '{serviceName}' (error: {Marshal.GetLastWin32Error()})");

			try {
				SetFailureActions(
					service,
					firstAction,
					secondAction,
					subsequentAction,
					resetPeriod
				);
			} finally {
				CloseServiceHandle(service);
			}
		} finally {
			CloseServiceHandle(manager);
		}
	}

	public static void SetFailureActions(
		IntPtr service,
		FailureAction firstAction,
		FailureAction secondAction,
		FailureAction subsequentAction,
		int resetPeriod
	) {
		IntPtr manager = OpenServiceManager(ScmAccessRights.Connect);

		try {
			try {
				List<FailureAction> actions = new List<FailureAction>() {
					firstAction,
					secondAction,
					subsequentAction
				};

				int index = 0;
				int[] serializedActions = new int[actions.Count * 2];

				foreach (FailureAction action in actions) {
					serializedActions[index] = (int)action.Type;
					serializedActions[++index] = action.Delay;

					index++;
				}

				// Need to pack 8 bytes per struct
				IntPtr buffer = Marshal.AllocHGlobal(actions.Count * 8);

				// Move array into marshallable pointer
				Marshal.Copy(serializedActions, 0, buffer, actions.Count * 2);

				ServiceFailureActions config = new ServiceFailureActions() {
					cActions = 3,
					dwResetPeriod = resetPeriod,
					lpCommand = null,
					lpRebootMsg = null,
					lpsaActions = buffer
				};

				if (!SetServiceFailureActions(service, ServiceConfigFailureActions, ref config)) {
					throw new InvalidOperationException($"Failed to set failure actions for service (pointer: {service}, error: {Marshal.GetLastWin32Error()})");
				}

				Marshal.FreeHGlobal(buffer);
			} finally {
				CloseServiceHandle(service);
			}
		} finally {
			CloseServiceHandle(manager);
		}
	}
}

public enum RecoverActionType {
	None = 0,
	Restart = 1,
	Reboot = 2,
	RunCommand = 3
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct ServiceFailureActions {
	public int dwResetPeriod;
	[MarshalAs(UnmanagedType.LPWStr)]

	public string lpRebootMsg;
	[MarshalAs(UnmanagedType.LPWStr)]

	public string lpCommand;
	public int cActions;
	public IntPtr lpsaActions;
}

[StructLayout(LayoutKind.Sequential)]
public class ScAction {
	public int type;
	public uint dwDelay;
}

public class FailureAction {
	public RecoverActionType Type { get; set; }
	public int Delay { get; set; }

	public FailureAction(RecoverActionType type, int delay) {
		Type = type;
		Delay = delay;
	}
}

public enum ServiceState {
	Unknown = -1, // The state cannot be (has not been) retrieved.
	NotFound = 0, // The service is not known on the host server.
	Stopped = 1,
	StartPending = 2,
	StopPending = 3,
	Running = 4,
	ContinuePending = 5,
	PausePending = 6,
	Paused = 7
}

[Flags]
public enum ScmAccessRights {
	Connect = 0x0001,
	CreateService = 0x0002,
	EnumerateService = 0x0004,
	Lock = 0x0008,
	QueryLockStatus = 0x0010,
	ModifyBootConfig = 0x0020,
	StandardRightsRequired = 0xF0000,
	AllAccess = StandardRightsRequired | Connect | CreateService | EnumerateService | Lock | QueryLockStatus | ModifyBootConfig
}

[Flags]
public enum ServiceAccessRights {
	QueryConfig = 0x1,
	ChangeConfig = 0x2,
	QueryStatus = 0x4,
	EnumerateDependants = 0x8,
	Start = 0x10,
	Stop = 0x20,
	PauseContinue = 0x40,
	Interrogate = 0x80,
	UserDefinedControl = 0x100,
	Delete = 0x00010000,
	StandardRightsRequired = 0xF0000,
	AllAccess = StandardRightsRequired | QueryConfig | ChangeConfig | QueryStatus | EnumerateDependants | Start | Stop | PauseContinue | Interrogate | UserDefinedControl
}

public enum ServiceBootFlag {
	Start = 0x00000000,
	SystemStart = 0x00000001,
	AutoStart = 0x00000002,
	DemandStart = 0x00000003,
	Disabled = 0x00000004
}

public enum ServiceControl {
	Stop = 0x00000001,
	Pause = 0x00000002,
	Continue = 0x00000003,
	Interrogate = 0x00000004,
	Shutdown = 0x00000005,
	ParamChange = 0x00000006,
	NetBindAdd = 0x00000007,
	NetBindRemove = 0x00000008,
	NetBindEnable = 0x00000009,
	NetBindDisable = 0x0000000A
}

public enum ServiceError {
	Ignore = 0x00000000,
	Normal = 0x00000001,
	Severe = 0x00000002,
	Critical = 0x00000003
}