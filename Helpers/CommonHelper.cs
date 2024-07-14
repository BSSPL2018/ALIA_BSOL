using BSOL.Core;
using BSOL.Core.Entities;
using BSOL.Core.Helpers;
using BSOL.Helpers;
using ExcelDataReader;
using Kendo.Mvc.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Constant = BSOL.Helpers.Constant;

namespace BSOL.Web.Helpers
{
    public class CommonHelper : ICommonHelper
    {
        private readonly BSOLWebContext _Webcontext;
        private readonly IMemoryCache _cache;
        private readonly AppUser _appUser;
        public CommonHelper(BSOLWebContext webContext, IMemoryCache cache, AppUser appUser)
        {
            _Webcontext = webContext;
            _cache = cache;
            _appUser = appUser;
        }
        public string GetConstant(Constant constant)
        {
            var constants = _cache.Get<List<BSOL.Core.Entities.Constant>>(CacheVariable.Constants.ToString());
            if (constants == null || !constants.Any())
            {
                constants = _Webcontext.Constants.ToList();
                _cache.Set(CacheVariable.Constants.ToString(), constants);
            }
            return constants.Where(x => x.Name == constant.ToString()).Select(x => x.Value).FirstOrDefault();
        }

        public string GetDocumentRoot()
        {
            return Path.Combine(GetConstant(Constant.DOCUMENT), _appUser.UniqueID);
        }
        public string GetDocumentRootDB()
        {
            return Path.Combine(GetConstant(Constant.ACCOUNTS));
        }
        public async Task<AccountSetting> GetAccountSettings()
        {
            return await _Webcontext.AccountSettings.FirstOrDefaultAsync(x => x.Id == _appUser.CompanyId);
        }
        public async Task<List<string>> GetCurrency()
        {
            return await _Webcontext.Currencies.Select(x => x.Currency).ToListAsync();
        }
    }
}
