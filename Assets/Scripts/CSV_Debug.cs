using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace ZekstersLab.DebugLib
{
    public class CSV_Debug
    {
        public enum CSV_DebugState
        {
            DISABLED = 0,
            ENABLED = 1,
            NUM_OF_STATES
        }
        private string _filename;
        private StreamWriter _streamWriter;
        private CSV_DebugState _state;

        public CSV_Debug(CSV_DebugState state)
        {
            _state = state;
        }

        public void SetFilename(string filename)
        {
            if (_state == CSV_DebugState.DISABLED) return;
            _filename = Application.dataPath + "/" + filename + ".csv";
            _streamWriter = new StreamWriter(_filename);
            if (_streamWriter != null) UnityEngine.Debug.Log("Opened file: " + _filename);
        }

        public void SetHeaders(List<string> headers)
        {
            if (_state == CSV_DebugState.DISABLED) return;
            if (_streamWriter == null) return;
            string headerLine = string.Join(",", headers);

            _streamWriter.WriteLine(headerLine);
        }

        public void SetHeaders(string[] headers)
        {
            if (_state == CSV_DebugState.DISABLED) return;
            if (_streamWriter == null) return;
            string headerLine = string.Join(",", headers);

            _streamWriter.WriteLine(headerLine);
        }

        public void WriteData(List<string> data)
        {
            if (_state == CSV_DebugState.DISABLED) return;
            string dataline = string.Join(",", data);
            _streamWriter.WriteLine(dataline);
        }

        public void WriteData(string[] data)
        {
            if (_state == CSV_DebugState.DISABLED) return;
            string dataline = string.Join(",", data);
            _streamWriter.WriteLine(dataline);
        }

        public void CloseFile()
        {
            if (_streamWriter == null) return;
            _streamWriter.Close();
            UnityEngine.Debug.Log("Closed file: " + _filename);
        }

        ~CSV_Debug()
        {
            CloseFile();
        }
    }
}
