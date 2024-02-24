using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roguelike.Damages
{
    public interface IDamageApplicable
    {
        public void ApplyDamage(Damage damage);
    }
}
