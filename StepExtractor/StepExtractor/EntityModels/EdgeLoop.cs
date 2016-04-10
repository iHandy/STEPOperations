using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StepExtractor.EntityModels
{
    class EdgeLoop : Loop, IEntityModel
    {
        /// <summary>
        /// Название сущности в STEP файле
        /// </summary>
        public const string NAME = "EDGE_LOOP";

        public string StepName
        {
            get { return NAME; }
        }

        public List<OrientedEdge> EdgeList { get; set; }
    }
}
