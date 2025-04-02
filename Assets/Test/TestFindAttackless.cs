using KBCore.Refs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestFindAttackless : ValidatedMonoBehaviour
{
    [SerializeField, SearchByName]
    private Transform m_MainGameObject; 

    [SerializeField,SearchByName]
    private Transform m_ChildGameObject;


    [SerializeField, SearchByName] 
    private Transform m_TestGameObject;


    [SerializeField, SearchByName]
    private Transform m_eeeGameObject;

    [SerializeField, SearchByName]
    private Animator m_ChildAnimator;

    
    protected override void OnValidate()
    {
        base.OnValidate();
    }

    void BubbleSort(int[] nums)
    {
        // ��ѭ����δ��������Ϊ [0, i]
        for (int i = nums.Length - 1; i > 0; i--)
        {
            // ��ѭ������δ�������� [0, i] �е����Ԫ�ؽ���������������Ҷ�
            for (int j = 0; j < i; j++)
            {
                if (nums[j] > nums[j + 1])
                {
                    // ���� nums[j] �� nums[j + 1]
                    (nums[j + 1], nums[j]) = (nums[j], nums[j + 1]);
                }
            }
        }
    }

}
