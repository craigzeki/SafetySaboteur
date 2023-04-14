using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private GameObject _playerPrefab;

    [SerializeField] private Skill[] _skills = new Skill[(int)Skill.SkillType.NUM_OF_SKILLS];

    private GameObject _player;

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }

    public GameObject Player { get => _player; }
    public Skill[] Skills { get => _skills; }

    public void SkillLearnt(Skill.SkillType skill)
    {
        if (_skills[(int)skill].State == Skill.SkillState.DISABLED) _skills[(int)skill].SkillLearnt();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            if ((_playerSpawnPoint != null) && (_player == null)) _player = Instantiate(_playerPrefab, _playerSpawnPoint);
            if (_player != null) CameraDirector.Instance.SetNewPlayer(_player);
        }

        //if (Input.GetKeyDown(KeyCode.Alpha1)) SkillLearnt(Skill.SkillType.PRECISION);
        //if (Input.GetKeyDown(KeyCode.Alpha2)) SkillLearnt(Skill.SkillType.SAFEOS);
        //if (Input.GetKeyDown(KeyCode.Alpha3)) SkillLearnt(Skill.SkillType.FAULT_INJECT);
        //if (Input.GetKeyDown(KeyCode.Alpha4)) SkillLearnt(Skill.SkillType.REDUNDANCY);

        if (Input.GetKeyDown(KeyCode.Alpha1)) _skills[(int)Skill.SkillType.PRECISION].UseSkill();
        if (Input.GetKeyDown(KeyCode.Alpha2)) _skills[(int)Skill.SkillType.SAFEOS].UseSkill();
        if (Input.GetKeyDown(KeyCode.Alpha3)) _skills[(int)Skill.SkillType.FAULT_INJECT].UseSkill();
        if (Input.GetKeyDown(KeyCode.Alpha4)) _skills[(int)Skill.SkillType.REDUNDANCY].UseSkill();

    }
}
