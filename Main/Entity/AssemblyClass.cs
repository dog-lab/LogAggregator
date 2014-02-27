namespace Broos.Monitor.LogAggregator.Entity {
    /// <summary>
    /// Simple definition class bringing together the goodness of an
    /// assembly file path and class name within that assembly file.
    /// They combine to create a mighty "Instance"...
    /// </summary>
    public class AssemblyClass {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyClass"/> class.
        /// </summary>
        /// <param name="assemblyName">Path of the assembly file.</param>
        /// <param name="className">Name of the class that is boss within this assembly file.</param>
        public AssemblyClass(string assemblyName, string className) {
            this.AssemblyName = assemblyName;
            this.ClassName = className;
        }

        /// <summary>
        /// Gets the path of the assembly file.
        /// </summary>
        /// <value>
        /// The name of the assembly.
        /// </value>
        public string AssemblyName { get; private set; }

        /// <summary>
        /// Gets the name of the class "of interest" in the assembly file.
        /// </summary>
        /// <value>
        /// The name of the class.
        /// </value>
        public string ClassName { get; private set; }
    }
}
