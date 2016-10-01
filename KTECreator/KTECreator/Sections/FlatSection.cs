using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StepExtractor.EntityModels;

namespace KTECreator.Sections
{
    public class FlatSection : Section
    {
        override public int getNumber()
        {
            return 1;
        }

        override public string getName()
        {
            return "плоский";
        }

        private Plane plane;

        public override IEntityModel EntityModel
        {
            get
            {
                return plane;
            }
            set
            {
                plane = (Plane)value;
            }
        }
    }
}
