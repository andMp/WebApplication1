using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{

    public class ReminderService : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<ReminderService> _log;

        public ReminderService(IServiceProvider sp, ILogger<ReminderService> log)
        {
            _sp = sp; _log = log;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var nowUtc = DateTime.UtcNow;

                    var toRemind = await db.Streams
                        .Where(s => s.Status == StrStatus.Ochikuytsa
                                    && s.ReminderMinutes > 0   // є сенс нагадувати
                                    && s.Pochatok.AddMinutes(-s.ReminderMinutes) <= nowUtc
                                    && s.Pochatok > nowUtc)
                        .ToListAsync(stoppingToken);

                    foreach (var s in toRemind)
                    {
                        _log.LogInformation("🔔 Нагадування: '{Title}' (Streamer {StreamerId}) о {Start:u} (за {Minutes} хв)",
                            s.Nazva, s.StreamerId, s.Pochatok, s.ReminderMinutes);
                        s.ReminderMinutes = 0;
                    }

                    if (toRemind.Count > 0)
                        await db.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "ReminderService error");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

    }
}
