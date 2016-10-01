using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StepExtractor.EntityModels;

namespace KTECreator.Sections
{
    public abstract class Section
    {
        public abstract int getNumber();
        public abstract string getName();
        public abstract IEntityModel EntityModel { get; set; }
        public int Position { get; set; }

        public CartesianPoint StartPoint { get; set; }
        public CartesianPoint EndPoint { get; set; }
        public List<CartesianPoint> allPoints { get; set; }

        public List<CartesianPoint> startPoints { get; set; }
        public List<CartesianPoint> endPoints { get; set; }
    }
}
