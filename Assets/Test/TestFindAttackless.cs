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
        // 外循环：未排序区间为 [0, i]
        for (int i = nums.Length - 1; i > 0; i--)
        {
            // 内循环：将未排序区间 [0, i] 中的最大元素交换至该区间的最右端
            for (int j = 0; j < i; j++)
            {
                if (nums[j] > nums[j + 1])
                {
                    // 交换 nums[j] 与 nums[j + 1]
                    (nums[j + 1], nums[j]) = (nums[j], nums[j + 1]);
                }
            }
        }
    }

}
