using Core.Entity;
using Core.Repository;

namespace Infrastructure.Repository;

public class NotificationsRepository : EfRepository<Notifications>, INotificationsRepository
{
    public NotificationsRepository(ApplicationDbContext context) : base(context)
    {
    }
}