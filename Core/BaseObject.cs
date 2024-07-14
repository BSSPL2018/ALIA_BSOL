using BSOL.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace BSOL.Core
{
    public abstract class BaseObject
    {
        #region Properties and Fields

        [JsonIgnore]
        public List<string> ErrorMessage;
        [JsonIgnore]
        public bool HasError;
        [JsonIgnore]
        public bool IsNew;
        [JsonIgnore]
        protected BSOLContext _context;
        [JsonIgnore]
        protected BSOLWebContext _Webcontext;

        #endregion

        protected virtual Task Validate()
        {
            return Task.CompletedTask;
        }
        protected abstract Task Add();
        protected abstract Task Update();
        protected abstract Task Delete();
        protected virtual Task CheckDependency()
        {
            return Task.CompletedTask;
        }

        public async Task<bool> SaveAsync()
        {
            using (var serviceScope = AppUser.Current.RequestServices.CreateScope())
            {
                _Webcontext = serviceScope.ServiceProvider.GetService<BSOLWebContext>();

                _context = serviceScope.ServiceProvider.GetService<BSOLContext>();

                await this.Validate();
                if (HasError)
                    return false;

                await OnSave();
                return true;
            }
        }

        protected virtual async Task OnSave()
        {
            long id = GetEntityID();
            if (id == 0)
            {
                this.IsNew = true;
                var properties = this.GetEntityType().GetProperties();

                /* Set EntryBy */
                PropertyInfo entyByProp = properties.FirstOrDefault(x => x.Name == "EntryBy" || x.Name == "CreatedBy" || x.Name == "Created");//TODO: removed Created
                entyByProp?.SetValue(this, GetAppUserName());

                /* Set EntryDate */
                PropertyInfo entyDateProp = properties.FirstOrDefault(x => x.Name == "EntryDate" || x.Name == "CreatedOn");
                entyDateProp?.SetValue(this, DateTime.Now);

                await Add();
            }
            else
                await Update();
        }

        public async Task<bool> RemoveAsync()
        {
            using (var serviceScope = AppUser.Current.RequestServices.CreateScope())
            {
                _Webcontext = serviceScope.ServiceProvider.GetService<BSOLWebContext>();

                _context = serviceScope.ServiceProvider.GetService<BSOLContext>();

                await CheckDependency();
                if (HasError)
                    return false;

                await Delete();
                return true;
            }
        }

        public async Task<bool> ApproveAsync()
        {
            using (var serviceScope = AppUser.Current.RequestServices.CreateScope())
            {
                _Webcontext = serviceScope.ServiceProvider.GetService<BSOLWebContext>();

                return await this.OnApprove();
            }
        }

        protected virtual Task<bool> OnApprove()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RejectAsync()
        {
            using (var serviceScope = AppUser.Current.RequestServices.CreateScope())
            {
                _Webcontext = serviceScope.ServiceProvider.GetService<BSOLWebContext>();

                return await this.OnReject();
            }
        }

        protected virtual Task<bool> OnReject()
        {
            throw new NotImplementedException();
        }

        protected async Task<bool> ModifyAsync(Func<Task<bool>> action)
        {
            using (var serviceScope = AppUser.Current.RequestServices.CreateScope())
            {
                _Webcontext = serviceScope.ServiceProvider.GetService<BSOLWebContext>();

                return await action();
            }
        }

        public void AddMessage(string Message)
        {
            if (ErrorMessage == null)
            {
                ResetMessage();
            }

            ErrorMessage.Add(Message);
            HasError = true;
        }
        public void AddMessage(List<string> Messages)
        {
            if (ErrorMessage == null)
            {
                ResetMessage();
            }

            ErrorMessage.AddRange(Messages);
            HasError = Messages.Any();
        }
        private void ResetMessage()
        {
            ErrorMessage = new List<string>();
            HasError = false;
        }

        #region Audit Logs
        protected void LogAdd(string module, params object[] reference)
        {
            var entry = _Webcontext.ChangeTracker.Entries().Where(p => p.State == EntityState.Added).FirstOrDefault();
            if (entry == null)
                return;

            //Log(ActionType.Add, module, entry.Entity.GetType().Name, string.Join(" - ", reference.Where(x => x != null)));
        }
        protected void LogUpdate(string module, params object[] reference)
        {
            var entry = _Webcontext.ChangeTracker.Entries().Where(p => p.State == EntityState.Modified).FirstOrDefault();
            if (entry == null)
                return;

            var entityName = entry.Entity.GetType().Name;
            var primaryKey = (long)Convert.ChangeType(entry.Metadata.FindPrimaryKey().Properties.Select(p => entry.Property(p.Name).CurrentValue).FirstOrDefault(), typeof(long));

            var data = entry.OriginalValues.Properties.Where(x => Convert.ToString(entry.OriginalValues[x]) != Convert.ToString(entry.CurrentValues[x]))
                .Select(x => new { F = x.Name, O = Convert.ToString(entry.OriginalValues[x]), N = Convert.ToString(entry.CurrentValues[x]) })
                .ToList();

            //Log(ActionType.Edit, module, entityName, string.Join(" - ", reference.Where(x => x != null)), primaryKey, JsonSerializer.Serialize(data));
        }
        
        protected void LogDelete(string module, params object[] reference)
        {
            var entry = _Webcontext.ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted).FirstOrDefault();
            if (entry == null)
                return;

            var primaryKey = (long)Convert.ChangeType(entry.Metadata.FindPrimaryKey().Properties.Select(p => entry.Property(p.Name).CurrentValue).FirstOrDefault(), typeof(long));
            //Log(ActionType.Delete, module, entry.Entity.GetType().Name, string.Join(" - ", reference.Where(x => x != null)), primaryKey);
        }

        protected void Log(string module, ActionType eventType, params object[] reference)
        {
            var entry = _Webcontext.ChangeTracker.Entries().Where(p => p.State == EntityState.Modified).FirstOrDefault();
            if (entry == null)
                return;

            var primaryKey = (long)Convert.ChangeType(entry.Metadata.FindPrimaryKey().Properties.Select(p => entry.Property(p.Name).CurrentValue).FirstOrDefault(), typeof(long));
            Log(eventType, module, entry.Entity.GetType().Name, string.Join(" - ", reference.Where(x => x != null)), primaryKey);
        }

        private void Log(ActionType eventType, string module, string entityName, string reference, long primaryKey = 0, string changes = null)
        {
            long userId = GetAppUser();
            long companyId = GetCompanyId();
            _Webcontext.EventLogs.Add(new Entities.EventLog
            {
                CompanyID = companyId,
                Module = module,
                ActionType = eventType.ToString(),
                Reference = reference,
                Changes = changes,
                ActionBy = GetAppUserName(),
                ActionDate = DateTime.Now,
                ObjectType = entityName,
                ObjectID = primaryKey,
                Machine = AppUser.Current.Connection.RemoteIpAddress.ToString(),
                UserID = userId
            });
        }
        #endregion

        #region Helper Methods

        private Type GetEntityType()
        {
            Type type = this.GetType();
            return type.BaseType == typeof(BaseObject) ? type : type.BaseType;
        }

        private long GetEntityID()
        {
            Type type = this.GetEntityType();

            var idProperty = type.Name.ToLower() + "id";
            PropertyInfo property = type.GetProperties().FirstOrDefault(x => x.Name.ToLower() == "id" || x.Name.ToLower() == idProperty);

            if (property == null)
                throw new KeyNotFoundException(string.Format("The property ID or {0} is not a member of the class '{1}'. To avoid this by override the method OnSave in the class '{1}'", idProperty, type.Name));

            return Convert.ToInt64(property.GetValue(this));
        }

        protected long GetAppUser()
        {
            return GetClaim<long>(ClaimType.EmployeeId);
        }

        protected string GetAppUserName()
        {
            //return "";//TODO:
            return GetClaim<string>(ClaimType.ShortName);
        }

        protected long GetCompanyId()
        {
            return GetClaim<long>(ClaimType.CompanyId);
        }

        protected T GetClaim<T>(ClaimType claim)
        {
            return (T)Convert.ChangeType(AppUser.Current?.User?.FindFirst(claim.ToString())?.Value, typeof(T));
        }

        #endregion
    }
}