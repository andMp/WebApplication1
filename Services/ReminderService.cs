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
                    var targetFrom = nowUtc.AddMinutes(9);   // вікно 9..11 хв
                    var targetTo = nowUtc.AddMinutes(11);

                    var toRemind = await db.Streams
                        .Where(s => s.Status == StrStatus.Ochikuytsa
                                    && !s.ReminderSent
                                    && s.Pochatok >= targetFrom
                                    && s.Pochatok <= targetTo)
                        .ToListAsync(stoppingToken);

                    foreach (var s in toRemind)
                    {
                        _log.LogInformation("🔔 Нагадування: '{Title}' (Streamer {StreamerId}) о {Start:u}",
                            s.Nazva, s.StreamerId, s.Pochatok);

                        s.ReminderSent = true;
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
