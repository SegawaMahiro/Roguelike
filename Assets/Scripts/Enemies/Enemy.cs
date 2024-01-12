using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Create EnemyData", fileName = "EnemyData")]
    public class Enemy : ScriptableObject
    {
        public string Name = "New Enemy";
        [SerializeField] uint _health;
        [SerializeField] uint _strength;
        [SerializeField] uint _walkSpeed;
        [SerializeField] uint _defence;

    }
}
