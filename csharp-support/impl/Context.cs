namespace io.cloudstate.csharpsupport.impl
{
    /// <summary>
    /// Base for all contexts
    /// </summary>
    public class Context : IContext
    {

        /// <summary>
        /// Service call lookup
        /// </summary>
        /// <value></value>
        public IServiceCallFactory ServiceCallFactory { get; }

        /// <summary>
        /// Create a context with reference to a service call factory
        /// </summary>
        /// <param name="serviceCallFactory">Main service call factory</param>
        public Context(IServiceCallFactory serviceCallFactory)
        {
            ServiceCallFactory = serviceCallFactory;
        }
    }

}