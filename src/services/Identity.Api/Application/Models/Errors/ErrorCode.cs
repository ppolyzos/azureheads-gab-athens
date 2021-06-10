using System;
using System.Collections.Generic;

namespace Identity.Api.Application.Models.Errors
{
    public enum ErrorCode
    {
        General = 1000,
        OnDelete = 1001,
        NotFound = 1002,
        FileNotValid = 1003,
        FileAlreadyExists = 1004,
        FileNotUploaded = 1005,

        UserEmailNotConfirmed = 1400,
        UserPhoneNotConfirmed = 1401,
        UserNotFound = 1402,
        RoleNotFound = 1403,
        UserAlreadyInCompany = 1404,
        UserNotAssociatedWithCompany = 1405,
        UserEmailConfirmed = 1406,
        UserPhoneConfirmed = 1407,

        AuthFailed = 1800

    }

    public static class ErrorMessages
    {
        private static readonly Dictionary<int, string> Messages = new Dictionary<int, string>
        {
            {(int)ErrorCode.General, "An error occured while handling your request."},
            {(int)ErrorCode.OnDelete, "An error occured while deleting the specified resource."},
            {(int)ErrorCode.NotFound, "The item you have requested does not exist." },
            {(int)ErrorCode.FileNotValid, "Please provide a valid file." },
            {(int)ErrorCode.FileAlreadyExists, "There is already a file of this type." },
            {(int)ErrorCode.FileNotUploaded, "There was an error uploading your file." },
            
            {(int)ErrorCode.UserEmailNotConfirmed, "You have not confirmed your email yet." },
            {(int)ErrorCode.UserPhoneNotConfirmed, "You have not confirmed your phone number." },
            {(int)ErrorCode.UserNotFound, "The user you have specified is not valid." },
            {(int)ErrorCode.RoleNotFound, "The role you have specified is not valid." },
            {(int)ErrorCode.UserAlreadyInCompany, "The specified user is already associated with this company." },
            {(int)ErrorCode.UserNotAssociatedWithCompany, "The specified user is not yet associated with this company." },
            {(int)ErrorCode.UserEmailConfirmed, "You have already confirmed your email." },
            {(int)ErrorCode.UserPhoneConfirmed, "You have already confirmed your phone." },

            {(int)ErrorCode.AuthFailed, "Please enter the correct email and/or password" },
        };

        private static string GeneralErrorMessage = "An error has occured. Details are not available for this error code";


        public static ErrorMessage GetErrorMessage<TEnum>(TEnum errorCode)
        {
            var type = typeof(TEnum);
            if (!type.IsEnum) return new ErrorMessage
            {
                Code = "0",
                Message = GeneralErrorMessage
            };

            var error = Enum.Parse(typeof(TEnum), errorCode.ToString()) as Enum;
            var codeNumber = Convert.ToInt32(error);
            Messages.TryGetValue(codeNumber, out var msg);

            return new ErrorMessage
            {
                Code = codeNumber.ToString(),
                Message = string.IsNullOrEmpty(msg) ? GeneralErrorMessage : msg
            };
        }
    }
}