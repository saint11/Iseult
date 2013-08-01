using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iseult.Castle
{
    public interface Mechanical
    {
        void OnLetGo();
        
        void OnHit();
        
        void OnStepped();

        int GetID();
    }
}
