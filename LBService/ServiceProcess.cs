using System;
using System.Runtime.InteropServices;

namespace LBService {
	public static class ServiceProcess {
		private const int CREATE_UNICODE_ENVIRONMENT = 0x00000400;
		private const int CREATE_NO_WINDOW = 0x08000000;
		private const int CREATE_NEW_CONSOLE = 0x00000010;

		private const uint INVALID_SESSION_ID = 0xFFFFFFFF;

		private static readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

		[DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUser", SetLastError = true, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.StdCall)]
		private static extern bool CreateProcessAsUser(
			IntPtr hToken,
			string lpApplicationName,
			string lpCommandLine,
			IntPtr lpProcessAttributes,
			IntPtr lpThreadAttributes,
			bool bInheritHandle,
			uint dwCreationFlags,
			IntPtr lpEnvironment,
			string lpCurrentDirectory,
			ref STARTUPINFO lpStartupInfo,
			out PROCESS_INFORMATION lpProcessInformation
		);

		[DllImport("advapi32.dll", EntryPoint = "DuplicateTokenEx")]
		private static extern bool DuplicateTokenEx(
			IntPtr ExistingTokenHandle,
			uint dwDesiredAccess,
			IntPtr lpThreadAttributes,
			int TokenType,
			int ImpersonationLevel,
			ref IntPtr DuplicateTokenHandle
		);

		[DllImport("userenv.dll", SetLastError = true)]
		private static extern bool CreateEnvironmentBlock(ref IntPtr lpEnvironment, IntPtr hToken, bool bInherit);

		[DllImport("userenv.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DestroyEnvironmentBlock(IntPtr lpEnvironment);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool CloseHandle(IntPtr hSnapshot);

		[DllImport("kernel32.dll")]
		private static extern uint WTSGetActiveConsoleSessionId();

		[DllImport("Wtsapi32.dll")]
		private static extern uint WTSQueryUserToken(uint SessionId, ref IntPtr phToken);

		[DllImport("wtsapi32.dll", SetLastError = true)]
		private static extern int WTSEnumerateSessions(
			IntPtr hServer,
			int Reserved,
			int Version,
			ref IntPtr ppSessionInfo,
			ref int pCount
		);

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool GetTokenInformation(
			IntPtr TokenHandle,
			TOKEN_INFORMATION_CLASS TokenInformationClass,
			IntPtr TokenInformation,
			uint TokenInformationLength,
			out uint ReturnLength
		);

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool SetTokenInformation(
			IntPtr TokenHandle,
			TOKEN_INFORMATION_CLASS TokenInformationClass,
			ref uint TokenInformation,
			uint TokenInformationLength
		);

		private enum TOKEN_INFORMATION_CLASS {
			TokenUser = 1,
			TokenGroups,
			TokenPrivileges,
			TokenOwner,
			TokenPrimaryGroup,
			TokenDefaultDacl,
			TokenSource,
			TokenType,
			TokenImpersonationLevel,
			TokenStatistics,
			TokenRestrictedSids,
			TokenSessionId,
			TokenGroupsAndPrivileges,
			TokenSessionReference,
			TokenSandBoxInert,
			TokenAuditPolicy,
			TokenOrigin,
			TokenElevationType,
			TokenLinkedToken,
			TokenElevation,
			TokenHasRestrictions,
			TokenAccessInformation,
			TokenVirtualizationAllowed,
			TokenVirtualizationEnabled,
			TokenIntegrityLevel,
			TokenUIAccess,
			TokenMandatoryPolicy,
			TokenLogonSid,
			MaxTokenInfoClass
		}

		private enum SW {
			SW_HIDE = 0,
			SW_SHOWNORMAL = 1,
			SW_NORMAL = 1,
			SW_SHOWMINIMIZED = 2,
			SW_SHOWMAXIMIZED = 3,
			SW_MAXIMIZE = 3,
			SW_SHOWNOACTIVATE = 4,
			SW_SHOW = 5,
			SW_MINIMIZE = 6,
			SW_SHOWMINNOACTIVE = 7,
			SW_SHOWNA = 8,
			SW_RESTORE = 9,
			SW_SHOWDEFAULT = 10,
			SW_MAX = 10
		}

		private enum WTS_CONNECTSTATE_CLASS {
			WTSActive,
			WTSConnected,
			WTSConnectQuery,
			WTSShadow,
			WTSDisconnected,
			WTSIdle,
			WTSListen,
			WTSReset,
			WTSDown,
			WTSInit
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct PROCESS_INFORMATION {
			public IntPtr hProcess;
			public IntPtr hThread;
			public uint dwProcessId;
			public uint dwThreadId;
		}

		private enum SECURITY_IMPERSONATION_LEVEL {
			SecurityAnonymous = 0,
			SecurityIdentification = 1,
			SecurityImpersonation = 2,
			SecurityDelegation = 3
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct STARTUPINFO {
			public int cb;
			public string lpReserved;
			public string lpDesktop;
			public string lpTitle;
			public uint dwX;
			public uint dwY;
			public uint dwXSize;
			public uint dwYSize;
			public uint dwXCountChars;
			public uint dwYCountChars;
			public uint dwFillAttribute;
			public uint dwFlags;
			public short wShowWindow;
			public short cbReserved2;
			public IntPtr lpReserved2;
			public IntPtr hStdInput;
			public IntPtr hStdOutput;
			public IntPtr hStdError;
		}

		private enum TOKEN_TYPE {
			TokenPrimary = 1,
			TokenImpersonation = 2
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct WTS_SESSION_INFO {
			public readonly uint SessionID;

			[MarshalAs(UnmanagedType.LPStr)] public readonly string pWinStationName;

			public readonly WTS_CONNECTSTATE_CLASS State;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct TOKEN_LINKED_TOKEN {
			public readonly IntPtr LinkedToken;
		}

		// Gets the user token from the currently active session
		private static bool GetSessionUserToken(ref IntPtr phUserToken) {
			bool bResult = false;
			IntPtr hImpersonationToken = IntPtr.Zero;
			uint activeSessionId = INVALID_SESSION_ID;
			IntPtr pSessionInfo = IntPtr.Zero;
			int sessionCount = 0;

			// Get a handle to the user access token for the current active session.
			if (WTSEnumerateSessions(WTS_CURRENT_SERVER_HANDLE, 0, 1, ref pSessionInfo, ref sessionCount) != 0) {
				int arrayElementSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
				IntPtr current = pSessionInfo;

				for (int i = 0; i < sessionCount; i++) {
					WTS_SESSION_INFO si = (WTS_SESSION_INFO) Marshal.PtrToStructure(current, typeof(WTS_SESSION_INFO));
					current += arrayElementSize;

					if (si.State == WTS_CONNECTSTATE_CLASS.WTSActive) {
						activeSessionId = si.SessionID;
					}
				}
			}

			// If enumerating did not work, fall back to the old method
			if (activeSessionId == INVALID_SESSION_ID) {
				activeSessionId = WTSGetActiveConsoleSessionId();
			}

			if (WTSQueryUserToken(activeSessionId, ref hImpersonationToken) != 0) {
				// Convert the impersonation token to a primary token
				bResult = DuplicateTokenEx(
					hImpersonationToken,
					0,
					IntPtr.Zero,
					(int) SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation,
					(int) TOKEN_TYPE.TokenPrimary,
					ref phUserToken
				);

				CloseHandle(hImpersonationToken);
			}

			return bResult;
		}

		public static bool StartProcessAsCurrentUser(string appPath, string cmdLine = null, string workDir = null,
			bool visible = true, bool elevated = true) {
			IntPtr hUserToken = IntPtr.Zero;
			STARTUPINFO startInfo = new STARTUPINFO();
			PROCESS_INFORMATION procInfo = new PROCESS_INFORMATION();
			IntPtr pEnv = IntPtr.Zero;

			startInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));

			try {
				if (!GetSessionUserToken(ref hUserToken)) {
					throw new Exception($"StartProcessAsCurrentUser: GetSessionUserToken failed.");
				}

				// System.Diagnostics.Debugger.Launch();

				IntPtr token = hUserToken;

				if (elevated) {
					uint tokenInformationLength = 0;

					GetTokenInformation(hUserToken, TOKEN_INFORMATION_CLASS.TokenLinkedToken, IntPtr.Zero,
						tokenInformationLength, out tokenInformationLength);

					IntPtr buffer = Marshal.AllocHGlobal((int) tokenInformationLength);
					if (!GetTokenInformation(hUserToken, TOKEN_INFORMATION_CLASS.TokenLinkedToken, buffer,
						    tokenInformationLength, out tokenInformationLength)) {
						throw new Exception(
							$"StartProcessAsCurrentUser: GetTokenInformation failed (error: {Marshal.GetLastWin32Error()})");
					}

					TOKEN_LINKED_TOKEN linkedToken =
						(TOKEN_LINKED_TOKEN) Marshal.PtrToStructure(buffer, typeof(TOKEN_LINKED_TOKEN));

					Marshal.FreeHGlobal(buffer);

					token = linkedToken.LinkedToken;
				}

				uint dwCreationFlags = CREATE_UNICODE_ENVIRONMENT |
				                       (uint) (visible ? CREATE_NEW_CONSOLE : CREATE_NO_WINDOW);
				startInfo.wShowWindow = (short) (visible ? SW.SW_SHOW : SW.SW_HIDE);
				startInfo.lpDesktop = @"winsta0\default";

				if (!CreateEnvironmentBlock(ref pEnv, hUserToken, false)) {
					throw new Exception(
						$"StartProcessAsCurrentUser: CreateEnvironmentBlock failed (error: {Marshal.GetLastWin32Error()})");
				}

				if (!CreateProcessAsUser(
					    token,
					    appPath, // Application Name
					    cmdLine, // Command Line
					    IntPtr.Zero,
					    IntPtr.Zero,
					    false,
					    dwCreationFlags,
					    pEnv,
					    workDir, // Working directory
					    ref startInfo,
					    out procInfo
				    )) {
					throw new Exception(
						$"StartProcessAsCurrentUser: CreateProcessAsUser failed (error: {Marshal.GetLastWin32Error()})");
				}
			} finally {
				CloseHandle(hUserToken);
				if (pEnv != IntPtr.Zero) {
					DestroyEnvironmentBlock(pEnv);
				}

				CloseHandle(procInfo.hThread);
				CloseHandle(procInfo.hProcess);
			}

			return true;
		}
	}
}