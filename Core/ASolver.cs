using System;
namespace AdventOfCode.Core {
    public abstract class ASolver : ISolver {
        public string Input { get; set; }
        public virtual void Setup() { }
        public virtual string SolvePart1() => string.Empty;
        public virtual string SolvePart2() => string.Empty;
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class SubmitAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class)]
    public class RunAttribute : Attribute {
        public string InputName { get; set; }
        public RunAttribute(string inputName) {
            InputName = inputName;
        }
    }
}