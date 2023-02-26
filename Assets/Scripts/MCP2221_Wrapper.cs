using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;
using System;
using UnityEditor.PackageManager;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using ZekstersLab.MCP2221;


namespace ZekstersLab.MCP2221
{
    public class MCP2221_Wrapper : IDisposable
    {
        #region Enums
        public enum Pins
        {
            PIN_0 = 0,
            PIN_1,
            PIN_2,
            PIN_3,
            NUM_OF_PINS
        }

        public enum Directions
        {
            OUTPUT = 0,
            INPUT,
            NUM_OF_DIRECTIONS
        }

        public enum LED : byte
        {
            LED_0 = 0,
            LED_1 = 1,
            LED_2 = 2,
            LED_3 = 3,
            NUM_OF_LEDS
        }

        #endregion

        #region DLL_Defines
        /********************************
        Error codes
        *********************************/
        public const int E_NO_ERR = 0;
        public const int E_ERR_UNKOWN_ERROR = -1;
        public const int E_ERR_CMD_FAILED = -2;
        public const int E_ERR_INVALID_HANDLE = -3;
        public const int E_ERR_INVALID_PARAMETER = -4;
        public const int E_ERR_INVALID_PASS = -5;
        public const int E_ERR_PASSWORD_LIMIT_REACHED = -6;
        public const int E_ERR_FLASH_WRITE_PROTECTED = -7;
        // null pointer received
        public const int E_ERR_NULL = -10;
        // destination string too small
        public const int E_ERR_DESTINATION_TOO_SMALL = -11;
        public const int E_ERR_INPUT_TOO_LARGE = -12;
        public const int E_ERR_FLASH_WRITE_FAILED = -13;
        public const int E_ERR_MALLOC = -14;



        //we tried to connect to a device with a non existent index
        public const int E_ERR_NO_SUCH_INDEX = -101;
        // no device matching the provided criteria was found
        public const int E_ERR_DEVICE_NOT_FOUND = -103;

        // one of the internal buffers of the function was too small
        public const int E_ERR_INTERNAL_BUFFER_TOO_SMALL = -104;
        // an error occurred when trying to get the device handle
        public const int E_ERR_OPEN_DEVICE_ERROR = -105;
        // connection already opened
        public const int E_ERR_CONNECTION_ALREADY_OPENED = -106;

        public const int E_ERR_CLOSE_FAILED = -107;


        /******* I2C errors *******/
        public const int E_ERR_INVALID_SPEED = -401;
        public const int E_ERR_SPEED_NOT_SET = -402;
        public const int E_ERR_INVALID_BYTE_NUMBER = -403;
        public const int E_ERR_INVALID_ADDRESS = -404;
        public const int E_ERR_I2C_BUSY = -405;
        //mcp2221 signaled an error during the i2c read operation
        public const int E_ERR_I2C_READ_ERROR = -406;
        public const int E_ERR_ADDRESS_NACK = -407;
        public const int E_ERR_TIMEOUT = -408;
        public const int E_ERR_TOO_MANY_RX_BYTES = -409;
        //could not copy the data received from the slave into the provided buffer;
        public const int E_ERR_COPY_RX_DATA_FAILED = -410;
        // failed to copy the data into the HID buffer
        public const int E_ERR_COPY_TX_DATA_FAILED = -412;
        // The i2c engine (inside mcp2221) was already idle. The cancellation command had no effect.
        public const int E_ERR_NO_EFFECT = -411;
        public const int E_ERR_INVALID_PEC = -413;
        // The slave sent a different value for the block size(byte count) than we expected
        public const int E_ERR_BLOCK_SIZE_MISMATCH = -414;


        public const int E_ERR_RAW_TX_TOO_LARGE = -301;
        public const int E_ERR_RAW_TX_COPYFAILED = -302;
        public const int E_ERR_RAW_RX_COPYFAILED = -303;



        /***********************************
        Constants
        ************************************/
        public const byte FLASH_SETTINGS = 0;
        public const byte RUNTIME_SETTINGS = 1;

        public const byte NO_CHANGE = 0xFF;

        //GPIO settings
        public const byte MCP2221_GPFUNC_IO = 0;

