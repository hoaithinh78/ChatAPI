namespace ChatR.Interface.FactoryMethod
{
    public class ChannelFactoryProvider
    {
        private readonly IEnumerable<IChannelFactory> _factories;

        public ChannelFactoryProvider(IEnumerable<IChannelFactory> factories)
        {
            _factories = factories;
        }

        public IChannelFactory GetFactory(int channelType)
        {
            var factory = _factories.FirstOrDefault(f => f.SupportedType == channelType);

            if (factory == null)
            {
                throw new ArgumentException($"Channel type {channelType} is not supported.");
            }

            return factory;
        }
    }
}
