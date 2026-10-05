using Claims.Domain.Models.Audit;
using System.Threading.Channels;

namespace Claims.Infrastructure.Services.Audit
{
    public class AuditWorker : BackgroundService
    {
        private readonly AuditQueue _auditQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AuditWorker(AuditQueue auditQueue, IServiceScopeFactory serviceScopeFactory)
        {
            _auditQueue = auditQueue;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var auditMessage in _auditQueue.Reader.ReadAllAsync(stoppingToken))
            {
                using (IServiceScope scope = _serviceScopeFactory.CreateScope())
                {
                    IAuditService auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                    if (auditMessage.Type == Domain.Enums.AuditType.Claim)
                    {
                        await auditService.AuditClaim(auditMessage.EntityId, auditMessage.HttpRequestType);
                    }
                    else if (auditMessage.Type == Domain.Enums.AuditType.Cover)
                    {
                        await auditService.AuditCover(auditMessage.EntityId, auditMessage.HttpRequestType);
                    }
                }
            }
        }
    }
}
