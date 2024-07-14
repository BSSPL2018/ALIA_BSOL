using BSOL.Core;
using BSOL.Core.Models.Common;
using BSOL.Core.Entities;
using BSOL.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOL.Core.Helpers;

namespace BSOL.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected BSOLContext _context;
        protected BSOLWebContext _Webcontext;
        protected AppUser _appUser;
        public BaseController(BSOLContext context, AppUser appUser)
        {
            _context = context;
            _appUser = appUser;
        }
        public BaseController( BSOLWebContext Webcontext, AppUser appUser)
        {
            _Webcontext = Webcontext;
            _appUser = appUser;
        }
        public BaseController(BSOLContext context, BSOLWebContext Webcontext, AppUser appUser)
        {
            _context = context;
            _Webcontext = Webcontext;
            _appUser = appUser;
        }
        protected ReturnMessage Error()
        {
            return Error("An error occurred while processing your request.");
        }

        protected ReturnMessage Error(string message)
        {
            return new ReturnMessage { HasError = true, Message = message };
        }

        protected ReturnMessage Error(IEnumerable<string> Errors, string Title = "")
        {
            var errors = Errors.ToList();
            if (Title.IsValid())
                errors.Insert(0, Title);
            return new ReturnMessage { HasError = true, Message = string.Join("\n", errors) };
        }

        protected ReturnMessage Message(string Message, object ID = null)
        {
            return new ReturnMessage { Message = Message, Id = ID };
        }

        protected async Task EventLog(string module, string events, params object[] remarks)
        {
            string machineID = HttpContext.Connection.RemoteIpAddress.ToString();
            await _context.ExecuteNonQueryAsync("spBSOL_EventLog", new
            {
                Options = "ADD_NEW",
                RefNo = module,
                Events = "WEB - " + events,
                CreatedBy = _appUser.CommonEmpNo,
                Remarks = string.Join(" - ", remarks),
                Department = "WEB",
                MachineID = machineID
            });
        }
        protected async Task EventLogAsync(string module, ActionType actionType, string ObjectType, long ObjectID, params object[] Description)
        {
            string actionBy = _appUser.AppUserName;
            long? companyId = _appUser.CompanyId;
            long? userId = _appUser.UserId;

            string Machine = HttpContext.Connection.RemoteIpAddress.ToString();

            _Webcontext.EventLogs.Add(new BSOL.Core.Entities.EventLog
            {
                CompanyID = companyId,
                Module = module,
                ActionType = actionType.ToString(),
                Reference = string.Join(" - ", Description.Where(x => x != null)),
                ActionBy = actionBy,
                ActionDate = DateTime.Now,
                ObjectType = ObjectType,
                ObjectID = ObjectID,
                Machine = Machine,
                UserID = userId
            });
            await _context.SaveChangesAsync();
        }
        protected ReturnMessage RejectError(IEnumerable<string> Errors)
        {
            return Error(Errors, "Cannot Reject.");
        }
        protected ReturnMessage ApproveError(IEnumerable<string> Errors)
        {
            return Error(Errors, "Cannot Approve.");
        }
        protected ReturnMessage DeleteError(IEnumerable<string> Errors)
        {
            return Error(Errors, "Cannot Delete.");
        }
        protected ReturnMessage SaveError(IEnumerable<string> Errors)
        {
            return Error(Errors, "Cannot Add/Edit.");
        }
        protected ReturnMessage UndoError(IEnumerable<string> Errors)
        {
            return Error(Errors, "Cannot Undo.");
        }
        protected ReturnMessage MessageWithId(string Message, long Id)
        {
            return new ReturnMessage { Message = Message, Id = Id == 0 ? null : Id.ToString() };
        }
        protected ReturnMessage MessageWithEncryptedID(string Message, long ID)
        {
            return new ReturnMessage { Message = Message, Id = ID == 0 ? null : IdentityEncryptor.EncryptParam(ID) };
        }
        protected ReturnMessage DBError()
        {
            return new ReturnMessage { HasError = true, Message = Constants.DBErrorMessage };
        }
    }
}
