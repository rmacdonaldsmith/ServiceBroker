namespace MHM.WinFlexOne.Services.Integration
{
    public interface IServiceLocator
    {
        TService Locate<TService>() where TService : class;
    }
}
