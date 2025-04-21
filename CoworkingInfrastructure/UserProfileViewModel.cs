using System.Collections.Generic;
using CoworkingDomain.Model;

namespace CoworkingInfrastructure.ViewModels
{
    public class UserProfileViewModel
    {
        // Бронювання поточного користувача
        public IEnumerable<Booking> MyBookings { get; set; }
        // Додатково, для адміна – усі бронювання
        public IEnumerable<Booking> AllBookings { get; set; }
    }
}
