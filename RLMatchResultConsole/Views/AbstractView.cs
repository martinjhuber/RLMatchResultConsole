using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLMatchResultConsole.Views
{
    internal abstract class AbstractView
    {

        public abstract string GetTitle();
        public abstract void InitGui();
        public abstract void Execute();
        public abstract void Update();

        public void Start()
        {
            InitGui();
            Execute();
        }


    }
}
