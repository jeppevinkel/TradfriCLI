using System.Threading.Tasks;

namespace TradfriCLI.Interfaces
{
    public interface ICommand
    {
        Task Execute();
    }
}