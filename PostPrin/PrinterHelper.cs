﻿using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.ComponentModel;
using System.Drawing.Printing;

namespace PostPrin
{
    public class PrinterHelper
    {
        private const int ERROR_FILE_NOT_FOUND = 2;

        private const int ERROR_INSUFFICIENT_BUFFER = 122;

        #region DLL导入函数

        [DllImport("winspool.drv", EntryPoint = "GetDefaultPrinter",CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetDefaultPrinter(StringBuilder pszBuffer, ref int pcchBuffer);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct structPrinterDefaults
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public String pDatatype;
            public IntPtr pDevMode;
            [MarshalAs(UnmanagedType.I4)]
            public int DesiredAccess;
        };

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinter", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall),
        SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPTStr)]
			string printerName,
            out IntPtr phPrinter,
            ref structPrinterDefaults pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern bool ClosePrinter(IntPtr phPrinter);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct structSize
        {
            public Int32 width;
            public Int32 height;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct structRect
        {
            public Int32 left;
            public Int32 top;
            public Int32 right;
            public Int32 bottom;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        internal struct FormInfo1
        {
            [FieldOffset(0), MarshalAs(UnmanagedType.I4)]
            public uint Flags;
            [FieldOffset(4), MarshalAs(UnmanagedType.LPWStr)]
            public String pName;
            [FieldOffset(8)]
            public structSize Size;
            [FieldOffset(16)]
            public structRect ImageableArea;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi/* changed from CharSet=CharSet.Auto */)]
        internal struct structDevMode
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public String
                dmDeviceName;
            [MarshalAs(UnmanagedType.U2)]
            public short dmSpecVersion;
            [MarshalAs(UnmanagedType.U2)]
            public short dmDriverVersion;
            [MarshalAs(UnmanagedType.U2)]
            public short dmSize;
            [MarshalAs(UnmanagedType.U2)]
            public short dmDriverExtra;
            [MarshalAs(UnmanagedType.U4)]
            public int dmFields;
            [MarshalAs(UnmanagedType.I2)]
            public short dmOrientation;
            [MarshalAs(UnmanagedType.I2)]
            public short dmPaperSize;
            [MarshalAs(UnmanagedType.I2)]
            public short dmPaperLength;
            [MarshalAs(UnmanagedType.I2)]
            public short dmPaperWidth;
            [MarshalAs(UnmanagedType.I2)]
            public short dmScale;
            [MarshalAs(UnmanagedType.I2)]
            public short dmCopies;
            [MarshalAs(UnmanagedType.I2)]
            public short dmDefaultSource;
            [MarshalAs(UnmanagedType.I2)]
            public short dmPrintQuality;
            [MarshalAs(UnmanagedType.I2)]
            public short dmColor;
            [MarshalAs(UnmanagedType.I2)]
            public short dmDuplex;
            [MarshalAs(UnmanagedType.I2)]
            public short dmYResolution;
            [MarshalAs(UnmanagedType.I2)]
            public short dmTTOption;
            [MarshalAs(UnmanagedType.I2)]
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public String dmFormName;
            [MarshalAs(UnmanagedType.U2)]
            public short dmLogPixels;
            [MarshalAs(UnmanagedType.U4)]
            public int dmBitsPerPel;
            [MarshalAs(UnmanagedType.U4)]
            public int dmPelsWidth;
            [MarshalAs(UnmanagedType.U4)]
            public int dmPelsHeight;
            [MarshalAs(UnmanagedType.U4)]
            public int dmNup;
            [MarshalAs(UnmanagedType.U4)]
            public int dmDisplayFrequency;
            [MarshalAs(UnmanagedType.U4)]
            public int dmICMMethod;
            [MarshalAs(UnmanagedType.U4)]
            public int dmICMIntent;
            [MarshalAs(UnmanagedType.U4)]
            public int dmMediaType;
            [MarshalAs(UnmanagedType.U4)]
            public int dmDitherType;
            [MarshalAs(UnmanagedType.U4)]
            public int dmReserved1;
            [MarshalAs(UnmanagedType.U4)]
            public int dmReserved2;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct PRINTER_INFO_9
        {
            public IntPtr pDevMode;
        }

        [DllImport("winspool.Drv", EntryPoint = "AddFormW", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = true,
             CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern bool AddForm(
         IntPtr phPrinter,
            [MarshalAs(UnmanagedType.I4)] int level,
         ref FormInfo1 form);

        /*    This method is not used
                [DllImport("winspool.Drv", EntryPoint="SetForm", SetLastError=true,
                     CharSet=CharSet.Unicode, ExactSpelling=false,
                     CallingConvention=CallingConvention.StdCall), SuppressUnmanagedCodeSecurityAttribute()]
                internal static extern bool SetForm(IntPtr phPrinter, string paperName,
                    [MarshalAs(UnmanagedType.I4)] int level, ref FormInfo1 form);
        */
        [DllImport("winspool.Drv", EntryPoint = "DeleteForm", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall),
        SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern bool DeleteForm(
         IntPtr phPrinter,
            [MarshalAs(UnmanagedType.LPTStr)] string pName);

        [DllImport("kernel32.dll", EntryPoint = "GetLastError", SetLastError = false,
             ExactSpelling = true, CallingConvention = CallingConvention.StdCall),
        SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern Int32 GetLastError();

        [DllImport("GDI32.dll", EntryPoint = "CreateDC", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall),
        SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern IntPtr CreateDC([MarshalAs(UnmanagedType.LPTStr)]
			string pDrive,
            [MarshalAs(UnmanagedType.LPTStr)] string pName,
            [MarshalAs(UnmanagedType.LPTStr)] string pOutput,
            ref structDevMode pDevMode);

        [DllImport("GDI32.dll", EntryPoint = "ResetDC", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall),
        SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern IntPtr ResetDC(
         IntPtr hDC,
         ref structDevMode
            pDevMode);

        [DllImport("GDI32.dll", EntryPoint = "DeleteDC", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall),
        SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern bool DeleteDC(IntPtr hDC);

        [DllImport("winspool.Drv", EntryPoint = "SetPrinterA", SetLastError = true,
            CharSet = CharSet.Auto, ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern bool SetPrinter(
           IntPtr hPrinter,
           [MarshalAs(UnmanagedType.I4)] int level,
           IntPtr pPrinter,
           [MarshalAs(UnmanagedType.I4)] int command);

        /*
         LONG DocumentProperties(
           HWND hWnd,               // handle to parent window 
           HANDLE hPrinter,         // handle to printer object
           LPTSTR pDeviceName,      // device name
           PDEVMODE pDevModeOutput, // modified device mode
           PDEVMODE pDevModeInput,  // original device mode
           DWORD fMode              // mode options
           );
         */
        [DllImport("winspool.Drv", EntryPoint = "DocumentPropertiesA", SetLastError = true,
        ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int DocumentProperties(
           IntPtr hwnd,
           IntPtr hPrinter,
           [MarshalAs(UnmanagedType.LPStr)] string pDeviceName /* changed from String to string */,
           IntPtr pDevModeOutput,
           IntPtr pDevModeInput,
           int fMode
           );

        [DllImport("winspool.Drv", EntryPoint = "GetPrinterA", SetLastError = true,
        ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool GetPrinter(
           IntPtr hPrinter,
           int dwLevel /* changed type from Int32 */,
           IntPtr pPrinter,
           int dwBuf /* chagned from Int32*/,
           out int dwNeeded /* changed from Int32*/
           );

        // SendMessageTimeout tools
        [Flags]
        public enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0000,
            SMTO_BLOCK = 0x0001,
            SMTO_ABORTIFHUNG = 0x0002,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x0008
        }
        const int WM_SETTINGCHANGE = 0x001A;
        const int HWND_BROADCAST = 0xffff;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(
           IntPtr windowHandle,
           uint Msg,
           IntPtr wParam,
           IntPtr lParam,
           SendMessageTimeoutFlags flags,
           uint timeout,
           out IntPtr result
           );
      
        #endregion

        //获取系统默认打印机名称
        public static String GetDefaultPrinter()
        {

            int pcchBuffer = 0;
            if (GetDefaultPrinter(null, ref pcchBuffer))
            {
                return null;
            }
            int lastWin32Error = Marshal.GetLastWin32Error();
            if (lastWin32Error == ERROR_INSUFFICIENT_BUFFER)
            {
                StringBuilder pszBuffer = new StringBuilder(pcchBuffer);
                if (GetDefaultPrinter(pszBuffer, ref pcchBuffer))
                {
                    return pszBuffer.ToString();
                }
                lastWin32Error = Marshal.GetLastWin32Error();
            }
            if (lastWin32Error == ERROR_FILE_NOT_FOUND)
            {
                return null;
            }
            throw new Win32Exception(Marshal.GetLastWin32Error());

        }

        //添加自定义尺寸纸张到知道打印机
        /// <summary>
        /// Add the printer form to a printer 
        /// </summary>
        /// <param name="printerName">The printer name</param>
        /// <param name="paperName">Name of the printer form</param>
        /// <param name="widthMm">Width given in millimeters</param>
        /// <param name="heightMm">Height given in millimeters</param>
        public static void AddCustomPaperSize(string printerName, string paperName, float
            widthMm, float heightMm)
        {
            if (PlatformID.Win32NT == Environment.OSVersion.Platform)
            {
                // The code to add a custom paper size is different for Windows NT then it is
                // for previous versions of windows

                const int PRINTER_ACCESS_USE = 0x00000008;
                const int PRINTER_ACCESS_ADMINISTER = 0x00000004;
                const int FORM_PRINTER = 0x00000002;

                structPrinterDefaults defaults = new structPrinterDefaults();
                defaults.pDatatype = null;
                defaults.pDevMode = IntPtr.Zero;
                defaults.DesiredAccess = PRINTER_ACCESS_ADMINISTER | PRINTER_ACCESS_USE;

                IntPtr hPrinter = IntPtr.Zero;

                // Open the printer.
                if (OpenPrinter(printerName, out hPrinter, ref defaults))
                {
                    try
                    {
                        // delete the form incase it already exists
                        DeleteForm(hPrinter, paperName);
                        // create and initialize the FORM_INFO_1 structure
                        FormInfo1 formInfo = new FormInfo1();
                        formInfo.Flags = 0;
                        formInfo.pName = paperName;
                        // all sizes in 1000ths of millimeters
                        formInfo.Size.width = (int)(widthMm * 1000.0);
                        formInfo.Size.height = (int)(heightMm * 1000.0);
                        formInfo.ImageableArea.left = 0;
                        formInfo.ImageableArea.right = formInfo.Size.width;
                        formInfo.ImageableArea.top = 0;
                        formInfo.ImageableArea.bottom = formInfo.Size.height;
                        if (!AddForm(hPrinter, 1, ref formInfo))
                        {
                            StringBuilder strBuilder = new StringBuilder();
                            strBuilder.AppendFormat("Failed to add the custom paper size {0} to the printer {1}, System error number: {2}",
                                paperName, printerName, GetLastError());
                            throw new ApplicationException(strBuilder.ToString());
                        }

                        // INIT
                        const int DM_OUT_BUFFER = 2;
                        const int DM_IN_BUFFER = 8;
                        structDevMode devMode = new structDevMode();
                        IntPtr hPrinterInfo, hDummy;
                        PRINTER_INFO_9 printerInfo;
                        printerInfo.pDevMode = IntPtr.Zero;
                        int iPrinterInfoSize, iDummyInt;


                        // GET THE SIZE OF THE DEV_MODE BUFFER
                        int iDevModeSize = DocumentProperties(IntPtr.Zero, hPrinter, printerName, IntPtr.Zero, IntPtr.Zero, 0);

                        if (iDevModeSize < 0)
                            throw new ApplicationException("Cannot get the size of the DEVMODE structure.");

                        // ALLOCATE THE BUFFER
                        IntPtr hDevMode = Marshal.AllocCoTaskMem(iDevModeSize + 100);

                        // GET A POINTER TO THE DEV_MODE BUFFER 
                        int iRet = DocumentProperties(IntPtr.Zero, hPrinter, printerName, hDevMode, IntPtr.Zero, DM_OUT_BUFFER);

                        if (iRet < 0)
                            throw new ApplicationException("Cannot get the DEVMODE structure.");

                        // FILL THE DEV_MODE STRUCTURE
                        devMode = (structDevMode)Marshal.PtrToStructure(hDevMode, devMode.GetType());

                        // SET THE FORM NAME FIELDS TO INDICATE THAT THIS FIELD WILL BE MODIFIED
                        devMode.dmFields = 0x10000; // DM_FORMNAME 
                        // SET THE FORM NAME
                        devMode.dmFormName = paperName;

                        // PUT THE DEV_MODE STRUCTURE BACK INTO THE POINTER
                        Marshal.StructureToPtr(devMode, hDevMode, true);

                        // MERGE THE NEW CHAGES WITH THE OLD
                        iRet = DocumentProperties(IntPtr.Zero, hPrinter, printerName,
                                 printerInfo.pDevMode, printerInfo.pDevMode, DM_IN_BUFFER | DM_OUT_BUFFER);

                        if (iRet < 0)
                            throw new ApplicationException("Unable to set the orientation setting for this printer.");

                        // GET THE PRINTER INFO SIZE
                        GetPrinter(hPrinter, 9, IntPtr.Zero, 0, out iPrinterInfoSize);
                        if (iPrinterInfoSize == 0)
                            throw new ApplicationException("GetPrinter failed. Couldn't get the # bytes needed for shared PRINTER_INFO_9 structure");

                        // ALLOCATE THE BUFFER
                        hPrinterInfo = Marshal.AllocCoTaskMem(iPrinterInfoSize + 100);

                        // GET A POINTER TO THE PRINTER INFO BUFFER
                        bool bSuccess = GetPrinter(hPrinter, 9, hPrinterInfo, iPrinterInfoSize, out iDummyInt);

                        if (!bSuccess)
                            throw new ApplicationException("GetPrinter failed. Couldn't get the shared PRINTER_INFO_9 structure");

                        // FILL THE PRINTER INFO STRUCTURE
                        printerInfo = (PRINTER_INFO_9)Marshal.PtrToStructure(hPrinterInfo, printerInfo.GetType());
                        printerInfo.pDevMode = hDevMode;

                        // GET A POINTER TO THE PRINTER INFO STRUCTURE
                        Marshal.StructureToPtr(printerInfo, hPrinterInfo, true);

                        // SET THE PRINTER SETTINGS
                        bSuccess = SetPrinter(hPrinter, 9, hPrinterInfo, 0);

                        if (!bSuccess)
                            throw new Win32Exception(Marshal.GetLastWin32Error(), "SetPrinter() failed.  Couldn't set the printer settings");

                        // Tell all open programs that this change occurred.
                        SendMessageTimeout(
                           new IntPtr(HWND_BROADCAST),
                           WM_SETTINGCHANGE,
                           IntPtr.Zero,
                           IntPtr.Zero,
                           SendMessageTimeoutFlags.SMTO_NORMAL,
                           1000,
                           out hDummy);
                    }
                    finally
                    {
                        ClosePrinter(hPrinter);
                    }
                }
                else
                {
                    StringBuilder strBuilder = new StringBuilder();
                    strBuilder.AppendFormat("打开打印机{0}失败,系统错误: {1}",
                        printerName, GetLastError());
                    throw new ApplicationException(strBuilder.ToString());
                }
            }
            else
            {
                structDevMode pDevMode = new structDevMode();
                IntPtr hDC = CreateDC(null, printerName, null, ref pDevMode);
                if (hDC != IntPtr.Zero)
                {
                    const long DM_PAPERSIZE = 0x00000002L;
                    const long DM_PAPERLENGTH = 0x00000004L;
                    const long DM_PAPERWIDTH = 0x00000008L;
                    pDevMode.dmFields = (int)(DM_PAPERSIZE | DM_PAPERWIDTH | DM_PAPERLENGTH);
                    pDevMode.dmPaperSize = 256;
                    pDevMode.dmPaperWidth = (short)(widthMm * 1000.0);
                    pDevMode.dmPaperLength = (short)(heightMm * 1000.0);
                    ResetDC(hDC, ref pDevMode);
                    DeleteDC(hDC);
                }
            }
        }

        //添加自定义尺寸纸张到默认打印机
        /// <summary>
        /// Adds the printer form to the default printer
        /// </summary>
        /// <param name="paperName">Name of the printer form</param>
        /// <param name="widthMm">Width given in millimeters</param>
        /// <param name="heightMm">Height given in millimeters</param>
        public static void AddCustomPaperSizeToDefaultPrinter(string paperName, float widthMm, float heightMm)
        {
            AddCustomPaperSize(GetDefaultPrinter(), paperName, widthMm, heightMm);
        }
    }
}
