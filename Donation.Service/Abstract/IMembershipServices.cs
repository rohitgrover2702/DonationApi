using Donation.Common.ViewModels;
using Donation.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Donation.Service.Abstract
{
   public interface IMembershipServices
    {
        Task<ResponseViewModel> RegisterUser(UserDto user);
        ResponseViewModel Login(string email, string password);
    }
}
