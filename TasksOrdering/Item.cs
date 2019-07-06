using System.Collections.Generic;
using System.Linq;

namespace TasksOrdering
{
    public class Job
    {
        public Job()
        {
        }
            public Job(string name)
        {
            this.Name = name;
        }    
        public string DependentOn { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// return bool value to indicate if current Item is leaf node
        /// i.e, no job is dependent on this job
        /// </summary>
        /// <param name="list">list of items</param>
        /// <returns>boolean</returns>
        public bool IsLeafNode(IEnumerable<Job> list)
        {
            return !list.Any(x => x.DependentOn!=null && x.DependentOn.Contains(Name));
        }
    }
}
