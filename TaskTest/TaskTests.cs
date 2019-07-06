using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TasksOrdering;

namespace TaskTest
{
    [TestClass]
    public class TaskTests
    {
        [TestMethod]
        public void EmptyCase_ReturnEmptyList()
        {
            TasksPlanner tasks = new TasksPlanner();
            var result = tasks.Sequence(string.Empty);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void SimpleCase_ReturnSequenceList()
        {
            TasksPlanner tasks = new TasksPlanner();
            var result = tasks.Sequence("a =>");
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("a", result[0]);
        }

        [TestMethod]
        public void SimpleCase2_ReturnSequenceList()
        {
            TasksPlanner tasks = new TasksPlanner();
            string strTasks = @"a =>
                                b =>
                                c =>";
            var result = tasks.Sequence(strTasks);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("a", result[0]);
            Assert.AreEqual("b", result[1]);
            Assert.AreEqual("c", result[2]);
        }

        [TestMethod]
        public void DependentCase_ReturnSequenceList()
        {
            TasksPlanner tasks = new TasksPlanner();
            string strTasks = @"a =>
                                b => c
                                c =>";
            var result = tasks.Sequence(strTasks);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("a", result[0]);
            Assert.AreEqual("c", result[1]);
            Assert.AreEqual("b", result[2]);
        }

        [TestMethod]
        public void DependentCase2_ReturnSequenceList()
        {
            TasksPlanner tasks = new TasksPlanner();
            string strTasks = @"a =>
                                b => c
                                c => f
                                d => a
                                e => b
                                f =>";
            var result = tasks.Sequence(strTasks);
            Assert.AreEqual(6, result.Count);
            Assert.AreEqual("a", result[0]);
            Assert.AreEqual("d", result[1]);
            Assert.AreEqual("f", result[2]);
            Assert.AreEqual("c", result[3]);
            Assert.AreEqual("b", result[4]);
            Assert.AreEqual("e", result[5]);
        }

        [TestMethod]
        public void ManyToOneDependencySimple_ReturnSequenceList()
        {
            TasksPlanner tasks = new TasksPlanner();
            string strTasks = @"a =>
                                b => a
                                c => a
                                d => a
                                e => a
                                f => a";
            var result = tasks.Sequence(strTasks);
            Assert.AreEqual(6, result.Count);
            Assert.AreEqual("a", result[0]);
            Assert.AreEqual("b", result[1]);
            Assert.AreEqual("c", result[2]);
            Assert.AreEqual("d", result[3]);
            Assert.AreEqual("e", result[4]);
            Assert.AreEqual("f", result[5]);
        }

        [TestMethod]
        public void ManyToOneDependencyComplex_ReturnSequenceList()
        {
            TasksPlanner tasks = new TasksPlanner();
            string strTasks = @"a =>
                                b => a
                                c => a
                                d => j
                                e => b
                                f => c
                                g => c
                                h => d
                                i => f
                                j => a";
            var result = tasks.Sequence(strTasks);
            Assert.AreEqual(10, result.Count);
            Assert.AreEqual("a", result[0]);
            Assert.AreEqual("b", result[1]);
            Assert.AreEqual("e", result[2]);
            Assert.AreEqual("c", result[3]);
            Assert.AreEqual("g", result[4]);
            Assert.AreEqual("j", result[5]);
            Assert.AreEqual("d", result[6]);
            Assert.AreEqual("h", result[7]);
            Assert.AreEqual("f", result[8]);
            Assert.AreEqual("i", result[9]);
        }

        [TestMethod]
        public void OneToManyDependencySimple_ReturnSequenceList()
        {
            TasksPlanner tasks = new TasksPlanner();
            string strTasks = @"a => d
                                a => b 
                                a => c 
                                b => 
                                c =>
                                d =>";
            var result = tasks.Sequence(strTasks);
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("d", result[0]);
            Assert.AreEqual("b", result[1]);
            Assert.AreEqual("c", result[2]);
            Assert.AreEqual("a", result[3]);

        }

        [TestMethod]
        public void OneToManyDependencyComplex_ReturnSequenceList()
        {
            TasksPlanner tasks = new TasksPlanner();
            string strTasks = @"a => d
                                a => b 
                                a => c 
                                b => 
                                c => b
                                d =>";
            var result = tasks.Sequence(strTasks);
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("d", result[0]);
            Assert.AreEqual("b", result[1]);
            Assert.AreEqual("c", result[2]);
            Assert.AreEqual("a", result[3]);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Jobs can’t depend on themselves.")]
        public void SelfDependency_ReturnError()
        {
            TasksPlanner tasks = new TasksPlanner();
            string strTasks = @"a =>
                                b =>
                                c => c";
            tasks.Sequence(strTasks);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Jobs can’t have circular dependencies.")]
        public void OneToManyCircularDependency_ReturnError()
        {
            TasksPlanner tasks = new TasksPlanner();
            string strTasks = @"a => d 
                                a => c 
                                b => a
                                c => b
                                d => c";
            tasks.Sequence(strTasks);

        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Jobs can’t have circular dependencies.")]
        public void CircularDependency_ReturnError()
        {
            TasksPlanner tasks = new TasksPlanner();
            string strTasks = @"a =>
                                b => c
                                c => f
                                d => a
                                e =>
                                f => b";
            tasks.Sequence(strTasks);
        }

    }
}
