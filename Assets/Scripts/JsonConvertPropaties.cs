using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// JSON�ɏo�͂���ۂɑ�����
/// </summary>
public class JsonConvertPropaties : MonoBehaviour
{
    [SerializeField] public bool isSpawnPoint;      // �G���e�B�e�B�̃X�|�[���|�C���g���ǂ���
    [SerializeField] public bool isRandomize;       // �����_�}�C�U(��]�������_���ɂ����邩�H)
    [SerializeField] public bool isCollision;       // �����蔻�肪���݂��邩�H
    [SerializeField] public bool isMonoSpaced;      // ����(�c�A���A���s�̑S�Ă��S�ē����傫����)
    [SerializeField] public bool isintrustUserMesh; // ���[�U�[��`���b�V�����f���`���C���邩
    [SerializeField] public int ID;                 // ���f��ID(�R���o�[�g���)
}
