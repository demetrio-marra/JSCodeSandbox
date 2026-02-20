namespace JSCodeSandbox.Exceptions
{
    public class InfrastructureError : Exception
    {
        public string Component { get; }

        public InfrastructureError(string component, string message) : base($"Infrastructure error on Component: {component}, Message: {message}")
        {
            Component = component;
        }

        public InfrastructureError(string component, string message, Exception innerException) : base($"Infrastructure error on Component: {component}, Message: {message}", innerException)
        {
            Component = component;
        }
    }
}
