using RPGCharacterAnims;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MaterializeOnSpawn : MonoBehaviour
{
    [SerializeField] private int _materializerMatSlot = 0;
    [SerializeField] private RPGCharacterController _RPGCharacterController;
    [SerializeField] private AnimationCurve _clipCurve;
    [SerializeField] private AudioSource _spawnAudioSource;

    private Material _thisMat;
    private Coroutine _materializeCoroutine;
    private List<Color> _originalColors = new List<Color>();
    private Color _transparent = new Color(0, 0, 0, 0);
    private Material[] _materials;
    private Renderer _renderer;
    private Material[] _tempMat = new Material[1];

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _materials = new Material[_renderer.materials.Length];
        _materials = _renderer.materials;
        //_renderer.materials.CopyTo(_materials, 0);
        _thisMat = _materials[_materializerMatSlot];
        _tempMat[0] = _thisMat;
        //foreach(Material mat in _materials)
        //{
        //    //_originalColors.Add(mat.color);
        //    if (mat == _thisMat) continue; //don't make the materializer transparent!
        //    //mat.color = _transparent;

        //}

        if (_thisMat != null)
        {
            _thisMat.SetFloat("_Clip", _clipCurve.Evaluate(0));
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (_thisMat == null) return;
        
        _materializeCoroutine = StartCoroutine(DoMaterialize());
        if (_spawnAudioSource != null) _spawnAudioSource.Play();
    }

    IEnumerator DoMaterialize()
    {
        float duration = 0;
        bool halfPointMet = false;

        
        
        _renderer.materials = _tempMat;
        //_tempMat.CopyTo(_renderer.materials, 0);

        //lock the characters movement and animations for the duration of the materializer
        _RPGCharacterController.Lock(true, true, true, 0, _clipCurve.keys[_clipCurve.length - 1].time);

        while (duration < _clipCurve.keys[_clipCurve.length - 1].time)
        {
            _thisMat.SetFloat("_Clip", _clipCurve.Evaluate(duration));
            if((!halfPointMet) && (duration > (_clipCurve.keys[_clipCurve.length - 1].time/2)))
            {
                halfPointMet = true;
                ////restore original colors
                //for (int i = 0; i < _materials.Length; i++)
                //{
                //    _materials[i].color = _originalColors[i];
                //}
                _renderer.materials = _materials;
            }
            yield return null;
            duration += Time.deltaTime;
        }

        _thisMat.SetFloat("_Clip", _clipCurve.Evaluate(_clipCurve.keys[_clipCurve.length - 1].time));

        _materializeCoroutine = null;
        
    }

    public void Materialize()
    {
        if (_thisMat == null) return;
        if (_materializeCoroutine != null) StopCoroutine(_materializeCoroutine);
        _materializeCoroutine = StartCoroutine(DoMaterialize());
    }

}
