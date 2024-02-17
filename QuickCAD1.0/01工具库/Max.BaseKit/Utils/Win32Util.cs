using Max.BaseKit.Mods;
using System;
using System.Runtime.InteropServices;

namespace Max.BaseKit.Utils
{
    public static class Win32Util
    {
        public const int SB_BOTTOM = 7;
        public const uint SND_ALIAS = 0x10000;
        public const uint SND_ALIAS_ID = 0x110000;
        public const uint SND_ALIAS_START = 0;
        public const uint SND_APPLICATION = 0x80;
        public const uint SND_ASYNC = 0x1;
        public const uint SND_FILENAME = 0x20000;
        public const uint SND_LOOP = 0x8;
        public const uint SND_MEMORY = 0x4;
        public const uint SND_NODEFAULT = 0x2;
        public const uint SND_NOSTOP = 0x10;
        public const uint SND_NOWAIT = 0x2000;
        public const uint SND_PURGE = 0x40;
        public const uint SND_RESERVED = 0xFF000000;
        public const uint SND_RESOURCE = 0x40004;
        public const uint SND_SYNC = 0x0;
        public const uint SND_TYPE_MASK = 0x170007;
        public const uint SND_VALID = 0x1F;
        public const uint SND_VALIDFLAGS = 0x17201F;
        public const int VK_F4 = 0x73;
        public const int WM_CLOSE = 0x10;
        public const int WM_HOTKEY = 0x312;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_KILLFOCUS = 0x8;
        public const int WM_LBUTTONDBLCLK = 0x203;
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;
        public const int WM_MBUTTONDBLCLK = 0x209;
        public const int WM_MBUTTONDOWN = 0x207;
        public const int WM_MBUTTONUP = 0x208;
        public const int WM_MOUSEFIRST = 0x200;
        public const int WM_MOUSELAST = 0x209;
        public const int WM_MOUSEMOVE = 0x200;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_RBUTTONDBLCLK = 0x206;
        public const int WM_RBUTTONDOWN = 0x204;
        public const int WM_RBUTTONUP = 0x205;
        public const int WM_SETFOCUS = 0x7;
        public const int WM_USER = 0x0400;
        public const int WM_VSCROLL = 0x115;
        /// <summary>
        /// 窗口模式：0不可见但仍然运行,1居中,2最小化,3最大化
        /// </summary>
        public const int WS_ShowNormal = 3;

        // private methods...
        /// <summary>
        /// The GetLastInputInfo function retrieves the time of the last input event.
        /// </summary>
        /// <param name="lii">[out] Pointer to a LASTINPUTINFO structure that receives the time of the last input event.</param>
        /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero. </returns>
        /// <remarks>BOOL GetLastInputInfo(PLASTINPUTINFO plii);</remarks>
        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LastInputInfo lii);

        /// <summary>
        /// 获取用户最后操作（鼠标、键盘）距离现在的时长（秒）
        /// </summary>
        /// <returns></returns>
        public static int GetLastInputTime()
        {
            var idleTime = 0;
            var lastInputInfo = new LastInputInfo();
            lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            var envTicks = Environment.TickCount;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                var lastInputTick = lastInputInfo.dwTime;

                idleTime = envTicks - lastInputTick;
            }