        public const byte MCP2221_GP_SSPND = 1;
        public const byte MCP2221_GP_CLOCK_OUT = 1;
        public const byte MCP2221_GP_USBCFG = 1;
        public const byte MCP2221_GP_LED_I2C = 1;

        public const byte MCP2221_GP_LED_UART_RX = 2;
        public const byte MCP2221_GP_ADC = 2;

        public const byte MCP2221_GP_LED_UART_TX = 3;
        public const byte MCP2221_GP_DAC = 3;

        public const byte MCP2221_GP_IOC = 4;

        public const byte MCP2221_GPDIR_INPUT = 1;
        public const byte MCP2221_GPDIR_OUTPUT = 0;

        public const byte MCP2221_GPVAL_HIGH = 1;
        public const byte MCP2221_GPVAL_LOW = 0;

        public const byte INTERRUPT_NONE = 0;
        public const byte INTERRUPT_POSITIVE_EDGE = 1;
        public const byte INTERRUPT_NEGATIVE_EDGE = 2;
        public const byte INTERRUPT_BOTH_EDGES = 3;

        public const byte VREF_VDD = 0;
        public const byte VREF_1024V = 1;
        public const byte VREF_2048V = 2;
        public const byte VREF_4096V = 3;

        public const byte MCP2221_USB_BUS = 0x80;
        public const byte MCP2221_USB_SELF = 0x40;
        public const byte MCP2221_USB_REMOTE = 0x20;

        public const byte MCP2221_PASS_ENABLE = 1;
        public const byte MCP2221_PASS_DISABLE = 0;
        public const byte MCP2221_PASS_CHANGE = 0xff;
        #endregion

        #region Constants
        private const int DEFAULT_VID = 0x04D8;
        private const int DEFAULT_PID = 0x00DD;

        private const byte GPIO_AW9523_WRITE_ADDRESS = 0xB2;
        private const byte GPIO_AW9523_READ_ADDRESS = 0xB3;

        private const byte GPIO_AW9523_P0_OUTPUT_STATE_ADDRESS = 0x02;
        private const byte GPIO_AW9523_P0_CONFIG_ADDRESS = 0x04;
        private const byte GPIO_AW9523_ID_ADDRESS = 0x10;
        private const byte GPIO_AW9523_GLOBAL_CTRL_ADDRESS = 0x11;
        private const byte GPIO_AW9523_P0_MODE_ADDRESS = 0x12;
        private const byte GPIO_AW9523_P0_0_LED_ADDRESS = 0x24;
        private const byte GPIO_AW9523_P0_1_LED_ADDRESS = 0x25;
        private const byte GPIO_AW9523_P0_2_LED_ADDRESS = 0x26;
        private const byte GPIO_AW9523_P0_3_LED_ADDRESS = 0x27;


        private const byte GPIO_AW9523_P0_CONFIG_OUTPUTS = 0x00;
        //private const byte GPIO_AW9523_GLOBAL_CTRL_SETTING = 0x00;
        private const byte GPIO_AW9523_GLOBAL_CTRL_SETTING = 0x01;
        private const byte GPIO_AW9523_P0_MODE = 0x00; //LED Mode
        private const uint GPIO_AW9523_I2C_SPEED = 200000; //200 kbps = 200khz
        private const float GPIO_AW9523_LED_ONE_PERCENT = 255 / 100;

        #endregion

        #region Unity_Code

        private uint _numOfDevices = 0;
        //private MCP2221SafeHandle _uVPHandle = Marshal.AllocHGlobal(MCP2221SafeHandle.Size);
        private MCP2221SafeHandle _uVPHandle;
        private byte[] _pinFunctions = new byte[4];
        private byte[] _pinDirections = new byte[4];
        private byte[] _pinValues = new byte[4];
        private int lastError = E_NO_ERR;
        private bool disposedValue;

        //External GPIO / LED Controller vars
        private byte _AW9523_ID = 0;
        private byte[] _readData = new byte[2];
        private byte[] _writeData = new byte[2];


