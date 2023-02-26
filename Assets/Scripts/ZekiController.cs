using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using ZekstersLab.MCP2221;

public class ZekiController : MonoBehaviour
{
    public enum BUTTON : byte
    {
        BUTTON_0 = 0,
        BUTTON_1 = 1,
        BUTTON_2 = 2,
        BUTTON_3 = 3,
        NUM_OF_BUTTONS
    }

    public enum BUTTON_STATE : byte
    {
        OFF = 0,
        ON = 1,
        UNKNOWN,
        NUM_OF_STATES
    }

    private Dictionary<BUTTON, MCP2221_Wrapper.Pins> _buttonMap = new Dictionary<BUTTON, MCP2221_Wrapper.Pins>();

    private static ZekiController instance;

    private MCP2221_Wrapper _mcpWrapper;
    private Coroutine _ledGlow;

    public static ZekiController Instance {
        get{
            if(instance == null) instance = FindObjectOfType<ZekiController>();
            return instance;
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        _buttonMap.Add(BUTTON.BUTTON_0, MCP2221_Wrapper.Pins.PIN_0);
        _buttonMap.Add(BUTTON.BUTTON_1, MCP2221_Wrapper.Pins.PIN_1);
        _buttonMap.Add(BUTTON.BUTTON_2, MCP2221_Wrapper.Pins.PIN_2);
        _buttonMap.Add(BUTTON.BUTTON_3, MCP2221_Wrapper.Pins.PIN_3);
        _mcpWrapper = new MCP2221_Wrapper();
    }

    // Update is called once per frame
    void Update()
    {
        _mcpWrapper.DoUpdate();
    }

    public BUTTON_STATE GetButtonState(BUTTON button)
    {
        if (button >= BUTTON.NUM_OF_BUTTONS) return BUTTON_STATE.UNKNOWN;
        
        _mcpWrapper.GetPin(_buttonMap[button], out byte switchValue);
        if (switchValue == 0)
        {
            return BUTTON_STATE.OFF;
        }
        else
        {
            return BUTTON_STATE.ON;
        }
    }

    public void TestLED()
    {
        if (_ledGlow == null)
        {
            _ledGlow = StartCoroutine(LEDGlow());
        }
    }

    IEnumerator LEDGlow()
    {
        float percent = 0f;

        while(true)
        {
            while ((percent <= 100) && (_mcpWrapper != null))
            {

                _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_0, percent);
                _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_1, percent);
                _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_2, percent);
                _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_3, percent);
                //yield return new WaitForSeconds(0.01f);
                yield return null;
                percent+=1;
            }

            while ((percent > 0) && (_mcpWrapper != null))
            {

                _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_0, percent);
                _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_1, percent);
                _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_2, percent);
                _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_3, percent);
                yield return null;
                //yield return new WaitForSeconds(0.01f);
                percent -=5;
            }

        }
        
    }

    private void OnDestroy()
    {
        _mcpWrapper.Destroy();
    }
}
