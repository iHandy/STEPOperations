using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StepExtractor.EntityModels;

namespace KTECreator.Sections
{
    public class CylindricalSection : Section
    {
        public override int getNumber()
        {
            return 3;
        }

        override public string getName()
        {
            return "цилиндрический";
        }

        private CylindricalSurface cylSurface;

        public override IEntityModel EntityModel
        {
            get
            {
                return cylSurface;
            }
            set
            {
                cylSurface = (CylindricalSurface)value;
            }
        }
    }
}