        public MCP2221_Wrapper()
        {

            StringBuilder str = new StringBuilder(255);

            if (CheckResultForError(MCP2221.Mcp2221_GetLibraryVersion(str), "Mcp2221_GetLibraryVersion")) return;
            Debug.Log("[Mcp2221] Version: " + str);

            if (CheckResultForError(MCP2221.Mcp2221_GetConnectedDevices(DEFAULT_VID, DEFAULT_PID, ref _numOfDevices), "Mcp2221_GetConnectedDevices")) return;
            Debug.Log("[Mcp2221] Number of connected devices: " + _numOfDevices.ToString());

            _uVPHandle = MCP2221.Mcp2221_OpenByIndex(DEFAULT_VID, DEFAULT_PID, 0);
            if (CheckResultForError(MCP2221.Mcp2221_GetLastError(), "Mcp2221_OpenByIndex")) return;
            Debug.Log("[Mcp2221] Device handle: " + _uVPHandle.ToString());

            if (CheckResultForError(MCP2221.Mcp2221_GetGpioSettings(_uVPHandle, RUNTIME_SETTINGS, _pinFunctions, _pinDirections, _pinValues), "Mcp2221_GetGpioSettings")) return;
            Debug.Log("[Mcp2221] Read Pin Functions: " + BitConverter.ToString(_pinFunctions));
            Debug.Log("[Mcp2221] Read Pin Directions: " + BitConverter.ToString(_pinDirections));
            Debug.Log("[Mcp2221] Read Pin Values: " + BitConverter.ToString(_pinValues));

            _pinFunctions[0] = MCP2221_GPFUNC_IO;
            _pinDirections[0] = MCP2221_GPDIR_INPUT;
            _pinValues[0] = NO_CHANGE;

            _pinFunctions[1] = MCP2221_GPFUNC_IO;
            _pinDirections[1] = MCP2221_GPDIR_INPUT;
            _pinValues[1] = NO_CHANGE;

            _pinFunctions[2] = MCP2221_GPFUNC_IO;
            _pinDirections[2] = MCP2221_GPDIR_INPUT;
            _pinValues[2] = NO_CHANGE;

            _pinFunctions[3] = MCP2221_GPFUNC_IO;
            _pinDirections[3] = MCP2221_GPDIR_INPUT;
            _pinValues[3] = NO_CHANGE;


            if (CheckResultForError(MCP2221.Mcp2221_SetGpioSettings(_uVPHandle, RUNTIME_SETTINGS, _pinFunctions, _pinDirections, _pinValues), "Mcp2221_SetGpioSettings")) return;
            Debug.Log("[Mcp2221] Write Pin Functions: " + BitConverter.ToString(_pinFunctions));
            Debug.Log("[Mcp2221] Write Pin Directions: " + BitConverter.ToString(_pinDirections));
            Debug.Log("[Mcp2221] Write Pin Values: " + BitConverter.ToString(_pinValues));

            if (CheckResultForError(MCP2221.Mcp2221_GetGpioSettings(_uVPHandle, RUNTIME_SETTINGS, _pinFunctions, _pinDirections, _pinValues), "Mcp2221_GetGpioSettings")) return;
            Debug.Log("[Mcp2221] Read Pin Functions: " + BitConverter.ToString(_pinFunctions));
            Debug.Log("[Mcp2221] Read Pin Directions: " + BitConverter.ToString(_pinDirections));
            Debug.Log("[Mcp2221] Read Pin Values: " + BitConverter.ToString(_pinValues));

            //_pinValues[0] = MCP2221_GPVAL_HIGH;
            //_pinValues[1] = NO_CHANGE;
            //_pinValues[2] = NO_CHANGE;
            //_pinValues[3] = NO_CHANGE;

            //if (CheckResultForError(MCP2221.Mcp2221_SetGpioSettings(_uVPHandle, RUNTIME_SETTINGS, _pinFunctions, _pinDirections, _pinValues), "Mcp2221_SetGpioSettings")) return;
            //Debug.Log("[Mcp2221] Write Pin Functions: " + BitConverter.ToString(_pinFunctions));
            //Debug.Log("[Mcp2221] Write Pin Directions: " + BitConverter.ToString(_pinDirections));
            //Debug.Log("[Mcp2221] Write Pin Values: " + BitConverter.ToString(_pinValues));

            //if (CheckResultForError(MCP2221.Mcp2221_GetGpioSettings(_uVPHandle, RUNTIME_SETTINGS, _pinFunctions, _pinDirections, _pinValues), "Mcp2221_GetGpioSettings")) return;
            //Debug.Log("[Mcp2221] Read Pin Functions: " + BitConverter.ToString(_pinFunctions));
            //Debug.Log("[Mcp2221] Read Pin Directions: " + BitConverter.ToString(_pinDirections));
            //Debug.Log("[Mcp2221] Read Pin Values: " + BitConverter.ToString(_pinValues));

            //if (CheckResultForError(Mcp2221_Close(_uVPHandle), "Mcp2221_Close", SetLastError = true)) return;
            //Debug.Log("[Mcp2221] Closed handle: " + _uVPHandle.ToString());

            SetupI2C();
        }

