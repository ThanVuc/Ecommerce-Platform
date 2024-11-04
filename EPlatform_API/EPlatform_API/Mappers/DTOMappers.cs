using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.DTOs.AuthDTOs;
using EPlatform_API.Models;

namespace EPlatform_API.Mappers
{
        // public string? Username {get; set;}
        // public string? PhoneNumber {get; set;}
        // public string PasswordHash {get; set;} = string.Empty;
        // public string Address {get; set;} = string.Empty;
        // public string First {get; set;} = string.Empty;
        // public string Last {get; set;} = string.Empty;
    public static class DTOMappers
    {
        public static Users ToUser(this RegisterRequestModel registerModel){
            return new Users(){
                Username = registerModel.Username,
                Email = registerModel.Username,
                PhoneNumber = registerModel.PhoneNumber,
                PasswordHash = registerModel.Password,
                Address = registerModel.Address,
                First = registerModel.First,
                Last = registerModel.Last
            };
        }

        public static Users ToUser(this LoginRequestModel loginModel){
            return new Users(){
                Username = loginModel.Username,
                Email = loginModel.Username,
                PasswordHash = loginModel.Password,
            };
        }
    }
}