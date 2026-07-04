using Bookings.Domain;


namespace Bookings.Application
{
    public interface IUserAccessor
    {
        public User GetUser();
    }
}