        private void SetupI2C()
        {
            if (_uVPHandle == null) return;
            if (_uVPHandle.IsInvalid) return;

            _AW9523_ID = 0;



            Debug.Log("Setting I2C speed = " + GPIO_AW9523_I2C_SPEED.ToString() + " hz / bps");
            if (CheckResultForError(MCP2221.Mcp2221_SetSpeed(_uVPHandle, GPIO_AW9523_I2C_SPEED), "Mcp2221_SetSpeed")) return;

            Debug.Log("Reading AW9523 ID");
            _writeData[0] = GPIO_AW9523_ID_ADDRESS;
            if (CheckResultForError(MCP2221.Mcp2221_I2cWrite(_uVPHandle, 1, GPIO_AW9523_WRITE_ADDRESS, 0, _writeData), "Mcp2221_I2cWrite - preRead")) return;
            if (CheckResultForError(MCP2221.Mcp2221_I2cRead(_uVPHandle, 1, GPIO_AW9523_READ_ADDRESS, 0, _readData), "Mcp2221_I2cRead - read")) return;
            _AW9523_ID = _readData[0];
            Debug.Log("AW9523 ID: " + _AW9523_ID.ToString("X"));

            if (!I2C_Send(GPIO_AW9523_P0_CONFIG_ADDRESS, GPIO_AW9523_P0_CONFIG_OUTPUTS, "P0 Config")) return;
            if (!I2C_Send(GPIO_AW9523_GLOBAL_CTRL_ADDRESS, GPIO_AW9523_GLOBAL_CTRL_SETTING, "Global Control")) return;
            if (!I2C_Send(GPIO_AW9523_P0_MODE_ADDRESS, GPIO_AW9523_P0_MODE, "P0 LED Mode")) return;

            for (LED i = LED.LED_0; i < LED.NUM_OF_LEDS; i++)
            {
                SetLEDBrightness(i, 0f);
            }

        }

        private bool I2C_Send(byte registerAddress, byte data, string registerName)
        {
            if (_uVPHandle == null) return false;
            if (_uVPHandle.IsInvalid) return false;

            _writeData[0] = registerAddress;
            _writeData[1] = data;
            Debug.Log("Setting " + registerName + " register: " + BitConverter.ToString(_writeData));
            if (CheckResultForError(MCP2221.Mcp2221_I2cWrite(_uVPHandle, (uint)_writeData.Length, GPIO_AW9523_WRITE_ADDRESS, 0, _writeData), "Mcp2221_I2cWrite")) return false;

            return true;
        }

        public bool SetLEDBrightness(LED led, float percent)
        {
            if (_uVPHandle == null) return false;
            if (_uVPHandle.IsInvalid) return false;

            //convert percentage to byte 0 - 255
            percent = Mathf.Clamp(percent, 0, 100);
            byte data = (byte)(Mathf.CeilToInt(percent * GPIO_AW9523_LED_ONE_PERCENT));
            Debug.Log("Setting LED " + ((byte)led).ToString() + " to " + percent + "%");
            return I2C_Send((byte)(GPIO_AW9523_P0_0_LED_ADDRESS + ((byte)led)), data, "LED P0_" + ((byte)led).ToString());
        }