            return ((idleTime > 0) ? (idleTime / 1000) : idleTime);
        }
        [DllImportAttribute("Kernel32.dll")]
        public static extern void GetLocalTime(SystemTime st);
        /// <summary>
        /// The GlobalAddAtom function adds a character string to the global atom table and 
        /// returns a unique value (an atom) identifying the string. 
        /// Syntax
        /// ATOM GlobalAddAtom(LPCTSTR lpString);
        /// Parameters
        /// lpString
        /// [in] Pointer to the null-terminated string to be added. The string can have a 
        /// maximum size of 255 bytes. Strings that differ only in case are considered 
        /// identical. The case of the first string of this name added to the table is 
        /// preserved and returned by the GlobalGetAtomName function. 
        /// Alternatively, you can use an integer atom that has been converted using the 
        /// MAKEINTATOM macro. See the Remarks for more information. 
        /// Return Value
        /// If the function succeeds, the return value is the newly created atom.
        /// If the function fails, the return value is zero. To get extended error information, 
        /// call GetLastError. 
        /// </summary>
        [DllImport("kernel32")]
        public static extern UInt32 GlobalAddAtom(String lpString);
        /// <summary>
        /// The GlobalDeleteAtom function decrements the reference count of a global string 
        /// atom. If the atom's reference count reaches zero, GlobalDeleteAtom removes the 
        /// string associated with the atom from the global atom table. 
        /// Syntax
        /// ATOM GlobalDeleteAtom(ATOM nAtom);
        /// Parameters
        /// nAtom
        /// [in] Identifies the atom and character string to be deleted. 
        /// Return Value
        /// The function always returns (ATOM) 0. 
        /// To determine whether the function has failed, call SetLastError (ERROR_SUCCESS) 
        /// before calling GlobalDeleteAtom, then call GetLastError. If the last error code 
        /// is still ERROR_SUCCESS, GlobalDeleteAtom has succeeded.
        /// </summary>
        [DllImport("kernel32")]
        public static extern UInt32 GlobalDeleteAtom(UInt32 nAtom);
        /// <summary>
        /// <para>The mciSendString function sends a command string to an MCI device. The device that the command is sent to is specified in the command string. </para>
        /// <para>MCIERROR mciSendString(
        /// LPCTSTR lpszCommand,  
        /// LPTSTR lpszReturnString,  
        /// UINT cchReturn,       
        /// HANDLE hwndCallback   
        /// );</para>
        /// <para>Parameters</para>
        /// <para>lpszCommand</para>
        /// <para>Pointer to a null-terminated string that specifies an MCI command string. For a list, see Multimedia Command Strings. </para>
        /// <para>lpszReturnString</para>
        /// <para>Pointer to a buffer that receives return information. If no return information is needed, this parameter can be NULL. </para>
        /// <para>cchReturn</para>
        /// <para>Size, in characters, of the return buffer specified by the lpszReturnString parameter. </para>
        /// <para>hwndCallback</para>
        /// <para>Handle to a callback window if the "notify" flag was specified in the command string. </para>
        /// </summary>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern uint mciSendString(string cmd, ref byte[] retStringBuffer, uint bufSize, int hwndCallback);
        /// <summary>
        /// <para>The PlaySound function plays a sound specified by the given filename, resource, 
        /// or system event. (A system event may be associated with a sound in the registry 
        /// or in the WIN.INI file.)</para>
        /// <para>BOOL PlaySound(LPCSTR pszSound, HMODULE hmod, DWORD fdwSound );</para>
        /// <para>Parameters</para>
        /// <para>pszSound</para>
        /// <para>A string that specifies the sound to play. The maximum length, including the null 
        /// terminator, is 256 characters. If this parameter is NULL, any currently playing 
        /// waveform sound is stopped. To stop a non-waveform sound, specify SND_PURGE in the 
        /// fdwSound parameter.</para>
        /// <para>Three flags in fdwSound (SND_ALIAS, SND_FILENAME, and SND_RESOURCE) determine 
        /// whether the name is interpreted as an alias for a system event, a filename, or 
        /// a resource identifier. If none of these flags are specified, PlaySound searches 
        /// the registry or the WIN.INI file for an association with the specified sound name. 
        /// If an association is found, the sound event is played. If no association is found 
        /// in the registry, the name is interpreted as a filename.</para>
        /// <para>hmod</para>
        /// <para>Handle to the executable file that contains the resource to be loaded. This parameter 
        /// must be NULL unless SND_RESOURCE is specified in fdwSound.</para>
        /// <para>fdwSound</para>
        /// <para>Flags for playing the sound. The following values are defined.</para>
        /// <para>Value Meaning </para>
        /// <para>SND_APPLICATION The sound is played using an application-specific association.</para> 
        ///
        /// <para>SND_ALIAS The pszSound parameter is a system-event alias in the registry or the 
        /// WIN.INI file. Do not use with either SND_FILENAME or SND_RESOURCE. </para>
        ///
        /// <para>SND_ALIAS_ID The pszSound parameter is a predefined sound identifier.</para> 
        ///
        /// <para>SND_ASYNC The sound is played asynchronously and PlaySound returns immediately 
        /// after beginning the sound. To terminate an asynchronously played waveform sound, 
        /// call PlaySound with pszSound set to NULL.</para> 
        ///
        /// <para>SND_FILENAME The pszSound parameter is a filename.</para> 
        ///
        /// <para>SND_LOOP The sound plays repeatedly until PlaySound is called again with the 
        /// pszSound parameter set to NULL. You must also specify the SND_ASYNC flag to 
        /// indicate an asynchronous sound event.</para> 
        ///
        /// <para>SND_MEMORY A sound event's file is loaded in RAM. The parameter specified by pszSound 
        /// must point to an image of a sound in memory.</para> 
        ///
        /// <para>SND_NODEFAULT No default sound event is used. If the sound cannot be found, PlaySound 
        /// returns silently without playing the default sound. </para>
        ///
        /// <para>SND_NOSTOP The specified sound event will yield to another sound event that is already 
        /// playing. If a sound cannot be played because the resource needed to generate that sound 
        /// is busy playing another sound, the function immediately returns FALSE without playing 
        /// the requested sound. 
        /// If this flag is not specified, PlaySound attempts to stop the currently playing sound 
        /// so that the device can be used to play the new sound.</para>
        ///
        /// <para>SND_NOWAIT If the driver is busy, return immediately without playing the sound. </para>
        ///
        /// <para>SND_PURGE Sounds are to be stopped for the calling task. If pszSound is not NULL, all 
        /// instances of the specified sound are stopped. If pszSound is NULL, all sounds that are 
        /// playing on behalf of the calling task are stopped. 
        /// You must also specify the instance handle to stop SND_RESOURCE events.</para>
        ///
        /// <para>SND_RESOURCE The pszSound parameter is a resource identifier; hmod must identify the 
        /// instance that contains the resource.</para> 
        ///
        /// <para>SND_SYNC Synchronous playback of a sound event. PlaySound returns after the sound event 
        /// completes. </para>
        /// <para>Return Values</para>
        /// <para>Returns TRUE if successful or FALSE otherwise.</para>
        /// </summary>
        [DllImport("winmm.dll")]
        public static extern bool PlaySound(string pszSound, int hModule, uint fdwSound);
        /// <summary>
        /// The PostMessage function places (posts) a message in the message queue associated 
        /// with the thread that created the specified window and returns without waiting for 
        /// the thread to process the message. 
        /// To post a message in the message queue associate with a thread, use the 
        /// PostThreadMessage function.
        /// Syntax
        ///
        /// BOOL PostMessage(HWND hWnd, UINT Msg, WPARAM wParam, LPARAM lParam);
        /// Parameters
        /// </summary>
        /// hWnd
        /// [in] Handle to the window whose window procedure is to receive the message. 
        /// The following values have special meanings.
        /// HWND_BROADCAST
        /// The message is posted to all top-level windows in the system, including disabled or 
        /// invisible unowned windows, overlapped windows, and pop-up windows. The message is 
        /// not posted to child windows.
        /// NULL
        /// The function behaves like a call to PostThreadMessage with the dwThreadId parameter 
        /// set to the identifier of the current thread.
        /// Msg
        /// [in] Specifies the message to be posted.
        /// wParam
        /// [in] Specifies additional message-specific information.
        /// lParam
        /// [in] Specifies additional message-specific information.
        /// Return Value
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, 
        /// call GetLastError. 
        [DllImport("user32")]
        public static extern IntPtr PostMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        /// <summary>
        /// The RegisterHotKey function defines a system-wide hot key. 
        /// Syntax
        /// BOOL RegisterHotKey(          HWND hWnd, int id, UINT fsModifiers, UINT vk );
        /// Parameters
        /// hWnd
        /// [in] Handle to the window that will receive WM_HOTKEY messages generated by the hot 
        /// key. If this parameter is NULL, WM_HOTKEY messages are posted to the message queue 
        /// of the calling thread and must be processed in the message loop. 
        /// id
        /// [in] Specifies the identifier of the hot key. No other hot key in the calling thread 
        /// should have the same identifier. An application must specify a value in the range 
        /// 0x0000 through 0xBFFF. A shared dynamic-link library (DLL) must specify a value in 
        /// the range 0xC000 through 0xFFFF (the range returned by the GlobalAddAtom function). 
        /// To avoid conflicts with hot-key identifiers defined by other shared DLLs, a DLL should 
        /// use the GlobalAddAtom function to obtain the hot-key identifier. 
        /// fsModifiers
        /// [in] Specifies keys that must be pressed in combination with the key specified by 
        /// the uVirtKey parameter in order to generate the WM_HOTKEY message. The fsModifiers 
        /// parameter can be a combination of the following values. 
        /// MOD_ALT
        /// Either ALT key must be held down.
        /// MOD_CONTROL
        /// Either CTRL key must be held down.
        /// MOD_SHIFT
        /// Either SHIFT key must be held down.
        /// MOD_WIN
        /// Either WINDOWS key was held down. These keys are labeled with the Microsoft Windows logo.
        /// vk
        /// [in] Specifies the virtual-key code of the hot key. 
        /// Return Value
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, 
        /// call GetLastError.
        /// </summary>
        [DllImport("user32")]
        public static extern UInt32 RegisterHotKey(IntPtr hWnd, UInt32 id, UInt32 fsModifiers, UInt32 vk);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hwnd);
        [DllImportAttribute("Kernel32.dll")]
        public static extern void SetLocalTime(ref SystemTime st);
        /// <summary>
        /// sets the show state of a window created by a different thread.
        /// </summary>
        /// <param name="hwnd">[in] Handle to the window. </param>
        /// <param name="cmdShow">[in] Specifies how the window is to be shown. For a list of possible values, see the description of the ShowWindow function. </param>
        /// <returns>If the window was previously visible, the return value is nonzero. If the window was previously hidden, the return value is zero.</returns>
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hwnd, int cmdShow);
        /// <summary>
        /// The UnregisterHotKey function frees a hot key previously registered by the calling thread. 
        /// Syntax
        /// BOOL UnregisterHotKey(HWND hWnd, int id);
        /// Parameters
        /// hWnd
        /// [in] Handle to the window associated with the hot key to be freed. This parameter should be NULL if the hot key is not associated with a window. 
        /// id
        /// [in] Specifies the identifier of the hot key to be freed. 
        /// </summary>
        [DllImport("user32")]
        public static extern UInt32 UnregisterHotKey(IntPtr hWnd, UInt32 id);

        // private structs...
        /// <summary>
        /// The LASTINPUTINFO structure contains the time of the last input.
        /// </summary>
        /// <remarks>
        /// typedef struct tagLASTINPUTINFO {
        /// UINT cbSize;
        /// DWORD dwTime;
        /// } LASTINPUTINFO, *PLASTINPUTINFO;
        /// </remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct LastInputInfo
        {
            // public fields...
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LastInputInfo));
            /// <summary>
            /// Must be set to sizeof(LASTINPUTINFO). 
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            /// <summary>
            /// Tick count when the last input event was received.
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public int dwTime;
        }

        // public structs...
        /// <summary>
        /// BMP文件头数据结构，含有BMP文件的类型、文件大小和位图起始位置等信息。
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BitmapFileHeader
        {
            // public fields...
            /// <summary>
            /// 位图文件的大小，以字节为单位
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public int bfSize;
            /// <summary>
            /// 位图数据的起始位置，以相对于位图文件头的偏移量表示，以字节为单位
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public int bfOffBits;
            /// <summary>
            /// 位图文件的类型，必须为BM
            /// </summary>
            [MarshalAs(UnmanagedType.I2)]
            public short bfType;
            /// <summary>
            /// 位图文件保留字，必须为0
            /// </summary>
            [MarshalAs(UnmanagedType.I2)]
            public short bfReserved1;
            /// <summary>
            /// 位图文件保留字，必须为0
            /// </summary>
            [MarshalAs(UnmanagedType.I2)]
            public short bfReserved2;
        }

        /// <summary>
        /// BMP位图信息头数据，用于说明位图的尺寸等信息。
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BitmapInfoHeader
        {
            // public fields...
            /// <summary>
            /// 本结构所占用字节数
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public int biSize;
            /// <summary>
            /// 位图的宽度，以像素为单位
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public int biWidth;
            /// <summary>
            /// 位图的高度，以像素为单位
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public int biHeight;
            /// <summary>
            /// 位图压缩类型，必须是 0(不压缩),1(BI_RLE8压缩类型)或2(BI_RLE4压缩类型)之一
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public int biCompression;
            /// <summary>
            /// 位图的大小，以字节为单位
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public int biSizeImage;
            /// <summary>
            /// 位图水平分辨率，每米像素数
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public int biXPelsPerMeter;
            /// <summary>
            /// 位图垂直分辨率，每米像素数
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public int biYPelsPerMeter;
            /// <summary>
            /// 位图实际使用的颜色表中的颜色数
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public int biClrUsed;
            /// <summary>
            /// 位图显示过程中重要的颜色数
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public int biClrImportant;
            /// <summary>
            /// 目标设备的级别，必须为1
            /// </summary>
            [MarshalAs(UnmanagedType.I2)]
            public short biPlanes;
            /// <summary>
            /// 每个像素所需的位数，必须是1(双色), 4(16色), 8(256色), 24(真彩色)或32(真彩色)之一
            /// </summary>
            [MarshalAs(UnmanagedType.I2)]
            public short biBitCount;
        }
    }
}

