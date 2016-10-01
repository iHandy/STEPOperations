using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StepExtractor.EntityModels;

namespace KTECreator.Sections
{
    public class ConicalSection : Section
    {
        public override int getNumber()
        {
            return 4;
        }

        override public string getName()
        {
            return "конический";
        }

        private ConicalSurface conicalSurface;

        public override StepExtractor.EntityModels.IEntityModel EntityModel
        {
            get
            {
                return conicalSurface;
            }
            set
            {
                conicalSurface = (ConicalSurface)value;
            }
        }
    }
}
