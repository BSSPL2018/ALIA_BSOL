using BSOL.Core.Entities;
using System.Reflection.Metadata;

namespace BSOL.Helpers
{
    public interface ICommonHelper
    {
        string GetConstant(Constant constant);
        string GetDocumentRoot();
        string GetDocumentRootDB();
        Task<List<string>> GetCurrency();
        Task<AccountSetting> GetAccountSettings();
    }
}
