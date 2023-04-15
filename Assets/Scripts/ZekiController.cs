using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
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

    public  enum BUTTON_LED_STATE : int
    {
        LED_OFF = 0,
        LED_ON,
        LED_GLOW,
        NUM_OF_STATES
    }

    public enum BUTTON_STATE : byte
    {
        OFF = 0,
        ON = 1,
        UNKNOWN,
        NUM_OF_STATES
    }

    [SerializeField] private BUTTON_STATE[] _buttonValues = new BUTTON_STATE[(int)BUTTON.NUM_OF_BUTTONS];
    [SerializeField] private BUTTON_LED_STATE[] _buttonLedState = new BUTTON_LED_STATE[(int)BUTTON.NUM_OF_BUTTONS];

    private Dictionary<BUTTON, MCP2221_Wrapper.Pins> _buttonMap = new Dictionary<BUTTON, MCP2221_Wrapper.Pins>();
    private Dictionary<BUTTON, MCP2221_Wrapper.LED> _ledMap = new Dictionary<BUTTON, MCP2221_Wrapper.LED>();

    private static ZekiController instance;

    private MCP2221_Wrapper _mcpWrapper;
    private Coroutine _controllerUpdateCoroutine;
    private Coroutine _controllerStartupCoroutine;
    private bool _controllerReady = false;

    public static ZekiController Instance {
        get{
            if(instance == null) instance = FindObjectOfType<ZekiController>();
            return instance;
        }
    }
    // Start is called before the first frame update
    void Awake()
    {

        for (int i = 0; i < (int)BUTTON.NUM_OF_BUTTONS; i++)
        {
            _buttonValues[i] = BUTTON_STATE.UNKNOWN;
            _buttonLedState[i] = BUTTON_LED_STATE.LED_OFF;
        }

        _buttonMap.Add(BUTTON.BUTTON_0, MCP2221_Wrapper.Pins.PIN_0);
        _buttonMap.Add(BUTTON.BUTTON_1, MCP2221_Wrapper.Pins.PIN_1);
        _buttonMap.Add(BUTTON.BUTTON_2, MCP2221_Wrapper.Pins.PIN_2);
        _buttonMap.Add(BUTTON.BUTTON_3, MCP2221_Wrapper.Pins.PIN_3);

        _ledMap.Add(BUTTON.BUTTON_0, MCP2221_Wrapper.LED.LED_0);
        _ledMap.Add(BUTTON.BUTTON_1, MCP2221_Wrapper.LED.LED_1);
        _ledMap.Add(BUTTON.BUTTON_2, MCP2221_Wrapper.LED.LED_2);
        _ledMap.Add(BUTTON.BUTTON_3, MCP2221_Wrapper.LED.LED_3);

        _mcpWrapper = new MCP2221_Wrapper();
        
    }

    void Start()
    {
        StartController();
    }

    private void Update()
    {
        if(!_controllerReady)
        {
            if(_mcpWrapper.SetupComplete)
            {  
                if(_controllerStartupCoroutine == null) _controllerStartupCoroutine = StartCoroutine(DoStartupLED());
            }
        }

        for(int i = 0; i < (int)BUTTON.NUM_OF_BUTTONS; i++)
        {
            _buttonValues[i] = GetButtonState((BUTTON)i);
            
        }
    }

    IEnumerator DoStartupLED()
    {
        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_0, 100);
        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_1, 0);
        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_2, 0);
        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_3, 0);
        yield return new WaitForSeconds(0.5f);

        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_0, 0);
        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_1, 100);
        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_2, 0);
        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_3, 0);
        yield return new WaitForSeconds(0.5f);

        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_0, 0);
        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_1, 0);
        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_2, 0);
        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_3, 100);
        yield return new WaitForSeconds(0.5f);

        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_0, 0);
        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_1, 0);
        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_2, 100);
        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_3, 0);
        yield return new WaitForSeconds(0.5f);

        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_0, 0);
        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_1, 0);
        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_2, 0);
        _mcpWrapper.SetLEDBrightness(MCP2221_Wrapper.LED.LED_3, 0);
        _controllerReady = true;
    }

    public BUTTON_STATE GetButtonState(BUTTON button)
    {
        if (!_controllerReady) return BUTTON_STATE.UNKNOWN;
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


    public void SetButtonLEDState(BUTTON button, BUTTON_LED_STATE state)
    {
        if (button >= BUTTON.NUM_OF_BUTTONS) return;
        if (state >= BUTTON_LED_STATE.NUM_OF_STATES) return;

        _buttonLedState[(int)button] = state;

    }

    public void StartController()
    {
        if (_controllerUpdateCoroutine == null)
        {
            _controllerUpdateCoroutine = StartCoroutine(ControllerUpdate());
        }
    }

    IEnumerator ControllerUpdate()
    {
        float percent = 0f;
        
        while (true)
        {
            if(_controllerReady)
            {
                while ((percent <= 25) && (_mcpWrapper != null))
                {
                    _mcpWrapper.DoUpdate();

                    for(BUTTON button = BUTTON.BUTTON_0; button < BUTTON.NUM_OF_BUTTONS; button++)
                    {
                        switch (_buttonLedState[(int)button])
                        {
                            case BUTTON_LED_STATE.LED_OFF:
                                _mcpWrapper.SetLEDBrightness(_ledMap[button], 0);
                                break;
                            case BUTTON_LED_STATE.LED_ON:
                                _mcpWrapper.SetLEDBrightness(_ledMap[button], 100);
                                break;
                            case BUTTON_LED_STATE.LED_GLOW:
                                _mcpWrapper.SetLEDBrightness(_ledMap[button], percent);
                                break;
                            case BUTTON_LED_STATE.NUM_OF_STATES:
                            default:
                                break;
                        }
                    }
                    //yield return new WaitForSeconds(0.01f);
                    yield return null;
                    percent += 1;
                }

                while ((percent > 0) && (_mcpWrapper != null))
                {
                    _mcpWrapper.DoUpdate();
                    for (BUTTON button = BUTTON.BUTTON_0; button < BUTTON.NUM_OF_BUTTONS; button++)
                    {
                        switch (_buttonLedState[(int)button])
                        {
                            case BUTTON_LED_STATE.LED_OFF:
                                _mcpWrapper.SetLEDBrightness(_ledMap[button], 0);
                                break;
                            case BUTTON_LED_STATE.LED_ON:
                                _mcpWrapper.SetLEDBrightness(_ledMap[button], 100);
                                break;
                            case BUTTON_LED_STATE.LED_GLOW:
                                _mcpWrapper.SetLEDBrightness(_ledMap[button], percent);
                                break;
                            case BUTTON_LED_STATE.NUM_OF_STATES:
                            default:
                                break;
                        }
                    }
                    yield return null;
                    //yield return new WaitForSeconds(0.01f);
                    percent -= 5;
                }
            }
            else
            {
                yield return null;
            }


        }

    }

    private void OnDestroy()
    {
        _mcpWrapper.Destroy();
    }
}
