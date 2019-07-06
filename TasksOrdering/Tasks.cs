using System;
using System.Collections.Generic;
using System.Linq;

namespace TasksOrdering
{
    public class TasksPlanner
    {
        #region constructors
        private readonly List<Job> _listJobs;

        public TasksPlanner()
        {
            //initialize list on object creation
            _listJobs = new List<Job>();
        }

        #endregion

        #region Public Actions

        /// <summary>
        /// returns the ordered list of jobs
        /// </summary>
        /// <param name="tasks">string of tasks</param>
        /// <returns>list of string</returns>
        public List<string> Sequence(string tasks)
        {
            //if task list is empty return empty list
            if (string.IsNullOrWhiteSpace(tasks))
            {
                return new List<string>();
            }
            generateTaskCollection(tasks);
            return getOrderedSequence();
        }
        #endregion

        #region Private Actions
        /// <summary>
        /// return orders sequnce of jobs from class level list of jobs
        /// </summary>
        /// <returns>list of string</returns>
        private List<string> getOrderedSequence()
        {
            //return distinct list of ordered sequence by iterating leaf nodes and merging there dependencies.
            return string.Join(",", _listJobs.Where(x => x.IsLeafNode(_listJobs)).Select(x => x.DependentOn + "," + x.Name))
                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
        }

        /// <summary>
        /// initialize the list with task receive in string
        /// </summary>
        /// <param name="tasks">task string</param>
        private void generateTaskCollection(string tasks)
        {
            //split string on new line 
            string[] lines = tasks.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var task in lines)
            {
                //split to get task names
                var dependentTasks = task.Split(new[] { "=>" }, StringSplitOptions.RemoveEmptyEntries);
                //if two tasks(dependecy) exists i.e, a => b
                if (dependentTasks.Length > 1)
                {
                    //trim to remove extra spaces
                    dependentTasks[0] = dependentTasks[0].Trim();
                    dependentTasks[1] = dependentTasks[1].Trim();

                    //if both tasks same, it is self dependency
                    if (dependentTasks[0] == dependentTasks[1])
                    {
                        throw new Exception("Jobs can’t depend on themselves.");
                    }
                    addItem(dependentTasks);
                }
                else
                {
                    _listJobs.Add(new Job(dependentTasks[0].Trim()));
                }
            }
        }

        /// <summary>
        /// add item to the list
        /// </summary>
        /// <param name="dependentTasks">array of tasks</param>
        private void addItem(string[] dependentTasks)
        {
            bool existingItem = true;
            //if item already exist in list i.e, a => b, a => c, a => d
            var item = _listJobs.FirstOrDefault(x => x.Name == dependentTasks[0]);
            if (item == null)
            {
                item = new Job();
                item.Name = dependentTasks[0];
                item.DependentOn = "";
                existingItem = false;
            }
            //get dependency of dependent item e,g, if a depends on b and b depends on c,d then a depends on c,d,b
            var dependency = String.Join(",", _listJobs.Where(x => x.Name == dependentTasks[1]).Select(x => x.DependentOn));
            dependency += string.IsNullOrEmpty(dependency) ? dependentTasks[1] : "," + dependentTasks[1];
            item.DependentOn += string.IsNullOrEmpty(item.DependentOn) ? dependency : "," + dependency;

            //if item exist in its dependency then it is circular dependency
            if (item.DependentOn.Contains(dependentTasks[0]))
            {
                throw new Exception("Jobs can’t have circular dependencies.");
            }
            //update existing collection for new dependecy occurs due to current item
            //e.g if a depends on b and in current item we found out b is dependent on c,d
            //then we need to update a as a is dependent on c,d,b
            updateExistingItemsDependency(dependentTasks[0], item.DependentOn);

            //if it existing item remove outdated item from list
            if (existingItem)
            {
                _listJobs.RemoveAll(x => x.Name == item.Name);
            }
            _listJobs.Add(item);
        }

        /// <summary>
        /// update existing list of jobs if depends on current job
        /// </summary>
        /// <param name="currentTask">current job entered in system</param>
        /// <param name="updatedDependency">updated dependency</param>
        private void updateExistingItemsDependency(string currentTask, string updatedDependency)
        {
            _listJobs.Where(x => x.DependentOn != null && x.DependentOn.Contains(currentTask))
                .ToList()
                .ForEach(task =>
                {
                    task.DependentOn = replaceDependency(currentTask, updatedDependency, task);
                });
        }

        /// <summary>
        /// updates dependency of existing task with provided updated dependency
        /// </summary>
        /// <param name="currentTask">task which get modified dependency</param>
        /// <param name="updatedDependency">updated depency to replace old one</param>
        /// <param name="currentItem">Jov to check if dependent on currnet task</param>
        /// <returns></returns>
        private static string replaceDependency(string currentTask, string updatedDependency, Job currentItem)
        {
            //if updated dependency already exists in old dependency we don't need to update
            int currentTaskPosition = currentItem.DependentOn.IndexOf(currentTask);
            int updatedDependencyPosition = currentItem.DependentOn.IndexOf(updatedDependency);
            if (updatedDependencyPosition == -1 || updatedDependencyPosition < currentTaskPosition)
            {
                return currentItem.DependentOn.Replace(currentTask, updatedDependency + "," + currentTask);
            }
            return currentItem.DependentOn;
        }

        #endregion
    }
}
