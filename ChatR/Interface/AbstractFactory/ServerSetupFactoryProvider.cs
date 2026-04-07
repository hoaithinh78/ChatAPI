using ChatR.Enums;

namespace ChatR.Interface.AbstractFactory
{
    public class ServerSetupFactoryProvider
    {
        public IServerSetupFactory GetFactory(ServerTemplateType templateType)
        {
            return templateType switch
            {
                ServerTemplateType.Study => new StudyServerFactory(),
                ServerTemplateType.Gaming => new GamingServerFactory(),
                ServerTemplateType.Company => new CompanyServerFactory(),
                _ => throw new ArgumentException("Loại template không hợp lệ.")
            };
        }
    }
}
