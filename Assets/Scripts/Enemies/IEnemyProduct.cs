using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enemies
{
    internal interface IEnemyProduct
    {
        public Enemy EnemyParameter { get; set; }
        public void Create();
    }
}
