using System.Threading.Tasks;

namespace BSOL.Helpers
{
    public interface ICommonNotification
    {
        Task Send(string Module, long ID);
        Task SendSMS(List<Notification> receivers);
    }
}
