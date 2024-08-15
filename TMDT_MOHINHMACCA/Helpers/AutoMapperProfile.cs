using AutoMapper;
using TMDT_MOHINHMACCA.Models;
using TMDT_MOHINHMACCA.ViewModels;

namespace TMDT_MOHINHMACCA.Helpers
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile() {
            CreateMap<SignupVM, Account>();
            CreateMap<LoginVM, Account>();
            CreateMap<Account, ProfileVM>();
            CreateMap<Account, UserMessagerVM>();
            CreateMap<Post, PostVM>();
        }
    }
}
