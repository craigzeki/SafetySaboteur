//MySafeFileHandle class modified from Microsoft: https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle?view=net-7.0


using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

namespace ZekstersLab.MCP2221
{
    internal class MCP2221SafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        // Create a SafeHandle, informing the base class
        // that this SafeHandle instance "owns" the handle,
        // and therefore SafeHandle should call
        // our ReleaseHandle method when the SafeHandle
        // is no longer in use.
        private MCP2221SafeHandle()
            : base(true)
        {
        }
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        override protected bool ReleaseHandle()
        {
            Debug.Log("MCP2221SafeHandle: Closing Handle");
            // Here, we must obey all rules for constrained execution regions.
            return MCP2221.Mcp2221_Close(handle) == 0 ? true: false;
            // If ReleaseHandle failed, it can be reported via the
            // "releaseHandleFailed" managed debugging assistant (MDA).  This
            // MDA is disabled by default, but can be enabled in a debugger
            // or during testing to diagnose handle corruption problems.
            // We do not throw an exception because most code could not recover
            // from the problem.
        }
    }

    //[SuppressUnmanagedCodeSecurity()] //Per Microsoft documentation: Allow managed code to call into unmanaged code without a stack walk - can cause security issues
    internal static class MCP2221
    {
        #region DLL_Import
#if UNITY_IPHONE
    [DllImport("__Internal", SetLastError = true)]
#else
        [DllImport("mcp2221_dll_um.dll", SetLastError = true)]
#endif
        internal extern static int Mcp2221_GetLibraryVersion(StringBuilder str);
#if UNITY_IPHONE
    [DllImport("__Internal", SetLastError = true)]
#else
        [DllImport("mcp2221_dll_um.dll", SetLastError = true)]
#endif
        internal extern static int Mcp2221_GetConnectedDevices(uint vid, uint pid, ref uint numberOfDevices);
#if UNITY_IPHONE
    [DllImport("__Internal", SetLastError = true)]
#else
        [DllImport("mcp2221_dll_um.dll", SetLastError = true)]
#endif
        internal extern static MCP2221SafeHandle Mcp2221_OpenByIndex(uint vid, uint pid, uint index);
#if UNITY_IPHONE
    [DllImport("__Internal", SetLastError = true)]
#else
        [DllImport("mcp2221_dll_um.dll", SetLastError = true)]
#endif
        internal extern static int Mcp2221_GetLastError();
#if UNITY_IPHONE
    [DllImport("__Internal", SetLastError = true)]
#else
        [DllImport("mcp2221_dll_um.dll", SetLastError = true)]
#endif
        internal extern static int Mcp2221_GetGpioSettings(MCP2221SafeHandle handle, byte whichToGet, byte[] pinFunctions, byte[] pinDirections, byte[] pinValues);
#if UNITY_IPHONE
    [DllImport("__Internal", SetLastError = true)]
#else
        [DllImport("mcp2221_dll_um.dll", SetLastError = true)]
#endif
        internal extern static int Mcp2221_SetGpioSettings(MCP2221SafeHandle handle, byte whichToSet, byte[] pinFunctions, byte[] pinDirections, byte[] pinValues);
#if UNITY_IPHONE
    [DllImport("__Internal", SetLastError = true)]
#else
        [DllImport("mcp2221_dll_um.dll", SetLastError = true)]
#endif
        internal extern static int Mcp2221_GetGpioValues(MCP2221SafeHandle handle, byte[] pinValues);
#if UNITY_IPHONE
    [DllImport("__Internal", SetLastError = true)]
#else
        [DllImport("mcp2221_dll_um.dll", SetLastError = true)]
#endif
        internal extern static int Mcp2221_SetGpioValues(MCP2221SafeHandle handle, byte[] pinValues);
#if UNITY_IPHONE
    [DllImport("__Internal", SetLastError = true)]
#else
        [DllImport("mcp2221_dll_um.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
#endif
        internal extern static int Mcp2221_Close(IntPtr handle);

        //*** NOT ALLOWING THIS FUNCTION DUE TO INABILITY TO THEN PROPERLY FREE HANDLE) ***
        //#if UNITY_IPHONE
        //    [DllImport("__Internal", SetLastError = true)]
        //#else
        //        [DllImport("mcp2221_dll_um.dll", SetLastError = true)]
        //        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        //#endif
        //        internal extern static int Mcp2221_CloseAll();
#if UNITY_IPHONE
    [DllImport("__Internal", SetLastError = true)]
#else
        [DllImport("mcp2221_dll_um.dll", SetLastError = true)]
#endif
        internal extern static int Mcp2221_SetSpeed(MCP2221SafeHandle handle, uint speed);

#if UNITY_IPHONE
    [DllImport("__Internal", SetLastError = true)]
#else
        [DllImport("mcp2221_dll_um.dll", SetLastError = true)]
#endif
        internal extern static int Mcp2221_SetAdvancedCommParams(MCP2221SafeHandle handle, byte timeout, byte maxRetries);

#if UNITY_IPHONE
    [DllImport("__Internal", SetLastError = true)]
#else
        [DllImport("mcp2221_dll_um.dll", SetLastError = true)]
#endif
        internal extern static int Mcp2221_CancelCurrentTransfer(MCP2221SafeHandle handle);

#if UNITY_IPHONE
    [DllImport("__Internal", SetLastError = true)]
#else
        [DllImport("mcp2221_dll_um.dll", SetLastError = true)]
#endif
        internal extern static int Mcp2221_I2cRead(MCP2221SafeHandle handle, uint bytesToRead, byte slaveAddress, byte use7bitAddress, byte[] i2cData);

#if UNITY_IPHONE
    [DllImport("__Internal", SetLastError = true)]
#else
        [DllImport("mcp2221_dll_um.dll", SetLastError = true)]
#endif
        internal extern static int Mcp2221_I2cWrite(MCP2221SafeHandle handle, uint bytesToWrite, byte slaveAddress, byte use7bitAddress, byte[] i2cTxData);
        #endregion
    }
}