        public void Destroy()
        {
            Debug.Log("Start of MCP2221_Wrapper OnDestroy");
            for (int i = 0; i < _pinFunctions.Length; i++)
            {
                if ((_pinFunctions[i] == MCP2221_GPFUNC_IO) && (_pinDirections[i] == MCP2221_GPDIR_OUTPUT))
                {
                    _pinValues[i] = MCP2221_GPVAL_LOW;
                }
                else
                {
                    _pinValues[i] = NO_CHANGE;
                }
            }

            CheckResultForError(MCP2221.Mcp2221_SetGpioValues(_uVPHandle, _pinValues), "Mcp2221_SetGpioValues");

            for (LED i = LED.LED_0; i < LED.NUM_OF_LEDS; i++)
            {
                SetLEDBrightness(i, 0f);
            }

            Dispose();

            Debug.Log("[Mcp2221] Closed All");
            //No longer required due to use of MCP2221SafeHandle class
            //Debug.Log("MCP2221_Wrapper OnDestroy: About to Marshal.FreeHGlobal(_uVPHandle)");
            //Marshal.FreeHGlobal(_uVPHandle);
            //Debug.Log("MCP2221_Wrapper OnDestroy: Completed Marshal.FreeHGlobal(_uVPHandle)");
        }

        private bool CheckResultForError(int result, string funcName)
        {
            bool returnVal = false;
            lastError = result;
            if (result != E_NO_ERR)
            {
                Debug.Log("Error: " + funcName + " returned: " + result.ToString());
                returnVal = true;
            }
            else
            {
                //Do nothing
            }

            return returnVal;
        }

        // DoUpdate should be called once per frame
        public void DoUpdate()
        {
            if (_uVPHandle == null) return;

            if (_uVPHandle.IsInvalid)
            {
                Debug.Log("UuVPHandle is disposed - connection to MCP2221 closed");
                return;
            }
            if (CheckResultForError(MCP2221.Mcp2221_GetGpioValues(_uVPHandle, _pinValues), "Mcp2221_GetGpioValues")) return;

            //if(_pinValues[2] == MCP2221_GPVAL_HIGH)
            //{

            //}

            //_pinValues[0] = _pinValues[2];
            //_pinValues[1] = NO_CHANGE;
            //_pinValues[2] = NO_CHANGE;
            //_pinValues[3] = NO_CHANGE;
            //if (CheckResultForError(Mcp2221_SetGpioValues(_uVPHandle, _pinValues), "Mcp2221_SetGpioValues", SetLastError = true)) return;
        }

        public bool GetPin(Pins pin, out byte pinValue)
        {
            pinValue = _pinValues[(int)pin];
            return (lastError == E_NO_ERR);
        }

        public bool SetPin(Pins pin, byte pinValue)
        {
            if (_uVPHandle == null) return false;
            if (_uVPHandle.IsInvalid)
            {
                Debug.Log("UuVPHandle is disposed - connection to MCP2221 closed");
                return false;
            }
            bool result;
            if (_pinDirections[(int)pin] == MCP2221_GPDIR_OUTPUT)
            {
                for (int i = 0; i < (int)Pins.NUM_OF_PINS; i++)
                {
                    if (i == (int)pin)
                    {
                        _pinValues[i] = pinValue;
                    }
                    else
                    {
                        _pinValues[i] = NO_CHANGE;
                    }
                }


                result = !CheckResultForError(MCP2221.Mcp2221_SetGpioValues(_uVPHandle, _pinValues), "Mcp2221_SetGpioValues");
            }
            else
            {
                result = false;
            }
            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            Debug.Log("MCP2221_Wrapper: Dispose called with disposing = " + disposing.ToString());
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_uVPHandle != null && !_uVPHandle.IsInvalid)
                    {
                        Debug.Log("MCP2221_Wrapper.Dispose: about to call _uVPHandle.Dispose() with _uVPHandle = " + _uVPHandle.ToString());
                        _uVPHandle.Dispose();
                    }
                    else
                    {
                        Debug.Log("MCP2221_Wrapper.Dispose: could not call _uVPDispose as it was null or invalid");
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

