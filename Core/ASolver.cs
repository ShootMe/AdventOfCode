namespace AdventOfCode.Core {
    public abstract class ASolver : ISolver {
        public string Input { get; set; }
        public string Name { get; set; }
        public ASolver(string input) => Input = input;
        public virtual void Setup() { }
        public virtual string SolvePart1() => string.Empty;
        public virtual string SolvePart2() => string.Empty;
    }
}