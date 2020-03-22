using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donation.Common.ViewModels
{
    public class ResponseViewModel
    {
        public ResponseViewModel()
        {
            Status = 0;
            Message = Constants.Error;
        }
        public string Message { get; set; }
        public object ResponseData { get; set; }
        public int Status { get; set; }
        public int StatusCode { get; set; }
        public int Total { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Constants
    {
        public const string Error = "Some internal error occurred";
        public const string Success = "Data saved successfully";
        public const string Delete = "Data deleted successfully";
        public const string Warning = "Data is not in proper format";
        public const string Retreived = "Data retrieved successfully";
        public const string NotFound = "Data not found";
        public const string Register = "Kobe Registration Successful";
        public const string PasswordRequired = "Password is required";
        public const string UserExist = "User already registered with email id provided";
        public const string InvalidPassword = "Invalid Password";
        public const string SHA256 = "SHA256";
        public const string DataEmpty = "Data are empty";
        public const string InvalidKeySize = "Key size is not valid";
        public const string KeyIsNullOrEmpty = "Key is null or empty";
        public const string RSAKeyValue = "RSAKeyValue";
        public const string Modulus = "Modulus";
        public const string Exponent = "Exponent";
        public const string P = "P";
        public const string Q = "Q";
        public const string DP = "DP";
        public const string DQ = "DQ";
        public const string InverseQ = "InverseQ";
        public const string D = "D";
        public const string InvalidXMLRSAkey = "Invalid XML RSA key";
        public const string ResetPassword = "Reset Password";
        public const string InvalidAccount = "Account not exist with the details provided";
        public const string PasswordUpdated = "Password updated successfully";
        public const string ResetPasswordSuccess = "Instructions sent for resetting password";
        public const string InvalidEmail = "Invalid email id";
        public const string Debit = "Debit";
        public const string MPesaTransactions = "Hey! Your initial 2 months transactions are fetched successfully ! Rest transactions are running in background";
        public const string MPesaError = "Invalid username or password";
        public const string JengaAccountExist = "Equity account already exist. Please add another account";
        public const string JengaFetchedIntialTransaction = "Hey! your first 20 transactions fetched and saved successfully. Fetching rest of the transactions in background";
        public const string JengaAccountValidationFailed = "Validation of account failed";
        public const string MPesaAccountExist = "Mpesa account already exist. Please add another account";
        public const string NotActive = "please follow link in email to activate account";
        public const string IncorrectEmail = "Email not registered with us";

    }
}
